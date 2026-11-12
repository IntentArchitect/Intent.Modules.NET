using System;
using System.Collections.Generic;
using Intent.Modules.Common.Drift;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// What the engine does for code that isn't one C# type: the files of one component read as one place, and the
    /// placeholder filters that name such files and the routes in them.
    /// </summary>
    public class ComponentFilesTests
    {
        private static readonly FactKind Route = new("page.route", "Ui", "route",
            new[] { new FactPlaceholder("Component", "the component's name"), new FactPlaceholder("Route", "the route") },
            "the route",
            "A page's route.",
            new[] { "Component" });

        private static ModelFact Page(string component, string route) => new()
        {
            Kind = Route,
            Element = component,
            Value = route,
            Placeholders = new Dictionary<string, string> { ["Component"] = component, ["Route"] = route }
        };

        private static DriftReport Check(string recipe, Dictionary<string, string> files, params ModelFact[] facts) => DriftCheck.Run(new DriftCheckInput
        {
            Designer = "Ui",
            Kinds = new[] { Route },
            RecipePath = "guardrails/ui.drift.json",
            RecipeJson = recipe,
            Code = new CodeFiles(files),
            Facts = facts,
            Generated = Array.Empty<GeneratedFile>()
        });

        private const string RazorRecipe = """
        {
          "facts": {
            "page.route": {
              "find": "file:**/{Component}.razor*",
              "read": [
                { "match": "@page\"/?([^\"]*)\"" },
                { "match": "\\[Route\\(\"/?([^\"]*)\"\\)\\]" }
              ],
              "default": ""
            }
          }
        }
        """;

        [Fact]
        public void A_component_s_markup_and_code_behind_are_read_as_one_place()
        {
            var files = new Dictionary<string, string>
            {
                ["Pages/Checkout.razor"] = "<h1>Checkout</h1>\n",
                ["Pages/Checkout.razor.cs"] = "[Route(\"/checkout\")]\npublic partial class Checkout { }\n",
                ["Pages/Cart.razor"] = "@page \"/cart\"\n<h1>Cart</h1>\n"
            };

            Check(RazorRecipe, files, Page("Checkout", "checkout"), Page("Cart", "cart")).WrongLines.ShouldBeEmpty();
            Check(RazorRecipe, files, Page("Checkout", "basket/checkout")).WrongLines
                .ShouldHaveSingleItem().ShouldStartWith("`Checkout` route: model `basket/checkout`, code `checkout`");
        }

        [Fact]
        public void Files_of_the_same_name_in_two_folders_are_still_ambiguous()
        {
            var files = new Dictionary<string, string>
            {
                ["Pages/Customers/Index.razor"] = "@page \"/customers\"\n",
                ["Pages/Orders/Index.razor"] = "@page \"/orders\"\n"
            };

            Check(RazorRecipe, files, Page("Index", "customers")).WrongLines.ShouldHaveSingleItem()
                .ShouldContain("matches 2 places (`Pages/Customers/Index.razor:1`, `Pages/Orders/Index.razor:1`); narrow the find");
        }

        [Fact]
        public void Files_whose_names_differ_before_the_first_dot_are_still_ambiguous()
        {
            var files = new Dictionary<string, string>
            {
                ["Pages/Checkout.razor"] = "@page \"/checkout\"\n",
                ["Pages/CheckoutSummary.razor"] = "<p>Summary</p>\n"
            };
            const string recipe = """
            { "facts": { "page.route": { "find": "file:Pages/Checkout*.razor", "read": [{ "match": "@page\"/?([^\"]*)\"" }], "default": "" } } }
            """;

            Check(recipe, files, Page("Checkout", "checkout")).WrongLines.ShouldHaveSingleItem().ShouldContain("matches 2 places");
        }

        [Fact]
        public void A_kebab_cased_name_finds_an_Angular_component_s_files()
        {
            var files = new Dictionary<string, string>
            {
                ["src/app/customer-add-page/customer-add-page.component.ts"] = "@Component({ selector: 'app-customer-add-page', templateUrl: 'customer-add-page.component.html' })\nexport class CustomerAddPageComponent { }\n",
                ["src/app/customer-add-page/customer-add-page.component.html"] = "<h1 data-route=\"customer/add\">Add</h1>\n"
            };
            const string recipe = """
            { "facts": { "page.route": { "find": "file:**/{Component|kebab}.component.*", "read": [{ "match": "data-route=\"([^\"]*)\"" }], "default": "" } } }
            """;

            Check(recipe, files, Page("CustomerAddPage", "customer/add")).WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void An_interpolated_string_reads_as_written()
        {
            var code = new CodeFiles(new Dictionary<string, string>
            {
                ["Pages/Customers.razor.cs"] = "class Customers\n{\n    void Edit(int id) => Nav.NavigateTo($\"customer/edit/{id}?tab={Tab:D}\");\n}\n"
            });

            code.Get("Pages/Customers.razor.cs").Canonical.Text.ShouldContain("Nav.NavigateTo($\"customer/edit/{id}?tab={Tab:D}\")");
        }

        [Theory]
        [InlineData("CustomerAddPage", "customer-add-page")]
        [InlineData("HTMLParser", "html-parser")]
        [InlineData("Page2Edit", "page2-edit")]
        [InlineData("Main_Layout", "main-layout")]
        [InlineData("home", "home")]
        public void Kebab_words_a_name_as_Intent_s_own_kebab_case_does(string name, string expected)
        {
            Placeholders.Render("{Name|kebab}", new Dictionary<string, string> { ["Name"] = name }).ShouldBe(expected);
        }

        [Fact]
        public void Replace_swaps_every_occurrence_and_chains_with_other_filters()
        {
            var values = new Dictionary<string, string> { ["Route"] = "customer/edit" };

            Placeholders.Render("['/{Route|replace:/:','}',", values).ShouldBe("['/customer','edit',");
            Placeholders.Render("{Route|replace:/:-|upper}", values).ShouldBe("CUSTOMER-EDIT");
        }

        [Fact]
        public void Replace_without_what_replaces_the_text_is_a_recipe_error()
        {
            Placeholders.Validate("{Route|replace:/}", new[] { "Route" }).ShouldBe("'replace' in {Route|replace:/} needs the text and what replaces it, as replace:<text>:<with>");
            Placeholders.Validate("{Route|dasherize}", new[] { "Route" })
                .ShouldBe("'dasherize' in {Route|dasherize} isn't a filter (filters: lower, upper, camel, pascal, kebab, trimStart:<text>, trimEnd:<text>, replace:<text>:<with>)");
        }
    }
}
