using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.NestedAssociations;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using Intent.IntegrationTest.EfCore.SqlServer.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.SqlServer;

public class NestedAssociationTests : SharedDatabaseFixture<ApplicationDbContext, NestedAssociationTests>
{
    public NestedAssociationTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [IgnoreOnCiBuildFact]
    public void Test_NestedAssociations()
    {
        var tree = new Tree();
        tree.TreeAttribute = "Tree Attribute";
        DbContext.Trees.Add(tree);

        var branch = new Branch();
        branch.BranchAttribute = tree.TreeAttribute + "_Branch_1";
        tree.Branches.Add(branch);

        var texture = new Texture();
        texture.TextureAttribute = "Texture Attribute";
        branch.Texture = texture;
        DbContext.Textures.Add(texture);

        branch.Internode = new Internode() { InternodeAttribute = "Internode Attribute" };

        var onlySun = new Sun();
        DbContext.Suns.Add(onlySun);
        
        var leaf1 = new Leaf() { LeafAttribute = "Leaf 1" };
        branch.Leaves.Add(leaf1);
        leaf1.Worms.Add(new Worm() { Color = "Red" });
        leaf1.Sun = onlySun;
        
        var leaf2 = new Leaf() { LeafAttribute = "Leaf 2" };
        branch.Leaves.Add(leaf2);
        leaf2.Worms.Add(new Worm() { Color = "Green" });
        leaf2.Sun = onlySun;

        var inhabitant1 = new Inhabitant();
        inhabitant1.InhabitantAttribute = "Inhabitant 1";
        DbContext.Inhabitants.Add(inhabitant1);
        branch.Inhabitants.Add(inhabitant1);

        var inhabitant2 = new Inhabitant();
        inhabitant2.InhabitantAttribute = "Inhabitant 2";
        DbContext.Inhabitants.Add(inhabitant2);
        branch.Inhabitants.Add(inhabitant2);

        DbContext.SaveChanges();

        var retrievedTree = DbContext.Trees.SingleOrDefault(p => p.Id == tree.Id);
        Assert.NotNull(retrievedTree);
        Assert.Equal(tree.TreeAttribute, retrievedTree.TreeAttribute);
        var retrievedBranch = retrievedTree.Branches.SingleOrDefault();
        Assert.NotNull(retrievedBranch);
        Assert.Equal(branch.BranchAttribute, retrievedBranch.BranchAttribute);
        Assert.Equal(branch.Internode.InternodeAttribute, retrievedBranch.Internode.InternodeAttribute);
        Assert.Equal(branch.Texture.TextureAttribute, retrievedBranch.Texture.TextureAttribute);
        Assert.Equal(branch.Inhabitants.Count, retrievedBranch.Inhabitants.Count);
        Assert.Equal(branch.Leaves.Count, retrievedBranch.Leaves.Count);
        Assert.Equal(onlySun, leaf1.Sun);
        Assert.Equal(onlySun, leaf2.Sun);
        Assert.Equal(1, leaf1.Worms.Count);
        Assert.Equal(1, leaf2.Worms.Count);
        Assert.Contains(leaf1.Worms, x => x.Color == "Red");
        Assert.Contains(leaf2.Worms, x => x.Color == "Green");

        branch.Leaves.Remove(leaf1);
        branch.Leaves.Remove(leaf2);
        DbContext.SaveChanges();

        Assert.Equal(0, branch.Leaves.Count);
        Assert.Contains(onlySun, DbContext.Suns);
        Assert.Equal(2, DbContext.Worms.Count());
    }
}