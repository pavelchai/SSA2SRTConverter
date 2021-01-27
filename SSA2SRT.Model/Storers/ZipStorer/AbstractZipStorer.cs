/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using SSA2SRT.Model.Utils;
using System.Text;

namespace SSA2SRT.Model
{
    /// <summary>
    /// Represents a base class for the file storer for the ZIP.
    /// </summary>
    internal abstract class AbstractZipStorer
    {
        /// <summary>
        /// UTF-8 Encoding.
        /// </summary>
        protected static readonly Encoding utf8Encoding = Encoding.UTF8;

        /// <summary>
        /// ZIP64 central dir signature.
        /// </summary>
        protected const uint zip64CentralDirSignature = 0xFFFFFFFF;

        /// <summary>
        /// ZIP64 extra block tag signature.
        /// </summary>
        protected const ushort zip64ExtraBlockTagSignature = 0x0001;

        /// <summary>
        /// ZIP64 end of central dir signature.
        /// </summary>
        protected const uint zip64EndOfCentralDirRecordSignature = 0x06064b50;

        /// <summary>
        /// ZIP64 end of central dir locator signature.
        /// </summary>
        protected const uint zip64EndOfCentralDirLocatorSignature = 0x07064b50;

        /// <summary>
        /// Local file header signature.
        /// </summary>
        protected const uint localFileHeaderSignature = 0x04034b50;

        /// <summary>
        /// Central directory file header signature.
        /// </summary>
        protected const uint centralDirectoryFileHeaderSignature = 0x02014b50;

        /// <summary>
        /// End of central directory record signature.
        /// </summary>
        protected const uint endOfCentralDirRecordSignature = 0x06054b50;

        /// <summary>
        /// Central directory file header signature (as bytes).
        /// </summary>
        protected static readonly byte[] centralDirectoryFileHeaderSignatureBytes = BytesConverter.GetBytes(centralDirectoryFileHeaderSignature);

        /// <summary>
        /// ZIP64 extra block tag signature (as bytes).
        /// </summary>
        protected static readonly byte[] zip64ExtraBlockTagSignatureBytes = BytesConverter.GetBytes(zip64ExtraBlockTagSignature);

        /// <summary>
        /// ZIP64 end of central dir record signature (as bytes).
        /// </summary>
        protected static readonly byte[] zip64EndOfCentralDirRecordSignatureBytes = BytesConverter.GetBytes(zip64EndOfCentralDirRecordSignature);

        /// <summary>
        /// ZIP64 end of central dir locator signature (as bytes).
        /// </summary>
        protected static readonly byte[] zip64EndOfCentralDirLocatorSignatureBytes = BytesConverter.GetBytes(zip64EndOfCentralDirLocatorSignature);

        /// <summary>
        /// ZIP64 end of central directory part of data. 
        /// 2 + 2 + 4 + 4 = 12 bytes,
        /// total number of entries in the central directory on this disk (2),
        /// total number of entries in the central directory (2), 
        /// size of the central directory (4),
        /// offset of start of central directory with respect to the starting disk number (4).
        /// </summary>
        protected readonly byte[] zip64EndOfCentralDirectoryDataPart = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        /// <summary>
        /// Local file header signature (as bytes).
        /// </summary>
        protected static readonly byte[] localFileHeaderSignatureBytes = BytesConverter.GetBytes(localFileHeaderSignature);

        /// <summary>
        /// End of central dir record signature (as bytes).
        /// </summary>
        protected static readonly byte[] endOfCentralDirRecordSignatureBytes = BytesConverter.GetBytes(endOfCentralDirRecordSignature);

        /// <summary>
        /// General purpose bit flags (as bytes).
        /// Deflate (Normal)/No compression + UTF-8 encoding.
        /// </summary>
        protected static readonly byte[] gpbg8Bytes = BytesConverter.GetBytes((ushort)(0x0800));

        /// <summary>
        /// External attributes (as bytes).
        /// </summary>
        protected static readonly byte[] externalAttributesBytes = BytesConverter.GetBytes((uint)(0x8100));

        /// <summary>
        /// 0 (2 bytes).
        /// </summary>
        protected static readonly byte[] zero2Bytes = BytesConverter.GetBytes((ushort)(0));

        /// <summary>
        /// 0 (4 bytes).
        /// </summary>
        protected static readonly byte[] zero4Bytes = BytesConverter.GetBytes((uint)(0));

        /// <summary>
        /// 1 (4 bytes).
        /// </summary>
        protected static readonly byte[] one4Bytes = BytesConverter.GetBytes((uint)(1));

        /// <summary>
        /// 32 (2 bytes).
        /// </summary>
        protected static readonly byte[] thirtyTwo2Bytes = BytesConverter.GetBytes((ushort)(32));

        /// <summary>
        /// 44 (8 bytes).
        /// </summary>
        protected static readonly byte[] fourtyFour8Bytes = BytesConverter.GetBytes((ulong)(44));

        /// <summary>
        /// 45 (2 bytes).
        /// </summary>
        protected static readonly byte[] fourtyFive2Bytes = BytesConverter.GetBytes((ushort)(45));

        /// <summary>
        /// Minimum version to extract (2 bytes).
        /// </summary>
        protected static readonly byte[] versionToExtractBytes = new byte[] { 20, 0 };
    }
}