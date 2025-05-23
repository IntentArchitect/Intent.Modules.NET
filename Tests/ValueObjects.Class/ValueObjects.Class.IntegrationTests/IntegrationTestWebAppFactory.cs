using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using ValueObjects.Class.Api;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.IntegrationTestWebAppFactory", Version = "1.0")]

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ValueObjects.Class.IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public IntegrationTestWebAppFactory()
        {
            RdbmsFixture = new EFContainerFixture();
        }

        public EFContainerFixture RdbmsFixture { get; }

        public async ValueTask InitializeAsync()
        {
            await RdbmsFixture.InitializeAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            await RdbmsFixture.DisposeAsync();
            await base.DisposeAsync();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var result = base.CreateHost(builder);
            RdbmsFixture.OnHostCreation(result.Services);
            return result;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                RdbmsFixture.ConfigureTestServices(services);
            });
        }
    }
}