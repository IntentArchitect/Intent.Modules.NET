using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPC.Polymorphic;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
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
        var derived = new TPC_DerivedClassForAbstract();
        derived.BaseAttribute = "Base Value";
        derived.DerivedAttribute = "Base Value";
        DbContext.TPC_DerivedClassForAbstracts.Add(derived);

        var derivedAssociated = new TPC_DerivedClassForAbstractAssociated();
        derivedAssociated.AssociatedField = "Derived Associated Value";
        derivedAssociated.DerivedClassForAbstract = derived;
        DbContext.TPC_DerivedClassForAbstractAssociateds.Add(derivedAssociated);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPC_DerivedClassForAbstracts.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.TPC_DerivedClassForAbstractAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForAbstract);
    }

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPC_ConcreteBaseClass()
    {
        var concreteBase = new TPC_ConcreteBaseClass();
        concreteBase.BaseAttribute = "Base Value";
        DbContext.TPC_ConcreteBaseClasses.Add(concreteBase);

        var concreteBaseAssociated = new TPC_ConcreteBaseClassAssociated();
        concreteBaseAssociated.AssociatedField = "Associated Value";
        concreteBaseAssociated.ConcreteBaseClass = concreteBase;
        DbContext.TPC_ConcreteBaseClassAssociateds.Add(concreteBaseAssociated);

        DbContext.SaveChanges();

        Assert.Equal((object)concreteBase, (object)DbContext.TPC_ConcreteBaseClasses.SingleOrDefault(p => p.Id == concreteBase.Id));
        Assert.Equal(concreteBase, DbContext.TPC_ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociated.Id).ConcreteBaseClass);

        var derived = new TPC_DerivedClassForConcrete();
        derived.BaseAttribute = "Derived Value";
        derived.DerivedAttribute = "Derived Value";
        DbContext.TPC_DerivedClassForConcretes.Add(derived);

        var derivedAssociated = new TPC_DerivedClassForConcreteAssociated();
        derivedAssociated.AssociatedField = "Associated Value";
        derivedAssociated.DerivedClassForConcrete = derived;
        DbContext.TPC_DerivedClassForConcreteAssociateds.Add(derivedAssociated);

        var concreteBaseAssociatedForDerived = new TPC_ConcreteBaseClassAssociated();
        concreteBaseAssociatedForDerived.AssociatedField = "Associated Value";
        concreteBaseAssociatedForDerived.ConcreteBaseClass = derived;
        DbContext.TPC_ConcreteBaseClassAssociateds.Add(concreteBaseAssociatedForDerived);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPC_DerivedClassForConcretes.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.TPC_DerivedClassForConcreteAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForConcrete);
        Assert.Equal(derived, DbContext.TPC_ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociatedForDerived.Id).ConcreteBaseClass);
    }

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPC_CompositeForeignKey()
    {
        var baseClass = new TPC_FkBaseClass();
        baseClass.CompositeKeyA = Guid.NewGuid();
        baseClass.CompositeKeyB = Guid.NewGuid();
        DbContext.TPC_FkBaseClasses.Add(baseClass);

        var baseAssociated = new TPC_FkBaseClassAssociated();
        baseAssociated.AssociatedField = "Associated Value";
        baseAssociated.FkBaseClass = baseClass;
        DbContext.TPC_FkBaseClassAssociateds.Add(baseAssociated);

        DbContext.SaveChanges();

        Assert.Equal(baseClass, DbContext.TPC_FkBaseClasses.Single(p => p.CompositeKeyA == baseClass.CompositeKeyA && p.CompositeKeyB == baseClass.CompositeKeyB));
        Assert.Equal(baseClass, DbContext.TPC_FkBaseClassAssociateds.Single(p => p.Id == baseAssociated.Id).FkBaseClass);

        var derived = new TPC_FkDerivedClass();
        derived.CompositeKeyA = Guid.NewGuid();
        derived.CompositeKeyB = Guid.NewGuid();
        derived.DerivedField = "Derived Value";
        DbContext.TPC_FkDerivedClasses.Add(derived);

        var associated = new TPC_FkAssociatedClass();
        associated.FkDerivedClass = derived;
        associated.AssociatedField = "Associated Value";
        DbContext.TPC_FkAssociatedClasses.Add(associated);

        var baseAssociated2 = new TPC_FkBaseClassAssociated();
        baseAssociated2.AssociatedField = "Associated Value";
        baseAssociated2.FkBaseClass = derived;
        DbContext.TPC_FkBaseClassAssociateds.Add(baseAssociated2);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPC_FkDerivedClasses.Single(p => p.CompositeKeyA == derived.CompositeKeyA && p.CompositeKeyB == derived.CompositeKeyB));
        Assert.Equal(derived, DbContext.TPC_FkAssociatedClasses.Single(p => p.Id == associated.Id).FkDerivedClass);
        Assert.Equal(derived, DbContext.TPC_FkBaseClassAssociateds.Single(p => p.Id == baseAssociated2.Id).FkBaseClass);
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
        var secondLevel = new TPC_Poly_SecondLevel();
        secondLevel.SecondField = "Second Level Value";
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item2);
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item3);
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item4);
        DbContext.TPC_Poly_SecondLevels.Add(secondLevel);
        
        DbContext.SaveChanges();

        // var retrievedTopLevel = DbContext.Poly_TopLevels.Single(p => p.Id == topLevel.Id);
        // Assert.Equal(topLevel.Poly_RootAbstract.Count, retrievedTopLevel.Poly_RootAbstract.Count);
        // foreach (var rootAbstract in retrievedTopLevel.Poly_RootAbstract)
        // {
        //     Assert.Equal(rootAbstract.AbstractField, rootAbstract.Poly_RootAbstract_Comp.CompField);
        //     Assert.Equal(topLevelConcretes.Item1.Id, rootAbstract.Poly_RootAbstract_Aggr.Id);
        // }
        
        var retrievedSecondLevel = DbContext.TPC_Poly_SecondLevels.Single(p => p.Id == secondLevel.Id);
        Assert.Equal(secondLevel.BaseClassNonAbstracts.Count, retrievedSecondLevel.BaseClassNonAbstracts.Count);
        foreach (var nonAbstract in retrievedSecondLevel.BaseClassNonAbstracts)
        {
            Assert.Equal(nonAbstract.BaseField, nonAbstract.Poly_RootAbstract_Comp.CompField);
            Assert.Equal(secondLevelConcretes.Item1.Id, nonAbstract.Poly_RootAbstract_Aggr.Id);
        }
        
        (TPC_Poly_RootAbstract_Aggr, TPC_Poly_BaseClassNonAbstract, TPC_Poly_ConcreteA, TPC_Poly_ConcreteB) CreateNewPolyClasses()
        {
            var rootAbstractAggr = new TPC_Poly_RootAbstract_Aggr();
            rootAbstractAggr.AggrField = "Root Abstract Aggregate Value";
            DbContext.TPC_Poly_RootAbstract_Aggrs.Add(rootAbstractAggr);
            
            var baseClass = new TPC_Poly_BaseClassNonAbstract();
            baseClass.AbstractField = "Base Class Non Abstract Value";
            baseClass.BaseField = "Base Class Non Abstract Value";
            baseClass.Poly_RootAbstract_Comp = new TPC_Poly_RootAbstract_Comp() { CompField = "Base Class Non Abstract Value" };
            baseClass.Poly_RootAbstract_Aggr = rootAbstractAggr;
        
            var concreteA = new TPC_Poly_ConcreteA();
            concreteA.ConcreteField = "Concrete Value";
            concreteA.AbstractField = "Concrete Value";
            concreteA.BaseField = "Concrete Value";
            concreteA.Poly_RootAbstract_Comp = new TPC_Poly_RootAbstract_Comp() { CompField = "Concrete Value" };
            concreteA.Poly_RootAbstract_Aggr = rootAbstractAggr;
            DbContext.TPC_Poly_ConcreteAs.Add(concreteA);
        
            var concreteB = new TPC_Poly_ConcreteB();
            concreteB.ConcreteField = "Concrete Value";
            concreteB.AbstractField = "Concrete Value";
            concreteB.BaseField = "Concrete Value";
            concreteB.Poly_RootAbstract_Comp = new TPC_Poly_RootAbstract_Comp() { CompField = "Concrete Value" };
            concreteB.Poly_RootAbstract_Aggr = rootAbstractAggr;
            DbContext.TPC_Poly_ConcreteBs.Add(concreteB);
            
            return (rootAbstractAggr, baseClass, concreteA, concreteB);
        }
    }
}