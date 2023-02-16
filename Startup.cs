using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


//Note: this assembly annotation is needed to provide an overloaded entrypoint for the Function host and give us a place to wire dependency injection and configuration
[assembly: FunctionsStartup(typeof(FunctionsDemo.Startup))]
namespace FunctionsDemo
{
    public class Startup : FunctionsStartup
    {
        public static IConfiguration _configuration = null;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            //builder.Services.Add();
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