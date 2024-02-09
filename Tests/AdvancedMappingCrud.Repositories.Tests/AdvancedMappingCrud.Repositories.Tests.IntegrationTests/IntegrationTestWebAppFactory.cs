using AdvancedMappingCrud.Repositories.Tests.Api;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.IntegrationTestWebAppFactory", Version = "1.0")]

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public IntegrationTestWebAppFactory()
        {
        }

        public async Task InitializeAsync()
        {
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var result = base.CreateHost(builder);
            return result;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
            });
        }
    }
}