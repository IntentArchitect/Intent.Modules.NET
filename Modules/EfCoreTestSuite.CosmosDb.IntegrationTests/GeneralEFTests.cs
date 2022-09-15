using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Core;
using EfCoreTestSuite.CosmosDb.IntentGenerated.DependencyInjection;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace EfCoreTestSuite.CosmosDb.IntegrationTests;

[Collection(CollectionFixture.CollectionDefinitionName)]
public class GeneralEFTests
{
    private readonly DataContainerFixture _dataContainerFixture;

    public GeneralEFTests(DataContainerFixture dataContainerFixture)
    {
        _dataContainerFixture = dataContainerFixture;
    }

    [IgnoreOnCiBuildFact]
    public void TestInitialSetup()
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseCosmos(_dataContainerFixture.ConnectionString,
            "TestDb",
            opt => opt.HttpClientFactory(() =>
            {
                HttpMessageHandler httpMessageHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                return new HttpClient(httpMessageHandler)
                {
                    BaseAddress = new Uri($"https://localhost:{_dataContainerFixture.GetMappedPort()}/")
                };
            })
                .ConnectionMode(ConnectionMode.Gateway)
        );
        var appDb = new ApplicationDbContext(
            builder.Options,
            new OptionsWrapper<DbContextConfiguration>(new DbContextConfiguration()));
        appDb.Database.EnsureCreated();
    }
}