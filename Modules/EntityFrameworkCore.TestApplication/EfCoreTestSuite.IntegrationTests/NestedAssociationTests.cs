using System;
using System.Linq;
using EfCoreTestSuite.IntentGenerated.Core;
using EfCoreTestSuite.IntentGenerated.Entities.NestedAssociations;
using Xunit;
using Xunit.Abstractions;

namespace EfCoreTestSuite.IntegrationTests;

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

        branch.Leaves.Add(new Leaf() { LeafAttribute = "Leaf 1" });
        branch.Leaves.Add(new Leaf() { LeafAttribute = "Leaf 2" });

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
    }
}