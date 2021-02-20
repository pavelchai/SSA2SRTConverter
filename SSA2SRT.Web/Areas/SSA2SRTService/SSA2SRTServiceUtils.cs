/*
 * SSA2SRT Converter service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SSA2SRTService.Models
{
    public static class SSA2SRTServiceUtils
    {
        public static void AddSSA2SRTConverterServiceSwaggerDoc(this SwaggerGenOptions options)
        {
            options.SwaggerDoc("SSA2SRTConverterService", new OpenApiInfo
            {
                Title = "SSA2SRT Converter Service",
                Version = "v1",
                Description = "The service converts the SubStationAlpha subtitles (*.ssa, *.ass) to the SubRip subtitles (*.srt)",
                Contact = new OpenApiContact { Name = "Pavel Chaimardanov", Email = "pchai@yandex.ru"},
                License = new OpenApiLicense { Name = "Use under MIT", Url = new Uri("https://mit-license.org/") }
            });

            var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            var xmlFile = Path.GetFileName(assemblyName + ".xml");

            options.IncludeXmlComments(xmlFile);
        }

        public static void AddSSA2SRTConverterServiceSwaggerEndpoint(this SwaggerUIOptions options)
        {
            options.SwaggerEndpoint("/swagger/SSA2SRTConverterService/swagger.json", "SSA2SRTConverterService");
        }

        public static void AddSSA2SRTConverterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
        }

        public static void UseSSA2SRTConverter(this IApplicationBuilder app, bool redirectToPage = false)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (redirectToPage)
            {
                app.Use((context, task) => {
                    var next = task();

                    if (context.Request.Path == "/")
                    {
                        context.Response.Redirect("/SSA2SRTService/");
                    }

                    return next;
                });
            }
        }

        public static void AddSSA2SRTConverterEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapAreaControllerRoute(
                    name: "SSA2SRT",
                    areaName: "SSA2SRT",
                    pattern: "{area}/{controller}/{action}/{id?}");
        }
    }
}