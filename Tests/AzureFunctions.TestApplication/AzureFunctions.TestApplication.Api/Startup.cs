using AzureFunctions.TestApplication.Api.Configuration;
using AzureFunctions.TestApplication.Application;
using AzureFunctions.TestApplication.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.Startup", Version = "1.0")]

[assembly: FunctionsStartup(typeof(AzureFunctions.TestApplication.Api.Startup))]

namespace AzureFunctions.TestApplication.Api
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