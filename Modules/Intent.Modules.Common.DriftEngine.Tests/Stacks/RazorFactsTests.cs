using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// What the C# language reads from a Razor component (code-reader.md, Readers): the C# the Razor compiler generates for it,
/// as one type with its code-behind, each node placed where it is written.
/// </summary>
public class RazorFactsTests
{
    private static readonly (string Path, string Source)[] Files =
    [
        ("src/Web/Web.csproj", """
            <Project Sdk="Microsoft.NET.Sdk.Razor">
              <PropertyGroup>
                <RootNamespace>Shop.Web</RootNamespace>
                <Nullable>enable</Nullable>
              </PropertyGroup>
            </Project>
            """),
        ("src/Web/_Imports.razor", """
            @using Shop.Web.Services
            @using Microsoft.AspNetCore.Components.Web
            """),
        ("src/Web/Pages/_Imports.razor", """
            @layout MainLayout
            @attribute [Authorize]
            """),
        ("src/Web/Pages/Orders.razor", """
            @page "/orders"
            @page "/orders/{Status}"
            @using Shop.Web.Models
            @inject OrderService Orders
            @implements IDisposable
            @attribute [StreamRendering]

            <h1>Orders</h1>
            @foreach (var order in orders)
            {
                <p>@order.Name</p>
            }

            @code {
                private List<OrderDto> orders = [];

                [Parameter] public string? Status { get; set; }

                protected override async Task OnInitializedAsync() => orders = await Orders.ListAsync(Status);

                public void Dispose() { }

                private sealed class Row { public string Name { get; set; } = ""; }
            }
            """),
        ("src/Web/Pages/Orders.razor.cs", """
            namespace Shop.Web.Pages;

            public partial class Orders
            {
                [Inject] public NavigationManager Nav { get; set; } = default!;

                private void Open(OrderDto order) => Nav.NavigateTo($"orders/{order.Id}");
            }
            """),
        ("src/Web/Services/OrderService.cs", """
            namespace Shop.Web.Services;

            public class OrderService
            {
                public Task<List<OrderDto>> ListAsync(string? status) => null!;
            }
            """),
        ("src/Web/Models/OrderDto.cs", """
            namespace Shop.Web.Models;

            public record OrderDto(Guid Id, string Name);
            """),
        ("src/Web/Shared/Grid.razor", """
            @namespace Shop.Web.Ui
            @typeparam TItem where TItem : class
            @inherits GridBase<TItem>

            <table></table>
            """),
        ("src/Web/Shared/GridBase.cs", """
            namespace Shop.Web.Ui;

            public abstract class GridBase<T> : ComponentBase { }
            """),
        ("src/Web/Broken.razor", """
            <p>@(</p>

            @code {
                private void Broken( { }
            }
            """)
    ];

    private static readonly CodeGraph Graph = ReadAll(Files);

    [Fact]
    public void A_component_is_one_type_with_its_code_behind_in_its_project_and_folder_namespace()
    {
        var orders = Type("Orders");

        Assert.Equal("component", orders.Kind);
        Assert.Equal("Shop.Web.Pages", orders.Namespace);
        Assert.Equal(["src/Web/Pages/Orders.razor", "src/Web/Pages/Orders.razor.cs"], orders.Files);
        Assert.Equal(new Span("src/Web/Pages/Orders.razor", 1, 1, 24, 2), orders.At);
        Assert.Equal("public", orders.Access);
        Assert.StartsWith("@page\"/orders\"@page\"/orders/{Status}\"@using Shop.Web.Models@inject OrderService Orders", orders.Text);
    }

    [Fact]
    public void Its_members_are_its_code_and_injects_and_its_code_behind_s_not_what_the_compiler_writes_for_its_markup()
    {
        Assert.Equal(["orders", "Status", "OnInitializedAsync", "Dispose", "Orders", "Nav", "Open"], Type("Orders").Members.Select(x => x.Name));
    }

    [Fact]
    public void Directives_are_the_code_the_compiler_sees_each_where_it_is_written()
    {
        var orders = Type("Orders");
        var attributes = orders.Attributes.Select(x => $"{x.Name}({string.Join(", ", x.Arguments)}) {x.At.File}:{x.At.Line}").ToList();

        Assert.Equal(
        [
            "Authorize() src/Web/Pages/_Imports.razor:2",
            "StreamRendering() src/Web/Pages/Orders.razor:6",
            "Layout(typeof(MainLayout)) src/Web/Pages/_Imports.razor:1",
            "Route(/orders) src/Web/Pages/Orders.razor:1",
            "Route(/orders/{Status}) src/Web/Pages/Orders.razor:2"
        ], attributes);
        Assert.Equal(["Microsoft.AspNetCore.Components.ComponentBase", "IDisposable"], orders.BaseTypes.Select(x => x.Text));
    }

    [Fact]
    public void An_inject_is_a_property_placed_at_its_directive_with_its_directive_as_its_text()
    {
        var inject = Member(Type("Orders"), "Orders");

        Assert.Equal("property", inject.Kind);
        Assert.Equal("OrderService", inject.Type!.Name);
        Assert.Equal(["Inject"], inject.Attributes.Select(x => x.Name));
        Assert.Equal(new Span("src/Web/Pages/Orders.razor", 4, 1, 4, 28), inject.At);
        Assert.Equal("@inject OrderService Orders", inject.Text);
    }

    [Fact]
    public void Code_is_where_it_is_written_with_its_nullability_body_and_call_targets()
    {
        var orders = Type("Orders");
        var status = Member(orders, "Status");
        var initialized = Member(orders, "OnInitializedAsync");
        var call = initialized.Calls.Single(x => x.Name == "ListAsync");

        Assert.Equal(new Span("src/Web/Pages/Orders.razor", 17, 5), status.At with { EndLine = null, EndColumn = null });
        Assert.Equal("true", status.Type!.Nullable);
        Assert.Equal("[Parameter]public string?Status{get;set;}", status.Text);
        Assert.Equal(19, initialized.At.Line);
        Assert.Equal(("src/Web/Pages/Orders.razor", 19), (call.At.File, call.At.Line));
        Assert.Equal(("OrderService", "ListAsync"), (call.Target!.Type.Name, call.Target.Member.Name));
        Assert.Equal(("NavigationManager", "src/Web/Pages/Orders.razor.cs"), (Member(orders, "Nav").Type!.Name, Member(orders, "Open").At.File));
    }

    [Fact]
    public void A_type_its_code_declares_is_a_type_of_its_own()
    {
        var row = Type("Row");

        Assert.Equal(("class", "Shop.Web.Pages"), (row.Kind, row.Namespace));
        Assert.Equal(23, row.At.Line);
    }

    [Fact]
    public void Its_own_usings_are_its_imports_and_its__Imports_razor_files_apply_to_it()
    {
        var file = Graph.Files.Single(x => x.Path == "src/Web/Pages/Orders.razor");
        var imports = Graph.Files.Single(x => x.Path == "src/Web/_Imports.razor");

        Assert.Equal(["@using Shop.Web.Models"], file.Imports.Select(x => x.Text));
        Assert.Equal(("Shop.Web.Models", 3), (file.Imports[0].Target, file.Imports[0].At.Line));
        Assert.Equal(["src/Web/_Imports.razor", "src/Web/Pages/_Imports.razor"], file.ImportsFrom);
        Assert.Equal(["Shop.Web.Models", "Shop.Web.Services", "Microsoft.AspNetCore.Components.Web"], Graph.ImportsOf(file).Select(x => x.Target));
        Assert.Equal(["Shop.Web.Pages"], file.Namespaces);
        Assert.Equal((2, 0), (imports.Imports.Count, imports.Namespaces.Count));
        Assert.DoesNotContain(Graph.Types, x => x.Name == "_Imports");
    }

    [Fact]
    public void A_generic_component_has_its_namespace_type_parameters_and_base_as_its_directives_say()
    {
        var grid = Type("Grid");

        Assert.Equal(("component", "Shop.Web.Ui"), (grid.Kind, grid.Namespace));
        Assert.Equal(["TItem"], grid.TypeParameters);
        Assert.Equal(["GridBase<TItem>"], grid.BaseTypes.Select(x => x.Text));
        Assert.Equal("GridBase", Graph.Bases(grid).Single().Name);
    }

    [Fact]
    public void A_component_that_doesn_t_compile_is_still_read()
    {
        Assert.Empty(Graph.Failures);
        Assert.Equal(("component", "Shop.Web"), (Type("Broken").Kind, Type("Broken").Namespace));
    }

    private static TypeNode Type(string name) => Graph.Types.Single(x => x.Name == name);

    private static MemberNode Member(TypeNode type, string name) => type.Members.Single(x => x.Name == name);

    private static CodeGraph ReadAll((string Path, string Source)[] files)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-razor-").FullName;
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
            language.Read(root, files.Select(x => x.Path).Where(language.Reads).ToList(), graph);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
