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

    public const string SkipMessage = null;
    //public const string SkipMessage = "CI/CD not ready";

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
        Assert.Equal(1, DbContext.B_OptionalAggregates.Count(p => p.B_OptionalDependentId == dst.Id));

        src.B_OptionalDependent = null;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.B_OptionalAggregates.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.B_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));
        Assert.Equal(0, DbContext.B_OptionalAggregates.Count(p => p.B_OptionalDependentId == dst.Id));
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

        src.C_MultipleDependents.AddRange(dstList);
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

        src.D_MultipleDependents.AddRange(dstList);
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

        src.G_MultipleDependents.AddRange(dstList);
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

        src.H_MultipleDependents.AddRange(dstList);
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

    [Fact(Skip = SkipMessage)]
    public void Test_K_Unidirectional_Many_To_0to1_Association()
    {
        var root = new K_SelfReference();
        DbContext.K_SelfReferences.Add(root);
        DbContext.SaveChanges();

        var children = new List<K_SelfReference>
        {
            new K_SelfReference(),
            new K_SelfReference()
        };
        children.ForEach(x =>
        {
            DbContext.K_SelfReferences.Add(x);
            x.K_SelfReferenceAssociation = root;
        });
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.K_SelfReferences.SingleOrDefault(p => p.Id == root.Id));
        Assert.Equal(children.Count, DbContext.K_SelfReferences.Count(p => children.Contains(p)));
        Assert.Equal(children.Count, DbContext.K_SelfReferences.Count(p => p.K_SelfReferenceAssociationId == root.Id));
    }

    [Fact(Skip = SkipMessage)]
    public void Test_L_Unidirectional_Many_To_Many_Association()
    {
        var listA = new List<L_SelfReferenceMultiple>
        {
            new L_SelfReferenceMultiple()
        };
        var listB = new List<L_SelfReferenceMultiple>
        {
            new L_SelfReferenceMultiple(),
            new L_SelfReferenceMultiple()
        };
        var listC = new List<L_SelfReferenceMultiple>
        {
            new L_SelfReferenceMultiple(),
            new L_SelfReferenceMultiple(),
            new L_SelfReferenceMultiple()
        };

        listA.First().L_SelfReferenceMultiplesDst.AddRange(listB);
        listB.First().L_SelfReferenceMultiplesDst.AddRange(listA);
        listB.Last().L_SelfReferenceMultiplesDst.AddRange(listC);
        listC.ForEach(x => x.L_SelfReferenceMultiplesDst.AddRange(listB));

        DbContext.L_SelfReferenceMultiples.AddRange(listA);
        DbContext.L_SelfReferenceMultiples.AddRange(listB);
        DbContext.L_SelfReferenceMultiples.AddRange(listC);
        DbContext.SaveChanges();

        Assert.Equal(listB.Count, listA.First().L_SelfReferenceMultiplesDst.Count);
        Assert.Equal(listA.Count, listB.First().L_SelfReferenceMultiplesDst.Count);
        Assert.Equal(listC.Count, listB.Last().L_SelfReferenceMultiplesDst.Count);
        Assert.Equal(listB.Count * listC.Count, listC.Sum(x => x.L_SelfReferenceMultiplesDst.Count));
    }

    [Fact(Skip = SkipMessage)]
    public void Test_M_Bidirectional_Many_To_0to1_Association()
    {
        var root = new M_SelfReferenceBiNav();
        DbContext.M_SelfReferenceBiNavs.Add(root);
        DbContext.SaveChanges();

        var children = new List<M_SelfReferenceBiNav>
        {
            new M_SelfReferenceBiNav(),
            new M_SelfReferenceBiNav()
        };
        children.ForEach(x =>
        {
            DbContext.M_SelfReferenceBiNavs.Add(x);
            x.M_SelfReferenceBiNavDst = root;
        });
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.M_SelfReferenceBiNavs.SingleOrDefault(p => p.Id == root.Id));
        Assert.Equal(children.Count, DbContext.M_SelfReferenceBiNavs.Count(p => children.Contains(p)));
        Assert.Equal(children.Count, root.M_SelfReferenceBiNavs.Count);
    }
}