/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SSA2SRT.Web
{
    public static class SSA2SRTExtensions
    {
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
                        context.Response.Redirect("/SSA2SRT/");
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