using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.DriftReporting.Api;
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
using DomainReporter = Intent.Modules.Domain.Guardrails.Contracts.FactoryExtensions.DomainDriftReporter;
using DomainTask = Intent.Modules.Domain.Guardrails.Contracts.Tasks.ValidateDriftRecipes;
using ServicesDesigner = Intent.Modelers.Services.Api.ApiMetadataDesignerExtensions;
using ServicesDrift = Intent.Modules.Services.Guardrails.Contracts.Drift.ServicesDrift;
using ServicesReporter = Intent.Modules.Services.Guardrails.Contracts.FactoryExtensions.ServicesDriftReporter;
using ServicesTask = Intent.Modules.Services.Guardrails.Contracts.Tasks.ValidateDriftRecipes;
using UiDrift = Intent.Modules.UI.Guardrails.Contracts.Drift.UiDrift;
using UiReporter = Intent.Modules.UI.Guardrails.Contracts.FactoryExtensions.UiDriftReporter;
using UiTask = Intent.Modules.UI.Guardrails.Contracts.Tasks.ValidateDriftRecipes;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// What a consumer sees, rendered and compared with the files in <c>Fixtures/Golden</c>: the rule and skills for each
    /// combination of installed Guardrails designers (the files each writes, and their text), what each designer's drift
    /// check adds to its Area of the drift report (and logs), and each ValidateDriftRecipes task's JSON. Set
    /// <c>GUARDRAILS_GOLDEN=update</c> to rewrite them.
    /// </summary>
    [Collection(nameof(GlobalStateCollection))]
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
        private readonly ITracing _previousTracing = Logging.Log;

        public GoldenTests()
        {
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
            Logging.SetTracing(_previousTracing);
            Directory.Delete(_root, recursive: true);
        }

        private const string UiRecipe = """
        {
          "facts": {
            "component.kind": { "find": "file:**/{Component}.razor*", "read": [{ "match": "@page\"", "value": "Page" }], "default": "Component" },
            "page.route": { "find": "file:**/{Component}.razor*", "read": [{ "match": "@page\"/?([^\"]*)\"" }], "default": "" },
            "security": { "find": "file:**/{Component}.razor*", "read": [{ "match": "\\[Authorize\\b", "value": "Secured" }], "default": "None" }
          },
          "own": [{ "kind": "page", "find": "file with @page\"", "files": "src/Web/**/*.razor" }]
        }
        """;

        [Theory]
        [InlineData("both", true, true, false)]
        [InlineData("domain", true, false, false)]
        [InlineData("services", false, true, false)]
        [InlineData("ui", false, false, true)]
        [InlineData("all", true, true, true)]
        public void The_rule_and_skills_are_as_captured(string install, bool domain, bool services, bool ui)
        {
            var rendered = RenderGuidance(domain, services, ui);

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
        public void The_Domain_drift_findings_and_log_are_as_captured()
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
            var reporter = new DomainReporter();
            reporter.Designer.ShouldBeSameAs(DomainDrift.Designer);

            ShouldMatchGolden("Findings/Domain.txt", Run(context, outputTarget, reporter, enumTemplate));
        }

        [Fact]
        public void The_Services_drift_findings_and_log_are_as_captured()
        {
            var (context, outputTarget) = Build(ServicesDesigner.ServicesDesignerId, ServicesPackage());
            var reporter = new ServicesReporter();
            reporter.Designer.ShouldBeSameAs(ServicesDrift.Designer);

            ShouldMatchGolden("Findings/Services.txt", Run(context, outputTarget, reporter));
        }

        [Fact]
        public void The_UI_drift_findings_and_log_are_as_captured()
        {
            WriteUiCode();
            var (context, outputTarget) = Build(UiDrift.Designer.DesignerId, UiPackage());
            var reporter = new UiReporter();
            reporter.Designer.ShouldBeSameAs(UiDrift.Designer);

            ShouldMatchGolden("Findings/UI.txt", Run(context, outputTarget, reporter));
        }

        [Fact]
        public void The_UI_ValidateDriftRecipes_task_returns_the_JSON_captured()
        {
            WriteUiCode();
            var (solution, metadata) = TaskHost(UiDrift.Designer.DesignerId, UiPackage());
            var task = new UiTask(solution, metadata);

            task.TaskTypeId.ShouldBe("Intent.UI.Guardrails.Contracts.ValidateDriftRecipes");
            var results = new[]
            {
                task.Execute(JsonSerializer.Serialize(new { applicationId = Fakes.ApplicationId })),
                task.Execute(JsonSerializer.Serialize(new { applicationId = Fakes.ApplicationId, element = "Checkout" })),
                task.Execute(JsonSerializer.Serialize(new { applicationId = Fakes.ApplicationId, fact = "table.name" }))
            };

            ShouldMatchGolden("Tasks/ValidateDriftRecipes.UI.txt", string.Join("\n", results.Select(x => x.Replace(_root, "<root>").Replace(_root.Replace('\\', '/'), "<root>"))) + "\n");
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
        /// them: each designer's drift reporter registers with the run's drift checks (in reverse, as registration order isn't
        /// theirs), and the drift module's guidance templates that can run write their file, keyed by its path under the
        /// template's output (no two may write the same path).
        /// </summary>
        private static IReadOnlyDictionary<string, string> RenderGuidance(bool domain, bool services, bool ui)
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
            var reporters = new (bool, DriftReporterBase)[] { (ui, new UiReporter()), (services, new ServicesReporter()), (domain, new DomainReporter()) }
                .Where(x => x.Item1)
                .Select(x => x.Item2)
                .ToArray();
            DriftHub.Register(context, reporters);

            outputTarget.TemplateInstances.Returns(templates.Cast<ITemplate>().ToList());
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

        private static IApplication GuidanceContext()
        {
            var solution = Substitute.For<ISolutionConfig>();
            solution.SolutionRootLocation.Returns(Path.Combine(Path.GetPathRoot(Path.GetTempPath()), "Repos", "Acme", "intent") + Path.DirectorySeparatorChar);
            var application = Substitute.For<IApplicationConfig>();
            application.Name.Returns("Acme");
            var context = Substitute.For<IApplication>();
            context.ReturnsForAll<ITemplate>((ITemplate)null);
            context.GetSolutionConfig().Returns(solution);
            context.GetApplicationConfig().Returns(application);
            context.EventDispatcher.Returns(DriftHub.Dispatcher());
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

        /// <summary>
        /// A UI package with one secured page whose route the code has changed, beside a page the model doesn't have: the
        /// UI recipe file, and the pages' Razor files.
        /// </summary>
        private static Intent.Metadata.Models.IPackage UiPackage()
        {
            var package = Package();
            var checkout = Element(package, "Component", "Checkout", Stereotype("Page", ("Route", "/checkout")), Stereotype("Secured", ("Roles", ""), ("Policy", "")));
            checkout.SpecializationTypeId.Returns("b1c481e1-e91e-4c29-9817-00ab9cad4b6b");
            return package;
        }

        private void WriteUiCode()
        {
            Directory.CreateDirectory(Path.Combine(_root, "src", "Web", "Pages"));
            File.WriteAllText(Path.Combine(_root, "guardrails", "ui.drift.json"), UiRecipe);
            File.WriteAllText(Path.Combine(_root, "src", "Web", "Pages", "Checkout.razor"), "@page \"/basket/checkout\"\n@attribute [Authorize]\n<h1>Checkout</h1>\n");
            File.WriteAllText(Path.Combine(_root, "src", "Web", "Pages", "Promotions.razor"), "@page \"/promotions\"\n<h1>Promotions</h1>\n");
        }

        private (IApplication Context, IOutputTarget OutputTarget) Build(string designerId, Intent.Metadata.Models.IPackage package)
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

            var context = Substitute.For<IApplication>();
            context.ReturnsForAll<ITemplate>((ITemplate)null);
            context.GetApplicationConfig().Returns(config);
            context.GetSolutionConfig().Returns(solution);
            context.MetadataManager.Returns(metadata);
            context.OutputRootDirectory.Returns(_root);

            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.ExecutionContext.Returns(context);
            outputTarget.Name.Returns("Acme.Domain");
            outputTarget.Metadata.Returns(new Dictionary<string, object>());
            outputTarget.GetTargetPath().Returns(new[] { outputTarget });
            outputTarget.Location.Returns(Path.Combine(_root, "src", "Api"));
            return (context, outputTarget);
        }

        /// <summary>
        /// The designer's drift check, as its reporter runs it once every template has been built: each Area it adds, with its
        /// declared categories, findings and notes (<see cref="DriftFindings"/>), then everything the run logged.
        /// </summary>
        private string Run(IApplication context, IOutputTarget outputTarget, DesignerDriftReporter reporter, params ITemplate[] contracts)
        {
            outputTarget.TemplateInstances.Returns(contracts.ToList());
            context.OutputTargets.Returns(new[] { outputTarget });
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: true);

            var drift = new RecordingDriftContext(context);
            DriftCheckRunner.Report(drift, reporter.Designer);
            var lines = drift.Areas.SelectMany(x => x.Snapshot(_root).Prepend($"area: {x.Name} ({string.Join(", ", x.Checks.OrderBy(c => c))})"));
            return string.Concat(lines.Select(x => x.Replace(_root, "<root>").Replace(_root.Replace('\\', '/'), "<root>") + "\n")) + "log:\n" + Log();
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
