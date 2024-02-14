using System.Reflection;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testcontainers.CosmosDb;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.CosmosContainerFixture", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests
{
    public class CosmosContainerFixture
    {
        private readonly string _accountEndpoint = "https://localhost:{0}/";
        private readonly string _accountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private readonly CosmosDbContainer _dbContainer;
        private int _portNumber = 0;
        private CosmosClient? _cosmosClient;
        private Type? _cosmosProviderType;

        public CosmosContainerFixture()
        {
            _dbContainer = new CosmosDbBuilder()
                          .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator")
                          .WithName("azure-cosmos-emulator")
                          .WithExposedPort(8081)
                            .WithExposedPort(10251)
                            .WithExposedPort(10252)
                            .WithExposedPort(10253)
                            .WithExposedPort(10254)
                          .WithPortBinding(8081, true)
                          .WithPortBinding(10251, true)
                          .WithPortBinding(10252, true)
                          .WithPortBinding(10253, true)
                          .WithPortBinding(10254, true)
                          .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "1")
                          .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE", "127.0.0.1")
                          .WithEnvironment("AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE", "false")
                          .WithWaitStrategy(Wait.ForUnixContainer()
                            .UntilPortIsAvailable(8081))
                          .Build();
        }

        public void ConfigureTestServices(IServiceCollection services)
        {
            var updatedEndpoint = string.Format(_accountEndpoint, _portNumber);

            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IOptions<RepositoryOptions>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<IOptions<RepositoryOptions>>(new OptionsMock(new RepositoryOptions() { CosmosConnectionString = $"AccountEndpoint={updatedEndpoint};AccountKey={_accountKey}", DatabaseId = "TestDb", ContainerId = "Container" }));

            var cosmosClientBuilder = new CosmosClientBuilder(updatedEndpoint, _accountKey);
            cosmosClientBuilder.WithHttpClientFactory(() =>
            {
                HttpMessageHandler httpMessageHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                return new HttpClient(new FixRequestLocationHandler(_portNumber, httpMessageHandler));
            });
            cosmosClientBuilder.WithConnectionModeGateway();
            cosmosClientBuilder.WithSerializerOptions(new CosmosSerializationOptions
            {
                IgnoreNullValues = false,
                Indented = false,
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            });

            _cosmosClient = cosmosClientBuilder.Build();
            _cosmosProviderType = services.Single(s => s.ServiceType.Name == "ICosmosClientProvider").ServiceType;
        }

        public void OnHostCreation(IServiceProvider services)
        {
            SwapCosmosClientToTestVersion(services);
        }

        private void SwapCosmosClientToTestVersion(IServiceProvider services)
        {
            var cosmosClientProvider = services.GetRequiredService(_cosmosProviderType!);
            var privateField = cosmosClientProvider.GetType().GetField("_lazyCosmosClient", BindingFlags.NonPublic | BindingFlags.Instance);
            if (privateField is null) throw new Exception("Unable to inject testing CosmosClient. Can't find _lazyCosmosClient");
            privateField.SetValue(cosmosClientProvider, new Lazy<CosmosClient>(_cosmosClient!));
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            _portNumber = _dbContainer.GetMappedPublicPort(8081);
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
        }
    }

    public class OptionsMock : IOptions<RepositoryOptions>
    {
        private readonly RepositoryOptions _options;

        public OptionsMock(RepositoryOptions options)
        {
            _options = options;
        }

        public RepositoryOptions Value => _options;
    }

    public class FixRequestLocationHandler : DelegatingHandler
    {
        private readonly int _portNumber;

        public FixRequestLocationHandler(int portNumber, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _portNumber = portNumber;
        }

        /// <summary>
        /// Override of the SendAsync method to allow for reconstruction of the request uri to point to the dynamic testcontainer
        /// port number. This needs to be done as otherwise it defaults back to 8081. I assume there is some hard coded port in
        /// the emulator somewhere. If this is not done then the requests are not successful.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.RequestUri = new Uri($"https://localhost:{_portNumber}{request.RequestUri.PathAndQuery}");
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}