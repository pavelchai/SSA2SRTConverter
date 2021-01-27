/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System;

namespace SSA2SRT.Model
{
    /// <summary>
    /// The exception that is thrown when data is invalid.
    /// </summary>
    internal sealed class InvalidDataException : Exception
    {
        /// <summary>
        /// The exception that is thrown when data is invalid.
        /// </summary>
        public InvalidDataException() : base("Data is invalid.")
        {
        }
    }
    
    /// <summary>
    /// The exception that is thrown when entry is invalid.
    /// </summary>
    internal sealed class InvalidEntryException : Exception
    {
        /// <summary>
        /// The exception that is thrown when entry is invalid.
        /// </summary>
    	/// <param name="name"> Name of the entry. </param>
    	public InvalidEntryException(string name) : base(string.Format("Entry <{0}> is invalid.", name))
        {
        }
    }
}