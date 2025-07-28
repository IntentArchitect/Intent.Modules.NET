using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Common.AwsConfiguration", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Configuration
{
    public static class AwsConfiguration
    {
        public static IServiceCollection ConfigureAws(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());

            if (configuration.GetValue<bool>("AWS:DynamoDB:UseLocalEmulator"))
            {
                services.AddSingleton<IAmazonDynamoDB>(_ =>
                {
                    var clientConfig = new AmazonDynamoDBConfig
                    {
                        ServiceURL = "http://localhost:8000",
                        UseHttp = true
                    };

                    return new AmazonDynamoDBClient("na", "na", clientConfig);
                });
            }
            else
            {
                services.AddAWSService<IAmazonDynamoDB>();
            }

            services.AddSingleton<IDynamoDBContext>(sp =>
            {
                var client = sp.GetRequiredService<IAmazonDynamoDB>();
                return new DynamoDBContextBuilder().WithDynamoDBClient(() => client).Build();
            });

            return services;
        }
    }
}