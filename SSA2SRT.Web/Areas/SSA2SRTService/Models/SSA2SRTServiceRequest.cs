/*
 * SSA2SRT Converter service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSA2SRTService.Models
{
    /// <summary>
    /// Request.
    /// </summary>
    public sealed class SSA2SRTServiceRequest
    {
        /// <summary>
        /// Indicates whether converted subtitles / zip storages with subtitles should be saved in one zip file.
        /// </summary>
        [Required(ErrorMessage = "JoinInOneFile isn't specified")]
        public bool SaveInOneFile { get; set; }

        /// <summary>
        /// Array of the input files.
        /// </summary>
        [Required(ErrorMessage = "Files aren't specified")]
        public IEnumerable<File> Files { get; set; }
    }
}
