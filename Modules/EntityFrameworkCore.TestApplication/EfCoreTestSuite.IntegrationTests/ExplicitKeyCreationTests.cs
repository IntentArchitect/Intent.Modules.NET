using System;
using EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Core;
using EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities;
using Xunit;

namespace EfCoreTestSuite.IntegrationTests;

public class ExplicitKeyCreationTests : SharedDatabaseFixture<ApplicationDbContext>
{
    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_PK_Explicit_CompositeKeys_FK_Explicit_CompositeForeignKeys()
    {
        var pk = new PK_ExplicitKeys_CompositeKey();
        pk.CompositeKeyA = Guid.NewGuid();
        pk.CompositeKeyB = Guid.NewGuid();
        DbContext.PK_ExplicitKeys_CompositeKeys.Add(pk);
        
        var fk = new FK_ExplicitKeys_CompositeForeignKey();
        fk.PK_ExplicitKeys_CompositeKey = pk;
        DbContext.FK_ExplicitKeys_CompositeForeignKeys.Add(fk);

        DbContext.SaveChanges();
        
        Assert.Equal(pk, fk.PK_ExplicitKeys_CompositeKey);
        Assert.Equal(pk.CompositeKeyA, fk.PK_ExplicitKeys_CompositeKeyCompositeKeyA);
        Assert.Equal(pk.CompositeKeyB, fk.PK_ExplicitKeys_CompositeKeyCompositeKeyB);
    }
}