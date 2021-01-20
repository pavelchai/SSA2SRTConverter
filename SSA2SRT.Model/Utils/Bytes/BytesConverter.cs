/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System;
using System.Runtime.CompilerServices;

namespace SSA2SRT.Model.Utils
{
    /// <summary>
    /// Represents the value/bytes converter.
    /// </summary>
    internal static class BytesConverter
    {
        /// <summary> 
        /// Returns the specified 32-bit unsigned integer value as an array of bytes (big endian). 
        /// </summary>
        /// <param name="value"> The number to convert. </param>
        /// <returns> An array of bytes with length 4. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytesBigEndian(int value)
        {
            unchecked
            {
                byte[] data = new byte[4];
                data[0] = (byte)((value >> 24) & 0xFF);
                data[1] = (byte)((value >> 16) & 0xFF);
                data[2] = (byte)((value >> 8) & 0xFF);
                data[3] = (byte)(value & 0xFF);
                return data;
            }
        }
        
        /// <summary>
        /// Returns the specified 32-bit unsigned integer value as an array of bytes (big endian). 
        /// </summary>
        /// <param name="value"> The number to convert. </param>
        /// <returns> An array of bytes with length 4. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static byte[] GetBytesBigEndian(uint value)
        {
            unchecked
            {
                byte[] data = new byte[4];
                data[0] = (byte)((value >> 24) & 0xFF);
                data[1] = (byte)((value >> 16) & 0xFF);
                data[2] = (byte)((value >> 8) & 0xFF);
                data[3] = (byte)(value & 0xFF);
                return data;
            }
        }

        /// <summary>
        /// Returns the specified 16-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value"> The number to convert. </param>
        /// <returns> An array of bytes with length 2. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(ushort value)
        {
            unchecked
            {
                byte[] data = new byte[2];
                data[0] = (byte)(value & 0xFF);
                data[1] = (byte)((value >> 8) & 0xFF);
                return data;
            }
        }

        /// <summary>
        /// Returns the specified 32-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value"> The number to convert. </param>
        /// <returns> An array of bytes with length 4. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(uint value)
        {
            unchecked
            {
                byte[] data = new byte[4];
                data[0] = (byte)(value & 0xFF);
                data[1] = (byte)((value >> 8) & 0xFF);
                data[2] = (byte)((value >> 16) & 0xFF);
                data[3] = (byte)((value >> 24) & 0xFF);
                return data;
            }
        }

        /// <summary>
        /// Returns the specified 64-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value"> The number to convert. </param>
        /// <returns> An array of bytes with length 8. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetBytes(ulong value)
        {
            unchecked
            {
                byte[] data = new byte[8];
                data[0] = (byte)(value & 0xFF);
                data[1] = (byte)((value >> 8) & 0xFF);
                data[2] = (byte)((value >> 16) & 0xFF);
                data[3] = (byte)((value >> 24) & 0xFF);
                data[4] = (byte)((value >> 32) & 0xFF);
                data[5] = (byte)((value >> 40) & 0xFF);
                data[6] = (byte)((value >> 48) & 0xFF);
                data[7] = (byte)((value >> 56) & 0xFF);
                return data;
            }
        }

        /// <summary> 
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array (big endian).
        /// </summary>
        /// <param name="value"> The array of bytes. </param>
        /// <param name="startIndex"> The starting position within value. </param>
        /// <returns> A 32-bit unsigned integer formed by four bytes beginning at start index. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt32BigEndian(byte[] value, int startIndex)
        {
            unchecked
            {
                return (int)(
                    (value[startIndex] << 24) +
                    (value[startIndex + 1] << 16) +
                    (value[startIndex + 2] << 8) +
                    value[startIndex + 3]
                );
            }
        }

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value"> The array of bytes. </param>
        /// <param name="startIndex"> The starting position within value. </param>
        /// <returns> A 16-bit unsigned integer formed by two bytes beginning at start index. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            unchecked
            {
                return (ushort)(value[startIndex] + (value[startIndex + 1] << 8));
            }
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value"> The array of bytes. </param>
        /// <param name="startIndex"> The starting position within value. </param>
        /// <returns> A 32-bit unsigned integer formed by four bytes beginning at start index. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32(byte[] value, int startIndex)
        {
            unchecked
            {
                return (uint)(
                    value[startIndex] +
                    (value[startIndex + 1] << 8) +
                    (value[startIndex + 2] << 16) +
                    (value[startIndex + 3] << 24)
                );
            }
        }

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value"> The array of bytes. </param>
        /// <param name="startIndex"> The starting position within value. </param>
        /// <returns> A 64-bit unsigned integer formed by eight bytes beginning at start index. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToUInt64(byte[] value, int startIndex)
        {
            unchecked
            {
                return (
                    (ulong)value[startIndex] +
                    ((ulong)value[startIndex + 1] << 8) +
                    ((ulong)value[startIndex + 2] << 16) +
                    ((ulong)value[startIndex + 3] << 24) +
                    ((ulong)value[startIndex + 4] << 32) +
                    ((ulong)value[startIndex + 5] << 40) +
                    ((ulong)value[startIndex + 6] << 48) +
                    ((ulong)value[startIndex + 7] << 56)
                );
            }
        }
    }
}