/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System;

namespace SSA2SRT.Model.Utils
{
    /// <summary>
    /// The exception that is thrown when value is null.
    /// </summary>
    public sealed class ValueNullException : Exception
    {
        /// <summary>
        /// The exception that is thrown when value is null.
        /// </summary>
        /// <param name="name"> Name of the value. </param>
        public ValueNullException(string name) : base(string.Format("<{0}> is null.", name)) { }
    }
}