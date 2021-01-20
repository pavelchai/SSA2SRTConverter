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
    /// Represents an entry for the <see cref="ZipReadOnlyStorer"></see>.
    /// </summary>
    internal sealed class ZipReadOnlyStorerEntry : AbstractZipDataEntry
    {
        /// <summary>
        /// Creates a new entry for the <see cref="ZipReadOnlyStorer"></see>.
        /// </summary>
        /// <param name="path"> Path to the data. </param>
        /// <param name="size"> Size of the data. </param>
        /// <param name="compressionMethod"> Compression method for the data. </param>
        /// <param name="compressedSize"> Size of the compressed data. </param>
        /// <param name="headerOffset"> Offset of header information. </param>
        /// <param name="fileOffset"> Offset of data in the storer. </param>
        /// <param name="crc32"> 32-bit checksum of the data. </param>
        /// <param name="modifyTime"> Modification time of the data. </param>
        /// <param name="comment"> User comment for the data. </param>
        public ZipReadOnlyStorerEntry(string path, uint size, CompressionMethod compressionMethod, uint compressedSize, uint headerOffset, uint fileOffset, uint crc32, DateTime modifyTime, string comment) : base(path, compressionMethod, compressedSize, headerOffset, crc32, modifyTime, comment)
        {
        	this.Size = size;
            this.FileOffset = fileOffset;
        }
        
        /// <summary>
        /// Size of the data.
        /// </summary>
        public readonly uint Size;

        /// <summary>
        /// Offset of the data.
        /// </summary>
        public readonly uint FileOffset;
    }
}