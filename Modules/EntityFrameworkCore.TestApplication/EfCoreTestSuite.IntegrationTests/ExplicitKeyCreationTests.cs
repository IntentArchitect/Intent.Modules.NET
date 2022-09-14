using System;
using EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Core;
using EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities;
using Xunit;
using Xunit.Abstractions;

namespace EfCoreTestSuite.IntegrationTests;

public class ExplicitKeyCreationTests : SharedDatabaseFixture<ApplicationDbContext>
{
    public ExplicitKeyCreationTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_ExplicitCompositeKeys_ExplicitCompositeForeignKeys()
    {
        var pk = new ExplicitKeysCompositeKey();
        pk.CompositeKeyA = Guid.NewGuid();
        pk.CompositeKeyB = Guid.NewGuid();
        DbContext.ExplicitKeysCompositeKeys.Add(pk);

        var fk = new ExplicitKeysCompositeForeignKey();
        fk.ExplicitKeysCompositeKey = pk;
        DbContext.ExplicitKeysCompositeForeignKeys.Add(fk);

        DbContext.SaveChanges();

        Assert.Equal(pk, fk.ExplicitKeysCompositeKey);
        Assert.Equal(pk.CompositeKeyA, fk.ExplicitKeysCompositeKeyCompositeKeyA);
        Assert.Equal(pk.CompositeKeyB, fk.ExplicitKeysCompositeKeyCompositeKeyB);
    }
}