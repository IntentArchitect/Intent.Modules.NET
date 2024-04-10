using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPH.Polymorphic;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using Intent.IntegrationTest.EfCore.SqlServer.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.SqlServer;

public class InheritanceTPHTests : SharedDatabaseFixture<ApplicationDbContext, InheritanceTPHTests>
{
    public InheritanceTPHTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPH_AbstractBaseClass()
    {
        var derived = new TPH_DerivedClassForAbstract();
        derived.BaseAttribute = "Base Value";
        derived.DerivedAttribute = "Base Value";
        DbContext.TPH_DerivedClassForAbstracts.Add(derived);

        var derivedAssociated = new TPH_DerivedClassForAbstractAssociated();
        derivedAssociated.AssociatedField = "Derived Associated Value";
        derivedAssociated.DerivedClassForAbstract = derived;
        DbContext.TPH_DerivedClassForAbstractAssociateds.Add(derivedAssociated);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPH_DerivedClassForAbstracts.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.TPH_DerivedClassForAbstractAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForAbstract);
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPH_ConcreteBaseClass()
    {
        var concreteBase = new TPH_ConcreteBaseClass();
        concreteBase.BaseAttribute = "Base Value";
        DbContext.TPH_ConcreteBaseClasses.Add(concreteBase);

        var concreteBaseAssociated = new TPH_ConcreteBaseClassAssociated();
        concreteBaseAssociated.AssociatedField = "Associated Value";
        concreteBaseAssociated.ConcreteBaseClass = concreteBase;
        DbContext.TPH_ConcreteBaseClassAssociateds.Add(concreteBaseAssociated);

        DbContext.SaveChanges();

        Assert.Equal((object)concreteBase, (object)DbContext.TPH_ConcreteBaseClasses.SingleOrDefault(p => p.Id == concreteBase.Id));
        Assert.Equal(concreteBase, DbContext.TPH_ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociated.Id).ConcreteBaseClass);

        var derived = new TPH_DerivedClassForConcrete();
        derived.BaseAttribute = "Derived Value";
        derived.DerivedAttribute = "Derived Value";
        DbContext.TPH_DerivedClassForConcretes.Add(derived);

        var derivedAssociated = new TPH_DerivedClassForConcreteAssociated();
        derivedAssociated.AssociatedField = "Associated Value";
        derivedAssociated.DerivedClassForConcrete = derived;
        DbContext.TPH_DerivedClassForConcreteAssociateds.Add(derivedAssociated);

        var concreteBaseAssociatedForDerived = new TPH_ConcreteBaseClassAssociated();
        concreteBaseAssociatedForDerived.AssociatedField = "Associated Value";
        concreteBaseAssociatedForDerived.ConcreteBaseClass = derived;
        DbContext.TPH_ConcreteBaseClassAssociateds.Add(concreteBaseAssociatedForDerived);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPH_DerivedClassForConcretes.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.TPH_DerivedClassForConcreteAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForConcrete);
        Assert.Equal(derived, DbContext.TPH_ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociatedForDerived.Id).ConcreteBaseClass);
    }

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPH_CompositeForeignKey()
    {
        var baseClass = new TPH_FkBaseClass();
        baseClass.CompositeKeyA = Guid.NewGuid();
        baseClass.CompositeKeyB = Guid.NewGuid();
        DbContext.TPH_FkBaseClasses.Add(baseClass);

        var baseAssociated = new TPH_FkBaseClassAssociated();
        baseAssociated.AssociatedField = "Associated Value";
        baseAssociated.FkBaseClass = baseClass;
        DbContext.TPH_FkBaseClassAssociateds.Add(baseAssociated);

        DbContext.SaveChanges();

        Assert.Equal(baseClass, DbContext.TPH_FkBaseClasses.Single(p => p.CompositeKeyA == baseClass.CompositeKeyA && p.CompositeKeyB == baseClass.CompositeKeyB));
        Assert.Equal(baseClass, DbContext.TPH_FkBaseClassAssociateds.Single(p => p.Id == baseAssociated.Id).FkBaseClass);

        var derived = new TPH_FkDerivedClass();
        derived.CompositeKeyA = Guid.NewGuid();
        derived.CompositeKeyB = Guid.NewGuid();
        derived.DerivedField = "Derived Value";
        DbContext.TPH_FkDerivedClasses.Add(derived);

        var associated = new TPH_FkAssociatedClass();
        associated.FkDerivedClass = derived;
        associated.AssociatedField = "Associated Value";
        DbContext.TPH_FkAssociatedClasses.Add(associated);

        var baseAssociated2 = new TPH_FkBaseClassAssociated();
        baseAssociated2.AssociatedField = "Associated Value";
        baseAssociated2.FkBaseClass = derived;
        DbContext.TPH_FkBaseClassAssociateds.Add(baseAssociated2);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPH_FkDerivedClasses.Single(p => p.CompositeKeyA == derived.CompositeKeyA && p.CompositeKeyB == derived.CompositeKeyB));
        Assert.Equal(derived, DbContext.TPH_FkAssociatedClasses.Single(p => p.Id == associated.Id).FkDerivedClass);
        Assert.Equal(derived, DbContext.TPH_FkBaseClassAssociateds.Single(p => p.Id == baseAssociated2.Id).FkBaseClass);
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPH_Polymorphic()
    {
        var topLevelConcretes = CreateNewPolyClasses();
        var topLevel = new TPH_Poly_TopLevel();
        topLevel.TopField = "Top Level Value";
        topLevel.RootAbstracts.Add(topLevelConcretes.Item2);
        topLevel.RootAbstracts.Add(topLevelConcretes.Item3);
        topLevel.RootAbstracts.Add(topLevelConcretes.Item4);
        DbContext.TPH_Poly_TopLevels.Add(topLevel);

        var secondLevelConcretes = CreateNewPolyClasses();
        var secondLevel = new TPH_Poly_SecondLevel();
        secondLevel.SecondField = "Second Level Value";
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item2);
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item3);
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item4);
        DbContext.TPH_Poly_SecondLevels.Add(secondLevel);
        
        DbContext.SaveChanges();
        
        var retrievedTopLevel = DbContext.TPH_Poly_TopLevels.Single(p => p.Id == topLevel.Id);
        Assert.Equal(topLevel.RootAbstracts.Count, retrievedTopLevel.RootAbstracts.Count);
        foreach (var rootAbstract in retrievedTopLevel.RootAbstracts)
        {
            Assert.Equal(rootAbstract.AbstractField, rootAbstract.Poly_RootAbstract_Comp.CompField);
            Assert.Equal(topLevelConcretes.Item1.Id, rootAbstract.Poly_RootAbstract_Aggr.Id);
        }
        
        var retrievedSecondLevel = DbContext.TPH_Poly_SecondLevels.Single(p => p.Id == secondLevel.Id);
        Assert.Equal(secondLevel.BaseClassNonAbstracts.Count, retrievedSecondLevel.BaseClassNonAbstracts.Count);
        foreach (var nonAbstract in retrievedSecondLevel.BaseClassNonAbstracts)
        {
            Assert.Equal(nonAbstract.BaseField, nonAbstract.Poly_RootAbstract_Comp.CompField);
            Assert.Equal(secondLevelConcretes.Item1.Id, nonAbstract.Poly_RootAbstract_Aggr.Id);
        }
        
        (TPH_Poly_RootAbstract_Aggr, TPH_Poly_BaseClassNonAbstract, TPH_Poly_ConcreteA, TPH_Poly_ConcreteB) CreateNewPolyClasses()
        {
            var rootAbstractAggr = new TPH_Poly_RootAbstract_Aggr();
            rootAbstractAggr.AggrField = "Root Abstract Aggregate Value";
            DbContext.TPH_Poly_RootAbstract_Aggrs.Add(rootAbstractAggr);
            
            var baseClass = new TPH_Poly_BaseClassNonAbstract();
            baseClass.AbstractField = "Base Class Non Abstract Value";
            baseClass.BaseField = "Base Class Non Abstract Value";
            baseClass.Poly_RootAbstract_Comp = new TPH_Poly_RootAbstract_Comp() { CompField = "Base Class Non Abstract Value" };
            baseClass.Poly_RootAbstract_Aggr = rootAbstractAggr;
        
            var concreteA = new TPH_Poly_ConcreteA();
            concreteA.ConcreteField = "Concrete Value";
            concreteA.AbstractField = "Concrete Value";
            concreteA.BaseField = "Concrete Value";
            concreteA.Poly_RootAbstract_Comp = new TPH_Poly_RootAbstract_Comp() { CompField = "Concrete Value" };
            concreteA.Poly_RootAbstract_Aggr = rootAbstractAggr;
            DbContext.TPH_Poly_ConcreteAs.Add(concreteA);
        
            var concreteB = new TPH_Poly_ConcreteB();
            concreteB.ConcreteField = "Concrete Value";
            concreteB.AbstractField = "Concrete Value";
            concreteB.BaseField = "Concrete Value";
            concreteB.Poly_RootAbstract_Comp = new TPH_Poly_RootAbstract_Comp() { CompField = "Concrete Value" };
            concreteB.Poly_RootAbstract_Aggr = rootAbstractAggr;
            DbContext.TPH_Poly_ConcreteBs.Add(concreteB);
            
            return (rootAbstractAggr, baseClass, concreteA, concreteB);
        }
    }
}