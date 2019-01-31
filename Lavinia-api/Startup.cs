using System;
using System.IO;
using LaviniaApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace LaviniaApi
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            // Swagger generation with default settings
            services.AddSwaggerGen(options =>
            {
                string xmlDocFile = Path.Combine(AppContext.BaseDirectory, $"{_env.ApplicationName}.xml");
                if (File.Exists(xmlDocFile))
                {
                    options.IncludeXmlComments(xmlDocFile);
                }

                options.SwaggerDoc("v1.0.0", new Info
                {
                    Title = "API for election result data",
                    Version = "v1.0.0",
                    Description =
                        "This API provides the back-end for calculating seats and data for the Mandater project."
                });
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            SetUpDatabase(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            _env = env;
            app.UseSwagger(options => { });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1.0.0/swagger.json", "API for election result data");
                options.RoutePrefix = String.Empty;
            });
            app.UseStaticFiles();

            app.UseMvc();
        }

        private static void SetUpDatabase(IServiceCollection services)
        {
            services.AddDbContext<ElectionContext>(options => options.UseInMemoryDatabase("ModelDB"));
            services.AddDbContext<NOContext>(options => options.UseInMemoryDatabase("ModelDB"));
        }
    }
}