using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Core;
using EfCoreTestSuite.CosmosDb.IntentGenerated.DependencyInjection;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace EfCoreTestSuite.CosmosDb.IntegrationTests;

[Collection(CollectionFixture.CollectionDefinitionName)]
public class InheritanceTPCTests
{
    private readonly DataContainerFixture _fixture;

    public InheritanceTPCTests(DataContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPC_DerivedAssociated()
    {
        var associated = new Associated();
        associated.PartitionKey = "ABC";
        associated.AssociatedField1 = "Associated Value";
        _fixture.DbContext.Associateds.Add(associated);
        
        var derived = new Derived();
        derived.DerivedField1 = "Derived Value";
        derived.BaseField1 = "Derived Value";
        derived.PartitionKey = "ABC";
        derived.Associated = associated;
        _fixture.DbContext.Deriveds.Add(derived);

        _fixture.DbContext.SaveChanges();
        
        var retrievedDerived = _fixture.DbContext.Deriveds.Single(p => p.Id == derived.Id);
        Assert.Equal(derived, retrievedDerived);
        Assert.Equal(associated, retrievedDerived.Associated);
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPC_InheritFromOwnedClass()
    {
        var composite = new Composite();
        composite.CompositeField1 = "Composite Value";

        var derived = new Derived();
        derived.DerivedField1 = "Derived Value";
        derived.BaseField1 = "Derived Value";
        derived.PartitionKey = "ABC";
        derived.Composites.Add(composite);
        _fixture.DbContext.Deriveds.Add(derived);

        var weirdClass = new WeirdClass();
        weirdClass.WeirdField = "Weird Class Value";
        weirdClass.CompositeField1 = "Weird Class Value";
        weirdClass.PartitionKey = "ABC";
        _fixture.DbContext.WeirdClasses.Add(weirdClass);
        
        _fixture.DbContext.SaveChanges();

        var retrievedDerived = _fixture.DbContext.Deriveds.Single(p => p.Id == derived.Id);
        Assert.Equal(derived, retrievedDerived);
        Assert.Equal(composite, retrievedDerived.Composites.Single(p=>p.Id == composite.Id));

        // Until something changes, I'm keeping track of this limitation by attempting to assign
        // this WeirdClass to Derived and expecting an error.
        Assert.Throws<InvalidOperationException>(() =>
        {
            derived.Composites.Add(weirdClass);

            _fixture.DbContext.SaveChanges();

            // And once this works somehow, we can add these checks and remove the "Throws" check.
            //var test2 = DbContext.A_OwnerClasses.Single(p => p.Id == ownerClass.Id)?.OwnedClasses;
            //var retrievedDerived2 = _fixture.DbContext.Deriveds.Single(p => p.Id == derived.Id);
            //Assert.Equal(weirdClass, retrievedDerived.Composites.Single(p => p.Id == weirdClass.Id));
        });
    }
}