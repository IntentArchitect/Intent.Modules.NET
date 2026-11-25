using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Drift.EngineMigration;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests.EngineMigration
{
    /// <summary>
    /// The ids the migration copies, checked against where they come from: the Guardrails modules on this branch and on
    /// <c>origin/master</c> (the published source, which consumers have installed), and Intent.Common.DriftEngine. A check
    /// against <c>origin/master</c> is left out when git or that branch isn't there.
    /// </summary>
    public class V1FootprintTests
    {
        private static readonly string[] ContractModules = { "Intent.Modules.Domain.Guardrails.Contracts", "Intent.Modules.Services.Guardrails.Contracts" };

        [Fact]
        public void The_contract_templates_are_every_structure_source_the_Domain_and_Services_modules_have()
        {
            var ids = ContractModules
                .Select(Assembly.Load)
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IDriftStructureSource).IsAssignableFrom(x))
                .Select(x => (string)x.GetField("TemplateId", BindingFlags.Public | BindingFlags.Static)!.GetRawConstantValue())
                .ToList();

            ids.Count.ShouldBe(18);
            ids.ShouldBe(V1Footprint.ContractTemplateIds, ignoreOrder: true);
        }

        [Fact]
        public void The_contract_templates_are_the_same_on_origin_master()
        {
            var master = Git("grep", "-h", "-E", "public const string TemplateId = ", "origin/master", "--",
                "Modules/Intent.Modules.Domain.Guardrails.Contracts/Templates", "Modules/Intent.Modules.Services.Guardrails.Contracts/Templates");
            if (master == null)
            {
                return;
            }

            // On master as here, a contract template's id ends with Contract and no other template's does.
            var ids = Regex.Matches(master, "\"([^\"]+Contract)\"").Select(x => x.Groups[1].Value).Distinct().ToList();
            ids.ShouldBe(V1Footprint.ContractTemplateIds, ignoreOrder: true);
            V1Footprint.ContractTemplateIds.ShouldAllBe(x => x.EndsWith("Contract", StringComparison.Ordinal));
        }

        [Fact]
        public void Every_Guardrails_stereotype_is_defined_in_a_package_named_for_its_module()
        {
            foreach (var module in V1Footprint.ModuleIds)
            {
                var metadata = Path.Combine(ModulesFolder(), "intent", ModuleFolder(module), "Intent.Metadata", "Module Builder");
                var packages = Directory.GetFiles(metadata, "*.pkg.config", SearchOption.AllDirectories);
                packages.Length.ShouldBe(1, module);
                XDocument.Load(packages[0]).Root!.Element("name")!.Value.ShouldBe(module);

                foreach (var stereotype in Directory.GetFiles(metadata, "*.xml", SearchOption.AllDirectories)
                             .Where(x => File.ReadAllText(x).Contains("<stereotypeDefinition ", StringComparison.Ordinal)))
                {
                    Path.GetRelativePath(metadata, stereotype).Split(Path.DirectorySeparatorChar)[0].ShouldBe(module, stereotype);
                }
            }
        }

        [Fact]
        public void Every_Guardrails_stereotype_on_origin_master_is_defined_in_a_package_named_for_its_module()
        {
            if (Git("rev-parse", "--verify", "origin/master") == null)
            {
                return;
            }

            foreach (var module in V1Footprint.ModuleIds)
            {
                // The UI module was published from another branch, so master may not have it.
                var root = $"Modules/intent/{ModuleFolder(module)}/Intent.Metadata/Module Builder/";
                var files = Git("ls-tree", "-r", "--name-only", "origin/master", "--", root);
                if (files == null)
                {
                    continue;
                }

                foreach (var file in files.Split('\n', StringSplitOptions.RemoveEmptyEntries).Where(x => x.Contains("/Stereotypes/", StringComparison.Ordinal)))
                {
                    file[root.Length..].Split('/')[0].ShouldBe(module, file);
                }

                var package = files.Split('\n').Single(x => x.EndsWith(".pkg.config", StringComparison.Ordinal));
                XDocument.Parse(Git("show", $"origin/master:{package}")!).Root!.Element("name")!.Value.ShouldBe(module);
            }
        }

        [Fact]
        public void The_subscription_association_types_are_the_Services_modules()
        {
            V1Footprint.SubscriptionAssociationTypeIds.ShouldBe(new[]
            {
                Intent.Services.Guardrails.Contracts.Api.IntegrationEventSubscriptionModel.SpecializationTypeId,
                Intent.Services.Guardrails.Contracts.Api.IntegrationCommandSubscriptionModel.SpecializationTypeId,
            });

            foreach (var (model, id) in new[] { ("IntegrationEventSubscriptionModel", V1Footprint.SubscriptionAssociationTypeIds[0]), ("IntegrationCommandSubscriptionModel", V1Footprint.SubscriptionAssociationTypeIds[1]) })
            {
                var master = Git("show", $"origin/master:Modules/Intent.Modules.Services.Guardrails.Contracts/Api/{model}.cs");
                if (master == null)
                {
                    return;
                }

                Regex.Match(master, "SpecializationTypeId = \"([^\"]+)\"").Groups[1].Value.ShouldBe(id, model);
            }
        }

        [Fact]
        public void The_eventing_designer_settings_are_the_Services_modules()
        {
            const string install = "modelers/Guardrails Eventing Extension.designer.settings";
            var spec = Path.Combine(ModulesFolder(), "Intent.Modules.Services.Guardrails.Contracts", "Intent.Services.Guardrails.Contracts.imodspec");
            ExternalReference(File.ReadAllText(spec), install).ShouldBe(V1Footprint.EventingExtensionDesignerSettingsId);

            var master = Git("show", "origin/master:Modules/Intent.Modules.Services.Guardrails.Contracts/Intent.Services.Guardrails.Contracts.imodspec");
            if (master != null)
            {
                ExternalReference(master, install).ShouldBe(V1Footprint.EventingExtensionDesignerSettingsId);
            }
        }

        [Fact]
        public void The_drift_engine_ids_are_its_own()
        {
            var metadata = Path.Combine(ModulesFolder(), "intent", "Intent.Modules.Common.DriftEngine", "Intent.Metadata", "Module Builder", "Intent.Common.DriftEngine");
            var package = XDocument.Load(Path.Combine(metadata, "Intent.Common.DriftEngine.pkg.config")).Root!;
            package.Attribute("id")!.Value.ShouldBe(V1Footprint.DriftEngine.PackageId);
            package.Element("name")!.Value.ShouldBe(V1Footprint.DriftEngine.PackageName);

            var stereotypes = Directory.GetFiles(Path.Combine(metadata, "Stereotypes"), "*.xml")
                .Select(x => XDocument.Load(x).Root!)
                .ToDictionary(x => x.Element("name")!.Value, x => x);

            void Check(string name, string definitionId, params (string Property, string Id)[] properties)
            {
                stereotypes[name].Attribute("id")!.Value.ShouldBe(definitionId, name);
                var actual = stereotypes[name].Descendants("property").ToDictionary(x => x.Element("name")!.Value, x => x.Attribute("id")!.Value);
                foreach (var (property, id) in properties)
                {
                    actual[property].ShouldBe(id, $"{name}.{property}");
                }
            }

            Check(V1Footprint.DriftEngine.DriftScope.Name, V1Footprint.DriftEngine.DriftScope.DefinitionId, ("Entries", V1Footprint.DriftEngine.DriftScope.Entries));
            Check(V1Footprint.DriftEngine.DriftScopeEntry.Name, V1Footprint.DriftEngine.DriftScopeEntry.DefinitionId,
                ("Glob", V1Footprint.DriftEngine.DriftScopeEntry.Glob), ("Types", V1Footprint.DriftEngine.DriftScopeEntry.Types),
                ("Scope", V1Footprint.DriftEngine.DriftScopeEntry.Scope), ("Reason", V1Footprint.DriftEngine.DriftScopeEntry.Reason));
            Check(V1Footprint.DriftEngine.DriftCheck.Name, V1Footprint.DriftEngine.DriftCheck.DefinitionId,
                ("Opt-in Rules", V1Footprint.DriftEngine.DriftCheck.OptInRules), ("Exceptions", V1Footprint.DriftEngine.DriftCheck.Exceptions));
            Check(V1Footprint.DriftEngine.OptInRule.Name, V1Footprint.DriftEngine.OptInRule.DefinitionId,
                ("Rule", V1Footprint.DriftEngine.OptInRule.Rule), ("Glob", V1Footprint.DriftEngine.OptInRule.Glob), ("Description", V1Footprint.DriftEngine.OptInRule.Description));
            Check(V1Footprint.DriftEngine.Exception.Name, V1Footprint.DriftEngine.Exception.DefinitionId,
                ("Rule", V1Footprint.DriftEngine.Exception.Rule), ("Match", V1Footprint.DriftEngine.Exception.Match), ("Reason", V1Footprint.DriftEngine.Exception.Reason));

            // The engine's generated API agrees with its metadata.
            var api = File.ReadAllText(Path.Combine(ModulesFolder(), "Intent.Modules.Common.DriftEngine", "Api", "RootFolderModelStereotypeExtensions.cs"));
            foreach (var id in new[] { V1Footprint.DriftEngine.DriftScope.DefinitionId, V1Footprint.DriftEngine.DriftScopeEntry.DefinitionId, V1Footprint.DriftEngine.DriftCheck.DefinitionId, V1Footprint.DriftEngine.OptInRule.DefinitionId, V1Footprint.DriftEngine.Exception.DefinitionId })
            {
                api.ShouldContain($"DefinitionId = \"{id}\"");
            }
        }

        /// <summary>The move's run of the engine reads the designers and stock packs the engine's own host does.</summary>
        [Fact]
        public void The_designers_and_packs_the_move_runs_the_engine_with_are_its_hosts()
        {
            var host = File.ReadAllText(Path.Combine(ModulesFolder(), "Intent.Modules.Common.DriftEngine", "Reporting", "DriftEngineReport.cs"));
            foreach (var (id, name) in V1Footprint.DriftEngine.DesignerNames)
            {
                host.ShouldContain($"[\"{name}\"] = \"{id}\"");
            }

            Regex.Match(host, @"StockPacks = \{ ([^}]*) \}").Groups[1].Value.ShouldBe(string.Join(", ", V1Footprint.DriftEngine.StockPacks.Select(x => $"\"{x}\"")));
        }

        [Theory]
        [InlineData("Intent.Services.Guardrails.Contracts.DtoContract#3f1c", true)]
        [InlineData("Intent.Domain.Guardrails.Contracts.EntityContract", true)]
        [InlineData("Intent.Services.Guardrails.Contracts.SyncModelSkill#3f1c", false)]
        [InlineData("Intent.Domain.Guardrails.Contracts.DriftReport#3f1c", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void A_contract_entry_is_known_by_the_template_id_before_the_hash(string correlationId, bool contract)
        {
            V1Footprint.IsContractCorrelation(correlationId).ShouldBe(contract);
        }

        [Theory]
        [InlineData("Intent.Domain.Guardrails.Contracts", "Anything", true)]
        [InlineData(null, "Intent.Services.Guardrails.Contracts.DtoContract", true)]
        [InlineData("Intent.Common.CSharp", "Intent.Common.CSharp.Templates.Program", false)]
        [InlineData(null, "Intent.Services.Guardrails.ContractsX.Dto", false)]
        public void A_Guardrails_template_output_is_the_modules_or_named_for_one_of_its_templates(string externalReference, string name, bool guardrails)
        {
            V1Footprint.IsGuardrailsTemplateOutput(externalReference, name).ShouldBe(guardrails);
        }

        private static string ModuleFolder(string moduleId) => "Intent.Modules." + moduleId["Intent.".Length..];

        private static string ExternalReference(string imodspec, string install) =>
            XDocument.Parse(imodspec).Descendants("install").Single(x => (string)x.Attribute("src") == install).Attribute("externalReference")!.Value;

        private static string ModulesFolder()
        {
            for (var folder = new DirectoryInfo(AppContext.BaseDirectory); folder != null; folder = folder.Parent)
            {
                if (Directory.Exists(Path.Combine(folder.FullName, "Intent.Modules.Common.Drift")) && Directory.Exists(Path.Combine(folder.FullName, "intent")))
                {
                    return folder.FullName;
                }
            }

            throw new InvalidOperationException("The Modules folder wasn't found above the test output.");
        }

        /// <summary>A git command's output in the repository, or null when git or <c>origin/master</c> isn't there.</summary>
        private static string Git(params string[] arguments)
        {
            try
            {
                var start = new ProcessStartInfo("git") { WorkingDirectory = ModulesFolder(), RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
                foreach (var argument in arguments)
                {
                    start.ArgumentList.Add(argument);
                }

                using var process = Process.Start(start)!;
                var output = process.StandardOutput.ReadToEnd();
                process.StandardError.ReadToEnd();
                process.WaitForExit();
                return process.ExitCode == 0 && output.Length > 0 ? output.Replace("\r\n", "\n") : null;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return null;
            }
        }
    }
}
