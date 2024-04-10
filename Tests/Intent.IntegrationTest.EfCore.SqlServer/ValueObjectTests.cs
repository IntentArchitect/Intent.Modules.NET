using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.ValueObjects;
using EntityFrameworkCore.SqlServer.EF8.Domain.ValueObjects;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using FluentAssertions;
using Intent.IntegrationTest.EfCore.SqlServer.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.CosmosDb;

public class ValueObjectTests:SharedDatabaseFixture<ApplicationDbContext, ValueObjectTests>
{
    public ValueObjectTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_PersonWithAddressNormal()
    {
        var person = new PersonWithAddressNormal();
        person.Name = "John";
        person.AddressNormal = new AddressNormal("221B", "Baker street", "London");
        DbContext.PersonWithAddressNormals.Add(person);
        await DbContext.SaveChangesAsync();

        var retrieved = DbContext.PersonWithAddressNormals.FirstOrDefault(p => p.Id == person.Id);
        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(person);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_PersonWithAddressSerialized()
    {
        var person = new PersonWithAddressSerialized();
        person.Name = "John";
        person.AddressSerialized = new AddressSerialized("221B", "Baker street", "London");
        DbContext.PersonWithAddressSerializeds.Add(person);
        await DbContext.SaveChangesAsync();

        var retrieved = DbContext.PersonWithAddressSerializeds.FirstOrDefault(p => p.Id == person.Id);
        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(person);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_DictionaryWithKvPNormal()
    {
        var dict = new DictionaryWithKvPNormal();
        dict.Title = "Dict1";
        dict.KeyValuePairNormals.Add(new KeyValuePairNormal("key1", "value1"));
        dict.KeyValuePairNormals.Add(new KeyValuePairNormal("key2", "value2"));
        DbContext.DictionaryWithKvPNormals.Add(dict);
        await DbContext.SaveChangesAsync();

        var retrieved = DbContext.DictionaryWithKvPNormals.FirstOrDefault(p => p.Id == dict.Id);
        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(dict);
    }
    
    [IgnoreOnCiBuildFact]
    public async Task Test_DictionaryWithKvPSerialized()
    {
        var dict = new DictionaryWithKvPSerialized();
        dict.Title = "Dict2";
        dict.KeyValuePairSerializeds.Add(new KeyValuePairSerialized("key1", "value1"));
        dict.KeyValuePairSerializeds.Add(new KeyValuePairSerialized("key2", "value2"));
        DbContext.DictionaryWithKvPSerializeds.Add(dict);
        await DbContext.SaveChangesAsync();

        var retrieved = DbContext.DictionaryWithKvPSerializeds.FirstOrDefault(p => p.Id == dict.Id);
        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(dict);
    }
}