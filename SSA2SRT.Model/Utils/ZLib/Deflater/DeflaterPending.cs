/*
 * Modified and refactored version of the file from SharpZipLib [based on https://github.com/icsharpcode/SharpZipLib, MIT License]
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
namespace SSA2SRT.Model.ZLib
{
    /// <summary>
    /// This class stores the pending output of the Deflater.
    ///
    /// author of the original java version : Jochen Hoenicke
    /// </summary>
    internal sealed class DeflaterPending : PendingBuffer
    {
        /// <summary>
        /// Construct instance with default buffer size
        /// </summary>
        public DeflaterPending() : base(DeflaterConstants.PENDING_BUF_SIZE) { }
    }
}