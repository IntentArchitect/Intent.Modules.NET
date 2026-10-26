using System;
using System.Collections.Generic;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// A fact about an attribute the model has just added is checked on the run that adds it, and a fact's row names its
    /// class and attribute and points at the code that states it, or where it would.
    /// </summary>
    public class FactRowTests
    {
        private const string Recipe = """
        {
          "facts": {
            "column.required": {
              "find": "statement with modelBuilder\\.Entity<{Entity}>\\(\\w+=> > chain with \\.Property\\(\\w+=>\\w+\\.{Attribute}\\)",
              "read": [{ "match": "\\.IsRequired\\(\\)", "value": "true" }],
              "default": "false",
              "absent": "false",
              "unstated": "no `IsRequired()` for `{Attribute}` in EF Core, which maps a string as nullable"
            }
          }
        }
        """;

        private const string DbContext = """
        namespace Acme
        {
            public class AppDbContext
            {
                protected void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<Order>(entity =>
                    {
                        entity.ToTable("Order");
                    });
                }
            }
        }
        """;

        private static readonly Dictionary<string, string> Ids = new() { ["Order"] = "order-id", ["Order.Id"] = "id-id", ["Order.Test"] = "test-id" };

        private static string Order(params string[] members) =>
            $"namespace Acme\n{{\n    public class Order\n    {{\n{string.Join("\n", System.Linq.Enumerable.Select(members, x => $"        {x}"))}\n    }}\n}}\n";

        private static readonly ModelFact TestRequired = new()
        {
            Kind = FactKinds.ColumnRequired,
            Element = "Order.Test",
            ElementIds = new[] { "order-id", "test-id" },
            Value = "true",
            Placeholders = new Dictionary<string, string> { ["Entity"] = "Order", ["Attribute"] = "Test" }
        };

        private static DriftReport Run(string model, string code, ModelBaseline baseline, IReadOnlyList<ModelFact> facts, string dbContext = DbContext, string recipe = Recipe)
        {
            var files = new Dictionary<string, string> { ["src/Order.cs"] = code };
            if (dbContext != null)
            {
                files["src/AppDbContext.cs"] = dbContext;
            }

            return DriftCheck.Run(new DriftCheckInput
            {
                Designer = "Domain",
                RecipePath = "guardrails/domain.drift.json",
                RecipeJson = recipe,
                Code = new CodeFiles(files),
                Facts = facts,
                Generated = new[] { new GeneratedFile { RelativePath = "src/Order.cs", Text = model, ElementIds = Ids } },
                Application = "Acme",
                Baseline = baseline,
                BaselineSavedAt = baseline == null ? null : DateTime.UtcNow
            });
        }

        [Fact]
        public void A_fact_about_an_attribute_the_model_just_added_is_checked_on_the_run_that_adds_it()
        {
            const string required =
                "`Order.Test` required: model `true`, code `false` (no `IsRequired()` for `Test` in EF Core, which maps a string as nullable). Near: `src/AppDbContext.cs:7`";
            var before = Order("public int Id { get; set; }");
            var model = Order("public int Id { get; set; }", "public string Test { get; set; }");
            var agreed = Run(before, before, baseline: null, new List<ModelFact>()).Baseline;

            // The property is the Software Factory's to write; the configuration the model asks for is a gap it won't fill.
            var staging = Run(model, before, agreed, new[] { TestRequired });
            staging.PendingCount.ShouldBe(1);
            staging.WrongLines.ShouldBe(new[] { required });
            staging.PendingMessage().ShouldEndWith("until the changes are applied.");

            // Applied: the same gap, until the code is fixed.
            Run(model, model, staging.Baseline, new[] { TestRequired }).WrongLines.ShouldBe(new[] { required });
        }

        [Fact]
        public void Configuration_written_before_the_attribute_was_modelled_counts_as_modelled_on_the_run_that_adds_it()
        {
            // The configuration is on disk, in a file the pending change doesn't touch: the fact read with the change
            // applied locates it there, so the ownership convention finds it modelled.
            var recipe = Recipe.Replace("\"facts\": {", "\"own\": [{ \"kind\": \"property configuration\", \"find\": \"statement with modelBuilder\\\\.Entity<\\\\w+>\\\\(\\\\w+=> > chain with \\\\.Property\\\\(\" }],\n  \"facts\": {");
            var configured = DbContext.Replace("entity.ToTable(\"Order\");", "entity.ToTable(\"Order\");\n                entity.Property(e => e.Test).IsRequired();");
            var before = Order("public int Id { get; set; }");
            var agreed = Run(before, before, baseline: null, new List<ModelFact>(), configured, recipe).Baseline;

            var staging = Run(Order("public int Id { get; set; }", "public string Test { get; set; }"), before, agreed, new[] { TestRequired }, configured, recipe);

            staging.PendingCount.ShouldBe(1);
            staging.WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void A_fact_row_names_the_class_and_attribute_and_points_where_the_code_would_state_it()
        {
            var model = Order("public int Id { get; set; }", "public string Test { get; set; }");

            var markdown = Run(model, model, baseline: null, new[] { TestRequired }).ToMarkdown().Replace("\r\n", "\n");

            markdown.ShouldContain(
                "| <ModelRef application=\"Acme\" elementId=\"order-id\" designer=\"Domain\" /> › <ModelRef application=\"Acme\" elementId=\"test-id\" designer=\"Domain\" /> " +
                "| <FileRef path=\"src/AppDbContext.cs\" line={7} /> " +
                "| `Order.Test` required: model `true`, code `false` (no `IsRequired()` for `Test` in EF Core, which maps a string as nullable) |");
        }

        [Fact]
        public void Unstated_needs_a_default_or_absent_value_to_explain()
        {
            var recipe = RecipeFile.Parse("""
            { "facts": { "column.required": { "find": "class:{Entity}", "read": [{ "match": "x", "value": "true" }], "unstated": "never used" } } }
            """, "Domain");

            recipe.Problems.ShouldContain(x => x.Message == "has unstated but no default or absent for it to explain");
        }

        [Fact]
        public void A_fact_whose_find_reaches_nothing_points_at_its_elements_file()
        {
            var model = Order("public int Id { get; set; }", "public string Test { get; set; }");

            var markdown = Run(model, model, baseline: null, new[] { TestRequired }, dbContext: null).ToMarkdown().Replace("\r\n", "\n");

            markdown.ShouldContain("| <FileRef path=\"src/Order.cs\" /> | `Order.Test` required: model `true`, code `false` (no `IsRequired()` for `Test` in EF Core, which maps a string as nullable) |");
        }
    }
}
