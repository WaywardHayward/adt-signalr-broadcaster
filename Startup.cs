using adt_signalr_broadcaster;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace adt_signalr_broadcaster
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.AddJsonFile("local.settings.json", true);
            builder.ConfigurationBuilder.AddEnvironmentVariables();
        }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();         
        }
    }
}