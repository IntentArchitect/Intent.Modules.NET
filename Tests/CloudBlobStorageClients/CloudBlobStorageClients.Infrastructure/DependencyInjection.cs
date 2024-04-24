using Amazon.S3;
using CloudBlobStorageClients.Application.Common.Storage;
using CloudBlobStorageClients.Infrastructure.BlobStorage;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CloudBlobStorageClients.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IBlobStorage, AzureBlobStorage>();
            
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IObjectStorage, AwsS3BlobStorage>();
            
            return services;
        }
    }
}