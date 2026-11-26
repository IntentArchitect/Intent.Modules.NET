using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Intent.Modules.Common.DriftEngine.Stacks.Ui;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// The shared finder behind a User Interface Uses from a plain class (<see cref="Dependencies"/>): what C# front ends (Blazor's
/// classes, a MAUI view model) read as one type depending on another, each way the designer's Uses names.
/// </summary>
public class DependenciesTests
{
    private static readonly CodeGraph Graph = ReadAll(
        ("src/App/Services/Basket.cs", """
            namespace Shop.App.Services;

            public interface IBasketState { }

            public class BasketState : IBasketState, IDisposable
            {
                public void Dispose() { }
            }

            public interface IIdentityService { Task<string> GetUserAsync(); }

            public class IdentityService : IIdentityService
            {
                public Task<string> GetUserAsync() => Task.FromResult("");
            }

            public interface IAppEnvironment { IIdentityService IdentityService { get; } }
            """),
        ("src/App/Legacy/BasketState.cs", """
            namespace Shop.App.Legacy;

            public class BasketState { }
            """),
        ("src/App/ViewModels/ViewModels.cs", """
            using Shop.App.Services;

            namespace Shop.App.ViewModels;

            public class FieldHolder
            {
                private readonly IBasketState _basket = null!;
            }

            public class PrimaryHolder(BasketState basket)
            {
            }

            public class ListHolder
            {
                public IReadOnlyList<BrandViewModel> Brands { get; } = [];
            }

            public class BrandViewModel { }

            public class Creator
            {
                public object Make() => new BrandViewModel();
            }

            public class ThroughProperty(IAppEnvironment environment)
            {
                public Task<string> LoadAsync() => environment.IdentityService.GetUserAsync();
            }

            public class LibraryOnly(IDisposable subscription)
            {
            }

            public class OtherNamespace(Shop.App.Legacy.BasketState basket)
            {
            }
            """));

    [Fact]
    public void A_field_of_an_interface_the_used_type_implements_is_a_dependency()
    {
        var found = Dependencies.Find(Graph, Type("FieldHolder"), Type("BasketState", "Shop.App.Services"));

        Assert.IsType<MemberNode>(found?.Node);
        Assert.Equal("_basket", ((MemberNode)found!.Node).Name);
    }

    [Fact]
    public void A_primary_constructor_parameter_is_a_dependency()
    {
        var found = Dependencies.Find(Graph, Type("PrimaryHolder"), Type("BasketState", "Shop.App.Services"));

        Assert.Equal("basket", Assert.IsType<ParameterNode>(found?.Node).Name);
    }

    [Fact]
    public void A_generic_type_over_the_used_type_is_a_dependency()
    {
        var found = Dependencies.Find(Graph, Type("ListHolder"), Type("BrandViewModel"));

        Assert.Equal("Brands", Assert.IsType<MemberNode>(found?.Node).Name);
    }

    [Fact]
    public void An_object_the_type_creates_is_a_dependency()
    {
        var found = Dependencies.Find(Graph, Type("Creator"), Type("BrandViewModel"));

        Assert.IsType<CreationNode>(found?.Node);
    }

    [Fact]
    public void A_call_through_a_dependencys_property_typed_by_the_used_type_is_a_dependency()
    {
        var found = Dependencies.Find(Graph, Type("ThroughProperty"), Type("IdentityService"));

        Assert.Equal("GetUserAsync", Assert.IsType<CallNode>(found?.Node).Name);
    }

    [Fact]
    public void A_library_interface_or_another_type_of_the_name_is_no_dependency()
    {
        Assert.Null(Dependencies.Find(Graph, Type("LibraryOnly"), Type("BasketState", "Shop.App.Services")));
        Assert.Null(Dependencies.Find(Graph, Type("OtherNamespace"), Type("BasketState", "Shop.App.Services")));
        Assert.NotNull(Dependencies.Find(Graph, Type("OtherNamespace"), Type("BasketState", "Shop.App.Legacy")));
    }

    private static TypeNode Type(string name, string? ns = null) => Graph.Types.Single(x => x.Name == name && (ns is null || x.Namespace == ns));

    private static CodeGraph ReadAll(params (string Path, string Source)[] files)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-dependencies-").FullName;
        try
        {
            foreach (var (path, source) in files)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var graph = new CodeGraph();
            var language = new CSharpExtension().Languages.Single();
            language.Read(root, files.Select(x => x.Path).Where(language.Reads).ToList(), graph, TestContext.Current.CancellationToken);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
