using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.Polymorphic;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using Intent.IntegrationTest.EfCore.SqlServer.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.SqlServer;

public class InheritanceTPTTests : SharedDatabaseFixture<ApplicationDbContext, InheritanceTPTTests>
{
    public InheritanceTPTTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPT_AbstractBaseClass()
    {
        var derived = new TPT_DerivedClassForAbstract();
        derived.BaseAttribute = "Base Value";
        derived.DerivedAttribute = "Base Value";
        DbContext.TPT_DerivedClassForAbstracts.Add(derived);

        var derivedAssociated = new TPT_DerivedClassForAbstractAssociated();
        derivedAssociated.AssociatedField = "Derived Associated Value";
        derivedAssociated.DerivedClassForAbstract = derived;
        DbContext.TPT_DerivedClassForAbstractAssociateds.Add(derivedAssociated);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPT_DerivedClassForAbstracts.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.TPT_DerivedClassForAbstractAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForAbstract);
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPT_ConcreteBaseClass()
    {
        var concreteBase = new TPT_ConcreteBaseClass();
        concreteBase.BaseAttribute = "Base Value";
        DbContext.TPT_ConcreteBaseClasses.Add(concreteBase);

        var concreteBaseAssociated = new TPT_ConcreteBaseClassAssociated();
        concreteBaseAssociated.AssociatedField = "Associated Value";
        concreteBaseAssociated.ConcreteBaseClass = concreteBase;
        DbContext.TPT_ConcreteBaseClassAssociateds.Add(concreteBaseAssociated);

        DbContext.SaveChanges();

        Assert.Equal((object)concreteBase, (object)DbContext.TPT_ConcreteBaseClasses.SingleOrDefault(p => p.Id == concreteBase.Id));
        Assert.Equal(concreteBase, DbContext.TPT_ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociated.Id).ConcreteBaseClass);

        var derived = new TPT_DerivedClassForConcrete();
        derived.BaseAttribute = "Derived Value";
        derived.DerivedAttribute = "Derived Value";
        DbContext.TPT_DerivedClassForConcretes.Add(derived);

        var derivedAssociated = new TPT_DerivedClassForConcreteAssociated();
        derivedAssociated.AssociatedField = "Associated Value";
        derivedAssociated.DerivedClassForConcrete = derived;
        DbContext.TPT_DerivedClassForConcreteAssociateds.Add(derivedAssociated);

        var concreteBaseAssociatedForDerived = new TPT_ConcreteBaseClassAssociated();
        concreteBaseAssociatedForDerived.AssociatedField = "Associated Value";
        concreteBaseAssociatedForDerived.ConcreteBaseClass = derived;
        DbContext.TPT_ConcreteBaseClassAssociateds.Add(concreteBaseAssociatedForDerived);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPT_DerivedClassForConcretes.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.TPT_DerivedClassForConcreteAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForConcrete);
        Assert.Equal(derived, DbContext.TPT_ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociatedForDerived.Id).ConcreteBaseClass);
    }

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPT_CompositeForeignKey()
    {
        var baseClass = new TPT_FkBaseClass();
        baseClass.CompositeKeyA = Guid.NewGuid();
        baseClass.CompositeKeyB = Guid.NewGuid();
        DbContext.TPT_FkBaseClasses.Add(baseClass);

        var baseAssociated = new TPT_FkBaseClassAssociated();
        baseAssociated.AssociatedField = "Associated Value";
        baseAssociated.FkBaseClass = baseClass;
        DbContext.TPT_FkBaseClassAssociateds.Add(baseAssociated);

        DbContext.SaveChanges();

        Assert.Equal(baseClass, DbContext.TPT_FkBaseClasses.Single(p => p.CompositeKeyA == baseClass.CompositeKeyA && p.CompositeKeyB == baseClass.CompositeKeyB));
        Assert.Equal(baseClass, DbContext.TPT_FkBaseClassAssociateds.Single(p => p.Id == baseAssociated.Id).FkBaseClass);

        var derived = new TPT_FkDerivedClass();
        derived.CompositeKeyA = Guid.NewGuid();
        derived.CompositeKeyB = Guid.NewGuid();
        derived.DerivedField = "Derived Value";
        DbContext.TPT_FkDerivedClasses.Add(derived);

        var associated = new TPT_FkAssociatedClass();
        associated.FkDerivedClass = derived;
        associated.AssociatedField = "Associated Value";
        DbContext.TPT_FkAssociatedClasses.Add(associated);

        var baseAssociated2 = new TPT_FkBaseClassAssociated();
        baseAssociated2.AssociatedField = "Associated Value";
        baseAssociated2.FkBaseClass = derived;
        DbContext.TPT_FkBaseClassAssociateds.Add(baseAssociated2);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.TPT_FkDerivedClasses.Single(p => p.CompositeKeyA == derived.CompositeKeyA && p.CompositeKeyB == derived.CompositeKeyB));
        Assert.Equal(derived, DbContext.TPT_FkAssociatedClasses.Single(p => p.Id == associated.Id).FkDerivedClass);
        Assert.Equal(derived, DbContext.TPT_FkBaseClassAssociateds.Single(p => p.Id == baseAssociated2.Id).FkBaseClass);
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPT_Polymorphic()
    {
        var topLevelConcretes = CreateNewPolyClasses();
        var topLevel = new TPT_Poly_TopLevel();
        topLevel.TopField = "Top Level Value";
        topLevel.RootAbstracts.Add(topLevelConcretes.Item2);
        topLevel.RootAbstracts.Add(topLevelConcretes.Item3);
        topLevel.RootAbstracts.Add(topLevelConcretes.Item4);
        DbContext.TPT_Poly_TopLevels.Add(topLevel);

        var secondLevelConcretes = CreateNewPolyClasses();
        var secondLevel = new TPT_Poly_SecondLevel();
        secondLevel.SecondField = "Second Level Value";
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item2);
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item3);
        secondLevel.BaseClassNonAbstracts.Add(secondLevelConcretes.Item4);
        DbContext.TPT_Poly_SecondLevels.Add(secondLevel);
        
        DbContext.SaveChanges();
        
        var retrievedTopLevel = DbContext.TPT_Poly_TopLevels.Single(p => p.Id == topLevel.Id);
        Assert.Equal(topLevel.RootAbstracts.Count, retrievedTopLevel.RootAbstracts.Count);
        foreach (var rootAbstract in retrievedTopLevel.RootAbstracts)
        {
            Assert.Equal(rootAbstract.AbstractField, rootAbstract.Poly_RootAbstract_Comp.CompField);
            Assert.Equal(topLevelConcretes.Item1.Id, rootAbstract.Poly_RootAbstract_Aggr.Id);
        }
        
        var retrievedSecondLevel = DbContext.TPT_Poly_SecondLevels.Single(p => p.Id == secondLevel.Id);
        Assert.Equal(secondLevel.BaseClassNonAbstracts.Count, retrievedSecondLevel.BaseClassNonAbstracts.Count);
        foreach (var nonAbstract in retrievedSecondLevel.BaseClassNonAbstracts)
        {
            Assert.Equal(nonAbstract.BaseField, nonAbstract.Poly_RootAbstract_Comp.CompField);
            Assert.Equal(secondLevelConcretes.Item1.Id, nonAbstract.Poly_RootAbstract_Aggr.Id);
        }
        
        (TPT_Poly_RootAbstract_Aggr, TPT_Poly_BaseClassNonAbstract, TPT_Poly_ConcreteA, TPT_Poly_ConcreteB) CreateNewPolyClasses()
        {
            var rootAbstractAggr = new TPT_Poly_RootAbstract_Aggr();
            rootAbstractAggr.AggrField = "Root Abstract Aggregate Value";
            DbContext.TPT_Poly_RootAbstract_Aggrs.Add(rootAbstractAggr);
            
            var baseClass = new TPT_Poly_BaseClassNonAbstract();
            baseClass.AbstractField = "Base Class Non Abstract Value";
            baseClass.BaseField = "Base Class Non Abstract Value";
            baseClass.Poly_RootAbstract_Comp = new TPT_Poly_RootAbstract_Comp() { CompField = "Base Class Non Abstract Value" };
            baseClass.Poly_RootAbstract_Aggr = rootAbstractAggr;
        
            var concreteA = new TPT_Poly_ConcreteA();
            concreteA.ConcreteField = "Concrete Value";
            concreteA.AbstractField = "Concrete Value";
            concreteA.BaseField = "Concrete Value";
            concreteA.Poly_RootAbstract_Comp = new TPT_Poly_RootAbstract_Comp() { CompField = "Concrete Value" };
            concreteA.Poly_RootAbstract_Aggr = rootAbstractAggr;
            DbContext.TPT_Poly_ConcreteAs.Add(concreteA);
        
            var concreteB = new TPT_Poly_ConcreteB();
            concreteB.ConcreteField = "Concrete Value";
            concreteB.AbstractField = "Concrete Value";
            concreteB.BaseField = "Concrete Value";
            concreteB.Poly_RootAbstract_Comp = new TPT_Poly_RootAbstract_Comp() { CompField = "Concrete Value" };
            concreteB.Poly_RootAbstract_Aggr = rootAbstractAggr;
            DbContext.TPT_Poly_ConcreteBs.Add(concreteB);
            
            return (rootAbstractAggr, baseClass, concreteA, concreteB);
        }
    }
}