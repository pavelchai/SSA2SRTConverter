/*
 * SSA2SRT Converter service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using SSA2SRTService.Models;

namespace SSA2SRTService.Controllers
{
    [Area("SSA2SRT")]
    public sealed class HomeController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Convert()
        {
            var formFiles = this.HttpContext.Request.Form.Files;
            var count = formFiles.Count;

            if (count == 0)
            {
                return new EmptyResult();
            }

            var preparedFiles = new LinkedList<File>();

            foreach (var formFile in formFiles)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var fs = formFile.OpenReadStream())
                    {
                        await fs.CopyToAsync(ms);

                        preparedFiles.AddLast(new File() {
                            Name = formFile.FileName,
                            DataBase64 = string.Concat(
                                "data:text/plain;base64,",
                                System.Convert.ToBase64String(ms.ToArray()))
                        });
                    }
                }
            }

            var isZip = count > 1;
            var controller = new SSA2SRTConverterServiceController();
            var request = new SSA2SRTServiceRequest() { SaveInOneFile = isZip, Files = preparedFiles };
            var response = (await controller.Post(request) as ObjectResult).Value as SSA2SRTServiceResponse;

            File file = response.Files.FirstOrDefault();

            if (file != null)
            {
                return new JsonResult(
                    new
                    {
                        isZip = isZip,
                        fileName = file.Name,
                        fileData = file.DataBase64.Split(',')[1]
                    });
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}