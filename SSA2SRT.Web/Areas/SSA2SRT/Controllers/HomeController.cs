/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using Microsoft.AspNetCore.Mvc;
using SSA2SRT.Model;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SSA2SRT.Web.Controllers
{
    [Area("SSA2SRT")]
    public sealed class HomeController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Convert()
        {
            var request = this.HttpContext.Request;
            var files = request.Form.Files;
            var count = files.Count;

            if (count == 0)
            {
                return new EmptyResult();
            }

            var data = new LinkedList<SSA2SRTConverterData>();

            foreach (var file in files)
            {
                using (var ms = new MemoryStream())
                {
                    using (var fs = file.OpenReadStream())
                    {
                        await fs.CopyToAsync(ms);
                        data.AddLast(new SSA2SRTConverterData(file.FileName, ms.ToArray()));
                    }
                }
            }

            var settings = new SSA2SRTConverterSettings();
            var isZip = count != 1;

            if (isZip)
            {
                settings.SaveInZipFile = true;
                settings.ZipFileName = string.Format("convert_{0:ddMMyyyy_HHmmss}.zip", DateTime.Now);
            }

            SSA2SRTConverterData converted = SSA2SRTConverter.Convert(data, settings).FirstOrDefault();
            
            if (converted != null)
            {
                return new JsonResult(
                    new {
                        isZip = isZip,
                        fileName = converted.Name,
                        fileData = System.Convert.ToBase64String(converted.Data)
                    });
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}