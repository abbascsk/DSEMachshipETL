using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using DSEMachshipETL.AutoMapper;
using DSEMachshipETL.Data;
using DSEMachshipETL.Models;
using DSEMachshipETL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        
        services.Configure<GeneralSettings>(configuration.GetSection("GeneralSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        services.AddDbContext<DseDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DseConnection"));
        });

        services.AddAutoMapper(typeof(Program).Assembly);
        services.AddSingleton<Profile, MappingProfile>();
        services.AddScoped<EmailService>();
        services.AddScoped<Logger>();
        services.AddScoped<MachshipXmlEtlService>();
    });

using var host = builder.Build();
await RunApplicationAsync(host.Services);

async Task RunApplicationAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var services = scope.ServiceProvider;
    
    var dseXmlService = services.GetRequiredService<MachshipXmlEtlService>();
    var logger = services.GetRequiredService<Logger>();

    try
    {
        Console.WriteLine("Starting DSE Machship Consignment ETL Service...");
        await dseXmlService.GetNewConsignments();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        logger.LogError(ex.InnerException?.Message ?? ex.Message);
    }
}