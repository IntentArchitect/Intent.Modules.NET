using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Drift.Templates;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Intent.Modules.Domain.Guardrails.Contracts.FactoryExtensions;
using Intent.Modules.Domain.Guardrails.Contracts.Templates.EnumContract;
using Intent.Plugins.FactoryExtensions;
using Intent.Templates;
using Intent.Utils;
using NSubstitute;
using NSubstitute.Extensions;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Fakes;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>Tests that set the global tracing, so they don't run at the same time.</summary>
    [CollectionDefinition(nameof(GlobalStateCollection), DisableParallelization = true)]
    public class GlobalStateCollection
    {
    }

    /// <summary>
    /// The Domain designer's drift check, run as its reporter runs it after template execution: next to a real contract
    /// template whose built output it compares with the code, adding what it finds to a recording drift context.
    /// </summary>
    [Collection(nameof(GlobalStateCollection))]
    public sealed class DriftCheckRunnerTests : IDisposable
    {
        private const string Recipe = """
        {
          "files": "src/**/*.cs",
          "facts": {
            "table.name": { "find": "statement:Entity<{Entity}>", "read": [{ "match": "ToTable\\(\"([^\"]+)\"" }] },
            "table.schema": { "find": "statement:Entity<{Entity}>", "default": "" },
            "table.kind": { "find": "statement:Entity<{Entity}>", "default": "Table" },
            "key": { "find": "statement:Entity<{Entity}>", "default": "" }
          }
        }
        """;

        private readonly string _root = Path.Combine(Path.GetTempPath(), $"guardrails-drift-{Guid.NewGuid():N}");
        private readonly ITracing _tracing = Substitute.For<ITracing>();
        private readonly ITracing _previousTracing = Logging.Log;

        public DriftCheckRunnerTests()
        {
            Directory.CreateDirectory(Path.Combine(_root, "guardrails"));
            Directory.CreateDirectory(Path.Combine(_root, "src", "Domain"));
            Directory.CreateDirectory(Path.Combine(_root, "src", "Infrastructure"));
            File.WriteAllText(Path.Combine(_root, "guardrails", "domain.drift.json"), Recipe);
            // The code lacks the modelled Closed, which the Software Factory is adding, and has Archived, which isn't modelled.
            File.WriteAllText(Path.Combine(_root, "src", "Domain", "OrderStatus.cs"), "namespace Acme.Domain\n{\n    public enum OrderStatus\n    {\n        Open,\n        Archived\n    }\n}\n");
            File.WriteAllText(Path.Combine(_root, "src", "Infrastructure", "AppDbContext.cs"), "class AppDbContext\n{\n    void OnModelCreating(ModelBuilder modelBuilder)\n    {\n        modelBuilder.Entity<Order>(e => e.ToTable(\"Order\"));\n    }\n}\n");
            Logging.SetTracing(_tracing);
        }

        public void Dispose()
        {
            Logging.SetTracing(_previousTracing);
            Directory.Delete(_root, recursive: true);
        }

        private string GuardrailsCache => Path.Combine(_root, "intent", ".cache", "guardrails", "Acme");

        [Fact]
        public void The_check_compares_every_built_file_with_the_code_and_adds_each_drift_to_the_Domain_area()
        {
            // The per-designer report an earlier version wrote is deleted: the drift report replaces it.
            Directory.CreateDirectory(GuardrailsCache);
            File.WriteAllText(Path.Combine(GuardrailsCache, "DomainDriftReport.mdx"), "# Domain drift report\n\nNo drift.\n");
            var (application, enumTemplate) = Build();

            var area = Run(application);

            area.Name.ShouldBe("Domain");
            area.Checks.ShouldBe(new[] { DriftCategory.ModelCode, DriftCategory.Code }, ignoreOrder: true);
            area.Snapshot(_root).ShouldBe(new[]
            {
                "Error Model/Code | element:Class:Order | src/Infrastructure/AppDbContext.cs:5 | `Order` table name: model `Orders`, code `Order`",
                "Error Model/Code | src/Domain/OrderStatus.cs:6 | — | `OrderStatus.Archived` (public literal) isn't modelled"
            });
            enumTemplate.CSharpFile.ToString().ShouldContain("Closed");
            _tracing.DidNotReceive().Failure(Arg.Any<Exception>());
            _tracing.DidNotReceive().Failure(Arg.Any<string>());
            File.Exists(Path.Combine(GuardrailsCache, "DomainDriftReport.mdx")).ShouldBeFalse();

            // The model as of this run, for the next run on this machine.
            var baseline = ModelBaseline.Parse(File.ReadAllText(Path.Combine(GuardrailsCache, "Domain.model.json")));
            baseline.Aspects.Keys.ShouldContain("OrderStatus.Open");
        }

        [Fact]
        public void Each_drift_is_its_own_error()
        {
            // Two more tables with no configuration: four facts each that can't be verified, all about a known element.
            var (application, _) = Build("Invoice", "Payment");

            var area = Run(application);

            area.Findings.Count(x => x.Severity == DriftSeverity.Error).ShouldBe(10);
            area.Findings.Where(x => x.Message.Contains("can't be verified")).ShouldAllBe(x => x.Subject.Kind == DriftAnchorKind.Element);
        }

        [Fact]
        public void The_element_ids_of_each_generated_type_and_member_are_read_from_the_builder()
        {
            var order = Substitute.For<IMetadataModel>();
            order.Id.Returns("order-id");
            var total = Substitute.For<IMetadataModel>();
            total.Id.Returns("total-id");
            // The check runs once every file is built, when the builder has applied each class's configuration.
            var file = new CSharpFile("Acme", "src").AddClass("Order");
            file.Classes.Single().RepresentsModel(order).AddProperty("decimal", "Total", property => property.RepresentsModel(total));

            DriftCheckRunner.ElementIds(file).ShouldBe(new Dictionary<string, string> { ["Order"] = "order-id", ["Order.Total"] = "total-id" }, ignoreOrder: true);
        }

        [Fact]
        public void A_run_with_no_drift_adds_only_notes_and_says_so()
        {
            File.WriteAllText(Path.Combine(_root, "src", "Domain", "OrderStatus.cs"), "namespace Acme.Domain\n{\n    public enum OrderStatus\n    {\n        Open,\n        Closed\n    }\n}\n");
            File.WriteAllText(Path.Combine(_root, "src", "Infrastructure", "AppDbContext.cs"), "class AppDbContext\n{\n    void OnModelCreating(ModelBuilder modelBuilder)\n    {\n        modelBuilder.Entity<Order>(e => e.ToTable(\"Orders\"));\n    }\n}\n");
            // The recipe memory earlier versions kept, now that the recipes are validated on demand.
            Directory.CreateDirectory(GuardrailsCache);
            File.WriteAllText(Path.Combine(GuardrailsCache, "Domain.recipe.json"), "{}");
            var (application, _) = Build();

            var area = Run(application);

            area.Findings.ShouldBeEmpty();
            _tracing.DidNotReceive().Warning(Arg.Any<string>());
            _tracing.Received(1).Info("The Domain drift check found no drift in 4 facts and 1 type.");
            File.Exists(Path.Combine(GuardrailsCache, "Domain.recipe.json")).ShouldBeFalse();
        }

        [Fact]
        public void Without_a_recipe_file_the_check_is_off_only_warns_and_has_no_area()
        {
            // The first run after installing: it generates the skill that writes the recipe file, and Intent applies
            // none of a failing run's changes. The code drifts on a fact and on the structure, and neither is reported.
            File.Delete(Path.Combine(_root, "guardrails", "domain.drift.json"));
            var (application, _) = Build();
            var context = new RecordingDriftContext(application);

            DriftCheckRunner.Report(context, DomainDrift.Designer);

            context.Areas.ShouldBeEmpty();
            _tracing.DidNotReceive().Failure(Arg.Any<Exception>());
            _tracing.Received(1).Warning("The Domain drift check is off: guardrails/domain.drift.json doesn't exist. Load the configure-drift-check skill to set it up.");
        }

        [Fact]
        public void On_CI_the_drift_is_still_reported_but_nothing_is_remembered()
        {
            var (application, _) = Build();
            BuildFiles(application);
            var context = new RecordingDriftContext(application);

            DriftCheckRunner.Report(context, DomainDrift.Designer, onCi: true);

            context.Areas.ShouldHaveSingleItem().Findings.Count.ShouldBe(2);
            File.Exists(Path.Combine(GuardrailsCache, "Domain.model.json")).ShouldBeFalse("on CI every run starts without a baseline");
        }

        [Fact]
        public void The_Domain_reporter_contributes_the_designer_and_reports_through_the_drift_checks()
        {
            var (application, _) = Build();
            var reporter = new DomainDriftReporter();
            reporter.Designer.ShouldBeSameAs(DomainDrift.Designer);
            reporter.Order.ShouldBe(DomainDrift.Designer.Order);
            BuildFiles(application);

            var checks = DriftHub.Register(application, reporter);
            ModelSyncGuidance.Designers(application).ShouldBe(new[] { DomainDrift.Designer });
            ((IExecutionLifeCycle)checks).OnStep(application, ExecutionLifeCycleSteps.AfterTemplateExecution);

            // One error for the run's drift, with each Area's counts and the report.
            _tracing.Received(1).Failure(Arg.Any<Exception>());
            _tracing.Received(1).Failure(Arg.Is<Exception>(x => x.Message == "Drift found: 2 errors, 0 warnings. Report: intent/.cache/drift/Acme/DriftReport.mdx\n- Drift in Domain: 2 errors, 0 warnings"));
            application.ChangeManager.Received(1).AddChange(
                Path.GetFullPath(Path.Combine(_root, "intent", ".cache", "drift", "Acme", "DriftReport.mdx")),
                Arg.Is<string>(x => x.Contains("## Domain") && x.Contains("### Model / Code drift (2)") && x.Contains("### Code drift\n\n✓ Clean")));
        }

        private RecordingDriftArea Run(IApplication application)
        {
            BuildFiles(application);
            var context = new RecordingDriftContext(application);
            DriftCheckRunner.Report(context, DomainDrift.Designer);
            return context.Areas.ShouldHaveSingleItem();
        }

        /// <summary>The file builders' two passes, which every template has been through by the time reporters run.</summary>
        private static void BuildFiles(IApplication application)
        {
            FileBuilderHelper.PerformConfiguration(application, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(application, isForAfterBuild: true);
        }

        private (IApplication Application, EnumContractTemplate Enum) Build(params string[] moreTables)
        {
            var package = Package();
            var status = Element(package, "Enum", "OrderStatus");
            Element(status, "Enum-Literal", "Open").SpecializationTypeId.Returns(EnumLiteralModel.SpecializationTypeId);
            Element(status, "Enum-Literal", "Closed").SpecializationTypeId.Returns(EnumLiteralModel.SpecializationTypeId);
            Element(package, "Class", "Order", Stereotype("Table", ("Name", "Orders")));
            foreach (var table in moreTables)
            {
                Element(package, "Class", table, Stereotype("Table", ("Name", table)));
            }

            var config = Substitute.For<IApplicationConfig>();
            config.Id.Returns(Fakes.ApplicationId);
            config.Name.Returns("Acme");
            config.DirectoryPath.Returns(_root);
            config.OutputLocation.Returns(".");
            var designer = Designer(new[] { package });
            var metadata = Substitute.For<IMetadataManager>();
            metadata.GetDesigner(Fakes.ApplicationId, ApiMetadataDesignerExtensions.DomainDesignerId).Returns(designer);

            var solution = Substitute.For<ISolutionConfig>();
            solution.SolutionRootLocation.Returns(Path.Combine(_root, "intent"));

            var application = Substitute.For<IApplication>();
            application.ReturnsForAll<ITemplate>((ITemplate)null);
            application.GetApplicationConfig().Returns(config);
            application.GetSolutionConfig().Returns(solution);
            application.MetadataManager.Returns(metadata);
            application.OutputRootDirectory.Returns(_root);
            application.EventDispatcher.Returns(DriftHub.Dispatcher());
            application.ChangeManager.Returns(Substitute.For<IChanges>());

            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.ExecutionContext.Returns(application);
            outputTarget.Name.Returns("Acme.Domain");
            outputTarget.Metadata.Returns(new Dictionary<string, object>());
            outputTarget.GetTargetPath().Returns(new[] { outputTarget });
            outputTarget.Location.Returns(Path.Combine(_root, "src", "Api"));

            var enumTemplate = new EnumContractTemplate(outputTarget, new EnumModel(status));
            enumTemplate.ConfigureFileMetadata(FileMetadata("OrderStatus", "cs", Path.Combine(_root, "src", "Domain")));
            outputTarget.TemplateInstances.Returns(new List<ITemplate> { enumTemplate });
            application.OutputTargets.Returns(new[] { outputTarget });
            return (application, enumTemplate);
        }

        private static IFileMetadata FileMetadata(string fileName, string extension, string folder)
        {
            var metadata = Substitute.For<IFileMetadata>();
            metadata.FileName.Returns(fileName);
            metadata.FileExtension.Returns(extension);
            metadata.CustomMetadata.Returns(new Dictionary<string, string>());
            metadata.GetFullLocationPath().Returns(folder);
            return metadata;
        }
    }
}
