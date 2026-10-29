using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Templates;
using Intent.Utils;
using NSubstitute;
using NSubstitute.Extensions;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Fakes;
using CoreTemplates = Intent.Modules.Common.Drift.Templates;
using DomainDesigner = Intent.Modelers.Domain.Api.ApiMetadataDesignerExtensions;
using DomainDrift = Intent.Modules.Domain.Guardrails.Contracts.Drift.DomainDrift;
using DomainEnumContractTemplate = Intent.Modules.Domain.Guardrails.Contracts.Templates.EnumContract.EnumContractTemplate;
using DomainReportTemplate = Intent.Modules.Domain.Guardrails.Contracts.Templates.DriftReport.DriftReportTemplate;
using DomainTask = Intent.Modules.Domain.Guardrails.Contracts.Tasks.ValidateDriftRecipes;
using ServicesDesigner = Intent.Modelers.Services.Api.ApiMetadataDesignerExtensions;
using ServicesDrift = Intent.Modules.Services.Guardrails.Contracts.Drift.ServicesDrift;
using ServicesReportTemplate = Intent.Modules.Services.Guardrails.Contracts.Templates.DriftReport.DriftReportTemplate;
using ServicesTask = Intent.Modules.Services.Guardrails.Contracts.Tasks.ValidateDriftRecipes;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// What a consumer sees, rendered and compared with the files in <c>Fixtures/Golden</c>: the rule and skills for each
    /// combination of installed Guardrails designers (the files each writes, and their text), each drift report template's
    /// report and log, and each ValidateDriftRecipes task's JSON. The files were captured before the drift check moved into
    /// its own module, and prove that the move changed none of it. Set <c>GUARDRAILS_GOLDEN=update</c> to rewrite them.
    /// </summary>
    [Collection(nameof(DriftReportTemplateTests))]
    public sealed class GoldenTests : IDisposable
    {
        private const string DomainRecipe = """
        {
          "files": "src/**/*.cs",
          "facts": {
            "table.name": { "find": "statement:Entity<{Entity}>", "read": [{ "match": "ToTable\\(\"([^\"]+)\"" }] },
            "table.schema": { "find": "statement:Entity<{Entity}>", "default": "" },
            "table.kind": { "find": "statement:Entity<{Entity}>", "default": "Table" },
            "key": { "find": "statement:Entity<{Entity}>", "default": "" }
          },
          "own": [{ "kind": "Entity", "find": "class with \\bclass \\w+\\b", "files": "src/Domain/**/*.cs" }],
          "exceptions": [{ "match": "Unused", "reason": "matches nothing" }]
        }
        """;

        private const string ServicesRecipe = """
        {
          "files": "src/Api/**/*.cs",
          "facts": {
            "service.route": { "find": "class:{Service}", "read": [{ "part": "declaration", "match": "\\[Route\\(\"([^\"]*)\"\\)\\]" }], "default": "" },
            "operation.verb": { "find": "class:{Service} > method:{Operation}", "read": [{ "part": "declaration", "match": "\\[Http(Get|Post|Put|Delete|Patch)\\b", "case": "upper" }] },
            "operation.route": { "find": "class:{Service} > method:{Operation}", "read": [{ "part": "declaration", "match": "\\[Http\\w+\\(\"([^\"]*)\"\\)\\]" }], "default": "" },
            "parameter.source": { "find": "class:{Service} > method:{Operation} > parameter:{Parameter}", "read": [{ "match": "\\[FromRoute\\]", "value": "From Route" }], "default": "Default" }
          },
          "own": [{ "kind": "endpoint", "find": "class with :ControllerBase\\b > method with \\[Http\\w+", "name": "{Class}.{Method}" }]
        }
        """;

        private readonly string _root = Path.Combine(Path.GetTempPath(), $"guardrails-golden-{Guid.NewGuid():N}");
        private readonly ITracing _tracing = Substitute.For<ITracing>();
        private readonly string _tfBuild = Environment.GetEnvironmentVariable("TF_BUILD");

        public GoldenTests()
        {
            Environment.SetEnvironmentVariable("TF_BUILD", null);
            Directory.CreateDirectory(Path.Combine(_root, "guardrails"));
            Directory.CreateDirectory(Path.Combine(_root, "src", "Domain"));
            Directory.CreateDirectory(Path.Combine(_root, "src", "Infrastructure"));
            Directory.CreateDirectory(Path.Combine(_root, "src", "Api"));
            File.WriteAllText(Path.Combine(_root, "guardrails", "domain.drift.json"), DomainRecipe);
            File.WriteAllText(Path.Combine(_root, "guardrails", "services.drift.json"), ServicesRecipe);
            File.WriteAllText(Path.Combine(_root, "src", "Domain", "OrderStatus.cs"), "namespace Acme.Domain\n{\n    public enum OrderStatus\n    {\n        Open,\n        Archived\n    }\n}\n");
            File.WriteAllText(Path.Combine(_root, "src", "Domain", "Order.cs"), "namespace Acme.Domain\n{\n    public class Order { }\n}\n");
            File.WriteAllText(Path.Combine(_root, "src", "Domain", "Helper.cs"), "namespace Acme.Domain\n{\n    public class Helper { }\n}\n");
            File.WriteAllText(Path.Combine(_root, "src", "Infrastructure", "AppDbContext.cs"), "class AppDbContext\n{\n    void OnModelCreating(ModelBuilder modelBuilder)\n    {\n        modelBuilder.Entity<Order>(e => e.ToTable(\"Order\"));\n    }\n}\n");
            File.WriteAllText(Path.Combine(_root, "src", "Api", "OrdersController.cs"), """
                [Route("api/order")]
                public class OrdersController : ControllerBase
                {
                    [HttpGet("{id}")]
                    public IActionResult GetOrder([FromRoute] int id) => Ok();

                    [HttpDelete("{id}")]
                    public IActionResult Delete(int id) => Ok();
                }

                """);
            Logging.SetTracing(_tracing);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("TF_BUILD", _tfBuild);
            Directory.Delete(_root, recursive: true);
        }

        [Theory]
        [InlineData("both", true, true)]
        [InlineData("domain", true, false)]
        [InlineData("services", false, true)]
        public void The_rule_and_skills_are_as_captured(string install, bool domain, bool services)
        {
            var rendered = RenderGuidance(domain, services);

            rendered.Keys.ShouldBe(new[]
            {
                "configure-drift-check/SKILL.md", "guardrails-model-sync.md", "sync-model/SKILL.md", "sync-model/references/intent-drift.md",
                "validate-drift-recipes/SKILL.md"
            }, ignoreOrder: true);
            foreach (var (path, text) in rendered)
            {
                ShouldMatchGolden($"Guidance/{install}/{path}", text);
            }
        }

        [Fact]
        public void The_Domain_drift_report_and_its_log_are_as_captured()
        {
            var package = Package();
            var status = Element(package, "Enum", "OrderStatus");
            Element(status, "Enum-Literal", "Open").SpecializationTypeId.Returns(EnumLiteralModel.SpecializationTypeId);
            Element(status, "Enum-Literal", "Closed").SpecializationTypeId.Returns(EnumLiteralModel.SpecializationTypeId);
            Element(package, "Class", "Order", Stereotype("Table", ("Name", "Orders")));
            Element(package, "Class", "Invoice", Stereotype("Table", ("Name", "Invoice")));
            var (context, outputTarget) = Build(DomainDesigner.DomainDesignerId, package);

            var enumTemplate = new DomainEnumContractTemplate(outputTarget, new EnumModel(status));
            enumTemplate.ConfigureFileMetadata(FileMetadata("OrderStatus", "cs", Path.Combine(_root, "src", "Domain")));
            var report = new DomainReportTemplate(outputTarget);
            report.ShouldBeAssignableTo<IDriftDesignerSource>().Designer.ShouldBeSameAs(DomainDrift.Designer);

            ShouldMatchGolden("Reports/Domain.mdx", Run(context, outputTarget, report, "DomainDriftReport", enumTemplate));
            ShouldMatchGolden("Reports/Domain.log", Log());
        }

        [Fact]
        public void The_Services_drift_report_and_its_log_are_as_captured()
        {
            var (context, outputTarget) = Build(ServicesDesigner.ServicesDesignerId, ServicesPackage());
            var report = new ServicesReportTemplate(outputTarget);
            report.ShouldBeAssignableTo<IDriftDesignerSource>().Designer.ShouldBeSameAs(ServicesDrift.Designer);

            ShouldMatchGolden("Reports/Services.mdx", Run(context, outputTarget, report, "ServicesDriftReport"));
            ShouldMatchGolden("Reports/Services.log", Log());
        }

        [Fact]
        public void Each_ValidateDriftRecipes_task_returns_the_JSON_captured()
        {
            var domainPackage = Package();
            Element(domainPackage, "Class", "Order", Stereotype("Table", ("Name", "Orders")));
            var (domainSolution, domainMetadata) = TaskHost(DomainDesigner.DomainDesignerId, domainPackage);
            var domain = new DomainTask(domainSolution, domainMetadata);
            var (servicesSolution, servicesMetadata) = TaskHost(ServicesDesigner.ServicesDesignerId, ServicesPackage());
            var services = new ServicesTask(servicesSolution, servicesMetadata);

            domain.TaskTypeId.ShouldBe("Intent.Domain.Guardrails.Contracts.ValidateDriftRecipes");
            services.TaskTypeId.ShouldBe("Intent.Services.Guardrails.Contracts.ValidateDriftRecipes");
            var results = new[]
            {
                domain.Execute(JsonSerializer.Serialize(new { applicationId = Fakes.ApplicationId })),
                domain.Execute(JsonSerializer.Serialize(new { applicationId = Fakes.ApplicationId, fact = "table.name", file = "AppDbContext.cs" })),
                domain.Execute(JsonSerializer.Serialize(new { applicationId = Fakes.ApplicationId, fact = "operation.route" })),
                domain.Execute(),
                domain.Execute("""{ "applicationId": "nope" }"""),
                services.Execute(JsonSerializer.Serialize(new { applicationId = Fakes.ApplicationId })),
                services.Execute(JsonSerializer.Serialize(new { applicationId = Fakes.ApplicationId, element = "OrdersController.GetOrder" })),
                services.Execute(JsonSerializer.Serialize(new { applicationId = Fakes.ApplicationId, fact = "table.name" }))
            };

            ShouldMatchGolden("Tasks/ValidateDriftRecipes.txt", string.Join("\n", results.Select(x => x.Replace(_root, "<root>").Replace(_root.Replace('\\', '/'), "<root>"))) + "\n");
        }

        /// <summary>
        /// The rule and skills for an application with the given Guardrails designers, as the Software Factory would write
        /// them: the drift module's guidance templates are registered beside a template contributing each designer (listed
        /// in reverse, as registration order isn't theirs), and those that can run write their file, keyed by its path under
        /// the template's output (no two may write the same path). That each Guardrails module's report template is the
        /// contribution is checked with its report.
        /// </summary>
        private static IReadOnlyDictionary<string, string> RenderGuidance(bool domain, bool services)
        {
            var context = GuidanceContext();
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.ExecutionContext.Returns(context);
            outputTarget.Metadata.Returns(new Dictionary<string, object>());
            var templates = new List<MarkdownBaseTemplate<object>>
            {
                new CoreTemplates.ModelSyncRule.ModelSyncRuleTemplate(outputTarget),
                new CoreTemplates.SyncModelSkill.SyncModelSkillTemplate(outputTarget),
                new CoreTemplates.IntentDriftReference.IntentDriftReferenceTemplate(outputTarget),
                new CoreTemplates.ConfigureDriftCheckSkill.ConfigureDriftCheckSkillTemplate(outputTarget),
                new CoreTemplates.ValidateDriftRecipesSkill.ValidateDriftRecipesSkillTemplate(outputTarget)
            };
            var contributions = new[] { (services, ServicesDrift.Designer), (domain, DomainDrift.Designer) }
                .Where(x => x.Item1)
                .Select(x => Contribution(x.Item2))
                .Concat(templates)
                .ToList();

            outputTarget.TemplateInstances.Returns(contributions);
            context.OutputTargets.Returns(new[] { outputTarget });
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: true);

            var result = new Dictionary<string, string>();
            foreach (var template in templates.Where(x => x.CanRunTemplate()))
            {
                var config = template.GetTemplateFileConfig();
                var path = string.Join("/", new[] { config.LocationInProject?.Replace('\\', '/'), $"{config.FileName}.{config.FileExtension}" }.Where(x => !string.IsNullOrEmpty(x)));
                result.Add(path, template.TransformText().Replace("\r\n", "\n"));
            }

            return result;
        }

        private static ITemplate Contribution(DriftDesigner designer)
        {
            var template = Substitute.For<ITemplate, IDriftDesignerSource>();
            ((IDriftDesignerSource)template).Designer.Returns(designer);
            return template;
        }

        private static ISoftwareFactoryExecutionContext GuidanceContext()
        {
            var solution = Substitute.For<ISolutionConfig>();
            solution.SolutionRootLocation.Returns(Path.Combine(Path.GetPathRoot(Path.GetTempPath()), "Repos", "Acme", "intent") + Path.DirectorySeparatorChar);
            var application = Substitute.For<IApplicationConfig>();
            application.Name.Returns("Acme");
            var context = Substitute.For<ISoftwareFactoryExecutionContext>();
            context.ReturnsForAll<ITemplate>((ITemplate)null);
            context.GetSolutionConfig().Returns(solution);
            context.GetApplicationConfig().Returns(application);
            return context;
        }

        /// <summary>A Services package with one controller: its route and one action's binding are modelled differently from the code.</summary>
        private static Intent.Metadata.Models.IPackage ServicesPackage()
        {
            var package = Package();
            var service = Element(package, "Service", "OrdersController", Stereotype("Http Service Settings", ("Route", "api/orders")));
            var get = Element(service, "Operation", "GetOrder", Stereotype("Http Settings", ("Verb", "GET"), ("Route", "{id}")));
            Element(get, "Parameter", "id", Stereotype("Parameter Settings", ("Source", "From Query")));
            return package;
        }

        private (ISoftwareFactoryExecutionContext Context, IOutputTarget OutputTarget) Build(string designerId, Intent.Metadata.Models.IPackage package)
        {
            var config = Substitute.For<IApplicationConfig>();
            config.Id.Returns(Fakes.ApplicationId);
            config.Name.Returns("Acme");
            config.DirectoryPath.Returns(_root);
            config.OutputLocation.Returns(".");
            var metadata = Substitute.For<IMetadataManager>();
            var designer = Designer(new[] { package });
            metadata.GetDesigner(Fakes.ApplicationId, designerId).Returns(designer);

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
            outputTarget.Location.Returns(Path.Combine(_root, "src", "Api"));
            return (context, outputTarget);
        }

        private string Run(ISoftwareFactoryExecutionContext context, IOutputTarget outputTarget, MarkdownBaseTemplate<object> report, string fileName, params ITemplate[] contracts)
        {
            report.ConfigureFileMetadata(FileMetadata(fileName, "mdx", Path.GetFullPath(Path.Combine(outputTarget.Location, report.GetTemplateFileConfig().LocationInProject))));
            outputTarget.TemplateInstances.Returns(contracts.Append(report).ToList());
            context.OutputTargets.Returns(new[] { outputTarget });

            report.CanRunTemplate().ShouldBeTrue();
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: true);
            report.CanRunTemplate().ShouldBeTrue("the report needs attention");
            return report.TransformText().Replace("\r\n", "\n");
        }

        /// <summary>Everything the run logged, one line each, with this test's folder written as <c>&lt;root&gt;</c>.</summary>
        private string Log()
        {
            var lines = _tracing.ReceivedCalls().Select(call =>
            {
                var arguments = call.GetArguments().Select(x => x switch
                {
                    ElementException e => $"{nameof(ElementException)} on {e.ElementId}: {e.Message}",
                    Exception e => $"{e.GetType().Name}: {e.Message}",
                    _ => x?.ToString()
                });
                return $"{call.GetMethodInfo().Name}: {string.Join(", ", arguments)}";
            });
            return string.Concat(lines.Select(x => x.Replace(_root, "<root>").Replace(_root.Replace('\\', '/'), "<root>").Replace('\\', '/') + "\n"));
        }

        private (ISolutionConfig Solution, IMetadataManager Metadata) TaskHost(string designerId, Intent.Metadata.Models.IPackage package)
        {
            var metadata = Substitute.For<IMetadataManager>();
            var designer = Designer(new[] { package });
            metadata.GetDesigner(Fakes.ApplicationId, designerId).Returns(designer);
            var config = Substitute.For<IApplicationConfig>();
            config.Id.Returns(Fakes.ApplicationId);
            config.DirectoryPath.Returns(_root);
            config.OutputLocation.Returns(".");
            var solution = Substitute.For<ISolutionConfig>();
            solution.GetApplicationConfig(Arg.Any<string>()).Returns((IApplicationConfig)null);
            solution.GetApplicationConfig(Fakes.ApplicationId).Returns(config);
            return (solution, metadata);
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

        private static void ShouldMatchGolden(string relativePath, string actual)
        {
            // Line breaks as git stores them: indented JSON uses the OS's.
            actual = actual.Replace("\r\n", "\n");
            if (Environment.GetEnvironmentVariable("GUARDRAILS_GOLDEN") == "update")
            {
                var path = Path.Combine(TestsFolder(), "Fixtures", "Golden", relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, actual);
                return;
            }

            var golden = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Golden", relativePath);
            File.Exists(golden).ShouldBeTrue($"No golden file {relativePath}: run the tests with GUARDRAILS_GOLDEN=update to capture it.");
            var expected = File.ReadAllText(golden).Replace("\r\n", "\n");
            if (actual != expected)
            {
                // Beside the test output, for a diff with the golden file.
                var path = Path.Combine(AppContext.BaseDirectory, "GoldenActual", relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, actual);
            }

            actual.ShouldBe(expected, $"{relativePath} differs from what was captured: see {Path.Combine(AppContext.BaseDirectory, "GoldenActual", relativePath)}");
        }

        private static string TestsFolder()
        {
            for (var folder = new DirectoryInfo(AppContext.BaseDirectory); folder != null; folder = folder.Parent)
            {
                if (File.Exists(Path.Combine(folder.FullName, "Intent.Modules.Guardrails.Contracts.Tests.csproj")))
                {
                    return folder.FullName;
                }
            }

            throw new InvalidOperationException("The test project's folder wasn't found above the test output.");
        }
    }
}
