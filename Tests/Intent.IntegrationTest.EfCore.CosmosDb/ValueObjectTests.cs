using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.ValueObjects;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.ValueObjects;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence;
using FluentAssertions;
using Intent.IntegrationTest.EfCore.CosmosDb.Helpers;
using Xunit;

namespace Intent.IntegrationTest.EfCore.CosmosDb;

[Collection(CollectionFixture.CollectionDefinitionName)]
public class ValueObjectTests
{
    private readonly DataContainerFixture _fixture;

    public ValueObjectTests(DataContainerFixture fixture)
    {
        _fixture = fixture;
    }
    
    private ApplicationDbContext DbContext => _fixture.DbContext;

    [IgnoreOnCiBuildFact]
    public async Task Test_PersonWithAddressNormal()
    {
        var person = new PersonWithAddressNormal();
        person.Name = "John";
        person.PartitionKey = "AAA";
        person.AddressNormal = new AddressNormal("221B", "Baker street", "London");
        DbContext.PersonWithAddressNormals.Add(person);
        await DbContext.SaveChangesAsync();

        var retrieved = DbContext.PersonWithAddressNormals.FirstOrDefault(p => p.Id == person.Id && p.PartitionKey == person.PartitionKey);
        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(person);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_PersonWithAddressSerialized()
    {
        var person = new PersonWithAddressSerialized();
        person.Name = "John";
        person.PartitionKey = "AAA";
        person.AddressSerialized = new AddressSerialized("221B", "Baker street", "London");
        DbContext.PersonWithAddressSerializeds.Add(person);
        await DbContext.SaveChangesAsync();

        var retrieved = DbContext.PersonWithAddressSerializeds.FirstOrDefault(p => p.Id == person.Id && p.PartitionKey == person.PartitionKey);
        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(person);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_DictionaryWithKvPNormal()
    {
        var dict = new DictionaryWithKvPNormal();
        dict.PartitionKey = "AAA";
        dict.Title = "Dict1";
        dict.KeyValuePairNormals.Add(new KeyValuePairNormal("key1", "value1"));
        dict.KeyValuePairNormals.Add(new KeyValuePairNormal("key2", "value2"));
        DbContext.DictionaryWithKvPNormals.Add(dict);
        await DbContext.SaveChangesAsync();

        var retrieved = DbContext.DictionaryWithKvPNormals.FirstOrDefault(p => p.Id == dict.Id && p.PartitionKey == dict.PartitionKey);
        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(dict);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_DictionaryWithKvPSerialized()
    {
        var dict = new DictionaryWithKvPSerialized();
        dict.PartitionKey = "AAA";
        dict.Title = "Dict2";
        dict.KeyValuePairSerializeds.Add(new KeyValuePairSerialized("key1", "value1"));
        dict.KeyValuePairSerializeds.Add(new KeyValuePairSerialized("key2", "value2"));
        DbContext.DictionaryWithKvPSerializeds.Add(dict);
        await DbContext.SaveChangesAsync();

        var retrieved = DbContext.DictionaryWithKvPSerializeds.FirstOrDefault(p => p.Id == dict.Id && p.PartitionKey == dict.PartitionKey);
        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(dict);
    }
}