/*
 * SSA2SRT Converter service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SSA2SRTService.Models;

namespace SSA2SRT.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSSA2SRTConverterServices(this.Configuration);

            services.AddSwaggerGen(c =>
            {
                c.AddSSA2SRTConverterServiceSwaggerDoc();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.AddSSA2SRTConverterServiceSwaggerEndpoint();
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseSSA2SRTConverter(true);

            app.UseEndpoints(endpoints =>
            {
                endpoints.AddSSA2SRTConverterEndpoint();
            });
        }
    }
}