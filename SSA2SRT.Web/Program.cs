/*
 * SSA2SRT Converter service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SSA2SRT.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
