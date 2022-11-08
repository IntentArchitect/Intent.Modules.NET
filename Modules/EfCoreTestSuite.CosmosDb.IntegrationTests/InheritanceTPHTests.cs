using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Core;
using EfCoreTestSuite.CosmosDb.IntentGenerated.DependencyInjection;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace EfCoreTestSuite.CosmosDb.IntegrationTests;

[Collection(CollectionFixture.CollectionDefinitionName)]
public class InheritanceTPHTests
{
    private readonly DataContainerFixture _fixture;

    public InheritanceTPHTests(DataContainerFixture fixture, ITestOutputHelper outputHelper)
    {
        _fixture = fixture;
        fixture.OutputHelper = outputHelper;
    }

    private ApplicationDbContext DbContext => _fixture.DbContext;

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPH_AbstractBaseClass()
    {
        var derived = new DerivedClassForAbstract
        {
            Id = Guid.NewGuid(),
            PartitionKey = "ABC",
            BaseAttribute = "Base Value",
            DerivedAttribute = "Base Value"
        };
        DbContext.DerivedClassForAbstracts.Add(derived);

        var derivedAssociated = new DerivedClassForAbstractAssociated
        {
            PartitionKey = "ABC",
            AssociatedField = "Derived Associated Value",
            DerivedClassForAbstract = derived
        };
        DbContext.DerivedClassForAbstractAssociateds.Add(derivedAssociated);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.DerivedClassForAbstracts.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.DerivedClassForAbstractAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForAbstract);
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPH_ConcreteBaseClass()
    {
        var concreteBase = new ConcreteBaseClass
        {
            Id = Guid.NewGuid(),
            PartitionKey = "ABC",
            BaseAttribute = "Base Value"
        };
        DbContext.ConcreteBaseClasses.Add(concreteBase);

        var concreteBaseAssociated = new ConcreteBaseClassAssociated
        {
            PartitionKey = "ABC",
            AssociatedField = "Associated Value",
            ConcreteBaseClass = concreteBase
        };
        DbContext.ConcreteBaseClassAssociateds.Add(concreteBaseAssociated);

        DbContext.SaveChanges();

        Assert.Equal((object)concreteBase, (object)DbContext.ConcreteBaseClasses.SingleOrDefault(p => p.Id == concreteBase.Id));
        Assert.Equal(concreteBase, DbContext.ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociated.Id).ConcreteBaseClass);

        var derived = new DerivedClassForConcrete
        {
            Id = Guid.NewGuid(),
            PartitionKey = "ABC",
            BaseAttribute = "Derived Value",
            DerivedAttribute = "Derived Value"
        };
        DbContext.DerivedClassForConcretes.Add(derived);

        var derivedAssociated = new DerivedClassForConcreteAssociated
        {
            PartitionKey = "ABC",
            Associatedfield = "Associated Value",
            DerivedClassForConcrete = derived
        };
        DbContext.DerivedClassForConcreteAssociateds.Add(derivedAssociated);

        var concreteBaseAssociatedForDerived = new ConcreteBaseClassAssociated
        {
            PartitionKey = "ABC",
            AssociatedField = "Associated Value",
            ConcreteBaseClass = derived
        };
        DbContext.ConcreteBaseClassAssociateds.Add(concreteBaseAssociatedForDerived);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.DerivedClassForConcretes.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.DerivedClassForConcreteAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForConcrete);
        Assert.Equal(derived, DbContext.ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociatedForDerived.Id).ConcreteBaseClass);
    }
    
    //[IgnoreOnCiBuildFact]
    //public void Test_Inheritance_TPH_Polymorphic()
    //{
    //    var topLevelConcretes = CreateNewPolyClasses();
    //    var topLevel = new Poly_TopLevel();
    //    topLevel.TopField = "Top Level Value";
    //    topLevel.PartitionKey = "ABC";
    //    topLevel.Poly_RootAbstracts.Add(topLevelConcretes.Item2);
    //    topLevel.Poly_RootAbstracts.Add(topLevelConcretes.Item3);
    //    topLevel.Poly_RootAbstracts.Add(topLevelConcretes.Item4);
    //    DbContext.Poly_TopLevels.Add(topLevel);

    //    var secondLevelConcretes = CreateNewPolyClasses();
    //    var secondLevel = new Poly_SecondLevel();
    //    secondLevel.SecondField = "Second Level Value";
    //    secondLevel.PartitionKey = "ABC";
    //    secondLevel.Poly_BaseClassNonAbstracts.Add(secondLevelConcretes.Item2);
    //    secondLevel.Poly_BaseClassNonAbstracts.Add(secondLevelConcretes.Item3);
    //    secondLevel.Poly_BaseClassNonAbstracts.Add(secondLevelConcretes.Item4);
    //    DbContext.Poly_SecondLevels.Add(secondLevel);
        
    //    DbContext.SaveChanges();
        
    //    var retrievedTopLevel = DbContext.Poly_TopLevels.Single(p => p.Id == topLevel.Id);
    //    Assert.Equal(topLevel.Poly_RootAbstracts.Count, retrievedTopLevel.Poly_RootAbstracts.Count);
    //    foreach (var rootAbstract in retrievedTopLevel.Poly_RootAbstracts)
    //    {
    //        Assert.Equal(rootAbstract.AbstractField, rootAbstract.Poly_RootAbstract_Comp.CompField);
    //        Assert.Equal(topLevelConcretes.Item1.Id, rootAbstract.Poly_RootAbstract_Aggr.Id);
    //    }
        
    //    var retrievedSecondLevel = DbContext.Poly_SecondLevels.Single(p => p.Id == secondLevel.Id);
    //    Assert.Equal(secondLevel.Poly_BaseClassNonAbstracts.Count, retrievedSecondLevel.Poly_BaseClassNonAbstracts.Count);
    //    foreach (var nonAbstract in retrievedSecondLevel.Poly_BaseClassNonAbstracts)
    //    {
    //        Assert.Equal(nonAbstract.BaseField, nonAbstract.Poly_RootAbstract_Comp.CompField);
    //        Assert.Equal(secondLevelConcretes.Item1.Id, nonAbstract.Poly_RootAbstract_Aggr.Id);
    //    }
        
    //    (Poly_RootAbstract_Aggr, Poly_BaseClassNonAbstract, Poly_ConcreteA, Poly_ConcreteB) CreateNewPolyClasses()
    //    {
    //        var rootAbstractAggr = new Poly_RootAbstract_Aggr();
    //        rootAbstractAggr.PartitionKey = "ABC";
    //        rootAbstractAggr.AggrField = "Root Abstract Aggregate Value";
    //        DbContext.Poly_RootAbstract_Aggrs.Add(rootAbstractAggr);
            
    //        var baseClass = new Poly_BaseClassNonAbstract();
    //        baseClass.Id = Guid.NewGuid(); 
    //        baseClass.PartitionKey = "ABC";
    //        baseClass.AbstractField = "Base Class Non Abstract Value";
    //        baseClass.BaseField = "Base Class Non Abstract Value";
    //        baseClass.Poly_RootAbstract_Comp = new Poly_RootAbstract_Comp() { CompField = "Base Class Non Abstract Value" };
    //        baseClass.Poly_RootAbstract_Aggr = rootAbstractAggr;
        
    //        var concreteA = new Poly_ConcreteA();
    //        concreteA.Id = Guid.NewGuid(); 
    //        concreteA.PartitionKey = "ABC";
    //        concreteA.ConcreteField = "Concrete Value";
    //        concreteA.AbstractField = "Concrete Value";
    //        concreteA.BaseField = "Concrete Value";
    //        concreteA.Poly_RootAbstract_Comp = new Poly_RootAbstract_Comp() { CompField = "Concrete Value" };
    //        concreteA.Poly_RootAbstract_Aggr = rootAbstractAggr;
    //        DbContext.Poly_ConcreteAs.Add(concreteA);
        
    //        var concreteB = new Poly_ConcreteB();
    //        concreteB.Id = Guid.NewGuid(); 
    //        concreteB.PartitionKey = "ABC";
    //        concreteB.ConcreteField = "Concrete Value";
    //        concreteB.AbstractField = "Concrete Value";
    //        concreteB.BaseField = "Concrete Value";
    //        concreteB.Poly_RootAbstract_Comp = new Poly_RootAbstract_Comp() { CompField = "Concrete Value" };
    //        concreteB.Poly_RootAbstract_Aggr = rootAbstractAggr;
    //        DbContext.Poly_ConcreteBs.Add(concreteB);
            
    //        return (rootAbstractAggr, baseClass, concreteA, concreteB);
    //    }
    //}
}