using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;

[assembly: FunctionsStartup(typeof(FunctionsDemo.Startup))]
namespace FunctionsDemo
{
    public class Startup : FunctionsStartup
    {
        public static IConfiguration _configuration = null;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddHttpClient();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();
            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            //base.ConfigureAppConfiguration(builder);
        }
    }
}