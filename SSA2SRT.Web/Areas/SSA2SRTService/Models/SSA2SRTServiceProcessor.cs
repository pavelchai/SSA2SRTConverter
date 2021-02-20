/*
 * SSA2SRT Converter service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using SSA2SRT.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSA2SRTService.Models
{
    public static class SSA2SRTServiceProcessor
    {
        public static SSA2SRTServiceResponse Process(SSA2SRTServiceRequest request)
        {
            return new SSA2SRTServiceResponse() { Files = Process(request.SaveInOneFile, request.Files) };
        }

        private static IEnumerable<File> Process(bool saveInOneFile, IEnumerable<File> files)
        {
            var data = GetData(files);
            var settings = new SSA2SRTConverterSettings();

            if (saveInOneFile)
            {
                settings.SaveInZipFile = true;
                settings.ZipFileName = string.Format("convert_{0:ddMMyyyy_HHmmss}.zip", DateTime.Now);

                var converted = SSA2SRTConverter.Convert(data.Select(d => d.Value), settings);
                var zipFile = converted.FirstOrDefault();

                if (zipFile != null)
                {
                    yield return new File()
                    {
                        Name = zipFile.Name,
                        DataBase64 = string.Concat("data:application/zip;base64,", Convert.ToBase64String(zipFile.Data))
                    };
                }
            }
            else
            {
                foreach (var pair in data)
                {
                    var converted = SSA2SRTConverter.Convert(new[] { pair.Value}, settings).FirstOrDefault();
                    if (converted != null)
                    {
                        yield return new File()
                        {
                            Name = converted.Name,
                            DataBase64 = string.Concat("data:text/plain;base64,", Convert.ToBase64String(converted.Data))
                        };
                    }
                }
            }

        }

        private static IEnumerable<KeyValuePair<string, SSA2SRTConverterData>> GetData(IEnumerable<File> files)
        {
            foreach (var file in files)
            {
                if (TryGetData(file, out SSA2SRTConverterData data, out string dataURLWithoutData))
                {
                    yield return new KeyValuePair<string, SSA2SRTConverterData>(dataURLWithoutData, data);
                }
            }
        }

        private static bool TryGetData(File file, out SSA2SRTConverterData output, out string dataURLWithoutData)
        {
            string[] parts = file.DataBase64.Split(',');
            if (parts.Length != 2)
            {
                dataURLWithoutData = null;
                output = null;
                return false;
            }

            byte[] fileData;
            try
            {
                fileData = Convert.FromBase64String(parts[1]);
            }
            catch
            {
                dataURLWithoutData = null;
                output = null;
                return false;
            }

            dataURLWithoutData = parts[0];
            output = new SSA2SRTConverterData(file.Name, fileData);
            return true;
        }
    }
}