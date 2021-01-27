/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using SSA2SRT.Model.Utils;
using System;
using System.Collections.Generic;

namespace SSA2SRT.Model
{
    /// <summary>
    /// Represents a read-only ZIP storer.
    /// </summary>
    internal sealed class ZipReadOnlyStorer : AbstractZipStorer, IReadOnlyDataStorer
    {
        /// <summary>
        /// Data of storage file.
        /// </summary>
        private byte[] zipFileData;
        
        /// <summary>
        /// Creates a new read-only ZIP storer.
        /// </summary>
        /// <param name="isZip64"> Indicates whether storer is ZIP64 storer. </param>
        /// <param name="data"> Data of the zip file. </param>
        /// <param name="entries"> Entries in the storer. </param>
    	private ZipReadOnlyStorer(bool isZip64, byte[] data, IReadOnlyList<IDataStorerEntry> entries)
        {
        	this.IsZip64 = isZip64;
        	this.zipFileData = data;
        	this.Entries = entries;
        }
    	
    	/// <summary>
        /// Indicates whether storer is ZIP64 storer.
        /// </summary>
        public readonly bool IsZip64;
        
        /// <inheritdoc/>
    	public IReadOnlyList<IDataStorerEntry> Entries { get; private set;}
        
        /// <inheritdoc/>
        public byte[] Read(IDataStorerEntry entry)
        {
        	Validation.NotNull("Entry", entry);
        	
            ZipReadOnlyStorerEntry zip64Entry = entry as ZipReadOnlyStorerEntry;
            if(zip64Entry == null)
            {
            	throw new InvalidEntryException(entry.Path);
            }

            byte[] data = null;
            int count = (int)zip64Entry.Size;
            
            if (zip64Entry.CompressionMethod == CompressionMethod.Store)
            {
                data = new byte[count];
                Buffer.BlockCopy(this.zipFileData, (int)zip64Entry.FileOffset, data, 0, count);
            }
            else
            {
                data = ZLibUtils.Inflate(this.zipFileData, (int)zip64Entry.FileOffset, (int)zip64Entry.CompressedSize, count);
            }
            
            if(zip64Entry.CRC32 != CRC32.Calculate(data))
            {
            	throw new InvalidEntryException(entry.Path);
            }

            return data;
        }
        
        /// <summary>
        /// Creates a new read-only zip storer from the data.
        /// </summary>
        /// <param name="data"> The data. </param>
        /// <returns> New read-only zip storer. </returns>
        /// <exception cref="ValueNullException">
        /// The exception that is thrown when data is null.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// The exception that is thrown when data is invalid.
        /// </exception>
        public static IReadOnlyDataStorer FromData(params byte[] data)
        {
        	Validation.NotNull("Data", data);
        	
            const int cfhSize = 46; // fixed size of the central file header
            const int lfhSize = 30; // fixed size of the local file header
            const int lfnOffset = 26; // relative offset to the size of the file name (local header)
            const int lefOffset = 28; // relative offset to the size of the extra field (local header)
            
            int length = data.Length;
            if (length < 22)
            {
            	// Min length of the zip data is more than or equals to 22 bytes
                throw new InvalidDataException();
            }

            int lastIndex = length - 1;
            int max = length - 5;
            int signaturePosition = -1;
            for (int i = max; i >= 0; i--)
            {
                signaturePosition = lastIndex - i - 4;
                
                if (BytesConverter.ToUInt32(data, signaturePosition) == endOfCentralDirRecordSignature)
                {
                	bool isZip64 = false;
                    long centralDirEntries = BytesConverter.ToUInt16(data, signaturePosition + 10);
                    long centralDirSize = BytesConverter.ToUInt32(data, signaturePosition + 12);
                    long centralDirOffset = BytesConverter.ToUInt32(data, signaturePosition + 16);

                    if (centralDirOffset == zip64CentralDirSignature) // It is a Zip64 file
                    {
                    	isZip64 = true;
                    	
                        int zip64CentralDirLocatorOffset = signaturePosition - 20;
                        if (BytesConverter.ToUInt32(data, zip64CentralDirLocatorOffset) != zip64EndOfCentralDirLocatorSignature)
                        {
                            // Not a ZIP64 central dir locator
                            throw new InvalidDataException();
                        }

                        int zip64CentralDirRecordOffset = (int)BytesConverter.ToUInt32(data, zip64CentralDirLocatorOffset + 8);
                        if (BytesConverter.ToUInt32(data, zip64CentralDirRecordOffset) != zip64EndOfCentralDirRecordSignature)
                        {
                            // Not a ZIP64 central dir record
                            throw new InvalidDataException();
                        }

                        centralDirEntries = (long)BytesConverter.ToUInt64(data, zip64CentralDirRecordOffset + 32);
                        centralDirSize = (long)BytesConverter.ToUInt64(data, zip64CentralDirRecordOffset + 40);
                        centralDirOffset = (long)BytesConverter.ToUInt64(data, zip64CentralDirRecordOffset + 48);
                    }

                    int centralDirStartIndex = (int)centralDirOffset;
                    int centralDirEndIndex = centralDirStartIndex + (int)centralDirSize;

                    IDataStorerEntry[] fileEntries = new IDataStorerEntry[centralDirEntries];
                    int index = 0;
                    
                    for (int pointer = centralDirStartIndex; pointer < centralDirEndIndex;)
                    {
                        if (BytesConverter.ToUInt32(data, pointer) != centralDirectoryFileHeaderSignature)
                        {
                            // Not a ZIP64 central dir file header
                            throw new InvalidDataException();
                        }

                        // 4 bytes, central directory file header signature
                        // 2 bytes, version made by
                        // 2 bytes, version needed to extract (minimum) 
                        // 2 bytes, general purpose bit flag
                        ushort method = BytesConverter.ToUInt16(data, pointer + 10); // 2 bytes, compression method 
                        uint modifyTime = BytesConverter.ToUInt32(data, pointer + 12); // 2+2 bytes, file last modification time and date
                        uint crc32 = BytesConverter.ToUInt32(data, pointer + 16); // 4 bytes, CRC-32
                        uint compressedSize = BytesConverter.ToUInt32(data, pointer + 20); // 4 bytes, compressed size
                        uint fileSize = BytesConverter.ToUInt32(data, pointer + 24); // 4 bytes, uncompressed size
                        ushort fileNameSize = BytesConverter.ToUInt16(data, pointer + 28); // 2 bytes, file name length
                        ushort extraFieldSize = BytesConverter.ToUInt16(data, pointer + 30); // 2 bytes, extra field length
                        ushort commentSize = BytesConverter.ToUInt16(data, pointer + 32); // 2 bytes, file comment length
                                                                                          // 2+2=4 bytes, disk number where file starts (disk=0), internal file attributes
                                                                                          // 4 bytes, External file attributes
                        uint headerOffset = BytesConverter.ToUInt32(data, pointer + 42); // 4 bytes, relative offset of header

                        uint headerSize = (uint)(cfhSize + fileNameSize + extraFieldSize + commentSize);
                        int commentsPointer = (int)(pointer + headerSize - commentSize);
                        int extraFieldBlockPointer = commentsPointer - extraFieldSize;

                        while (true)
                        {
                            if (extraFieldBlockPointer >= commentsPointer)
                            {
                                break;
                            }

                            if (BytesConverter.ToUInt16(data, extraFieldBlockPointer) == zip64ExtraBlockTagSignature)
                            {
                                if (fileSize == zip64CentralDirSignature)
                                {
                                	fileSize = BytesConverter.ToUInt32(data, extraFieldBlockPointer + 12);
                                }
                                
                                if (compressedSize == zip64CentralDirSignature)
                                {
                                	compressedSize = BytesConverter.ToUInt32(data, extraFieldBlockPointer + 20);
                                }
                                
                                if (headerOffset == zip64CentralDirSignature)
                                {
                                	headerOffset = BytesConverter.ToUInt32(data, extraFieldBlockPointer + 28);
                                }
                                
                                break;
                            }
                            else
                            {
                                extraFieldBlockPointer += BytesConverter.ToUInt16(data, extraFieldBlockPointer + 2);
                            }
                        }
                        
                        if (BytesConverter.ToUInt32(data, (int)headerOffset) != localFileHeaderSignature)
			            {
			                throw new InvalidDataException();
			            }

                        uint lfeSize = (uint)(BytesConverter.ToUInt16(data, (int)headerOffset + lfnOffset) + BytesConverter.ToUInt16(data, (int)headerOffset + lefOffset));

                        ZipReadOnlyStorerEntry fileEntry = new ZipReadOnlyStorerEntry(
                            utf8Encoding.GetString(data, cfhSize + pointer, fileNameSize),
                            fileSize,
                            (CompressionMethod)method,
                            compressedSize,
                            headerOffset,
                            (uint)(lfhSize + lfeSize + headerOffset),
                            crc32,
                            ZipStorerUtils.DosTimeToDateTime(modifyTime),
                            (commentSize > 0) ? utf8Encoding.GetString(data, (int)(pointer + headerSize - commentSize), commentSize) : ""
                        );

                        fileEntries[index++] = fileEntry;
                        pointer += (int)headerSize;
                    }
                    
                    return new ZipReadOnlyStorer(isZip64, data, fileEntries);
                }
            }

            throw new InvalidDataException();
        }
    }
}