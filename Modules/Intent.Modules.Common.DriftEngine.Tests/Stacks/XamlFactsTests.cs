using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Intent.Modules.Common.DriftEngine.Stacks.Xaml;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// What the XAML language reads (code-reader.md, Readers): a view is the markup half of the C# class its <c>x:Class</c> names,
/// so it adds its file, markup and document and no type, and its code reader joins it to that class. Resource dictionaries and
/// the application's own XAML add nothing. The same for every XAML framework: a .NET MAUI view and a WPF window alike.
/// </summary>
public class XamlFactsTests
{
    private static readonly (string Path, string Source)[] Files =
    [
        ("src/Shop/Shop.csproj", """<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><UseMaui>true</UseMaui></PropertyGroup></Project>"""),
        ("src/Shop/App.xaml", """
            <?xml version="1.0" encoding="UTF-8" ?>
            <Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Shop.App">
                <Application.Resources><ResourceDictionary /></Application.Resources>
            </Application>
            """),
        ("src/Shop/App.xaml.cs", "namespace Shop; public partial class App : Application { }"),
        ("src/Shop/Platforms/Windows/App.xaml", """
            <maui:MauiWinUIApplication x:Class="Shop.WinUI.App"
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                xmlns:maui="using:Microsoft.Maui" />
            """),
        ("src/Shop/Resources/Colors.xaml", """
            <?xml version="1.0" encoding="UTF-8" ?>
            <ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
                <Color x:Key="Accent">#000000</Color>
            </ResourceDictionary>
            """),
        ("src/Shop/AppShell.xaml", """
            <?xml version="1.0" encoding="UTF-8" ?>
            <Shell
                x:Class="Shop.AppShell"
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                xmlns:views="clr-namespace:Shop.Views">
                <TabBar Route="Main">
                    <ShellContent
                        Title="CATALOG"
                        ContentTemplate="{DataTemplate views:CatalogView}"
                        Route="Catalog" />
                </TabBar>
            </Shell>
            """),
        ("src/Shop/AppShell.xaml.cs", "namespace Shop; public partial class AppShell : Shell { }"),
        ("src/Shop/Views/ContentPageBase.cs", "namespace Shop.Views; public abstract class ContentPageBase : ContentPage { }"),
        ("src/Shop/Views/CatalogView.xaml", """
            <?xml version="1.0" encoding="utf-8" ?>
            <views:ContentPageBase
                x:Class="Shop.Views.CatalogView"
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                xmlns:templates="clr-namespace:Shop.Views.Templates;assembly=Shop"
                xmlns:views="clr-namespace:Shop.Views"
                Title="Catalog">
                <CollectionView x:Name="products" ItemsSource="{Binding Products}">
                    <CollectionView.ItemTemplate>
                        <DataTemplate>
                            <templates:ProductTemplate />
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
                <Label>  Two
                    lines  </Label>
            </views:ContentPageBase>
            """),
        ("src/Shop/Views/CatalogView.xaml.cs", "namespace Shop.Views; public partial class CatalogView : IQueryAttributable { }"),
        ("src/Shop/Views/Templates/ProductTemplate.cs", "namespace Shop.Views.Templates; public class ProductTemplate : ContentView { }"),
        ("src/Desk/MainWindow.xaml", """
            <Window x:Class="Desk.MainWindow"
                    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid />
            </Window>
            """),
        ("src/Desk/MainWindow.xaml.cs", "namespace Desk; public partial class MainWindow { }"),
        ("src/Desk/Broken.xaml", "<Window x:Class=\"Desk.Broken\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><Grid></Window>")
    ];

    private static readonly CodeGraph Graph = ReadAll(Files);

    [Fact]
    public void A_view_adds_its_file_and_document_and_no_type()
    {
        Graph.Files.Where(x => x.Language == "xaml").Select(x => x.Path).Order(StringComparer.Ordinal)
            .ShouldBe(["src/Desk/MainWindow.xaml", "src/Shop/AppShell.xaml", "src/Shop/Views/CatalogView.xaml"]);
        Graph.Files.Single(x => x.Path == "src/Shop/Views/CatalogView.xaml").Namespaces.ShouldBe(["Shop.Views"]);
        Graph.Files.Single(x => x.Path == "src/Shop/Views/CatalogView.xaml").Project.ShouldBe("src/Shop/Shop.csproj");
        Graph.Types.Where(x => x.At.File.EndsWith(".xaml", StringComparison.Ordinal)).ShouldBeEmpty();
        XamlSyntax.Of(Graph)!.Documents["src/Shop/Views/CatalogView.xaml"].Class.ShouldBe("Shop.Views.CatalogView");
    }

    [Fact]
    public void The_applications_own_xaml_and_resource_dictionaries_add_nothing()
    {
        Graph.Files.Select(x => x.Path).ShouldNotContain("src/Shop/App.xaml");
        Graph.Files.Select(x => x.Path).ShouldNotContain("src/Shop/Platforms/Windows/App.xaml");
        Graph.Files.Select(x => x.Path).ShouldNotContain("src/Shop/Resources/Colors.xaml");
        XamlSyntax.Of(Graph)!.Documents.Keys.ShouldNotContain("src/Shop/App.xaml");
    }

    [Fact]
    public void A_file_that_isnt_well_formed_fails_alone()
    {
        Graph.Failures.Select(x => x.File).ShouldBe(["src/Desk/Broken.xaml"]);
    }

    [Fact]
    public void A_code_behind_gains_its_views_file_and_without_a_base_class_the_views_root_as_its_base()
    {
        var catalog = Type("CatalogView");
        catalog.Files.ShouldBe(["src/Shop/Views/CatalogView.xaml.cs", "src/Shop/Views/CatalogView.xaml"]);
        catalog.BaseTypes.Select(x => x.QualifiedName).ShouldBe(["Shop.Views.ContentPageBase", "IQueryAttributable"]);
        Graph.Bases(catalog).Select(x => x.Name).ShouldBe(["ContentPageBase"]);
        XamlSyntax.Of(Graph)!.DocumentOf(catalog)!.Path.ShouldBe("src/Shop/Views/CatalogView.xaml");

        // A code-behind that writes its base keeps it, and a WPF window's XAML 2006 x:Class joins it as MAUI's does.
        Type("AppShell").BaseTypes.Select(x => x.Name).ShouldBe(["Shell"]);
        Type("MainWindow").BaseTypes.Select(x => x.Name).ShouldBe(["Window"]);
        Type("MainWindow").Files.ShouldContain("src/Desk/MainWindow.xaml");
    }

    [Fact]
    public void Markup_is_one_shape_with_every_language()
    {
        var root = Graph.Files.Single(x => x.Path == "src/Shop/Views/CatalogView.xaml").Markup.Single();
        root.Tag.ShouldBe("views:ContentPageBase");
        root.Component!.QualifiedName.ShouldBe("Shop.Views.ContentPageBase");
        root.Attributes.Select(x => x.Name).ShouldBe(["x:Class", "Title"]);
        root.Attributes.Single(x => x.Name == "Title").Literal!.Value.ShouldBe("Catalog");

        var list = root.Children[0];
        list.Tag.ShouldBe("CollectionView");
        list.Component!.QualifiedName.ShouldBe("CollectionView");
        list.Attributes.Single(x => x.Name == "x:Name").Literal!.Value.ShouldBe("products");
        list.Attributes.Single(x => x.Name == "ItemsSource").Literal.ShouldBeNull();
        list.Children[0].Tag.ShouldBe("CollectionView.ItemTemplate");
        list.Children[0].Component.ShouldBeNull();
        root.Children[1].Content!.Value.ShouldBe("Two lines");

        // A tag of the code's own type resolves to it, as any type reference does.
        var tag = Graph.MarkupOf(Type("CatalogView")).Single(x => x.Tag == "templates:ProductTemplate");
        Graph.TypeFor(tag.Component!, Type("CatalogView")).ShouldBe(Type("ProductTemplate"));
    }

    [Fact]
    public void An_element_is_placed_from_its_start_tag_to_its_end_tag_and_an_attribute_where_it_starts()
    {
        var shell = Graph.Files.Single(x => x.Path == "src/Shop/AppShell.xaml").Markup.Single();
        shell.At.ShouldBe(new Span("src/Shop/AppShell.xaml", 2, 1, 13, 9));
        var content = shell.Children[0].Children[0];
        content.At.ShouldBe(new Span("src/Shop/AppShell.xaml", 8, 9, 11, 31));
        content.Attributes.Single(x => x.Name == "Route").At.ShouldBe(new Span("src/Shop/AppShell.xaml", 11, 13));
        content.Text.ShouldContain("Route=\"Catalog\"");
    }

    [Theory]
    [InlineData("{Binding Name}", "Binding", "Name", "")]
    [InlineData("{x:Type local:Routes}", "x:Type", "local:Routes", "")]
    [InlineData("{Binding Path=Total, Converter={StaticResource Money}, StringFormat='{0:C}'}", "Binding", null, "Path=Total;Converter={StaticResource Money};StringFormat={0:C}")]
    [InlineData("{RelativeSource AncestorType={x:Type vm:Catalog}}", "RelativeSource", null, "AncestorType={x:Type vm:Catalog}")]
    [InlineData("{AppThemeBinding Dark=\\#000, Light=#CCCCCC}", "AppThemeBinding", null, "Dark=#000;Light=#CCCCCC")]
    public void A_markup_extension_is_its_name_and_arguments(string value, string name, string? argument, string named)
    {
        var extension = XamlMarkupExtension.Parse(value)!;
        extension.Name.ShouldBe(name);
        extension.Argument.ShouldBe(argument);
        string.Join(";", extension.Named.Select(x => $"{x.Name}={x.Value.Text}")).ShouldBe(named);
    }

    [Fact]
    public void A_markup_extension_holds_the_ones_it_nests_and_an_escaped_value_is_text()
    {
        XamlMarkupExtension.Parse("{Binding Source={RelativeSource AncestorType={x:Type vm:Catalog}}}")!.DescendantsAndSelf().Select(x => x.Name)
            .ShouldBe(["Binding", "RelativeSource", "x:Type"]);
        XamlMarkupExtension.Parse("{}{0:C}").ShouldBeNull();
        XamlMarkupExtension.Unescaped("{}{0:C}").ShouldBe("{0:C}");
        XamlMarkupExtension.Parse("Plain").ShouldBeNull();
    }

    private static TypeNode Type(string name) => Graph.Types.Single(x => x.Name == name);

    private static CodeGraph ReadAll((string Path, string Source)[] files)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-xaml-").FullName;
        try
        {
            foreach (var (path, source) in files)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var graph = new CodeGraph();
            var xaml = new XamlExtension();
            foreach (var language in new CSharpExtension().Languages.Concat(xaml.Languages))
            {
                language.Read(root, files.Select(x => x.Path).Where(language.Reads).ToList(), graph, TestContext.Current.CancellationToken);
            }

            foreach (var reader in xaml.Readers)
            {
                reader.Read(graph);
            }

            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
