using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Application.Common.Storage;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.CloudStorage;
using Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Google.CloudStorage.GoogleCloudStorageConfiguration", Version = "1.0")]

namespace Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.Configuration
{
    public static class GoogleCloudStorageConfiguration
    {
        public static IServiceCollection AddGoogleCloudStorage(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<ICloudStorage, GoogleCloudStorage>();
            services.AddSingleton<GoogleCloudStorageMultiTenantConnectionFactory>();
            services.AddScoped(
                sp =>
                {
                    var tenantConnections = sp.GetService<ITenantConnections>();

                    if (tenantConnections is null || tenantConnections.Id is null || tenantConnections.GoogleCloudStorageConnection is null)
                    {
                        throw new Finbuckle.MultiTenant.MultiTenantException("Failed to resolve tenant MongoDb connection information");
                    }
                    var factory = sp.GetRequiredService<GoogleCloudStorageMultiTenantConnectionFactory>();
                    return factory.GetStorageClient(tenantConnections.Id, tenantConnections.GoogleCloudStorageConnection);
                });

            return services;
        }
    }
}