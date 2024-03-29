﻿using System;
using System.Runtime.CompilerServices;

using Lavinia.Api;
using Lavinia.Api.Data;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("ApiTests")]

IWebHost host = WebHost.CreateDefaultBuilder(args)
    .UseStartup<Startup>()
    .Build();
using (IServiceScope scope = host.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        NOContext noContext = services.GetRequiredService<NOContext>();
        NOInitializer.Initialize(noContext, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the contexts");
    }
}

await host.RunAsync();