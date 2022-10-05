using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Core;
using EfCoreTestSuite.CosmosDb.IntentGenerated.DependencyInjection;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic;
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
        
        //This causes trouble when being run with other tests. Keep it commented out until EF Core can support these inheritance scenarios.
        // var weirdClass = new WeirdClass();
        // weirdClass.WeirdField = "Weird Class Value";
        // weirdClass.CompositeField1 = "Weird Class Value";
        // weirdClass.PartitionKey = "ABC";
        // _fixture.DbContext.WeirdClasses.Add(weirdClass);
        
        _fixture.DbContext.SaveChanges();

        var retrievedDerived = _fixture.DbContext.Deriveds.Single(p => p.Id == derived.Id);
        Assert.Equal(derived, retrievedDerived);
        Assert.Equal(composite, retrievedDerived.Composites.Single(p=>p.Id == composite.Id));

        // Until something changes, I'm keeping track of this limitation by attempting to assign
        // this WeirdClass to Derived and expecting an error.
        //Assert.Throws<InvalidOperationException>(() =>
        //{
            //derived.Composites.Add(weirdClass);

            //_fixture.DbContext.SaveChanges();

            // And once this works somehow, we can add these checks and remove the "Throws" check.
            //var test2 = DbContext.A_OwnerClasses.Single(p => p.Id == ownerClass.Id)?.OwnedClasses;
            //var retrievedDerived2 = _fixture.DbContext.Deriveds.Single(p => p.Id == derived.Id);
            //Assert.Equal(weirdClass, retrievedDerived.Composites.Single(p => p.Id == weirdClass.Id));
        //});
    }

    // EF for Cosmos doesn't support any form of polymorphism at this point.
    // I've attempted an abstract class, a normal class for inheritance.
    // Also the relationship has to be of a composite type. I'm keeping things here for future reference.
    
    // [IgnoreOnCiBuildFact]
    // public void Test_Inheritance_TPC_Polymorphic()
    // {
    //     var secondLevel = new Poly_SecondLevel();
    //     secondLevel.PartitionKey = "ABC";
    //     secondLevel.SecondField = "Second Level Value";
    //     _fixture.DbContext.Poly_SecondLevels.Add(secondLevel);
    //     
    //     var baseClass = new Poly_BaseClassNonAbstract();
    //     baseClass.BaseField = "Base Class Non Abstract Value";
    //     secondLevel.BaseClassNonAbstracts = baseClass;
    //     _fixture.DbContext.SaveChanges();
    //     
    //     var concreteA = new Poly_ConcreteA();
    //     concreteA.PartitionKey = "ABC";
    //     concreteA.ConcreteField = "Concrete Value";
    //     _fixture.DbContext.Poly_ConcreteAs.Add(concreteA);
    //     secondLevel.BaseClassNonAbstracts = concreteA;
    //     _fixture.DbContext.SaveChanges();
    //     
    //     var concreteB = new Poly_ConcreteB();
    //     concreteB.PartitionKey = "ABC";
    //     concreteB.ConcreteField = "Concrete Value";
    //     _fixture.DbContext.Poly_ConcreteBs.Add(concreteB);
    //     secondLevel.BaseClassNonAbstracts = concreteB;
    //     _fixture.DbContext.SaveChanges();
    //     
    //     _fixture.DbContext.SaveChanges();
    // }
}