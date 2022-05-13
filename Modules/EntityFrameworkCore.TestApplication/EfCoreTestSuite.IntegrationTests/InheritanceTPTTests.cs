using System;
using System.Linq;
using EfCoreTestSuite.TPT.IntentGenerated.Core;
using EfCoreTestSuite.TPT.IntentGenerated.Entities;
using Xunit;

namespace EfCoreTestSuite.IntegrationTests;

public class InheritanceTPTTests : SharedDatabaseFixture<ApplicationDbContext>
{
    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_Inheritance_TPT_ConcreteBaseClass()
    {
        var test = new DerivedClassForConcrete();
        test.BaseAttribute = "Base Value";
        test.DerivedAttribute = "Derived Value";
        DbContext.DerivedClassForConcretes.Add(test);
        DbContext.SaveChanges();

        Assert.Equal((object)test, (object)DbContext.ConcreteBaseClasses.SingleOrDefault(p => p.BaseAttribute == test.BaseAttribute));
        Assert.Equal(test, DbContext.DerivedClassForConcretes.SingleOrDefault(p => p.BaseAttribute == test.BaseAttribute && p.DerivedAttribute == test.DerivedAttribute));
    }
    
    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_Inheritance_TPT_AbstractBaseClass()
    {
        var test = new DerivedClassForAbstract();
        test.BaseAttribute = "Base Value";
        test.DerivedAttribute = "Derived Value";
        DbContext.DerivedClassForAbstracts.Add(test);
        DbContext.SaveChanges();

        Assert.Equal((object)test, (object)DbContext.AbstractBaseClasses.SingleOrDefault(p => p.BaseAttribute == test.BaseAttribute));
        Assert.Equal(test, DbContext.DerivedClassForAbstracts.SingleOrDefault(p => p.BaseAttribute == test.BaseAttribute && p.DerivedAttribute == test.DerivedAttribute));
    }
    
    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_Inheritance_CompositeForeignKey()
    {
        var derived = new FkDerivedClass();
        derived.CompositeKeyA = Guid.NewGuid();
        derived.CompositeKeyB = Guid.NewGuid();
        DbContext.FkDerivedClasses.Add(derived);

        var associated = new FkAssociatedClass();
        associated.FkDerivedClass = derived;
        DbContext.FkAssociatedClasses.Add(associated);
        DbContext.SaveChanges();
        
        Assert.Equal(derived.CompositeKeyA, associated.FkDerivedClassCompositeKeyA);
        Assert.Equal(derived.CompositeKeyB, associated.FkDerivedClassCompositeKeyB);
    }
}