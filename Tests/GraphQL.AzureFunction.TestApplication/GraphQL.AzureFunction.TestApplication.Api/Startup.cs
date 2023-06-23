using GraphQL.AzureFunction.TestApplication.Api.Configuration;
using GraphQL.AzureFunction.TestApplication.Application;
using GraphQL.AzureFunction.TestApplication.Infrastructure;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]

[assembly: FunctionsStartup(typeof(GraphQL.AzureFunction.TestApplication.Api.Startup))]
[assembly: IntentTemplate("Intent.AzureFunctions.Startup", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            builder.Services.AddApplication();
            builder.Services.ConfigureApplicationSecurity(configuration);
            builder.Services.AddInfrastructure(configuration);
            builder.Services.ConfigureGraphQL();
        }
    }
}