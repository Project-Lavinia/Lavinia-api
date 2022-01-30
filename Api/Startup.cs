﻿using AspNetCoreRateLimit;
using Lavinia.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace Lavinia.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _env = environment;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            // Swagger generation with default settings
            services.AddSwaggerGen(options =>
            {
                string xmlDocFile = Path.Combine(AppContext.BaseDirectory, $"{_env.ApplicationName}.xml");
                if (File.Exists(xmlDocFile))
                {
                    options.IncludeXmlComments(xmlDocFile);
                }
                options.SwaggerDoc("v3", new OpenApiInfo
                {
                    Title = "API v3.0.0 for election result data",
                    Version = "v3",
                    Description = "This API provides the back-end for calculating seats and data for the Lavinia project."
                });
            });

            services.AddControllers(options =>
            {
                options.EnableEndpointRouting = false;
                options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
            });
            SetUpDatabase(services);
            services.AddRateLimiting(Configuration);
            services.AddApplicationInsightsTelemetry();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseRouting();
            app.UseSwagger(options => { });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v3/swagger.json", "Lavinia API v3");
                options.EnableFilter();
                options.RoutePrefix = string.Empty;
                options.DocumentTitle = "Lavinia API - Swagger";
            });
            app.UseIpRateLimiting();

            app.UseEndpoints(configure =>
            {
                configure.MapControllers();
            });
        }

        private static void SetUpDatabase(IServiceCollection services)
        {
            services.AddDbContext<NOContext>(options => options.UseInMemoryDatabase("NODatabase"));
        }
    }
}