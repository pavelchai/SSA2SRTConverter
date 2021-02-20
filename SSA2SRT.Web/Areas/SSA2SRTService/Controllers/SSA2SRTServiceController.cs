/*
 * SSA2SRT Converter service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using Microsoft.AspNetCore.Mvc;
using SSA2SRTService.Models;

namespace SSA2SRTService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "SSA2SRTConverterService")]
    public sealed class SSA2SRTConverterServiceController : ControllerBase
    {
        /// <summary>
        /// Converts the SubStationAlpha subtitles (*.ssa, *.ass) to the SubRip subtitles (*.srt).
        /// </summary>
        /// <param name="request"> Request info. </param>
        /// <returns> Response info. </returns>
        [HttpPost]
        [ProducesResponseType(typeof(SSA2SRTServiceResponse), 200)]
        [ProducesResponseType(400)]
        public IActionResult Post(SSA2SRTServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return new ObjectResult(SSA2SRTServiceProcessor.Process(request));
        }
    }
}