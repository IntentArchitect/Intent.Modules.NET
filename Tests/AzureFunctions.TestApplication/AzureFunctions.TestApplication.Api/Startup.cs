using AzureFunctions.TestApplication.Application;
using AzureFunctions.TestApplication.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]

[assembly: FunctionsStartup(typeof(AzureFunctions.TestApplication.Api.Startup))]
[assembly: IntentTemplate("Intent.AzureFunctions.Startup", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(configuration);
        }
    }
}