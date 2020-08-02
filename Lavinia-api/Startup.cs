using System;
using System.IO;
using Lavinia_api;
using LaviniaApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace LaviniaApi
{
    public class Startup
    {
        private IWebHostEnvironment _env;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static void ConfigureMVC(MvcOptions options)
        {
            options.EnableEndpointRouting = false;
            options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
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

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API for election result data",
                    Version = "v1",
                    Description =
                        "This API provides the back-end for calculating seats and data for the Mandater project."
                });
                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "API v2.0.0 for election result data",
                    Version = "v2",
                    Description = "This API provides the back-end for calculating seats and data for the Lavinia project."
                });
                options.SwaggerDoc("v3", new OpenApiInfo
                {
                    Title = "API v3.0.0 for election result data",
                    Version = "v3",
                    Description = "This API provides the back-end for calculating seats and data for the Lavinia project."
                });
            });
            services.AddMvc(c => ConfigureMVC(c)).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            SetUpDatabase(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _env = env;
            app.UseSwagger(options => {});
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v3/swagger.json", "Lavinia API v3");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "Lavinia API v2");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Lavinia API v1");
                options.EnableFilter();
                options.RoutePrefix = String.Empty;
                options.DocumentTitle = "Lavinia API - Swagger";
            });
            app.UseStaticFiles();

            app.UseMvc();
        }

        private static void SetUpDatabase(IServiceCollection services)
        {
            services.AddDbContext<ElectionContext>(options => options.UseInMemoryDatabase("ModelDB"));
            services.AddDbContext<NOContext>(options => options.UseInMemoryDatabase("NODatabase"));
        }
    }
}