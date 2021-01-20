/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using SSA2SRT.Model.Utils;
using System;
using System.Text;

namespace SSA2SRT.Model
{
    /// <summary>
    /// Represents an entry for the <see cref="ZipWriteOnlyStorer"></see>.
    /// </summary>
    internal sealed class ZipWriteOnlyStorerEntry : AbstractZipDataEntry
    {
        /// <summary>
        /// UTF-8 Encoding.
        /// </summary>
        private static readonly Encoding utf8Encoding = Encoding.UTF8;

        /// <summary>
        /// Creates a new entry for the <see cref="ZipWriteOnlyStorer"></see>.
        /// </summary>
        /// <param name="path"> Path to the data. </param>
        /// <param name="size"> Size of the data. </param>
        /// <param name="compressionMethod"> Compression method for the data. </param>
        /// <param name="compressedSize"> Size of the compressed data. </param>
        /// <param name="headerOffset"> Offset of header information. </param>
        /// <param name="crc32"> 32-bit checksum of the data. </param>
        /// <param name="modifyTime"> Modification time of the data. </param>
        /// <param name="comment"> User comment for the data. </param>
        public ZipWriteOnlyStorerEntry(string path, uint size, CompressionMethod compressionMethod, uint compressedSize, uint headerOffset, uint crc32, DateTime modifyTime, string comment) : base(path, compressionMethod, compressedSize, headerOffset, crc32, modifyTime, comment)
        {
            this.PathAsBytes = utf8Encoding.GetBytes(path);
            this.PathLengthAsBytes = BytesConverter.GetBytes((ushort)this.PathAsBytes.Length);

            this.CommentAsBytes = utf8Encoding.GetBytes(comment);
            this.CommentLengthAsBytes = BytesConverter.GetBytes((ushort)this.CommentAsBytes.Length);

            this.CompressionMethodAsBytes = BytesConverter.GetBytes((ushort)compressionMethod);
            this.ModifyTimeAsBytes = BytesConverter.GetBytes(ZipStorerUtils.DateTimeToDosTime(modifyTime));
            this.CRC32AsBytes = BytesConverter.GetBytes(crc32);

            this.CompressedSizeAsBytes = BytesConverter.GetBytes(compressedSize >= 0xFFFFFFFF ? 0xFFFFFFFF : compressedSize);
            this.SizeAsBytes = BytesConverter.GetBytes(size >= 0xFFFFFFFF ? 0xFFFFFFFF : size);
            this.HeaderOffsetAsBytes = BytesConverter.GetBytes(headerOffset >= 0xFFFFFFFF ? 0xFFFFFFFF : (uint)headerOffset);

            this.CompressedSizeZip64AsBytes = BytesConverter.GetBytes(compressedSize);
            this.SizeZip64AsBytes = BytesConverter.GetBytes(size);
            this.HeaderOffsetZip64AsBytes = BytesConverter.GetBytes(headerOffset);
        }

        /// <summary>
        /// Path of the data (as bytes).
        /// </summary>
        public readonly byte[] PathAsBytes;

        /// <summary>
        /// Length of the path (as bytes).
        /// </summary>
        public readonly byte[] PathLengthAsBytes;

        /// <summary>
        /// User comment for the data (as bytes).
        /// </summary>
        public readonly byte[] CommentAsBytes;

        /// <summary>
        /// Length of the user comment for the data (as bytes).
        /// </summary>
        public readonly byte[] CommentLengthAsBytes;

        /// <summary>
        /// Compression method for the data (as bytes).
        /// </summary>
        public readonly byte[] CompressionMethodAsBytes;

        /// <summary>
        /// Modification time of the data (as bytes).
        /// </summary>
        public readonly byte[] ModifyTimeAsBytes;

        /// <summary>
        /// 32-bit checksum of the data (as bytes).
        /// </summary>
        public readonly byte[] CRC32AsBytes;

        /// <summary>
        /// Size of the compressed data (as 4 bytes).
        /// </summary>
        public readonly byte[] CompressedSizeAsBytes;

        /// <summary>
        /// Size of the data (as 4 bytes).
        /// </summary>
        public readonly byte[] SizeAsBytes;

        /// <summary>
        /// Offset of header information (as 4 bytes).
        /// </summary>
        public readonly byte[] HeaderOffsetAsBytes;

        /// <summary>
        /// Size of the compressed data (as 4 bytes).
        /// </summary>
        public readonly byte[] CompressedSizeZip64AsBytes;

        /// <summary>
        /// Size of the data (as 4 bytes).
        /// </summary>
        public readonly byte[] SizeZip64AsBytes;

        /// <summary>
        /// Offset of header information (as 4 bytes).
        /// </summary>
        public readonly byte[] HeaderOffsetZip64AsBytes;
    }
}