using EfCoreTestSuite.TPH.IntentGenerated.Core;
using EfCoreTestSuite.TPH.IntentGenerated.Entities;
using Xunit;

namespace EfCoreTestSuite.IntegrationTests;

public class InheritanceTPHTests : SharedDatabaseFixture<ApplicationDbContext>
{
    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_Inheritance_TPH()
    {
        var derived = new DerivedClass();
        derived.BaseAttribute = "Base Value";
        derived.DerivedAttribute = "Derived Value";
        DbContext.DerivedClasses.Add(derived);
        DbContext.SaveChanges();
    }
}