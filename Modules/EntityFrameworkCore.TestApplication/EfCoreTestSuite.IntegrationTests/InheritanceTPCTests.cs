using System.Linq;
using EfCoreTestSuite.TPC.IntentGenerated.Core;
using EfCoreTestSuite.TPC.IntentGenerated.Entities;
using Xunit;

namespace EfCoreTestSuite.IntegrationTests;

public class InheritanceTPCTests : SharedDatabaseFixture<ApplicationDbContext>
{
    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_Inheritance_TPC_ConcreteBaseClass()
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
    public void Test_Inheritance_TPC_AbstractBaseClass()
    {
        var test = new DerivedClassForAbstract();
        test.BaseAttribute = "Base Value";
        test.DerivedAttribute = "Derived Value";
        DbContext.DerivedClassForAbstracts.Add(test);
        DbContext.SaveChanges();
        
        Assert.Equal(test, DbContext.DerivedClassForAbstracts.SingleOrDefault(p => p.BaseAttribute == test.BaseAttribute && p.DerivedAttribute == test.DerivedAttribute));
    }
}