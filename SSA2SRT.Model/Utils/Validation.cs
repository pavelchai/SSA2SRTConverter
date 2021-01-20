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
    /// Represents a collection of static methods that implement the most common validations.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Verifies that a value with the specified name that is passed in is not null.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="value"> The value. </param>
        /// <exception cref="ValueNullException">
        /// The exception that is thrown when value is null.
        /// </exception>
        /// <typeparam name="T"> Type of the value. </typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(string name, T value) where T : class
        {
            if (value == null)
            {
                throw new ValueNullException(name);
            }
        }
    }
}