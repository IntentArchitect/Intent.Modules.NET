using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Common.AwsConfiguration", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupB.Infrastructure.Configuration
{
    public static class AwsConfiguration
    {
        public static IServiceCollection ConfigureAws(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());

            return services;
        }
    }
}