/*
 * SSA2SRT Converter service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System.Collections.Generic;

namespace SSA2SRTService.Models
{
    /// <summary>
    /// Response.
    /// </summary>
    public sealed class SSA2SRTServiceResponse
    {
        /// <summary>
        /// Array of the output files.
        /// </summary>
        public IEnumerable<File> Files { get; set; }
    }
}