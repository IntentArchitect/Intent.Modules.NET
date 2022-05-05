using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreTestSuite.IntentGenerated.Core;
using EfCoreTestSuite.IntentGenerated.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfCoreTestSuite.IntegrationTests;

public class MigrationSetupTest : SharedDatabaseFixture
{
    private static bool _migrationsCompleted;

    private ApplicationDbContext DbContext { get; set; }

    //public const string SkipMessage = null;
    public const string SkipMessage = "CI/CD not ready";

    public MigrationSetupTest()
    {
        DbContext = CreateContext();

        if (!_migrationsCompleted)
        {
            DbContext.Database.Migrate();
            _migrationsCompleted = true;
        }
    }

    public override void Dispose()
    {
        DbContext.Dispose();
        base.Dispose();
    }

    [Fact(Skip = SkipMessage)]
    public void Test_A_Unidirectional_1_To_0to1_Association()
    {
        var src = new A_RequiredComposite();
        DbContext.A_RequiredComposites.Add(src);
        DbContext.SaveChanges();

        var dst = new A_OptionalDependent();
        src.A_OptionalDependent = dst;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.A_RequiredComposites.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.A_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));
    }

    [Fact(Skip = SkipMessage)]
    public void Test_B_Unidirectional_0to1_To_0to1_Association()
    {
        var src = new B_OptionalAggregate();
        DbContext.B_OptionalAggregates.Add(src);
        DbContext.SaveChanges();

        var dst = new B_OptionalDependent();
        DbContext.B_OptionalDependents.Add(dst);
        DbContext.SaveChanges();

        src.B_OptionalDependent = dst;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.B_OptionalAggregates.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.B_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));

        src.B_OptionalDependent = null;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.B_OptionalAggregates.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.B_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));
    }

    [Fact(Skip = SkipMessage)]
    public void Test_C_Unidirectional_1_To_Many_Association()
    {
        var src = new C_RequiredComposite();
        DbContext.C_RequiredComposites.Add(src);
        DbContext.SaveChanges();

        var dstList = new List<C_MultipleDependent>
        {
            new C_MultipleDependent(),
            new C_MultipleDependent()
        };

        dstList.ForEach(x => src.C_MultipleDependents.Add(x));
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.C_RequiredComposites.SingleOrDefault(p => p.Id == src.Id));
        Assert.Equal(dstList.Count, DbContext.C_MultipleDependents.Count(p => dstList.Contains(p)));

        Assert.Throws<DbUpdateException>(() =>
        {
            var orphan = new C_MultipleDependent();
            DbContext.C_MultipleDependents.Add(orphan);
            DbContext.SaveChanges();
        });
    }

    [Fact(Skip = SkipMessage)]
    public void Test_D_Unidirectional_0to1_To_Many_Association()
    {
        var src = new D_OptionalAggregate();
        DbContext.D_OptionalAggregates.Add(src);
        DbContext.SaveChanges();

        var dstList = new List<D_MultipleDependent>
        {
            new D_MultipleDependent(),
            new D_MultipleDependent()
        };

        dstList.ForEach(x => src.D_MultipleDependents.Add(x));
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.D_OptionalAggregates.SingleOrDefault(p => p.Id == src.Id));
        Assert.Equal(dstList.Count, DbContext.D_MultipleDependents.Count(p => dstList.Contains(p)));

        dstList.ForEach(x => x.D_OptionalAggregateId = null);
        DbContext.SaveChanges();

        Assert.Equal(dstList.Count, DbContext.D_MultipleDependents.Count(p => dstList.Contains(p)));
    }

    [Fact(Skip = SkipMessage)]
    public void Test_E_Bidirectional_1_To_1_Association()
    {
        var src = new E_RequiredCompositeNav();
        DbContext.E_RequiredCompositeNavs.Add(src);

        var dst = new E_RequiredDependent();
        src.E_RequiredDependent = dst;

        DbContext.SaveChanges();

        Assert.NotNull(DbContext.E_RequiredCompositeNavs.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.E_RequiredDependents.SingleOrDefault(p => p.Id == dst.Id));

        Assert.Throws<InvalidOperationException>(() =>
        {
            DbContext.E_RequiredCompositeNavs.Add(new E_RequiredCompositeNav());
            DbContext.SaveChanges();
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            DbContext.E_RequiredDependents.Add(new E_RequiredDependent());
            DbContext.SaveChanges();
        });
    }

    [Fact(Skip = SkipMessage)]
    public void Test_F_Bidirectional_0to1_To_0to1_Association()
    {
        var src = new F_OptionalAggregateNav();
        DbContext.F_OptionalAggregateNavs.Add(src);
        DbContext.SaveChanges();

        var dst = new F_OptionalDependent();
        DbContext.F_OptionalDependents.Add(dst);
        DbContext.SaveChanges();

        src.F_OptionalDependent = dst;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.F_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.F_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));

        src.F_OptionalDependent = null;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.F_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.F_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));
    }

    [Fact(Skip = SkipMessage)]
    public void Test_G_Bidirectional_1_To_Many_Association()
    {
        var src = new G_RequiredCompositeNav();
        DbContext.G_RequiredCompositeNavs.Add(src);
        DbContext.SaveChanges();

        var dstList = new List<G_MultipleDependent>
        {
            new G_MultipleDependent(),
            new G_MultipleDependent()
        };

        dstList.ForEach(x => src.G_MultipleDependents.Add(x));
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.G_RequiredCompositeNavs.SingleOrDefault(p => p.Id == src.Id));
        Assert.Equal(dstList.Count, DbContext.G_MultipleDependents.Count(p => dstList.Contains(p)));

        Assert.Throws<DbUpdateException>(() =>
        {
            var orphan = new G_MultipleDependent();
            DbContext.G_MultipleDependents.Add(orphan);
            DbContext.SaveChanges();
        });
    }

    [Fact(Skip = SkipMessage)]
    public void Test_H_Bidirectional_0to1_To_Many_Association()
    {
        var src = new H_OptionalAggregateNav();
        DbContext.H_OptionalAggregateNavs.Add(src);
        DbContext.SaveChanges();

        var dstList = new List<H_MultipleDependent>
        {
            new H_MultipleDependent(),
            new H_MultipleDependent()
        };

        dstList.ForEach(x => src.H_MultipleDependents.Add(x));
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.H_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id));
        Assert.Equal(dstList.Count, DbContext.H_MultipleDependents.Count(p => dstList.Contains(p)));

        dstList.ForEach(x => x.H_OptionalAggregateNavId = null);
        DbContext.SaveChanges();

        Assert.Equal(dstList.Count, DbContext.H_MultipleDependents.Count(p => dstList.Contains(p)));
    }

    [Fact(Skip = SkipMessage)]
    public void Test_J_Unidirectional_Many_To_1_Association()
    {
        var dst = new J_RequiredDependent();
        DbContext.J_RequiredDependents.Add(dst);
        DbContext.SaveChanges();

        var srcList = new List<J_MultipleAggregate>
        {
            new J_MultipleAggregate(),
            new J_MultipleAggregate()
        };
        srcList.ForEach(x =>
        {
            DbContext.J_MultipleAggregates.Add(x);
            x.J_RequiredDependent = dst;
        });
        DbContext.SaveChanges();
        
        Assert.NotNull(DbContext.J_RequiredDependents.SingleOrDefault(p => p.Id == dst.Id));
        Assert.Equal(srcList.Count, DbContext.J_MultipleAggregates.Count(p => srcList.Contains(p)));
        
        Assert.Throws<DbUpdateException>(() =>
        {
            var orphan = new J_MultipleAggregate();
            DbContext.J_MultipleAggregates.Add(orphan);
            DbContext.SaveChanges();
        });
    }

    // TODO: Self reference tests - once ready
}