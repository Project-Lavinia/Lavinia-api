using System;
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

    string? staticExportPath = TryGetArgumentValue(args, "--export-static");
    if (!string.IsNullOrWhiteSpace(staticExportPath))
    {
        try
        {
            NOContext noContext = services.GetRequiredService<NOContext>();
            await StaticDataExporter.ExportAsync(noContext, staticExportPath);
            logger.LogInformation("Static data export completed. Output directory: {OutputDirectory}", staticExportPath);
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Static data export failed for path {OutputDirectory}", staticExportPath);
            throw;
        }
    }
}

await host.RunAsync();

static string? TryGetArgumentValue(string[] args, string optionName)
{
    int optionIndex = Array.IndexOf(args, optionName);
    if (optionIndex < 0)
    {
        return null;
    }

    int optionValueIndex = optionIndex + 1;
    if (optionValueIndex >= args.Length)
    {
        throw new ArgumentException($"The option {optionName} requires a value.");
    }

    return args[optionValueIndex];
}