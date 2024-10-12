using CloudBlobStorageClients.Application.Common.Storage;
using CloudBlobStorageClients.Infrastructure.CloudStorage;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Google.CloudStorage.GoogleCloudStorageConfiguration", Version = "1.0")]

namespace CloudBlobStorageClients.Infrastructure.Configuration
{
    public static class GoogleCloudStorageConfiguration
    {
        public static IServiceCollection AddGoogleCloudStorage(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<ICloudStorage, GoogleCloudStorage>();
            services.AddSingleton(
                sp =>
                {
                    var credentialFileLocation = sp.GetRequiredService<IConfiguration>().GetValue<string>("GCP:CloudStorageAuthFileLocation");
                    var _googleCredential = GoogleCredential.FromFile(credentialFileLocation);
                    return StorageClient.Create(_googleCredential);
                });

            return services;
        }
    }
}