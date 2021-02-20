/*
 * SSA2SRT Converter service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System.ComponentModel.DataAnnotations;

namespace SSA2SRTService.Models
{
    /// <summary>
    /// File.
    /// </summary>
    public sealed class File
    {
        /// <summary>
        /// Name of the file.
        /// </summary>
        [Required(ErrorMessage = "Name isn't specified")]
        public string Name { get; set; }

        /// <summary>
        /// Data of the file (as Base64 string, with MIME type).
        /// </summary>
        [Required(ErrorMessage = "Data of the file (base64 + MIME type) isn't specified")]
        public string DataBase64 { get; set; }
    }
}