/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System.Collections.Generic;

namespace SSA2SRT.Model.Utils
{
    /// <summary>
    /// Represents a 32-bit Cyclic Redundancy Code (CRC-32) checksum calculator.
    /// </summary>
    internal static class CRC32
    {
        /// <summary>
        /// CRC-32 table.
        /// </summary>
        private static readonly uint[] crc32Table;

        /// <summary>
        /// Initializes the <see cref="CRC32"></see>.
        /// </summary>
        static CRC32()
        {
            crc32Table = new uint[256];
            uint value;
            for (uint i = 0; i < 256; i++)
            {
                value = i;
                for (uint j = 0; j < 8; ++j)
                {
                    if ((value & 1) != 0)
                    {
                        value = (value >> 1) ^ 0xEDB88320;
                    }
                    else
                    {
                        value >>= 1;
                    }
                }
                crc32Table[i] = value;
            }
        }

        /// <summary>
        /// Calculates the CRC-32 checksum for the data.
        /// </summary>
        /// <param name="data"> Data. </param>
        /// <returns> CRC-32 checksum for the data. </returns>
        public static uint Calculate(params IList<byte>[] data)
        {
            unchecked
            {
                uint crc32 = uint.MaxValue;

                int dataLength = data.Length;
                int listCount = 0;
                IList<byte> list;
                for (int i = 0; i < dataLength; i++)
                {
                    list = data[i];
                    listCount = list.Count;
                    for (int k = 0; k < listCount; k++)
                    {
                        crc32 = (crc32 >> 8) ^ crc32Table[(crc32 ^ list[k]) & 0xFF];
                    }
                }

                return crc32 ^ uint.MaxValue;
            }
        }
    }
}