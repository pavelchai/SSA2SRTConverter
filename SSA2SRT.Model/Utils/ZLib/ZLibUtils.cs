/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using SSA2SRT.Model.ZLib;

namespace SSA2SRT.Model.Utils
{
    /// <summary>
    /// Represents the utils for the ZLib compression/decompression.
    /// </summary>
    internal static class ZLibUtils
    {
        /// <summary>
        /// Inflates the compressed bytes.
        /// </summary>
        /// <param name="compressedData"> Data. </param>
        /// <param name="decompressedSize"> Decompressed size. Optional parameter, but if it is known, then passing of the parameter may increasing performance. </param>
        /// <returns> Array of the decompressed bytes. </returns>
        public static byte[] Inflate(byte[] compressedData, int? decompressedSize = null)
        {
            return Inflate(compressedData, 0, compressedData.Length, decompressedSize);
        }

        /// <summary>
        /// Inflates the compressed bytes.
        /// </summary>
        /// <param name="compressedData"> Data. </param>
        /// <param name="offset"> Offset in the data. </param>
        /// <param name="count"> Number of the written bytes. </param>
        /// <param name="decompressedSize"> Decompressed size. Optional parameter, but if it is known, then passing of the parameter may increasing performance. </param>
        /// <returns> Array of the decompressed bytes. </returns>
        public static byte[] Inflate(byte[] compressedData, int offset, int count, int? decompressedSize = null)
        {
            IByteArrayWriter writer = new ByteArrayWriter(false);
            Inflater inflater = new Inflater();
            inflater.SetInput(compressedData, offset, count);

            int length = (decompressedSize != null) ? decompressedSize.Value : count;
            byte[] decompressedData;
            int processedBytes;
            
            while (true)
            {
                decompressedData = new byte[length];
                processedBytes = inflater.Inflate(decompressedData, 0, length);
                
                if (processedBytes != 0)
                {
                    writer.WriteBytes(decompressedData, 0, processedBytes);
                }
                else
                {
                    if (inflater.IsFinished)
                    {
                    	break;
                    }
                }
            }

            return writer.GetBytes();
        }

        /// <summary>
        /// Deflates the decompressed bytes.
        /// </summary>
        /// <param name="decompressedData"> Data. </param>
        /// <param name="compressedSize"> Compressed size. Optional parameter, but if it is known, then passing of the parameter may increasing performance. </param>
        /// <returns> Array of the compressed bytes. </returns>
        public static byte[] Deflate(byte[] decompressedData, int? compressedSize = null)
        {
            return Deflate(decompressedData, 0, decompressedData.Length, compressedSize);
        }

        /// <summary>
        /// Deflates the decompressed bytes.
        /// </summary>
        /// <param name="decompressedData"> Data. </param>
        /// <param name="offset"> Offset in the data. </param>
        /// <param name="count"> Number of the written bytes. </param>
        /// <param name="compressedSize"> Compressed size. Optional parameter, but if it is known, then passing of the parameter may increasing performance. </param>
        /// <returns> Array of the compressed bytes. </returns>
        public static byte[] Deflate(byte[] decompressedData, int offset, int count, int? compressedSize = null)
        {
            IByteArrayWriter writer = new ByteArrayWriter(true);
            Deflater deflater = new Deflater(Deflater.DEFLATED);
            deflater.SetInput(decompressedData, offset, count);
            deflater.Finish();

            int length = (compressedSize != null) ? compressedSize.Value : count;
            byte[] compressedData;
            int processedBytes;
            
            while (true)
            {
                compressedData = new byte[length];
                processedBytes = deflater.Deflate(compressedData, 0, length);
                
                if (processedBytes != 0)
                {
                    writer.WriteBytes(compressedData, 0, processedBytes);
                }
                else
                {
                    if (deflater.IsFinished)
                    {
                    	break;
                    }
                }
            }

            return writer.GetBytes();
        }
    }
}