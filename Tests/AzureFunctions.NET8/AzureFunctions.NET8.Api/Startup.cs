using AzureFunctions.NET6.Api.Configuration;
using AzureFunctions.NET6.Application;
using AzureFunctions.NET6.Infrastructure;
using AzureFunctions.NET8.Api.Configuration;
using AzureFunctions.NET8.Application;
using AzureFunctions.NET8.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.Startup", Version = "1.0")]

[assembly: FunctionsStartup(typeof(AzureFunctions.NET6.Api.Startup))]

namespace AzureFunctions.NET8.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            builder.Services.AddApplication(configuration);
            builder.Services.ConfigureApplicationSecurity(configuration);
            builder.Services.AddInfrastructure(configuration);
        }
    }
}