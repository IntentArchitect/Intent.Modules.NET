using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The check remembers, per machine, what the model and the code looked like when they last agreed. A difference
    /// only the model made since is the Software Factory's to apply, so it isn't reported until the code agrees. A
    /// difference the code made is reported, and so is one both made, which applying the model's change would overwrite.
    /// A type or member the code lacks is always the Software Factory's to write, and its Changes pane shows it.
    /// </summary>
    public class ModelBaselineTests
    {
        private const string Path = "src/OrderDTO.cs";
        private const string BothChanged = " (the model and the code have both changed since they last agreed)";

        private static string Dto(params string[] members) =>
            $"namespace Acme\n{{\n    public class OrderDTO\n    {{\n{string.Join("\n", System.Linq.Enumerable.Select(members, x => $"        {x}"))}\n    }}\n}}\n";

        private static DriftReport Run(string model, string code, ModelBaseline baseline, string recipe = "{}", Dictionary<string, string> extraCode = null)
        {
            var files = new Dictionary<string, string>(extraCode ?? new Dictionary<string, string>());
            if (code != null)
            {
                files[Path] = code;
            }

            return DriftCheck.Run(new DriftCheckInput
            {
                Designer = "Services",
                RecipePath = "guardrails/services.drift.json",
                RecipeJson = recipe,
                Code = new CodeFiles(files),
                Facts = new List<ModelFact>(),
                Generated = model == null ? new GeneratedFile[0] : new[] { new GeneratedFile { RelativePath = Path, Text = model } },
                Baseline = baseline
            });
        }

        private static ModelBaseline Agreed(string model) => Run(model, model, baseline: null).Baseline;

        [Fact]
        public void Without_a_baseline_a_model_first_change_is_reported_and_the_agreement_is_remembered()
        {
            var model = Dto("public int Id { get; set; }", "public string Name { get; set; }", "public long Total { get; set; }");
            var report = Run(model, Dto("public int Id { get; set; }", "public int Total { get; set; }"), baseline: null);

            report.WrongLines.ShouldBe(new[] { "`OrderDTO.Total` type: model `long`, code `int`. Location: `src/OrderDTO.cs:6`" });
            report.Baseline.Aspects["OrderDTO.Id"].Model.ShouldNotBeNull();
            report.Baseline.Aspects["OrderDTO.Id"].Code.ShouldNotBeNull();
            report.Baseline.Aspects["OrderDTO.Total"].Model.ShouldBeNull("it was already different when the check first saw it");
            report.Baseline.Aspects["OrderDTO.Total"].Code.ShouldBeNull();

            // Never agreed, so reported until they do.
            Run(model, Dto("public int Id { get; set; }", "public int Total { get; set; }"), report.Baseline).WrongLines.Count.ShouldBe(1);

            // What the code lacks is the Software Factory's to write, with or without a baseline.
            report.PendingCount.ShouldBe(1);
            report.Baseline.Aspects.ShouldNotContainKey("OrderDTO.Name");
        }

        [Fact]
        public void A_member_the_model_adds_is_left_to_the_Software_Factory_until_the_code_has_it()
        {
            var agreed = Agreed(Dto("public int Id { get; set; }"));
            var model = Dto("public int Id { get; set; }", "public string Name { get; set; }");

            var generating = Run(model, Dto("public int Id { get; set; }"), agreed);
            generating.WrongLines.ShouldBeEmpty();
            generating.PendingCount.ShouldBe(1);

            // Until applied: a run that applied nothing still leaves it to the Software Factory.
            var notApplied = Run(model, Dto("public int Id { get; set; }"), generating.Baseline);
            notApplied.WrongLines.ShouldBeEmpty();
            notApplied.PendingCount.ShouldBe(1);

            var applied = Run(model, model, notApplied.Baseline);
            applied.WrongLines.ShouldBeEmpty();
            applied.PendingCount.ShouldBe(0);
            applied.Baseline.Aspects["OrderDTO.Name"].Code.ShouldNotBeNull();

            // The code removing it later is the Software Factory's to put back, which its Changes pane shows.
            var removed = Run(model, Dto("public int Id { get; set; }"), applied.Baseline);
            removed.WrongLines.ShouldBeEmpty();
            removed.PendingCount.ShouldBe(1);
        }

        [Fact]
        public void A_member_the_model_removes_and_adds_back_is_left_to_the_Software_Factory()
        {
            // eShop8: Test was modelled and applied, then removed from the model and the removal applied, then modelled
            // again, with no run in between to see the model and the code agree it was gone.
            var idOnly = Dto("public int Id { get; set; }");
            var withTest = Dto("public int Id { get; set; }", "public string Test { get; set; }");
            var removing = Run(idOnly, withTest, Agreed(withTest));
            removing.PendingCount.ShouldBe(1);

            var addedBack = Run(withTest, idOnly, removing.Baseline);
            addedBack.WrongLines.ShouldBeEmpty();
            addedBack.PendingCount.ShouldBe(1);
        }

        [Fact]
        public void A_member_the_model_adds_is_reported_where_the_code_already_has_its_own()
        {
            var agreed = Agreed(Dto("public int Id { get; set; }"));

            Run(Dto("public int Id { get; set; }", "public string Name { get; set; }"), Dto("public int Id { get; set; }", "public int Name { get; set; }"), agreed)
                .WrongLines.ShouldBe(new[] { "`OrderDTO.Name` type: model `string`, code `int`. Location: `src/OrderDTO.cs:6`" });
        }

        [Fact]
        public void A_member_the_model_changes_is_left_to_the_Software_Factory_and_one_the_code_changes_is_reported()
        {
            var model = Dto("public int Id { get; set; }");
            var agreed = Agreed(model);

            var modelChanged = Run(Dto("public long Id { get; set; }"), model, agreed);
            modelChanged.WrongLines.ShouldBeEmpty();
            modelChanged.PendingCount.ShouldBe(1);

            var codeChanged = Run(model, Dto("public long Id { get; set; }"), agreed);
            codeChanged.WrongLines.ShouldBe(new[] { "`OrderDTO.Id` type: model `int`, code `long`. Location: `src/OrderDTO.cs:5`" });

            // Reported until the model and the code agree again: here, the model catching up with the code.
            Run(model, Dto("public long Id { get; set; }"), codeChanged.Baseline).WrongLines.Count.ShouldBe(1);
            var caughtUp = Run(Dto("public long Id { get; set; }"), Dto("public long Id { get; set; }"), codeChanged.Baseline);
            caughtUp.WrongLines.ShouldBeEmpty();
            caughtUp.PendingCount.ShouldBe(0);
        }

        [Fact]
        public void A_member_both_the_model_and_the_code_changed_is_reported()
        {
            var agreed = Agreed(Dto("public int Id { get; set; }"));

            var report = Run(Dto("public long Id { get; set; }"), Dto("public short Id { get; set; }"), agreed);

            report.WrongLines.ShouldBe(new[] { $"`OrderDTO.Id` type: model `long`, code `short`. Location: `src/OrderDTO.cs:5`{BothChanged}" });
            report.PendingCount.ShouldBe(0);
            report.ToMarkdown().ShouldContain($"| `OrderDTO.Id` type: model `long`, code `short`{BothChanged} |");

            // Still reported on the next run: the code hasn't gone back to what it was.
            Run(Dto("public long Id { get; set; }"), Dto("public short Id { get; set; }"), report.Baseline).WrongLines.Count.ShouldBe(1);
        }

        [Fact]
        public void A_member_the_model_changes_and_the_code_deletes_is_left_to_the_Software_Factory()
        {
            var agreed = Agreed(Dto("public int Id { get; set; }", "public string Name { get; set; }"));

            var report = Run(Dto("public int Id { get; set; }", "public string? Name { get; set; }"), Dto("public int Id { get; set; }"), agreed);
            report.WrongLines.ShouldBeEmpty();
            report.PendingCount.ShouldBe(1);
        }

        [Fact]
        public void A_member_the_model_removes_is_left_to_the_Software_Factory_unless_the_code_changed_it()
        {
            var both = Dto("public int Id { get; set; }", "public string Name { get; set; }");
            var agreed = Agreed(both);
            var idOnly = Dto("public int Id { get; set; }");

            var removing = Run(idOnly, both, agreed);
            removing.WrongLines.ShouldBeEmpty();
            removing.PendingCount.ShouldBe(1);

            Run(idOnly, Dto("public int Id { get; set; }", "public string? Name { get; set; }"), agreed).WrongLines.ShouldBe(new[]
            {
                $"`OrderDTO.Name` (public property): not modelled. Location: `src/OrderDTO.cs:6`{BothChanged}"
            });

            Run(idOnly, Dto("public int Id { get; set; }", "public string Notes { get; set; }"), agreed).WrongLines.ShouldBe(new[]
            {
                "`OrderDTO.Notes` (public property): not modelled. Location: `src/OrderDTO.cs:6`"
            });
        }

        [Fact]
        public void A_method_whose_parameters_the_model_changes_is_matched_to_the_code_under_its_old_key()
        {
            var agreed = Agreed(Dto("public void Rename(string name) { }"));

            var modelChanged = Run(Dto("public void Rename(string name, bool force) { }"), Dto("public void Rename(string name) { }"), agreed);
            modelChanged.WrongLines.ShouldBeEmpty();
            modelChanged.PendingCount.ShouldBe(1);

            Run(Dto("public void Rename(string name, bool force) { }"), Dto("public void Rename(string name) { }"), modelChanged.Baseline).PendingCount.ShouldBe(1);

            Run(Dto("public void Rename(string name, bool force) { }"), Dto("public void Rename(string newName) { }"), agreed).WrongLines.ShouldBe(new[]
            {
                $"`OrderDTO.Rename(string,bool)` parameters: model `(string name,bool force)`, code `(string newName)`. Location: `src/OrderDTO.cs:5`{BothChanged}"
            });
        }

        [Fact]
        public void A_type_the_model_adds_or_removes_is_left_to_the_Software_Factory_unless_the_code_changed_it()
        {
            const string recipe = """
                { "own": [{ "kind": "DTO", "find": "class with \\bclass \\w+DTO\\b" }] }
                """;
            var model = Dto("public int Id { get; set; }");

            // Added in the model since the last run: its file isn't in the code yet.
            var before = Run(model: null, code: null, baseline: null, recipe).Baseline;
            var adding = Run(model, code: null, before, recipe);
            adding.WrongLines.ShouldBeEmpty();
            adding.PendingCount.ShouldBe(1);

            // Its file deleted from the code once they agreed: the Software Factory's to write again.
            var agreed = Run(model, model, baseline: null, recipe).Baseline;
            var deleted = Run(model, code: null, agreed, recipe);
            deleted.WrongLines.ShouldBeEmpty();
            deleted.PendingCount.ShouldBe(1);

            // Removed from the model: the Software Factory is deleting its file, which the DTO convention still finds,
            // and keeps doing so on every run until it's applied.
            var removing = Run(model: null, model, agreed, recipe);
            removing.WrongLines.ShouldBeEmpty();
            removing.PendingCount.ShouldBe(1);
            Run(model: null, model, removing.Baseline, recipe).PendingCount.ShouldBe(1);

            // Removed from the model while the code changed it: deleting the file would lose that.
            Run(model: null, Dto("public long Id { get; set; }"), agreed, recipe).WrongLines.ShouldBe(new[]
            {
                $"DTO `OrderDTO`: not modelled. Location: `src/OrderDTO.cs:3`{BothChanged}"
            });
        }

        [Fact]
        public void A_type_the_model_removes_is_reported_when_the_code_added_a_member_to_it()
        {
            // MEM5b: deleting the file would lose the member, at any access level.
            const string recipe = """
                { "own": [{ "kind": "DTO", "find": "class with \\bclass \\w+DTO\\b" }] }
                """;
            var model = Dto("public int Id { get; set; }");
            var agreed = Run(model, model, baseline: null, recipe).Baseline;

            Run(model: null, Dto("public int Id { get; set; }", "public int Extra { get; set; }"), agreed, recipe).WrongLines
                .ShouldBe(new[] { $"DTO `OrderDTO`: not modelled. Location: `src/OrderDTO.cs:3`{BothChanged}" });
            Run(model: null, Dto("public int Id { get; set; }", "private int _cache;"), agreed, recipe).WrongLines
                .ShouldBe(new[] { $"DTO `OrderDTO`: not modelled. Location: `src/OrderDTO.cs:3`{BothChanged}" });

            // A member the code added while the model still had the type is reported then, and not taken as agreed.
            var withExtra = Run(model, Dto("public int Id { get; set; }", "public int Extra { get; set; }"), agreed, recipe);
            withExtra.WrongLines.Count.ShouldBe(1);
            Run(model: null, Dto("public int Id { get; set; }", "public int Extra { get; set; }"), withExtra.Baseline, recipe).WrongLines.Count.ShouldBe(1);

            // A baseline from before members were remembered can't show the code added none.
            var earlier = ModelBaseline.Parse(agreed.ToJson());
            earlier.Aspects["OrderDTO"].Members = null;
            Run(model: null, model, earlier, recipe).WrongLines.ShouldBe(new[] { "DTO `OrderDTO`: not modelled. Location: `src/OrderDTO.cs:3`" });
        }

        [Fact]
        public void A_method_whose_parameters_the_model_and_the_code_both_changed_is_reported_as_changed_on_both_sides()
        {
            // MEM7: neither the model's nor the code's signature is in the baseline; the old one is what they agreed on.
            var agreed = Agreed(Dto("public void CountEnabled() { }"));
            var model = Dto("public void CountEnabled(string province) { }");
            var code = Dto("public void CountEnabled(bool includeDisabled) { }");

            var report = Run(model, code, agreed);
            report.WrongLines.ShouldBe(new[]
            {
                $"`OrderDTO.CountEnabled(string)` parameters: model `(string province)`, code `(bool includeDisabled)`. Location: `src/OrderDTO.cs:5`{BothChanged}"
            });

            // The old entry is kept, so the next run says the same.
            Run(model, code, report.Baseline).WrongLines.ShouldBe(report.WrongLines);

            // The code deleting it while the model changed its parameters: the Software Factory's to write.
            var deleted = Run(model, Dto(), agreed);
            deleted.WrongLines.ShouldBeEmpty();
            deleted.PendingCount.ShouldBe(1);
        }

        [Fact]
        public void The_baseline_survives_being_saved_and_a_damaged_or_earlier_one_is_ignored()
        {
            var baseline = Agreed(Dto("public int Id { get; set; }"));

            var reloaded = ModelBaseline.Parse(baseline.ToJson());
            reloaded.Aspects.Keys.ShouldBe(baseline.Aspects.Keys, ignoreOrder: true);
            reloaded.Aspects["OrderDTO.Id"].Model.ShouldBe(baseline.Aspects["OrderDTO.Id"].Model);
            reloaded.Aspects["OrderDTO.Id"].Code.ShouldBe(baseline.Aspects["OrderDTO.Id"].Code);
            ModelBaseline.Parse("not json").ShouldBeNull();
            ModelBaseline.Parse("""{ "OrderDTO.Id": { "agreed": "property|public|int||||", "pending": false } }""").ShouldBeNull("it has no code side");
        }
    }
}
