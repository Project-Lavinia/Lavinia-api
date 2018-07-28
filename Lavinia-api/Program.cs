using System;
using LaviniaApi.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LaviniaApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost host = BuildWebHost(args);
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                try
                {
                    ElectionContext context = services.GetRequiredService<ElectionContext>();
                    ILogger logger = services.GetRequiredService<ILogger<Program>>();
                    logger.Log(LogLevel.Information, "Called 1!");
                    ElectionInitializer.Initialize(context, logger);
                }
                catch (Exception ex)
                {
                    ILogger logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding ElectionContext");
                }
            }

            host.Run();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        }
    }
}