using Amazon.S3;
using CloudBlobStorageClients.Application.Common.Storage;
using CloudBlobStorageClients.Infrastructure.BlobStorage;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AmazonS3.ObjectStorage.AmazonS3ObjectStorageConfiguration", Version = "1.0")]

namespace CloudBlobStorageClients.Infrastructure.Configuration
{
    public static class AmazonS3ObjectStorageConfiguration
    {
        public static IServiceCollection AddAmazonS3ObjectStorage(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions<AmazonS3Config>());
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IObjectStorage, AmazonS3ObjectStorage>();

            return services;
        }
    }
}