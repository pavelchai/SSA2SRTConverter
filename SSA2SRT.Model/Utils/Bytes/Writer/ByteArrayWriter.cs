/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System;
using System.Collections.Generic;

namespace SSA2SRT.Model.Utils
{
    /// <summary>
    /// Represents a writer for the array of the bytes.
    /// </summary>
    internal sealed class ByteArrayWriter : IByteArrayWriter
    {
        /// <summary>
        /// Arrays of the bytes.
        /// </summary>
        private readonly LinkedList<byte[]> bytes = new LinkedList<byte[]>();

        /// <summary>
        /// Position in current array.
        /// </summary>
        private int position = 0;

        /// <summary>
        /// Indicates whether input data should be always copied.
        /// </summary>
        private bool alwaysCopyInputData = true;

        /// <summary>
        /// Creates a new writer for the array of the bytes.
        /// </summary>
        /// <param name="alwaysCopyInputData"> Indicates whether input data should be always copied. </param>
        public ByteArrayWriter(bool alwaysCopyInputData)
        {
            this.alwaysCopyInputData = alwaysCopyInputData;
        }

        /// <summary>
        /// Gets a current position in the <see cref="IByteArrayWriter"></see>.
        /// </summary>
        /// <returns> Current position in the <see cref="IByteArrayWriter"></see>. </returns>
        public int Position
        {
            get
            {
                return this.position;
            }
        }

        /// <summary>
        /// Writes the array of the bytes.
        /// </summary>
        /// <param name="data"> Array of the bytes. </param>
        /// <param name="offset"> Offset in the data. </param>
        /// <param name="count"> Number of the written bytes. </param>
        public void WriteBytes(byte[] data, int offset, int count)
        {
            if (this.alwaysCopyInputData || offset != 0 || count != data.Length)
            {
                byte[] copied = new byte[count];
                Buffer.BlockCopy(data, offset, copied, 0, count);
                this.bytes.AddLast(copied);
                this.position += count;
            }
            else
            {
                this.bytes.AddLast(data);
                this.position += count;
            }
        }

        /// <summary>
        /// Creates a new array of the bytes from the all written data.
        /// </summary>
        /// <returns> New array of the bytes. </returns>
        public byte[] GetBytes()
        {
            int allocatedBytesCount = this.bytes.Count;
            if (allocatedBytesCount == 0)
            {
            	return new byte[0];
            }

            if (allocatedBytesCount == 1)
            {
                byte[] data = this.bytes.First.Value;
                if (!this.alwaysCopyInputData)
                {
                    int length = data.Length;
                    byte[] copied = new byte[length];
                    Buffer.BlockCopy(data, 0, copied, 0, length);
                    return copied;
                }
                else
                {
                    return data;
                }
            }
            else
            {
                int outputLength = 0;
                for (var node = this.bytes.First; node != null; node = node.Next)
                {
                    outputLength += node.Value.Length;
                }

                byte[] output = new byte[outputLength];
                byte[] data;

                int offset = 0;
                int length = 0;
                for (var node = this.bytes.First; node != null; node = node.Next)
                {
                    data = node.Value;
                    length = data.Length;

                    Buffer.BlockCopy(data, 0, output, offset, length);
                    offset += length;
                }

                return output;
            }
        }
    }
}