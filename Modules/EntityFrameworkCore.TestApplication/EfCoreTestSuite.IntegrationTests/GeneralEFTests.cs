using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreTestSuite.IntentGenerated.Core;
using EfCoreTestSuite.IntentGenerated.Entities.Associations;
using EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfCoreTestSuite.IntegrationTests;

public class GeneralEFTests : SharedDatabaseFixture<ApplicationDbContext>
{
    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_A_Unidirectional_1_To_0to1_Association()
    {
        var src = new A_RequiredComposite();
        DbContext.A_RequiredComposites.Add(src);
        DbContext.SaveChanges();

        var dst = new A_OptionalDependent();
        src.A_OptionalDependent = dst;
        DbContext.SaveChanges();

        var owner = DbContext.A_RequiredComposites.SingleOrDefault(p => p.Id == src.Id);
        Assert.NotNull(owner);
        Assert.NotNull(owner.A_OptionalDependent);
    }

    [Fact(Skip = Helpers.SkipMessage)]
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
        Assert.Equal(dst, DbContext.B_OptionalAggregates.SingleOrDefault(p => p.Id == src.Id)?.B_OptionalDependent);

        src.B_OptionalDependent = null;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.B_OptionalAggregates.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.B_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));
        Assert.Null(DbContext.B_OptionalAggregates.SingleOrDefault(p => p.B_OptionalDependentId == dst.Id));
    }

    [Fact(Skip = Helpers.SkipMessage)]
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

    [Fact(Skip = Helpers.SkipMessage)]
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

    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_E_Bidirectional_1_To_1_Association()
    {
        var src = new E_RequiredCompositeNav();
        DbContext.E_RequiredCompositeNavs.Add(src);

        var dst = new E_RequiredDependent();
        src.E_RequiredDependent = dst;

        DbContext.SaveChanges();

        var owner = DbContext.E_RequiredCompositeNavs.SingleOrDefault(p => p.Id == src.Id);
        Assert.NotNull(owner);
        Assert.NotNull(owner.E_RequiredDependent);

        // Due to EF Core's new OwnsOne configuration, there currently isn't a way to make
        // this relationship Required.
        // Assert.Throws<InvalidOperationException>(() =>
        // {
        //     DbContext.E_RequiredCompositeNavs.Add(new E_RequiredCompositeNav());
        //     DbContext.SaveChanges();
        // });

        Assert.Throws<InvalidOperationException>(() =>
        {
            DbContext.Set<E_RequiredDependent>().Add(new E_RequiredDependent());
            DbContext.SaveChanges();
        });
    }

    [Fact(Skip = Helpers.SkipMessage)]
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
        Assert.Equal(dst, DbContext.F_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id)?.F_OptionalDependent);
        Assert.Equal(src, DbContext.F_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id)?.F_OptionalAggregateNav);

        src.F_OptionalDependent = null;
        DbContext.SaveChanges();

        Assert.NotNull(DbContext.F_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id));
        Assert.NotNull(DbContext.F_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id));
        Assert.Null(DbContext.F_OptionalAggregateNavs.SingleOrDefault(p => p.Id == src.Id)?.F_OptionalDependent);
        Assert.Null(DbContext.F_OptionalDependents.SingleOrDefault(p => p.Id == dst.Id)?.F_OptionalAggregateNav);
    }

    [Fact(Skip = Helpers.SkipMessage)]
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

    [Fact(Skip = Helpers.SkipMessage)]
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

    [Fact(Skip = Helpers.SkipMessage)]
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

    [Fact(Skip = Helpers.SkipMessage)]
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

    [Fact(Skip = Helpers.SkipMessage)]
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

    [Fact(Skip = Helpers.SkipMessage)]
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

    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_PK_PrimaryKeyInt()
    {
        var pk = new PK_PrimaryKeyInt();
        DbContext.PK_PrimaryKeyInts.Add(pk);
        DbContext.SaveChanges();
        
        Assert.Equal(1, pk.PrimaryKeyId);
    }
    
    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_PK_PrimaryKeyLong()
    {
        var pk = new PK_PrimaryKeyLong();
        DbContext.PK_PrimaryKeyLongs.Add(pk);
        DbContext.SaveChanges();
        
        Assert.Equal(1, pk.PrimaryKeyLong);
    }

    [Fact(Skip = Helpers.SkipMessage)]
    public void Test_PK_A_CompositeKeys_FK_A_CompositeForeignKeys()
    {
        var pk = new PK_A_CompositeKey();
        pk.CompositeKeyA = Guid.NewGuid();
        pk.CompositeKeyB = Guid.NewGuid();
        DbContext.PK_A_CompositeKeys.Add(pk);
        
        var fk = new FK_A_CompositeForeignKey();
        fk.PK_CompositeKey = pk;
        DbContext.FK_A_CompositeForeignKeys.Add(fk);

        DbContext.SaveChanges();
        
        Assert.Equal(pk, fk.PK_CompositeKey);
        Assert.Equal(pk.CompositeKeyA, fk.ForeignCompositeKeyA);
        Assert.Equal(pk.CompositeKeyB, fk.ForeignCompositeKeyB);
    }
    
    [Fact(Skip = Helpers.SkipMessage)]
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
}