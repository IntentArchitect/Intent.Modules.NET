using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Intent.Modules.Domain.Guardrails.Contracts.Templates.DriftReport;
using Intent.Modules.Domain.Guardrails.Contracts.Templates.EnumContract;
using Intent.Templates;
using Intent.Utils;
using NSubstitute;
using NSubstitute.Extensions;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Fakes;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The real Domain drift report template, built the way the Software Factory builds file builder templates,
    /// next to a real contract template whose output it compares with the code.
    /// </summary>
    public sealed class DriftReportTemplateTests : IDisposable
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

        public DriftReportTemplateTests()
        {
            Directory.CreateDirectory(Path.Combine(_root, "guardrails"));
            Directory.CreateDirectory(Path.Combine(_root, "src", "Domain"));
            Directory.CreateDirectory(Path.Combine(_root, "src", "Infrastructure"));
            File.WriteAllText(Path.Combine(_root, "guardrails", "domain.drift.json"), Recipe);
            File.WriteAllText(Path.Combine(_root, "src", "Domain", "OrderStatus.cs"), "namespace Acme.Domain\n{\n    public enum OrderStatus\n    {\n        Open\n    }\n}\n");
            File.WriteAllText(Path.Combine(_root, "src", "Infrastructure", "AppDbContext.cs"), "class AppDbContext\n{\n    void OnModelCreating(ModelBuilder modelBuilder)\n    {\n        modelBuilder.Entity<Order>(e => e.ToTable(\"Order\"));\n    }\n}\n");
            Logging.SetTracing(_tracing);
        }

        [Fact]
        public void The_check_runs_after_every_file_is_built_stages_the_report_in_the_cache_folder_and_fails_the_run_with_one_error()
        {
            // The last run's report is deleted at registration, so this run's is always a new file in the Changes pane.
            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
            File.WriteAllText(ReportPath, "# Domain drift report\n\nNo drift.\n");
            var (report, enumTemplate) = Build();
            File.Exists(ReportPath).ShouldBeFalse();
            Path.GetFullPath(Path.Combine(ProjectFolder, report.GetTemplateFileConfig().LocationInProject)).ShouldBe(Path.GetDirectoryName(ReportPath));

            report.CanRunTemplate().ShouldBeTrue("the builder only builds templates that can run, so the check must be built");
            FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: true);

            var text = report.TransformText().Replace("\r\n", "\n");
            text.ShouldBe("""
                # Domain drift report

                ## Model and code drift

                | Model element | Code file | Drift |
                | --- | --- | --- |
                | <ModelRef application="Acme" elementId="Class:Order" designer="Domain" /> | <FileRef path="src/Infrastructure/AppDbContext.cs" line={5} /> | `Order` table name: model `Orders`, code `Order` |
                | `OrderStatus.Closed` | <FileRef path="src/Domain/OrderStatus.cs" /> | `OrderStatus.Closed` (literal) is modelled, but not in the code |

                """.ReplaceLineEndings("\n"));
            enumTemplate.CSharpFile.ToString().ShouldContain("Closed");
            // One error, naming the report and how many drifts it lists, on the first drifted model element, so Intent
            // offers to fix it with AI.
            _tracing.Received(1).Failure(Arg.Any<Exception>());
            _tracing.Received(1).Failure(Arg.Is<Exception>(x => x is ElementException && ((ElementException)x).ElementId == "Class:Order" &&
                x.Message == $"The Domain model and the code have drifted: 2 drifts detected and listed in the drift report ({ReportPath.Replace('\\', '/')})."));
            _tracing.DidNotReceive().Failure(Arg.Any<string>());
            report.CanRunTemplate().ShouldBeTrue("the report is the template's output");

            // The model as of this run, for the next run on this machine, beside the report.
            var baseline = ModelBaseline.Parse(File.ReadAllText(Path.Combine(Path.GetDirectoryName(ReportPath), "Domain.model.json")));
            baseline.Aspects.Keys.ShouldContain("OrderStatus.Closed");
        }

        [Fact]
        public void A_large_drift_is_still_one_error()
        {
            // Two more tables with no configuration: four facts each that can't be verified, all about a known element.
            var (report, _) = Build("Invoice", "Payment");

            FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: true);

            _tracing.Received(1).Failure(Arg.Any<Exception>());
            _tracing.Received(1).Failure(Arg.Is<Exception>(x => x is ElementException && x.Message.StartsWith("The Domain model and the code have drifted: 10 drifts detected and listed in the drift report")));
        }

        [Fact]
        public void A_drift_with_no_model_element_is_a_friendly_error()
        {
            // Only the enum literal the code lacks, whose row names no model element.
            File.WriteAllText(Path.Combine(_root, "src", "Infrastructure", "AppDbContext.cs"), "class AppDbContext\n{\n    void OnModelCreating(ModelBuilder modelBuilder)\n    {\n        modelBuilder.Entity<Order>(e => e.ToTable(\"Orders\"));\n    }\n}\n");
            var (report, _) = Build();

            FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: true);

            _tracing.Received(1).Failure(Arg.Any<Exception>());
            _tracing.Received(1).Failure(Arg.Is<Exception>(x => x is FriendlyException && x.Message ==
                $"The Domain model and the code have drifted: 1 drift detected and listed in the drift report ({ReportPath.Replace('\\', '/')})."));
        }

        [Fact]
        public void The_report_links_each_generated_type_and_member_to_the_model_element_it_represents()
        {
            var order = Substitute.For<IMetadataModel>();
            order.Id.Returns("order-id");
            var total = Substitute.For<IMetadataModel>();
            total.Id.Returns("total-id");
            // The check runs once every file is built, when the builder has applied each class's configuration.
            var file = new CSharpFile("Acme", "src").AddClass("Order");
            file.Classes.Single().RepresentsModel(order).AddProperty("decimal", "Total", property => property.RepresentsModel(total));

            DriftReportTemplate.ElementIds(file).ShouldBe(new Dictionary<string, string> { ["Order"] = "order-id", ["Order.Total"] = "total-id" }, ignoreOrder: true);
        }

        [Fact]
        public void A_run_with_no_drift_writes_no_report_and_says_so()
        {
            File.WriteAllText(Path.Combine(_root, "src", "Domain", "OrderStatus.cs"), "namespace Acme.Domain\n{\n    public enum OrderStatus\n    {\n        Open,\n        Closed\n    }\n}\n");
            File.WriteAllText(Path.Combine(_root, "src", "Infrastructure", "AppDbContext.cs"), "class AppDbContext\n{\n    void OnModelCreating(ModelBuilder modelBuilder)\n    {\n        modelBuilder.Entity<Order>(e => e.ToTable(\"Orders\"));\n    }\n}\n");
            // The recipe memory earlier versions kept, now that the recipes are validated on demand.
            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(ReportPath), "Domain.recipe.json"), "{}");
            var (report, _) = Build();

            FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: true);

            _tracing.DidNotReceive().Failure(Arg.Any<string>());
            _tracing.DidNotReceive().Warning(Arg.Any<string>());
            _tracing.Received(1).Info("The Domain drift check found no drift in 4 facts and 1 type, so it wrote no report.");

            // The template doesn't run, and the last report was deleted at registration, so nothing is proposed for it.
            report.CanRunTemplate().ShouldBeFalse();
            File.Exists(ReportPath).ShouldBeFalse();
            File.Exists(Path.Combine(Path.GetDirectoryName(ReportPath), "Domain.recipe.json")).ShouldBeFalse();
        }

        [Fact]
        public void Without_a_recipe_file_the_check_is_off_and_only_warns()
        {
            // The first run after installing: it generates the skill that writes the recipe file, and Intent applies
            // none of a failing run's changes. The code drifts on a fact and on the structure, and neither is reported.
            File.Delete(Path.Combine(_root, "guardrails", "domain.drift.json"));
            var (report, _) = Build();

            FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: true);

            var text = report.TransformText().Replace("\r\n", "\n");
            text.ShouldContain("## Warnings\n\n| Warning | Detail |\n| --- | --- |\n| The check is off | `guardrails/domain.drift.json` doesn't exist. Load the configure-drift-check skill to write it. |");
            text.ShouldNotContain("Wrong");
            _tracing.DidNotReceive().Failure(Arg.Any<string>());
            _tracing.DidNotReceive().Failure(Arg.Any<Exception>());
            _tracing.Received(1).Warning($"The Domain drift check is off: guardrails/domain.drift.json doesn't exist. Load the configure-drift-check skill to set it up. Report: {ReportPath}");
        }

        [Fact]
        public void On_CI_the_check_still_fails_the_run_but_the_report_is_neither_written_nor_deleted()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
            File.WriteAllText(ReportPath, "# Domain drift report\n\nNo drift.\n");
            Environment.SetEnvironmentVariable("TF_BUILD", "True");
            try
            {
                var (report, _) = Build();
                report.CanRunTemplate().ShouldBeTrue("the builder only builds templates that can run, so the check must be built");
                FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: false);
                FileBuilderHelper.PerformConfiguration(report.ExecutionContext, isForAfterBuild: true);

                // No report on CI, so the one error is a friendly error that lists the lines.
                _tracing.Received(1).Failure(Arg.Any<Exception>());
                _tracing.Received(1).Failure(Arg.Is<Exception>(x => x is FriendlyException && x.Message.StartsWith("The Domain model and the code have drifted: 2 drifts detected:\n\n- `Order` table name:")));
                report.CanRunTemplate().ShouldBeFalse();
                File.Exists(ReportPath).ShouldBeTrue();
                File.Exists(Path.Combine(Path.GetDirectoryName(ReportPath), "Domain.model.json")).ShouldBeFalse("on CI every run starts without a baseline");
            }
            finally
            {
                Environment.SetEnvironmentVariable("TF_BUILD", null);
            }
        }

        private string ProjectFolder => Path.Combine(_root, "src", "Api");

        private string ReportPath => Path.Combine(_root, "intent", ".cache", "guardrails", "Acme", "DomainDriftReport.mdx");

        public void Dispose()
        {
            Directory.Delete(_root, recursive: true);
        }

        private (DriftReportTemplate Report, EnumContractTemplate Enum) Build(params string[] moreTables)
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

            var context = Substitute.For<ISoftwareFactoryExecutionContext>();
            context.ReturnsForAll<ITemplate>((ITemplate)null);
            context.GetApplicationConfig().Returns(config);
            context.GetSolutionConfig().Returns(solution);
            context.MetadataManager.Returns(metadata);

            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.ExecutionContext.Returns(context);
            outputTarget.Name.Returns("Acme.Domain");
            outputTarget.Metadata.Returns(new Dictionary<string, object>());
            outputTarget.GetTargetPath().Returns(new[] { outputTarget });
            outputTarget.Location.Returns(ProjectFolder);

            var enumTemplate = new EnumContractTemplate(outputTarget, new EnumModel(status));
            enumTemplate.ConfigureFileMetadata(FileMetadata("OrderStatus", "cs", Path.Combine(_root, "src", "Domain")));
            var report = new DriftReportTemplate(outputTarget);

            // As the Software Factory resolves it: the output target's location combined with the template's location.
            report.ConfigureFileMetadata(FileMetadata("DomainDriftReport", "mdx", Path.GetFullPath(Path.Combine(ProjectFolder, report.GetTemplateFileConfig().LocationInProject))));

            var templates = new List<ITemplate> { enumTemplate, report };
            outputTarget.TemplateInstances.Returns(templates);
            context.OutputTargets.Returns(new[] { outputTarget });
            return (report, enumTemplate);
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
