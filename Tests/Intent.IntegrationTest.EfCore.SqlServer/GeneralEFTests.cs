using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using FluentAssertions;
using Intent.IntegrationTest.EfCore.SqlServer.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.SqlServer;

public class GeneralEFTests : SharedDatabaseFixture<ApplicationDbContext, GeneralEFTests>
{
    public GeneralEFTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public void Test_A_Unidirectional_1_To_0to1_Association()
    {
        var src = new A_RequiredComposite() { RequiredCompAttr = "test 1" };
        DbContext.A_RequiredComposites.Add(src);
        DbContext.SaveChanges();

        var dst = new A_OptionalDependent() { OptionalDepAttr = "test 2" };
        src.A_OptionalDependent = dst;
        DbContext.SaveChanges();

        var owner = DbContext.A_RequiredComposites.SingleOrDefault(p => p.Id == src.Id);
        Assert.NotNull(owner);
        Assert.NotNull(owner.A_OptionalDependent);
    }

    [IgnoreOnCiBuildFact]
    public void Test_B_Unidirectional_0to1_To_0to1_Association()
    {
        var src = new B_OptionalAggregate() { OptionalAggrAttr = "test 1" };
        DbContext.B_OptionalAggregates.Add(src);
        DbContext.SaveChanges();

        var dst = new B_OptionalDependent() { OptionalDepAttr = "test 2" };
        DbContext.B_OptionalDependents.Add(dst);
        DbContext.SaveChanges();

        src.B_OptionalDependent = dst;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.B_OptionalAggregates.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.B_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));
        Assert.Equal(dst, DbContext.B_OptionalAggregates.SingleOrDefault(p => p.Id == src.Id)?.B_OptionalDependent);

        src.B_OptionalDependent = null;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.B_OptionalAggregates.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.B_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));
        Assert.Null(DbContext.B_OptionalAggregates.SingleOrDefault(p => p.B_OptionalDependentId == dst.Id));
    }

    [IgnoreOnCiBuildFact]
    public void Test_C_Unidirectional_1_To_Many_Association()
    {
        var src = new C_RequiredComposite() { RequiredCompAttr = "test 1" };
        DbContext.C_RequiredComposites.Add(src);
        DbContext.SaveChanges();

        var dstList = new List<C_MultipleDependent>
        {
            new C_MultipleDependent() { MultipleDepAttr = "test 2" },
            new C_MultipleDependent() { MultipleDepAttr = "test 3" }
        };

        src.C_MultipleDependents.AddRange(dstList);
        DbContext.SaveChanges();

        var owner = DbContext.C_RequiredComposites.SingleOrDefault(p => p.Id == src.Id);
        Assert.NotNull(owner);
        Assert.Equal(dstList.Count, owner.C_MultipleDependents.Count(p => dstList.Contains(p)));

        // I previously had an orphan check here. Keeping just in case.
        // Assert.Throws<DbUpdateException>(() =>
        // {
        //     var orphan = new C_MultipleDependent();
        //     DbContext.Set<C_MultipleDependent>().Add(orphan);
        //     
        //     DbContext.SaveChanges();
        // });
    }

    [IgnoreOnCiBuildFact]
    public void Test_D_Unidirectional_0to1_To_Many_Association()
    {
        var src = new D_OptionalAggregate() { OptionalAggrAttr = "test 1" };
        DbContext.D_OptionalAggregates.Add(src);
        DbContext.SaveChanges();

        var dstList = new List<D_MultipleDependent>
        {
            new D_MultipleDependent() { MultipleDepAttr = "test 2" },
            new D_MultipleDependent() { MultipleDepAttr = "test 3" }
        };

        src.D_MultipleDependents.AddRange(dstList);
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.D_OptionalAggregates.SingleOrDefault(p => p.Id == src.Id));
        Assert.Equal(dstList.Count, DbContext.D_MultipleDependents.Count(p => dstList.Contains(p)));

        dstList.ForEach(x => x.D_OptionalAggregateId = null);
        DbContext.SaveChanges();

        Assert.Equal(dstList.Count, DbContext.D_MultipleDependents.Count(p => dstList.Contains(p)));
    }

    [IgnoreOnCiBuildFact]
    public void Test_E_Bidirectional_1_To_1_Association()
    {
        var src = new E_RequiredCompositeNav() { RequiredCompNavAttr = "test 1" };
        DbContext.E_RequiredCompositeNavs.Add(src);

        var dst = new E_RequiredDependent() { RequiredDepAttr = "test 2" };
        src.E_RequiredDependent = dst;

        DbContext.SaveChanges();

        var owner = DbContext.E_RequiredCompositeNavs.SingleOrDefault(p => p.Id == src.Id);
        Assert.NotNull(owner);
        Assert.NotNull(owner.E_RequiredDependent);

        Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(() =>
        {
            DbContext.E_RequiredCompositeNavs.Add(new E_RequiredCompositeNav() { RequiredCompNavAttr = "test 3" });
            DbContext.SaveChanges();
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            DbContext.Set<E_RequiredDependent>().Add(new E_RequiredDependent());
            DbContext.SaveChanges();
        });
    }

    [IgnoreOnCiBuildFact]
    public void Test_F_Bidirectional_0to1_To_0to1_Association()
    {
        var src = new F_OptionalAggregateNav() { OptionalAggrNavAttr = "test 1" };
        DbContext.F_OptionalAggregateNavs.Add(src);
        DbContext.SaveChanges();

        var dst = new F_OptionalDependent() { OptionalDepAttr = "test 2" };
        DbContext.F_OptionalDependents.Add(dst);
        DbContext.SaveChanges();

        src.F_OptionalDependent = dst;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.F_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.F_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));
        Assert.Equal(dst, DbContext.F_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id)?.F_OptionalDependent);
        Assert.Equal(src, DbContext.F_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id)?.F_OptionalAggregateNav);

        src.F_OptionalDependent = null;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.F_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.F_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));
        Assert.Null(DbContext.F_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id)?.F_OptionalDependent);
        Assert.Null(DbContext.F_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id)?.F_OptionalAggregateNav);
    }

    [IgnoreOnCiBuildFact]
    public void Test_G_Bidirectional_1_To_Many_Association()
    {
        var src = new G_RequiredCompositeNav() { ReqCompNavAttr = "test 1" };
        DbContext.G_RequiredCompositeNavs.Add(src);
        DbContext.SaveChanges();

        var dstList = new List<G_MultipleDependent>
        {
            new G_MultipleDependent() { MultipleDepAttr = "test 2" },
            new G_MultipleDependent() { MultipleDepAttr = "test 3" }
        };

        src.G_MultipleDependents.AddRange(dstList);
        DbContext.SaveChanges();

        var owner = DbContext.G_RequiredCompositeNavs.SingleOrDefault(p => p.Id == src.Id);
        Assert.NotNull(owner);
        Assert.Equal(dstList.Count, owner.G_MultipleDependents.Count(p => dstList.Contains(p)));

        // I previously had an orphan check here. Keeping just in case.
        // Assert.Throws<DbUpdateException>(() =>
        // {
        //     var orphan = new G_MultipleDependent();
        //     DbContext.Set<G_MultipleDependent>().Add(orphan);
        //     DbContext.SaveChanges();
        // });
    }

    [IgnoreOnCiBuildFact]
    public void Test_H_Bidirectional_0to1_To_Many_Association()
    {
        var src = new H_OptionalAggregateNav() { OptionalAggrNavAttr = "test 1" };
        DbContext.H_OptionalAggregateNavs.Add(src);
        DbContext.SaveChanges();

        var dstList = new List<H_MultipleDependent>
        {
            new H_MultipleDependent() { MultipleDepAttr = "test 2" },
            new H_MultipleDependent() { MultipleDepAttr = "test 3" }
        };

        src.H_MultipleDependents.AddRange(dstList);
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.H_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id));
        Assert.Equal(dstList.Count, DbContext.H_MultipleDependents.Count(p => dstList.Contains(p)));

        dstList.ForEach(x => x.H_OptionalAggregateNavId = null);
        DbContext.SaveChanges();

        Assert.Equal(dstList.Count, DbContext.H_MultipleDependents.Count(p => dstList.Contains(p)));
    }

    [IgnoreOnCiBuildFact]
    public void Test_J_Unidirectional_Many_To_1_Association()
    {
        var dst = new J_RequiredDependent() { ReqDepAttr = "test 1" };
        DbContext.J_RequiredDependents.Add(dst);
        DbContext.SaveChanges();

        var srcList = new List<J_MultipleAggregate>
        {
            new J_MultipleAggregate() { MultipleAggrAttr = "test 2" },
            new J_MultipleAggregate() { MultipleAggrAttr = "test 3" }
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

    [IgnoreOnCiBuildFact]
    public void Test_K_Unidirectional_Many_To_0to1_Association()
    {
        var root = new K_SelfReference() { SelfRefAttr = "test 1" };
        DbContext.K_SelfReferences.Add(root);
        DbContext.SaveChanges();

        var children = new List<K_SelfReference>
        {
            new K_SelfReference() { SelfRefAttr = "test 2" },
            new K_SelfReference() { SelfRefAttr = "test 3" }
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

    [IgnoreOnCiBuildFact]
    public void Test_L_Unidirectional_Many_To_Many_Association()
    {
        var listA = new List<L_SelfReferenceMultiple>
        {
            new L_SelfReferenceMultiple() { SelfRefMultipleAttr = "test 1" }
        };
        var listB = new List<L_SelfReferenceMultiple>
        {
            new L_SelfReferenceMultiple() { SelfRefMultipleAttr = "test 2" },
            new L_SelfReferenceMultiple() { SelfRefMultipleAttr = "test 3" }
        };
        var listC = new List<L_SelfReferenceMultiple>
        {
            new L_SelfReferenceMultiple() { SelfRefMultipleAttr = "test 4" },
            new L_SelfReferenceMultiple() { SelfRefMultipleAttr = "test 5" },
            new L_SelfReferenceMultiple() { SelfRefMultipleAttr = "test 6" }
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

    [IgnoreOnCiBuildFact]
    public void Test_M_Bidirectional_Many_To_0to1_Association()
    {
        var root = new M_SelfReferenceBiNav() { SelfRefBiNavAttr = "test 1" };
        DbContext.M_SelfReferenceBiNavs.Add(root);
        DbContext.SaveChanges();

        var children = new List<M_SelfReferenceBiNav>
        {
            new M_SelfReferenceBiNav() { SelfRefBiNavAttr = "test 2" },
            new M_SelfReferenceBiNav() { SelfRefBiNavAttr = "test 3" }
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

    [IgnoreOnCiBuildFact]
    public void Test_PK_PrimaryKeyInt()
    {
        var pk = new PK_PrimaryKeyInt();
        DbContext.PK_PrimaryKeyInts.Add(pk);
        DbContext.SaveChanges();

        Assert.Equal(1, pk.PrimaryKeyId);
    }

    [IgnoreOnCiBuildFact]
    public void Test_PK_PrimaryKeyLong()
    {
        var pk = new PK_PrimaryKeyLong();
        DbContext.PK_PrimaryKeyLongs.Add(pk);
        DbContext.SaveChanges();

        Assert.Equal(1, pk.PrimaryKeyLong);
    }

    [IgnoreOnCiBuildFact]
    public void Test_PK_A_CompositeKeys_FK_A_CompositeForeignKeys()
    {
        var pk = new PK_A_CompositeKey();
        pk.CompositeKeyA = Guid.NewGuid();
        pk.CompositeKeyB = Guid.NewGuid();
        DbContext.PK_A_CompositeKeys.Add(pk);

        var fk = new FK_A_CompositeForeignKey();
        fk.PK_A_CompositeKey = pk;
        DbContext.FK_A_CompositeForeignKeys.Add(fk);

        DbContext.SaveChanges();

        Assert.Equal(pk, fk.PK_A_CompositeKey);
        Assert.Equal(pk.CompositeKeyA, fk.PK_A_CompositeKeyCompositeKeyA);
        Assert.Equal(pk.CompositeKeyB, fk.PK_A_CompositeKeyCompositeKeyB);
    }

    [IgnoreOnCiBuildFact]
    public void Test_PK_B_CompositeKeys_FK_B_CompositeForeignKeys()
    {
        var pk = new PK_B_CompositeKey();
        pk.CompositeKeyA = Guid.NewGuid();
        pk.CompositeKeyB = Guid.NewGuid();
        DbContext.PK_B_CompositeKeys.Add(pk);

        var fk = new FK_B_CompositeForeignKey();
        fk.PK_CompositeKey = pk;
        DbContext.FK_B_CompositeForeignKeys.Add(fk);

        DbContext.SaveChanges();

        Assert.Equal(pk, fk.PK_CompositeKey);
        Assert.Equal(pk.CompositeKeyA, fk.PK_CompositeKeyCompositeKeyA);
        Assert.Equal(pk.CompositeKeyB, fk.PK_CompositeKeyCompositeKeyB);
    }

    [IgnoreOnCiBuildFact]
    public async Task Test_N_ComplexRoot()
    {
        var root = new N_ComplexRoot();
        root.ComplexAttr = "ComplexRoot_" + Guid.NewGuid();
        root.N_CompositeOne = new N_CompositeOne() { CompositeOneAttr = "Composite One" };
        root.N_CompositeTwo = new N_CompositeTwo() { CompositeTwoAttr = "Composite Two" };
        root.N_CompositeManies.Add(new N_CompositeMany() { ManyAttr = "Item 1" });
        root.N_CompositeManies.Add(new N_CompositeMany() { ManyAttr = "Item 2" });
        root.N_CompositeManies.Add(new N_CompositeMany() { ManyAttr = "Item 3" });

        DbContext.N_ComplexRoots.Add(root);
        await DbContext.SaveChangesAsync();

        var retrieved = DbContext.N_ComplexRoots.FirstOrDefault(p => p.Id == root.Id);
        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(root);
    }
}