/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using SSA2SRT.Model.Utils;
using SSA2SRT.Model.ZLib;
using System;
using System.Collections.Generic;

namespace SSA2SRT.Model
{
    /// <summary>
    /// Represents a write-only ZIP storer.
    /// </summary>
    internal sealed class ZipWriteOnlyStorer : AbstractZipStorer, IWriteOnlyDataStorer
    {
        /// <summary>
        /// Entries.
        /// </summary>
        private readonly LinkedList<ZipWriteOnlyStorerEntry> entries = new LinkedList<ZipWriteOnlyStorerEntry>();

        /// <summary>
        /// Writer of storage file.
        /// </summary>
        private readonly IByteArrayWriter zipFileWriter = new ByteArrayWriter(false);

        /// <summary>
        /// Comment of the ZIP file.
        /// </summary>
        private readonly string comment = "";

        /// <summary>
        /// Indicates whether compression is on.
        /// </summary>
        private readonly bool enableCompression;
        
        /// <summary>
        /// Indicates whether storer is ZIP64 storer.
        /// </summary>
        private readonly bool isZip64;

        /// <summary>
        /// Creates a new write-only ZIP storer.
        /// </summary>
        /// <param name="enableCompression"> Indicates whether compression is on. </param>
        /// <param name="isZip64"> Indicates whether storer is ZIP64 storer. </param>
        public ZipWriteOnlyStorer(bool enableCompression, bool isZip64)
        {
            this.enableCompression = enableCompression;
            this.isZip64 = isZip64;
        }

        /// <inheritdoc/>
        public IDataStorerEntry Add(string path, params byte[] data)
        {
        	Validation.NotNull("Path", path);
        	Validation.NotNull("Data", data);
        	
            string normalizedPath = path.Replace('\\', '/');
           
            int pos = normalizedPath.IndexOf(':');
            if (pos >= 0)
            {
            	normalizedPath = normalizedPath.Remove(0, pos + 1);
            }
            
            normalizedPath = normalizedPath.Trim('/');
            
            int uncompressedLength = data.Length;
            int compressedLength;
            byte[] compressedData;
            CompressionMethod compressionMethod;
            
            if (this.enableCompression)
            {
                Deflater deflater = new Deflater(Deflater.DEFLATED);
                deflater.SetInput(data, 0, uncompressedLength);
                deflater.Finish();

                compressedData = new byte[uncompressedLength];
                compressedLength = deflater.Deflate(compressedData, 0, uncompressedLength);
                if (deflater.IsFinished && compressedLength <= uncompressedLength)
                {
                    // Use deflate
                    compressionMethod = CompressionMethod.Deflate;
                }
                else
                {
                    // Force to store
                    compressedData = data;
                    compressedLength = uncompressedLength;
                    compressionMethod = CompressionMethod.Store;
                }
            }
            else
            {
                // Only store
                compressedData = data;
                compressedLength = uncompressedLength;
                compressionMethod = CompressionMethod.Store;
            }
            
            uint uncompressedSize = (uint)uncompressedLength;
            uint compressedSize = (uint)compressedLength;
            uint crc32 = CRC32.Calculate(data);
            uint headerOffset = (uint)this.zipFileWriter.Position;

            ZipWriteOnlyStorerEntry fileEntry = new ZipWriteOnlyStorerEntry(
                   normalizedPath,
                   uncompressedSize,
                   compressionMethod,
                   compressedSize,
                   headerOffset,
                   crc32,
                   DateTime.Now,
                   comment ?? ""
               );

            this.WriteFileHeader(this.zipFileWriter, fileEntry, false);

            this.zipFileWriter.WriteBytes(compressedData, 0, compressedLength);
            this.entries.AddLast(fileEntry);

            return fileEntry;
        }

        /// <inheritdoc/>
        public byte[] Close()
        {
            // Writes the central directory header
            ulong centralOffset = (ulong)this.zipFileWriter.Position;
            ulong centralSize = 0;

            long position = 0;
            for (var node = this.entries.First; node != null; node = node.Next)
            {
                position = this.zipFileWriter.Position;
                this.WriteFileHeader(this.zipFileWriter, node.Value, true);
                centralSize += (ulong)(this.zipFileWriter.Position - position);
            }

            ulong dirOffset = (ulong)this.zipFileWriter.Position;
            byte[] encodedComment = utf8Encoding.GetBytes(this.comment);
            
            if (this.isZip64)
            {
            	// Writes a ZIP64 end of central directory record (56 bytes)
            	
            	byte[] countBytes = BytesConverter.GetBytes((ulong)this.entries.Count);
	            
	            this.zipFileWriter.WriteBytes(zip64EndOfCentralDirRecordSignatureBytes); // 4 bytes, ZIP64 end of central dir signature
	            this.zipFileWriter.WriteBytes(fourtyFour8Bytes); // 8 bytes, size of ZIP64 end of central directory record. Size = SizeOfFixedFields + SizeOfVariableData - 12.
	            this.zipFileWriter.WriteBytes(fourtyFive2Bytes); // 2 bytes, version made by
	            this.zipFileWriter.WriteBytes(fourtyFive2Bytes); // 2 bytes, version needed to extract 
	            this.zipFileWriter.WriteBytes(zero4Bytes); // 4 bytes, number of this disk
	            this.zipFileWriter.WriteBytes(zero4Bytes); // 4 bytes, number of the disk with the start of the central directory
	            this.zipFileWriter.WriteBytes(countBytes); // 8 bytes, total number of entries in the central directory on this disk
	            this.zipFileWriter.WriteBytes(countBytes); // 8 bytes, total number of entries in the central directory
	            this.zipFileWriter.WriteBytes(BytesConverter.GetBytes(centralSize)); // 8 bytes, size of the central directory
	            this.zipFileWriter.WriteBytes(BytesConverter.GetBytes(centralOffset)); // 8 bytes, offset of start of central directory with respect to the starting disk number
	
	            // Writes the ZIP64 end of central directory locator (20 bytes)
	            this.zipFileWriter.WriteBytes(zip64EndOfCentralDirLocatorSignatureBytes); // 4 bytes, ZIP64 end of central dir locator signature
	            this.zipFileWriter.WriteBytes(zero4Bytes); // 4 bytes, number of the disk with the start of the zip64 end of central directory
	            this.zipFileWriter.WriteBytes(BytesConverter.GetBytes(dirOffset)); // 8 bytes, relative offset of the zip64 end of central directory record
	            this.zipFileWriter.WriteBytes(one4Bytes); // 4 bytes, total number of disks 
            }
            
            // Writes the end of central directory record (22 bytes)
            this.zipFileWriter.WriteBytes(endOfCentralDirRecordSignatureBytes); // 4 bytes, end of central dir signature
            this.zipFileWriter.WriteBytes(zero2Bytes); // 2 bytes, number of this disk
            this.zipFileWriter.WriteBytes(zero2Bytes); // 2 bytes, number of the disk with the start of the central directory
            
            if (isZip64)
            {
            	this.zipFileWriter.WriteBytes(zip64EndOfCentralDirectoryDataPart); // 2 + 2 + 4 + 4 = 12 bytes, total number of entries in the central directory on this disk (2),total number of entries in the central directory (2), size of the central directory (4), offset of start of central directory with respect to the starting disk number (4)
            }
            else
            {
            	byte[] countBytes = BytesConverter.GetBytes((ushort)this.entries.Count);
            	
            	this.zipFileWriter.WriteBytes(countBytes); // 2 bytes, total number of entries in the central directory on this disk
	            this.zipFileWriter.WriteBytes(countBytes); // 2 bytes, total number of entries in the central directory
	            this.zipFileWriter.WriteBytes(BytesConverter.GetBytes((uint)centralSize)); // 4 bytes, size of the central directory
	            this.zipFileWriter.WriteBytes(BytesConverter.GetBytes((uint)centralOffset)); // 4 bytes, offset of start of central directory with respect to the starting disk number
            }
            
            this.zipFileWriter.WriteBytes(BytesConverter.GetBytes((ushort)encodedComment.Length)); // 2 bytes, .ZIP file comment length
            this.zipFileWriter.WriteBytes(encodedComment); // variable size, .ZIP file comment
            
            return this.zipFileWriter.GetBytes();
        }

        /// <summary>
        /// Writes the header of the file with the writer.
        /// </summary>
        /// <param name="writer"> The writer. </param>
        /// <param name="entry"> The entry. </param>
        /// <param name="isCentralFileHeader"> Determines whether header is central file header or not (local). </param>
        private void WriteFileHeader(IByteArrayWriter writer, ZipWriteOnlyStorerEntry entry, bool isCentralFileHeader)
        {
            if (isCentralFileHeader)
            {
                writer.WriteBytes(centralDirectoryFileHeaderSignatureBytes); // 4 bytes, central directory file header signature
                writer.WriteBytes(23, 0xB); // 2 bytes, version made by
            }
            else
            {
                writer.WriteBytes(localFileHeaderSignatureBytes); // 4 bytes, local file header signature
            }

            writer.WriteBytes(versionToExtractBytes); // 2 bytes, version needed to extract (minimum) 
            writer.WriteBytes(gpbg8Bytes); // 2 bytes, general purpose bit flag
            writer.WriteBytes(entry.CompressionMethodAsBytes);  // 2 bytes, compression method 
            writer.WriteBytes(entry.ModifyTimeAsBytes);  // 2+2 bytes, file last modification time and date
            writer.WriteBytes(entry.CRC32AsBytes); // 4 bytes, CRC-32
            writer.WriteBytes(entry.CompressedSizeAsBytes); // 4 bytes, compressed size
            writer.WriteBytes(entry.SizeAsBytes); // 4 bytes, uncompressed size
            writer.WriteBytes(entry.PathLengthAsBytes); // 2 bytes, file name length
            
            if (this.isZip64)
            {
            	writer.WriteBytes(thirtyTwo2Bytes); // 2 bytes, extra field length = 32
            }
            else
            {
            	writer.WriteBytes(zero2Bytes); // 2 bytes, extra field length = 32
            }

            if (isCentralFileHeader)
            {
                writer.WriteBytes(entry.CommentLengthAsBytes); // 2 bytes, file comment length
                writer.WriteBytes(zero4Bytes); // 2+2=4 bytes, disk number where file starts (disk=0), internal file attributes
                writer.WriteBytes(externalAttributesBytes); // 4 bytes, External file attributes
                writer.WriteBytes(entry.HeaderOffsetAsBytes);  // 4 bytes, relative offset of header
            }

            writer.WriteBytes(entry.PathAsBytes); // variable size, file name

            if (this.isZip64)
            {
            	writer.WriteBytes(zip64ExtraBlockTagSignatureBytes); // 2 bytes, tag for the extra block (ZIP64 Extended Information Extra Field)
	            writer.WriteBytes(thirtyTwo2Bytes); // 2 bytes, size of the extra block
	            writer.WriteBytes(zero4Bytes);
	            writer.WriteBytes(entry.SizeZip64AsBytes); // 8 bytes, uncompressed size
	            writer.WriteBytes(zero4Bytes);
	            writer.WriteBytes(entry.CompressedSizeZip64AsBytes); // 8 bytes, compressed size
	            writer.WriteBytes(zero4Bytes);
	            writer.WriteBytes(entry.HeaderOffsetZip64AsBytes); // 8 bytes, compressed size
	            writer.WriteBytes(zero4Bytes); // 4 bytes, disk number where file starts (disk=0)
            }
            
            if (isCentralFileHeader)
            {
                writer.WriteBytes(entry.CommentAsBytes); // variable size, file comment
            }
        }
    }
}