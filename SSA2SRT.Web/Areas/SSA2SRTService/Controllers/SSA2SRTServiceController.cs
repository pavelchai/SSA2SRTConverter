/*
 * SSA2SRT Converter service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using Microsoft.AspNetCore.Mvc;
using SSA2SRTService.Models;
using System.Net;
using System.Threading.Tasks;

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
        [ProducesResponseType(typeof(SSA2SRTServiceResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post(SSA2SRTServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await Task.Run(() => SSA2SRTServiceProcessor.Process(request));
            return new ObjectResult(response);
        }
    }
}