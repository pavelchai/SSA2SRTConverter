/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System;

namespace SSA2SRT.Model
{
    /// <summary>
    /// Represents the utils for the ZIP storer.
    /// </summary>
    internal static class ZipStorerUtils
    {
        /// <summary>
        /// Converts a DOS time to the <see cref="DateTime"></see>.
        /// </summary>
        /// <param name="dt"> DOS time. </param>
        /// <returns> <see cref="DateTime"></see>. </returns>
        public static DateTime DosTimeToDateTime(uint dt)
        {
            int year = (int)(dt >> 25) + 1980;
            int month = (int)(dt >> 21) & 15;
            int day = (int)(dt >> 16) & 31;
            int hours = (int)(dt >> 11) & 31;
            int minutes = (int)(dt >> 5) & 63;
            int seconds = (int)(dt & 31) * 2;

            if (month == 0 || day == 0)
            {
                return DateTime.Now;
            }

            return new DateTime(year, month, day, hours, minutes, seconds);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"></see> to the DOS time.
        /// </summary>
        /// <param name="dt"> <see cref="DateTime"></see>. </param>
        /// <returns> Dos time. </returns>
        public static uint DateTimeToDosTime(DateTime dt)
        {
            return (uint)(
                (dt.Second / 2) |
                (dt.Minute << 5) |
                (dt.Hour << 11) |
                (dt.Day << 16) |
                (dt.Month << 21) |
                ((dt.Year - 1980) << 25)
            );
        }
    }
}