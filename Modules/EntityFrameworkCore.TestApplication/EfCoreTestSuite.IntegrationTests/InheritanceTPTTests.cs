using System;
using System.Linq;
using EfCoreTestSuite.TPT.IntentGenerated.Core;
using EfCoreTestSuite.TPT.IntentGenerated.Entities;
using Xunit;
using Xunit.Abstractions;

namespace EfCoreTestSuite.IntegrationTests;

public class InheritanceTPTTests : SharedDatabaseFixture<ApplicationDbContext>
{
    public InheritanceTPTTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPT_AbstractBaseClass()
    {
        var derived = new DerivedClassForAbstract();
        derived.BaseAttribute = "Base Value";
        derived.DerivedAttribute = "Base Value";
        DbContext.DerivedClassForAbstracts.Add(derived);

        var derivedAssociated = new DerivedClassForAbstractAssociated();
        derivedAssociated.AssociatedField = "Derived Associated Value";
        derivedAssociated.DerivedClassForAbstract = derived;
        DbContext.DerivedClassForAbstractAssociateds.Add(derivedAssociated);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.DerivedClassForAbstracts.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.DerivedClassForAbstractAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForAbstract);
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPT_ConcreteBaseClass()
    {
        var concreteBase = new ConcreteBaseClass();
        concreteBase.BaseAttribute = "Base Value";
        DbContext.ConcreteBaseClasses.Add(concreteBase);

        var concreteBaseAssociated = new ConcreteBaseClassAssociated();
        concreteBaseAssociated.AssociatedField = "Associated Value";
        concreteBaseAssociated.ConcreteBaseClass = concreteBase;
        DbContext.ConcreteBaseClassAssociateds.Add(concreteBaseAssociated);

        DbContext.SaveChanges();

        Assert.Equal((object)concreteBase, (object)DbContext.ConcreteBaseClasses.SingleOrDefault(p => p.Id == concreteBase.Id));
        Assert.Equal(concreteBase, DbContext.ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociated.Id).ConcreteBaseClass);

        var derived = new DerivedClassForConcrete();
        derived.BaseAttribute = "Derived Value";
        derived.DerivedAttribute = "Derived Value";
        DbContext.DerivedClassForConcretes.Add(derived);

        var derivedAssociated = new DerivedClassForConcreteAssociated();
        derivedAssociated.AssociatedField = "Associated Value";
        derivedAssociated.DerivedClassForConcrete = derived;
        DbContext.DerivedClassForConcreteAssociateds.Add(derivedAssociated);

        var concreteBaseAssociatedForDerived = new ConcreteBaseClassAssociated();
        concreteBaseAssociatedForDerived.AssociatedField = "Associated Value";
        concreteBaseAssociatedForDerived.ConcreteBaseClass = derived;
        DbContext.ConcreteBaseClassAssociateds.Add(concreteBaseAssociatedForDerived);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.DerivedClassForConcretes.Single(p => p.Id == derived.Id));
        Assert.Equal(derived, DbContext.DerivedClassForConcreteAssociateds.Single(p => p.Id == derivedAssociated.Id).DerivedClassForConcrete);
        Assert.Equal(derived, DbContext.ConcreteBaseClassAssociateds.Single(p => p.Id == concreteBaseAssociatedForDerived.Id).ConcreteBaseClass);
    }

    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPT_CompositeForeignKey()
    {
        var baseClass = new FkBaseClass();
        baseClass.CompositeKeyA = Guid.NewGuid();
        baseClass.CompositeKeyB = Guid.NewGuid();
        DbContext.FkBaseClasses.Add(baseClass);

        var baseAssociated = new FkBaseClassAssociated();
        baseAssociated.AssociatedField = "Associated Value";
        baseAssociated.FkBaseClass = baseClass;
        DbContext.FkBaseClassAssociateds.Add(baseAssociated);

        DbContext.SaveChanges();

        Assert.Equal(baseClass, DbContext.FkBaseClasses.Single(p => p.CompositeKeyA == baseClass.CompositeKeyA && p.CompositeKeyB == baseClass.CompositeKeyB));
        Assert.Equal(baseClass, DbContext.FkBaseClassAssociateds.Single(p => p.Id == baseAssociated.Id).FkBaseClass);

        var derived = new FkDerivedClass();
        derived.CompositeKeyA = Guid.NewGuid();
        derived.CompositeKeyB = Guid.NewGuid();
        derived.DerivedField = "Derived Value";
        DbContext.FkDerivedClasses.Add(derived);

        var associated = new FkAssociatedClass();
        associated.FkDerivedClass = derived;
        associated.AssociatedField = "Associated Value";
        DbContext.FkAssociatedClasses.Add(associated);

        var baseAssociated2 = new FkBaseClassAssociated();
        baseAssociated2.AssociatedField = "Associated Value";
        baseAssociated2.FkBaseClass = derived;
        DbContext.FkBaseClassAssociateds.Add(baseAssociated2);

        DbContext.SaveChanges();

        Assert.Equal(derived, DbContext.FkDerivedClasses.Single(p => p.CompositeKeyA == derived.CompositeKeyA && p.CompositeKeyB == derived.CompositeKeyB));
        Assert.Equal(derived, DbContext.FkAssociatedClasses.Single(p => p.Id == associated.Id).FkDerivedClass);
        Assert.Equal(derived, DbContext.FkBaseClassAssociateds.Single(p => p.Id == baseAssociated2.Id).FkBaseClass);
    }
    
    [IgnoreOnCiBuildFact]
    public void Test_Inheritance_TPT_InheritFromOwnedClass()
    {
        var ownedClass = new A_OwnedClass();
        ownedClass.OwnedField = "Owned Class Value";
        ownedClass.AbstractField = "Owned Class Value";

        var ownerClass = new A_OwnerClass();
        ownerClass.OwnerField = "Owner Class Value";
        ownerClass.A_OwnedClasses.Add(ownedClass);
        DbContext.A_OwnerClasses.Add(ownerClass);

        var weirdClass = new A_WeirdClass();
        weirdClass.WeirdField = "Weird Class Value";
        weirdClass.OwnedField = "Weird Class Value";
        weirdClass.AbstractField = "Weird Class Value";
        DbContext.A_WeirdClasses.Add(weirdClass);
        
        DbContext.SaveChanges();

        var test = DbContext.A_OwnerClasses.Single(p => p.Id == ownerClass.Id)?.A_OwnedClasses;
        Assert.Equal(ownedClass, test.Single(p => p.Id == ownedClass.Id));

        // Until something changes, I'm keeping track of this limitation by attempting to assign
        // this WeirdClass to OwnerClass and expecting an error.
        Assert.Throws<InvalidOperationException>(() =>
        {
            ownerClass.A_OwnedClasses.Add(weirdClass);
            
            DbContext.SaveChanges();
            
            // And once this works somehow, we can add these checks and remove the "Throws" check.
            //var test2 = DbContext.A_OwnerClasses.Single(p => p.Id == ownerClass.Id)?.OwnedClasses;
            //Assert.Equal(weirdClass, test2.SingleOrDefault(p => p.Id == weirdClass.Id));
        });
    }
}