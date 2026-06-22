using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wolverine.AwsLambdaFunctions.Application;
using Wolverine.AwsLambdaFunctions.Infrastructure;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Lambda.Functions.StartupTemplate", Version = "1.0")]

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Wolverine.AwsLambdaFunctions.Api
{
    [LambdaStartup]
    public class Startup
    {
        public HostApplicationBuilder ConfigureHostBuilder()
        {
            var hostBuilder = new HostApplicationBuilder();
            var configuration = hostBuilder.Configuration
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            hostBuilder.Services.AddApplication(configuration);
            hostBuilder.Services.AddInfrastructure(configuration);
            hostBuilder.Logging.AddLambdaLogger();
            return hostBuilder;
        }
    }
}