using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// Markup as the languages read it (code-reader.md, Markup): a Razor component's markup and a Vue component's template give
/// the same nodes, elements with their attributes, content and the components their tags name.
/// </summary>
public class MarkupFactsTests
{
    private static readonly CodeGraph Razor = ReadAll(new CSharpExtension().Languages.Single(),
    [
        ("src/Web/Web.csproj", """
            <Project Sdk="Microsoft.NET.Sdk.Razor">
              <PropertyGroup>
                <RootNamespace>Shop.Web</RootNamespace>
              </PropertyGroup>
            </Project>
            """),
        ("src/Web/_Imports.razor", """
            @using Shop.Web.Ui
            """),
        ("src/Web/Pages/Catalog.razor", """
            @page "/catalog"

            <PageTitle>Shopping   Bag | Shop</PageTitle>
            <PageTitle>@item.Name | Shop</PageTitle>

            <div class="items">
                @foreach (var item in items)
                {
                    <ItemCard Item="@item" @key="item.Id" />
                }
                <a href="/orders/@item.Id?x=1" @onclick="() => Open(item)">Go &amp; see</a>
                <Ui.Grid TItem="OrderDto" @bind-Value:event="oninput" />
                @if (items.Count > 0) { <span>Some</span> }
            </div>

            @code {
                private List<OrderDto> items = [];
                private OrderDto item = default!;

                private void Open(OrderDto order) { }
            }
            """),
        ("src/Web/Shared/ItemCard.razor", """
            @namespace Shop.Web.Ui

            <article></article>
            """),
        ("src/Web/Shared/Grid.razor", """
            @namespace Shop.Web.Ui

            <table></table>
            """)
    ]);

    private static readonly CodeGraph Vue = ReadAll(new TypeScriptExtension().Languages.Single(),
    [
        ("src/pages/CatalogPage.vue", """
            <script setup lang="ts">
            import StockPill from '../components/StockPill.vue';
            import * as ui from '../components/ui';
            </script>

            <template>
                <h1>Shopping   Bag | {{ shop.name }}</h1>
                <ul class="items">
                    <li v-for="product in products" :key="product.id">
                        <RouterLink :to="`/products/${product.id}`" @click.prevent="open(product)">Go &amp; see</RouterLink>
                        <stock-pill :in-stock="product.inStock" v-model:count.number="count" />
                        <ui.Badge />
                    </li>
                </ul>
                <textarea>{{ note }}</textarea>
            </template>
            """),
        ("src/components/StockPill.vue", """
            <script setup lang="ts">
            defineProps<{ inStock: boolean }>();
            </script>

            <template>
                <span class="pill"><slot /></span>
            </template>
            """),
        ("src/components/ui.ts", """
            export class Badge {}
            """)
    ]);

    [Fact]
    public void Razor_markup_is_a_tree_with_the_elements_inside_code_inside_the_element_that_holds_it()
    {
        var file = Razor.Files.Single(x => x.Path == "src/Web/Pages/Catalog.razor");

        Assert.Equal(["PageTitle", "PageTitle", "div"], file.Markup.Select(x => x.Tag));
        Assert.Equal(["ItemCard", "a", "Ui.Grid", "span"], file.Markup[2].Children.Select(x => x.Tag));
        Assert.Equal(new Span("src/Web/Pages/Catalog.razor", 6, 1, 14, 7), file.Markup[2].At);
        Assert.Equal(new Span("src/Web/Pages/Catalog.razor", 9, 9, 9, 49), file.Markup[2].Children[0].At);
        Assert.Empty(Razor.Files.Single(x => x.Path == "src/Web/_Imports.razor").Markup);
        Assert.Empty(Razor.Failures);
    }

    [Fact]
    public void Vue_markup_is_the_template_s_elements()
    {
        var file = Vue.Files.Single(x => x.Path == "src/pages/CatalogPage.vue");

        Assert.Equal(["h1", "ul", "textarea"], file.Markup.Select(x => x.Tag));
        Assert.Equal(["RouterLink", "stock-pill", "ui.Badge"], file.Markup[1].Children.Single().Children.Select(x => x.Tag));
        Assert.Equal(new Span("src/pages/CatalogPage.vue", 8, 5, 14, 10), file.Markup[1].At);
        Assert.Empty(Vue.Failures);
    }

    [Fact]
    public void A_tag_names_a_component_resolved_as_the_language_resolves_it()
    {
        var catalog = Razor.Types.Single(x => x.Name == "Catalog");
        var razor = Razor.MarkupOf(catalog).Where(x => x.Component is not null)
            .Select(x => (x.Tag, Razor.TypeFor(x.Component!, catalog) is { } type ? $"{type.Namespace}.{type.Name}" : null));
        var page = Vue.Types.Single(x => x.Name == "CatalogPage");
        var vue = Vue.MarkupOf(page).Where(x => x.Component is not null)
            .Select(x => (x.Tag, x.Component!.QualifiedName, Vue.TypeFor(x.Component!, page) is { } type ? $"{type.Namespace}.{type.Name}" : null));

        // A library's component, such as Blazor's PageTitle or Vue Router's RouterLink, isn't in the graph.
        Assert.Equal([("PageTitle", null), ("PageTitle", null), ("ItemCard", "Shop.Web.Ui.ItemCard"), ("Ui.Grid", "Shop.Web.Ui.Grid")], razor);
        Assert.Equal([("RouterLink", "RouterLink", null), ("stock-pill", "StockPill", "src/components/StockPill.StockPill"), ("ui.Badge", "ui.Badge", "src/components/ui.Badge")], vue);
    }

    [Fact]
    public void Content_is_a_literal_its_text_trimmed_and_its_values_holes()
    {
        var titles = Razor.Files.Single(x => x.Path == "src/Web/Pages/Catalog.razor").Markup;
        var vue = Vue.Files.Single(x => x.Path == "src/pages/CatalogPage.vue").Markup;
        var link = Razor.MarkupOf(Razor.Types.Single(x => x.Name == "Catalog")).Single(x => x.Tag == "a");

        Assert.Equal(("string", "Shopping Bag | Shop"), (titles[0].Content!.Kind, titles[0].Content!.Value));
        Assert.Equal(["{item.Name}", "' | Shop'"], Parts(titles[1].Content!));
        Assert.Equal(["'Shopping Bag | '", "{shop.name}"], Parts(vue[0].Content!));
        Assert.Equal(["{note}"], Parts(vue[2].Content!));
        Assert.Equal("Go & see", link.Content!.Value);
        Assert.Equal("Go & see", vue[1].Children.Single().Children[0].Content!.Value);
        Assert.Null(titles[2].Content);
    }

    [Fact]
    public void A_plain_attribute_s_value_is_a_literal_with_Razor_s_values_as_holes()
    {
        var link = Razor.MarkupOf(Razor.Types.Single(x => x.Name == "Catalog")).Single(x => x.Tag == "a");
        var card = Razor.MarkupOf(Razor.Types.Single(x => x.Name == "Catalog")).Single(x => x.Tag == "ItemCard");
        var href = link.Attributes.Single(x => x.Name == "href");

        Assert.Equal("/orders/@item.Id?x=1", href.Value);
        Assert.Equal(["'/orders/'", "{item.Id}", "'?x=1'"], Parts(href.Literal!));
        Assert.Equal(new Span("src/Web/Pages/Catalog.razor", 11, 8, 11, 35), href.At);
        Assert.Equal(["{item}"], Parts(card.Attributes.Single(x => x.Name == "Item").Literal!));
        Assert.Equal(("string", "items"), (Razor.Files.Single(x => x.Path == "src/Web/Pages/Catalog.razor").Markup[2].Attributes[0].Literal!.Kind, Razor.Files.Single(x => x.Path == "src/Web/Pages/Catalog.razor").Markup[2].Attributes[0].Literal!.Value));
    }

    [Fact]
    public void A_directive_is_split_as_its_language_writes_it_and_its_value_is_code()
    {
        var catalog = Razor.MarkupOf(Razor.Types.Single(x => x.Name == "Catalog")).SelectMany(x => x.Attributes).Where(x => x.Directive is not null);
        var page = Vue.MarkupOf(Vue.Types.Single(x => x.Name == "CatalogPage")).SelectMany(x => x.Attributes).Where(x => x.Directive is not null);

        Assert.Equal(
        [
            "@key: key () item.Id",
            "@onclick: onclick () () => Open(item)",
            "@bind-Value:event: bind Value (event) oninput"
        ], catalog.Select(Describe));
        Assert.Equal(
        [
            "v-for: for () product in products",
            ":key: bind key () product.id",
            ":to: bind to () `/products/${product.id}`",
            "@click.prevent: on click (prevent) open(product)",
            ":in-stock: bind in-stock () product.inStock",
            "v-model:count.number: model count (number) count"
        ], page.Select(Describe));
        Assert.All(catalog.Concat(page), x => Assert.Null(x.Literal));
    }

    [Fact]
    public void An_element_s_text_is_markup_s_canonical_text_with_the_elements_inside_it()
    {
        var file = Razor.Files.Single(x => x.Path == "src/Web/Pages/Catalog.razor");

        Assert.Equal("<PageTitle>Shopping Bag | Shop</PageTitle>", file.Markup[0].Text);
        Assert.StartsWith("<div class=\"items\">@foreach (var item in items) {<ItemCard Item=\"@item\" @key=\"item.Id\"/>}<a href=", file.Markup[2].Text);
    }

    [Fact]
    public void A_find_path_reaches_a_component_s_markup_and_keeps_the_innermost_element_that_matches()
    {
        var types = Razor.Types.Where(x => x.Name == "Catalog").ToList();

        var title = Assert.Single(FindPath.Run("type > markup with ^<PageTitle>([^<@]*)</PageTitle>$", types, Razor));
        var link = Assert.Single(FindPath.Run("type > markup with Go &amp; see", types, Razor));
        var inside = FindPath.Run("type > markup with ^<div > markup", types, Razor);

        Assert.Equal(("PageTitle", "Shopping Bag | Shop", 3), (title.Name("{Name}"), title.Groups[1], title.At.Line));
        Assert.Equal("a", ((MarkupNode)link.Node).Tag);
        Assert.Equal(["ItemCard", "a", "Ui.Grid", "span"], inside.Select(x => ((MarkupNode)x.Node).Tag));
    }

    private static string Describe(MarkupAttributeNode attribute) =>
        $"{attribute.Name}: {attribute.Directive} {attribute.Argument} ({string.Join(".", attribute.Modifiers)}) {attribute.Value}".Replace("  (", " (");

    private static List<string> Parts(LiteralNode literal) =>
        literal.Kind == "string" ? [$"'{literal.Value}'"] : literal.Parts.Select(x => x.Literal is { } text ? $"'{text.Value}'" : $"{{{x.Text}}}").ToList();

    private static CodeGraph ReadAll(ILanguage language, (string Path, string Source)[] files)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-markup-").FullName;
        try
        {
            foreach (var (path, source) in files)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var graph = new CodeGraph();
            language.Read(root, files.Select(x => x.Path).Where(language.Reads).ToList(), graph, TestContext.Current.CancellationToken);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
