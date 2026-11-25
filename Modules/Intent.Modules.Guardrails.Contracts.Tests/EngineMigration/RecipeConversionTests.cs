using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Drift.EngineMigration;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests.EngineMigration
{
    /// <summary>What each recipe construct becomes when an application moves onto the drift engine.</summary>
    public class RecipeConversionTests
    {
        [Fact]
        public void Everything_is_in_scope_as_v1_checked_code_wherever_it_was()
        {
            var result = Convert(
                ("services.drift.json", """{ "files": ["src/Api/**/*.cs", "src/Application/**/*.cs"] }"""),
                ("domain.drift.json", """{ "files": "src/Domain/**/*.cs" }"""));

            Text(result.Scope.ShouldHaveSingleItem()).ShouldBe("In ** — From guardrails/domain.drift.json, guardrails/services.drift.json: v1 checked code wherever it was");
        }

        [Fact]
        public void An_own_entry_without_files_is_bound_by_its_recipes_files()
        {
            var result = Convert(("ui.drift.json", """{ "files": ["src/Web/**/*.razor", "src/Web/**/*.razor.cs"], "own": [{ "kind": "page", "find": "file with @page" }] }"""));

            Text(result.OptIn.ShouldHaveSingleItem()).ShouldBe("optin/unmodelled.type src/Web/**/*.razor | src/Web/**/*.razor.cs — page");
        }

        [Fact]
        public void A_recipe_file_without_files_reads_every_cs_file_as_v1_did()
        {
            var result = Convert(("ui.drift.json", """{ "own": [{ "kind": "component", "find": "class with :ComponentBase" }] }"""));

            result.OptIn.ShouldHaveSingleItem().Globs.ShouldBe(new[] { "**/*.cs" });
        }

        [Fact]
        public void What_v1_read_is_each_recipes_files_and_the_files_its_facts_and_own_entries_name()
        {
            var result = Convert(
                ("services.drift.json", """
                    {
                      "files": "Api/Controllers/**/*.cs",
                      "facts": {
                        "intent.invokes": { "files": ["Host/Handlers/**/*.cs", "Api/Controllers/**/*.cs"], "find": "class:{Service}" },
                        "service.route": { "find": "class:{Service}" }
                      },
                      "own": [{ "kind": "DTO", "find": "class with \\[DataContract\\]", "files": "Contracts/**/*.cs" }]
                    }
                    """),
                ("domain.drift.json", """{ "own": [{ "kind": "Entity", "find": "class with :Entity" }] }"""));

            result.V1Read.ShouldBe(new[] { "**/*.cs", "Api/Controllers/**/*.cs", "Host/Handlers/**/*.cs", "Contracts/**/*.cs" });
        }

        [Fact]
        public void A_recipe_file_with_only_comments_turned_the_check_on_with_nothing_in_it()
        {
            var result = Convert(("domain.drift.json", "// set up later\n/* nothing yet */\n"));

            result.Scope.ShouldHaveSingleItem().Globs.ShouldBe(new[] { "**" });
            result.OptIn.ShouldHaveSingleItem().Rule.ShouldBe(RecipeConversion.UnmodelledMember);
            result.Notes.ShouldBeEmpty();
        }

        [Fact]
        public void A_file_pattern_exception_is_a_Context_row_after_every_In_row_with_its_reason()
        {
            var result = Convert(
                ("domain.drift.json", """{ "files": "src/**/*.cs", "exceptions": [{ "match": "src/Legacy/**", "reason": "Old code" }, { "match": "src/Tools/Seed.cs", "reason": "Old code" }] }"""),
                ("services.drift.json", """{ "files": "api/**/*.cs" }"""));

            result.Scope.Select(x => x.Scope).ShouldBe(new[] { "In", "Context" });
            Text(result.Scope[1]).ShouldBe("Context src/Legacy/** | src/Tools/Seed.cs — Old code");
        }

        [Fact]
        public void An_own_entry_is_an_unmodelled_type_row_over_its_files_named_by_its_kind()
        {
            var result = Convert(("services.drift.json", """
                {
                  "files": "src/**/*.cs",
                  // Each element type's convention.
                  "own": [
                    { "kind": "DTO", "find": "class with \\bpublic class \\w+Dto\\b", "files": "src/Dtos/**/*.cs", "note": "every DTO" },
                    { "kind": "Service", "find": "class with :ControllerBase\\b" }
                  ]
                }
                """));

            result.OptIn.Where(x => x.Rule == RecipeConversion.UnmodelledType).Select(Text).ShouldBe(new[]
            {
                "optin/unmodelled.type src/Dtos/**/*.cs — DTO",
                "optin/unmodelled.type src/**/*.cs — Service",
            });
        }

        [Fact]
        public void Own_entries_over_the_same_files_are_one_row_named_by_each_kind()
        {
            var result = Convert(("domain.drift.json", """
                {
                  "files": "src/**/*.cs",
                  "own": [
                    { "kind": "entity configuration", "find": "statement with \\bEntity<(\\w+)>\\(", "name": "$1" },
                    { "kind": "entity configuration", "find": "class with IEntityTypeConfiguration<(\\w+)>", "name": "$1" },
                    { "kind": "index", "find": "chain with HasIndex\\(" }
                  ]
                }
                """));

            Text(result.OptIn.Where(x => x.Rule == RecipeConversion.UnmodelledType).ShouldHaveSingleItem())
                .ShouldBe("optin/unmodelled.type src/**/*.cs — entity configuration, index");
        }

        [Fact]
        public void Unmodelled_public_members_are_reported_in_all_the_CSharp_as_v1s_contract_templates_did()
        {
            var result = Convert(
                ("domain.drift.json", """{ "files": "src/Domain/**/*.cs" }"""),
                ("services.drift.json", """{ "files": "src/Api/**/*.cs" }"""),
                ("ui.drift.json", """{ "files": "src/Web/**/*.razor" }"""));

            result.OptIn.Select(Text).ShouldBe(new[] { "optin/unmodelled.member **/*.cs — Public members of modelled types" });
        }

        [Fact]
        public void A_UI_recipe_alone_has_no_member_row_as_v1_had_no_UI_structure_check()
        {
            Convert(("ui.drift.json", """{ "files": "src/Web/**/*.razor" }""")).OptIn.ShouldBeEmpty();
        }

        [Fact]
        public void Type_and_member_exceptions_are_Exceptions_rows_one_per_rule_and_reason()
        {
            var result = Convert(("services.drift.json", """
                {
                  "exceptions": [
                    { "match": "AuditService", "reason": "Plumbing" },
                    { "match": "LoggerAudit", "reason": "Plumbing" },
                    { "match": "OrdersApi.MapOrdersApiV1", "reason": "Maps the endpoints" },
                    { "match": "CatalogApi.MapCatalogApi", "reason": "Maps the endpoints" },
                    "HealthCheck"
                  ]
                }
                """));

            result.Exceptions.Select(Text).ShouldBe(new[]
            {
                "optin/unmodelled.type AuditService | LoggerAudit — Plumbing",
                "optin/unmodelled.member OrdersApi.MapOrdersApiV1 | CatalogApi.MapCatalogApi — Maps the endpoints",
                "optin/unmodelled.type HealthCheck — From guardrails/services.drift.json, which gives no reason",
            });
            result.Scope.ShouldHaveSingleItem();
        }

        [Fact]
        public void An_exception_naming_an_own_entrys_finding_keeps_that_entry_from_becoming_a_row()
        {
            var result = Convert(("services.drift.json", """
                {
                  "own": [
                    { "kind": "endpoint", "find": "statement with \\.Map(Get|Post)\\(\"/?([^\"]*)\"", "name": "{Class}: Map$1 [$2]" },
                    { "kind": "Command", "find": "class with :IRequest<", "files": "src/Commands/*.cs", "name": "{Class}" }
                  ],
                  "exceptions": [
                    { "match": "WebHooksApi: MapGet []", "reason": "Inline lambda" },
                    { "match": "Something: else", "reason": "Nothing names this" }
                  ]
                }
                """));

            result.OptIn.Where(x => x.Rule == RecipeConversion.UnmodelledType).ShouldHaveSingleItem().Description.ShouldBe("Command");
            result.Exceptions.ShouldBeEmpty();
            result.Notes.ShouldBe(new[]
            {
                "guardrails/services.drift.json: the exception 'WebHooksApi: MapGet []' names a finding of the 'endpoint' convention, so that convention wasn't carried over.",
                "guardrails/services.drift.json: the exception 'Something: else' names nothing the drift engine has, so it wasn't carried over.",
            });
        }

        [Fact]
        public void A_name_that_is_a_type_or_member_is_never_taken_for_an_own_entrys_finding()
        {
            var result = Convert(("services.drift.json", """
                {
                  "own": [{ "kind": "endpoint", "find": "class with :ApiController\\b > method", "name": "{Class}.{Method}" }],
                  "exceptions": [{ "match": "OrdersController.Ping", "reason": "Health probe" }]
                }
                """));

            result.OptIn.Count(x => x.Rule == RecipeConversion.UnmodelledType).ShouldBe(1);
            result.Exceptions.ShouldHaveSingleItem().Rule.ShouldBe(RecipeConversion.UnmodelledMember);
        }

        [Fact]
        public void Facts_notes_and_comments_are_dropped()
        {
            var result = Convert(("domain.drift.json", """
                {
                  "note": "EF Core recipes",
                  // the table name
                  "facts": { "table.name": { "find": "class:{Entity}", "read": [{ "match": "ToTable\\(\"(\\w+)\"" }] } }
                }
                """));

            result.Scope.ShouldHaveSingleItem();
            result.OptIn.ShouldHaveSingleItem().Rule.ShouldBe(RecipeConversion.UnmodelledMember);
            result.Exceptions.ShouldBeEmpty();
            result.Notes.ShouldBeEmpty();
        }

        [Fact]
        public void Globs_are_read_as_v1_read_them_with_braces_expanded()
        {
            RecipeConversion.Globs(new[] { "/src/{Ordering,Catalog}.API/**/*.cs", "./tests\\Unit/*.cs", "src/{a,{b,c}}/x.cs", "docs/{version}/*.md" })
                .ShouldBe(new[] { "src/Ordering.API/**/*.cs", "src/Catalog.API/**/*.cs", "tests/Unit/*.cs", "src/a/x.cs", "src/b/x.cs", "src/c/x.cs", "docs/{version}/*.md" });
        }

        [Fact]
        public void A_recipe_file_that_isnt_valid_JSON_gives_no_rows_and_says_so()
        {
            var result = Convert(("domain.drift.json", """{ "files": ["src/**/*.cs" """));

            result.HasRows.ShouldBeFalse();
            result.Notes.ShouldHaveSingleItem().ShouldStartWith("guardrails/domain.drift.json isn't valid JSON");
        }

        /// <summary>Rows expected from each local codebase's recipe files, read in place. A codebase that isn't on this machine is skipped.</summary>
        [Theory]
        [InlineData("ClientA", 1, 0, 6, 1, 0, 0, 0)]
        [InlineData("ClientB", 1, 0, 10, 1, 10, 12, 0)]
        [InlineData("eShop10", 1, 0, 9, 1, 3, 3, 6)]
        [InlineData("eShop13", 1, 1, 4, 1, 0, 0, 0)]
        [InlineData("eShop14", 1, 0, 9, 1, 2, 3, 0)]
        public void Each_real_codebases_recipe_files_give_the_rows_expected(string codebase, int inRows, int contextRows, int typeRows, int memberRows, int exceptionRows, int matches, int notes)
        {
            var folders = RecipeFolders(codebase);
            if (folders.Count == 0)
            {
                return;
            }

            var result = RecipeConversion.Convert(folders.SelectMany(x => Directory.GetFiles(x, "*.drift.json")).Select(x => new RecipeSource(Path.GetFileName(x), File.ReadAllText(x))));

            result.Scope.Count(x => x.Scope == "In").ShouldBe(inRows);
            result.Scope.Count(x => x.Scope == "Context").ShouldBe(contextRows);
            result.OptIn.Count(x => x.Rule == RecipeConversion.UnmodelledType).ShouldBe(typeRows);
            result.OptIn.Count(x => x.Rule == RecipeConversion.UnmodelledMember).ShouldBe(memberRows);
            result.Exceptions.Count.ShouldBe(exceptionRows);
            result.Exceptions.Sum(x => x.Matches.Count).ShouldBe(matches);
            result.Notes.Count.ShouldBe(notes);
            result.OptIn.Where(x => x.Rule == RecipeConversion.UnmodelledType).ShouldAllBe(x => x.Globs.Count > 0 && !string.IsNullOrWhiteSpace(x.Description));
            result.Exceptions.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x.Reason));

            // The public samples' rows, by what they are (client code is checked by its counts, and by a row's shape without
            // quoting it).
            switch (codebase)
            {
                case "ClientA":
                    result.OptIn.ShouldContain(x => x.Rule == RecipeConversion.UnmodelledType && x.Description == "entity configuration, index"
                        && x.Globs.Count == 1 && x.Globs.First().EndsWith(".Data/**/*.cs"));
                    break;
                case "eShop10":
                    result.OptIn.ShouldNotContain(x => x.Description.Contains("endpoint"));
                    result.OptIn.ShouldContain(x => x.Description == "Entity, Value Object" && x.Globs.SequenceEqual(new[] { "src/Ordering.Domain/**/*.cs" }));
                    result.Exceptions.ShouldContain(x => x.Rule == RecipeConversion.UnmodelledType && x.Matches.SequenceEqual(new[] { "IdentifiedCommand" }));
                    break;
                case "eShop13":
                    result.Scope.Single(x => x.Scope == "Context").Globs.ShouldBe(new[] { "src/IntegrationEventLogEF/**" });
                    break;
            }
        }

        private static ConvertedRecipes Convert(params (string File, string Json)[] files) =>
            RecipeConversion.Convert(files.Select(x => new RecipeSource(x.File, x.Json)));

        /// <summary>
        /// A codebase's recipe folders from the drift engine suite's <c>codebases.json</c>, a <c>*</c> segment standing for any one
        /// folder (the file never names a client's application); none when it isn't on this machine.
        /// </summary>
        private static IReadOnlyList<string> RecipeFolders(string codebase)
        {
            for (var folder = new DirectoryInfo(AppContext.BaseDirectory); folder != null; folder = folder.Parent)
            {
                var file = Path.Combine(folder.FullName, "Intent.Modules.Common.DriftEngine.Tests", "Acceptance", "codebases.json");
                if (!File.Exists(file))
                {
                    continue;
                }

                using var document = JsonDocument.Parse(File.ReadAllText(file), RecipeFile.JsonOptions);
                var entry = document.RootElement.GetProperty("codebases").EnumerateArray().Single(x => x.GetProperty("name").GetString() == codebase);
                var root = Environment.GetEnvironmentVariable($"DRIFTENGINE_CODEBASE_{codebase.ToUpperInvariant()}") is { Length: > 0 } overridden
                    ? overridden
                    : entry.GetProperty("root").GetString();
                IEnumerable<string> folders = new[] { root! }.Where(Directory.Exists);
                foreach (var segment in entry.GetProperty("recipes").GetString()!.Split('/', '\\'))
                {
                    folders = segment == "*"
                        ? folders.SelectMany(x => Directory.GetDirectories(x))
                        : folders.Select(x => Path.Combine(x, segment)).Where(Directory.Exists);
                }

                return folders.ToList();
            }

            return new string[0];
        }

        // Rows hold lists, which compare by reference, so they are compared as text.
        private static string Text(ScopeRowSpec row) => $"{row.Scope} {string.Join(" | ", row.Globs)} — {row.Reason}";

        private static string Text(OptInRowSpec row) => $"{row.Rule} {string.Join(" | ", row.Globs)} — {row.Description}";

        private static string Text(ExceptionRowSpec row) => $"{row.Rule} {string.Join(" | ", row.Matches)} — {row.Reason}";
    }
}
