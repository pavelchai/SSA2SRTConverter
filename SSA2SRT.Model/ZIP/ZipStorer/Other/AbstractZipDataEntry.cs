/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using SSA2SRT.Model.Utils;
using System;

namespace SSA2SRT.Model
{
    /// <summary>
    /// Represents a base class for the ZIP entry.
    /// </summary>
    internal abstract class AbstractZipDataEntry : AbstractDataStoreEntry
    {
        /// <summary>
        /// Represents a base class for the ZIP entry.
        /// </summary>
        /// <param name="path"> Path to the data. </param>
        /// <param name="compressionMethod"> Compression method for the data. </param>
        /// <param name="compressedSize"> Size of the compressed data. </param>
        /// <param name="headerOffset"> Offset of header information. </param>
        /// <param name="crc32"> 32-bit checksum of the data. </param>
        /// <param name="modifyTime"> Modification time of the data. </param>
        /// <param name="comment"> User comment for the data. </param>
        protected AbstractZipDataEntry(string path, CompressionMethod compressionMethod, uint compressedSize, uint headerOffset, uint crc32, DateTime modifyTime, string comment) : base(path)
        {
            this.CompressionMethod = compressionMethod;
            this.CompressedSize = compressedSize;
            this.HeaderOffset = headerOffset;
            this.CRC32 = crc32;
            this.ModifyTime = modifyTime;
            this.Comment = comment;
        }
        
        /// <summary>
        /// Compression method for the data.
        /// </summary>
        public readonly CompressionMethod CompressionMethod;

        /// <summary>
        /// Size of the compressed data.
        /// </summary>
        public readonly uint CompressedSize;

        /// <summary>
        /// Offset of header information.
        /// </summary>
        public readonly uint HeaderOffset;

        /// <summary>
        /// 32-bit checksum of the data.
        /// </summary>
        public readonly uint CRC32;

        /// <summary>
        /// Modification time of the data.
        /// </summary>
        public readonly DateTime ModifyTime;

        /// <summary>
        /// User comment for the data.
        /// </summary>
        public readonly string Comment;
    }
}