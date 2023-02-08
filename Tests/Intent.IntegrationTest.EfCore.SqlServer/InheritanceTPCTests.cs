using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence;
using Intent.IntegrationTest.EfCore.SqlServer.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.SqlServer;

public class InheritanceTPCTests : SharedDatabaseFixture<ApplicationDbContext, InheritanceTPCTests>
{
    public InheritanceTPCTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPC_AbstractBaseClass()
    {
        var derived = new DerivedClassForAbstract();
        derived.BaseAttribute = "Base Value";
        derived.DerivedAttribute = "Base Value";
        DbContext.DerivedClassForAbstracts.Add(derived);

        var derivedAssociated = new DerivedClassForAbstractAssociated();
        derivedAssociated.AssociatedField = "Derived Associated Value";
        derivedAssociated.DerivedClassForAbstract = derived;
        DbContext.TPCInheritanceAssociationsDerivedClassForAbstractAssociateds.Add(derivedAssociated);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.DerivedClassForAbstracts.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.TPCInheritanceAssociationsDerivedClassForAbstractAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForAbstract);
    }

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPC_ConcreteBaseClass()
    {
        var concreteBase = new ConcreteBaseClass();
        concreteBase.BaseAttribute = "Base Value";
        DbContext.TPCInheritanceAssociationsConcreteBaseClasses.Add(concreteBase);

        var concreteBaseAssociated = new ConcreteBaseClassAssociated();
        concreteBaseAssociated.AssociatedField = "Associated Value";
        concreteBaseAssociated.ConcreteBaseClass = concreteBase;
        DbContext.ConcreteBaseClassAssociateds.Add(concreteBaseAssociated);

        DbContext.SaveChanges();

        Assert.Equal((object)concreteBase, (object)DbContext.TPCInheritanceAssociationsConcreteBaseClasses.SingleOrDefault(p => p.Id == concreteBase.Id));
        Assert.Equal(concreteBase, DbContext.ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociated.Id).ConcreteBaseClass);

        var derived = new DerivedClassForConcrete();
        derived.BaseAttribute = "Derived Value";
        derived.DerivedAttribute = "Derived Value";
        DbContext.TPCInheritanceAssociationsDerivedClassForConcretes.Add(derived);

        var derivedAssociated = new DerivedClassForConcreteAssociated();
        derivedAssociated.AssociatedField = "Associated Value";
        derivedAssociated.DerivedClassForConcrete = derived;
        DbContext.TPCInheritanceAssociationsDerivedClassForConcreteAssociateds.Add(derivedAssociated);

        var concreteBaseAssociatedForDerived = new ConcreteBaseClassAssociated();
        concreteBaseAssociatedForDerived.AssociatedField = "Associated Value";
        concreteBaseAssociatedForDerived.ConcreteBaseClass = derived;
        DbContext.ConcreteBaseClassAssociateds.Add(concreteBaseAssociatedForDerived);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPCInheritanceAssociationsDerivedClassForConcretes.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.TPCInheritanceAssociationsDerivedClassForConcreteAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForConcrete);
        Assert.Equal(derived, DbContext.ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociatedForDerived.Id).ConcreteBaseClass);
    }

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPC_CompositeForeignKey()
    {
        var baseClass = new FkBaseClass();
        baseClass.CompositeKeyA = Guid.NewGuid();
        baseClass.CompositeKeyB = Guid.NewGuid();
        DbContext.TPCInheritanceAssociationsFkBaseClasses.Add(baseClass);

        var baseAssociated = new FkBaseClassAssociated();
        baseAssociated.AssociatedField = "Associated Value";
        baseAssociated.FkBaseClass = baseClass;
        DbContext.TPCInheritanceAssociationsFkBaseClassAssociateds.Add(baseAssociated);

        DbContext.SaveChanges();

        Assert.Equal(baseClass, DbContext.TPCInheritanceAssociationsFkBaseClasses.Single(p => p.CompositeKeyA == baseClass.CompositeKeyA && p.CompositeKeyB == baseClass.CompositeKeyB));
        Assert.Equal(baseClass, DbContext.TPCInheritanceAssociationsFkBaseClassAssociateds.Single(p => p.Id == baseAssociated.Id).FkBaseClass);

        var derived = new FkDerivedClass();
        derived.CompositeKeyA = Guid.NewGuid();
        derived.CompositeKeyB = Guid.NewGuid();
        derived.DerivedField = "Derived Value";
        DbContext.TPCInheritanceAssociationsFkDerivedClasses.Add(derived);

        var associated = new FkAssociatedClass();
        associated.FkDerivedClass = derived;
        associated.AssociatedField = "Associated Value";
        DbContext.FkAssociatedClasses.Add(associated);

        var baseAssociated2 = new FkBaseClassAssociated();
        baseAssociated2.AssociatedField = "Associated Value";
        baseAssociated2.FkBaseClass = derived;
        DbContext.TPCInheritanceAssociationsFkBaseClassAssociateds.Add(baseAssociated2);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPCInheritanceAssociationsFkDerivedClasses.Single(p => p.CompositeKeyA == derived.CompositeKeyA && p.CompositeKeyB == derived.CompositeKeyB));
        Assert.Equal(derived, DbContext.FkAssociatedClasses.Single(p => p.Id == associated.Id).FkDerivedClass);
        Assert.Equal(derived, DbContext.TPCInheritanceAssociationsFkBaseClassAssociateds.Single(p => p.Id == baseAssociated2.Id).FkBaseClass);
    }

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPC_Polymorphic()
    {
        // var topLevelConcretes = CreateNewPolyClasses();
        // var topLevel = new Poly_TopLevel();
        // topLevel.TopField = "Top Level Value";
        // topLevel.Poly_RootAbstract.Add(topLevelConcretes.Item2);
        // topLevel.Poly_RootAbstract.Add(topLevelConcretes.Item3);
        // topLevel.Poly_RootAbstract.Add(topLevelConcretes.Item4);
        // DbContext.Poly_TopLevels.Add(topLevel);

        var secondLevelConcretes = CreateNewPolyClasses();
        var secondLevel = new Poly_SecondLevel();
        secondLevel.SecondField = "Second Level Value";
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item2);
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item3);
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item4);
        DbContext.Poly_SecondLevels.Add(secondLevel);
        
        DbContext.SaveChanges();

        // var retrievedTopLevel = DbContext.Poly_TopLevels.Single(p => p.Id == topLevel.Id);
        // Assert.Equal(topLevel.Poly_RootAbstract.Count, retrievedTopLevel.Poly_RootAbstract.Count);
        // foreach (var rootAbstract in retrievedTopLevel.Poly_RootAbstract)
        // {
        //     Assert.Equal(rootAbstract.AbstractField, rootAbstract.Poly_RootAbstract_Comp.CompField);
        //     Assert.Equal(topLevelConcretes.Item1.Id, rootAbstract.Poly_RootAbstract_Aggr.Id);
        // }
        
        var retrievedSecondLevel = DbContext.Poly_SecondLevels.Single(p => p.Id == secondLevel.Id);
        Assert.Equal(secondLevel.BaseClassNonAbstracts.Count, retrievedSecondLevel.BaseClassNonAbstracts.Count);
        foreach (var nonAbstract in retrievedSecondLevel.BaseClassNonAbstracts)
        {
            Assert.Equal(nonAbstract.BaseField, nonAbstract.Poly_RootAbstract_Comp.CompField);
            Assert.Equal(secondLevelConcretes.Item1.Id, nonAbstract.Poly_RootAbstract_Aggr.Id);
        }
        
        (Poly_RootAbstract_Aggr, Poly_BaseClassNonAbstract, Poly_ConcreteA, Poly_ConcreteB) CreateNewPolyClasses()
        {
            var rootAbstractAggr = new Poly_RootAbstract_Aggr();
            rootAbstractAggr.AggrField = "Root Abstract Aggregate Value";
            DbContext.TPCPolymorphicPoly_RootAbstract_Aggrs.Add(rootAbstractAggr);
            
            var baseClass = new Poly_BaseClassNonAbstract();
            baseClass.AbstractField = "Base Class Non Abstract Value";
            baseClass.BaseField = "Base Class Non Abstract Value";
            baseClass.Poly_RootAbstract_Comp = new Poly_RootAbstract_Comp() { CompField = "Base Class Non Abstract Value" };
            baseClass.Poly_RootAbstract_Aggr = rootAbstractAggr;
        
            var concreteA = new Poly_ConcreteA();
            concreteA.ConcreteField = "Concrete Value";
            concreteA.AbstractField = "Concrete Value";
            concreteA.BaseField = "Concrete Value";
            concreteA.Poly_RootAbstract_Comp = new Poly_RootAbstract_Comp() { CompField = "Concrete Value" };
            concreteA.Poly_RootAbstract_Aggr = rootAbstractAggr;
            DbContext.TPCPolymorphicPoly_ConcreteAs.Add(concreteA);
        
            var concreteB = new Poly_ConcreteB();
            concreteB.ConcreteField = "Concrete Value";
            concreteB.AbstractField = "Concrete Value";
            concreteB.BaseField = "Concrete Value";
            concreteB.Poly_RootAbstract_Comp = new Poly_RootAbstract_Comp() { CompField = "Concrete Value" };
            concreteB.Poly_RootAbstract_Aggr = rootAbstractAggr;
            DbContext.TPCPolymorphicPoly_ConcreteBs.Add(concreteB);
            
            return (rootAbstractAggr, baseClass, concreteA, concreteB);
        }
    }
}