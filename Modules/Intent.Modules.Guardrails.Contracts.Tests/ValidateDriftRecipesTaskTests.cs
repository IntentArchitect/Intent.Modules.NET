using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Domain.Guardrails.Contracts.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Fakes;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The ValidateDriftRecipes module task: given an application's id, it validates that application's recipe file
    /// against its code and saved model, and returns the result as JSON, or an error message.
    /// </summary>
    public sealed class ValidateDriftRecipesTaskTests : IDisposable
    {
        private const string Recipe = """
        {
          "files": "src/**/*.cs",
          "facts": {
            "table.name": { "find": "statement:Entity<{Entity}>", "read": [{ "match": "ToTable\\(\"([^\"]+)\"" }] }
          },
          "own": [{ "kind": "Entity", "find": "class with \\bclass \\w+\\b", "files": "src/Domain/**/*.cs" }]
        }
        """;

        private readonly string _root = Path.Combine(Path.GetTempPath(), $"guardrails-validate-{Guid.NewGuid():N}");

        public ValidateDriftRecipesTaskTests()
        {
            Directory.CreateDirectory(Path.Combine(_root, "guardrails"));
            Directory.CreateDirectory(Path.Combine(_root, "src", "Domain"));
            File.WriteAllText(Path.Combine(_root, "guardrails", "domain.drift.json"), Recipe);
            File.WriteAllText(Path.Combine(_root, "src", "Domain", "Order.cs"), "public class Order { }\n");
            File.WriteAllText(Path.Combine(_root, "src", "Domain", "Helper.cs"), "public class Helper { }\n");
            File.WriteAllText(Path.Combine(_root, "src", "AppDbContext.cs"), "class AppDbContext\n{\n    void OnModelCreating(ModelBuilder modelBuilder)\n    {\n        modelBuilder.Entity<Order>(e => e.ToTable(\"Order\"));\n    }\n}\n");
        }

        public void Dispose()
        {
            Directory.Delete(_root, recursive: true);
        }

        [Fact]
        public void It_returns_what_each_recipe_finds_and_reads_as_JSON()
        {
            var result = JsonDocument.Parse(Task().Execute(JsonSerializer.Serialize(new { applicationId = Fakes.ApplicationId }))).RootElement;

            result.GetProperty("designer").GetString().ShouldBe("Domain");
            result.GetProperty("recipePath").GetString().ShouldBe("guardrails/domain.drift.json");
            var tableName = result.GetProperty("facts").EnumerateArray().Single(x => x.GetProperty("kind").GetString() == "table.name");
            tableName.GetProperty("readFromCode").GetProperty("count").GetInt32().ShouldBe(1);
            var contradicted = tableName.GetProperty("contradicted").GetProperty("samples")[0];
            (contradicted.GetProperty("model").GetString(), contradicted.GetProperty("code").GetString(), contradicted.GetProperty("location").GetString())
                .ShouldBe(("Orders", "Order", "src/AppDbContext.cs:5"));

            // What the convention finds, whether or not the model describes it: tuning its find is about what it matches.
            var entities = result.GetProperty("conventions")[0];
            entities.GetProperty("found").GetInt32().ShouldBe(2);
            entities.GetProperty("samples").EnumerateArray().Select(x => x.GetString()).ShouldBe(new[] { "Helper (src/Domain/Helper.cs:1)", "Order (src/Domain/Order.cs:1)" });
        }

        [Fact]
        public void It_returns_an_error_message_for_a_missing_or_unknown_application()
        {
            JsonDocument.Parse(Task().Execute()).RootElement.GetProperty("errorMessage").GetString().ShouldStartWith("Expected one argument");
            JsonDocument.Parse(Task().Execute("""{ "applicationId": "nope" }""")).RootElement.GetProperty("errorMessage").GetString().ShouldBe("No application has the id nope.");
        }

        private ValidateDriftRecipes Task()
        {
            var package = Package();
            Element(package, "Class", "Order", Stereotype("Table", ("Name", "Orders")));
            var designer = Designer(new[] { package });
            var metadata = Substitute.For<IMetadataManager>();
            metadata.GetDesigner(Fakes.ApplicationId, ApiMetadataDesignerExtensions.DomainDesignerId).Returns(designer);

            var config = Substitute.For<IApplicationConfig>();
            config.Id.Returns(Fakes.ApplicationId);
            config.DirectoryPath.Returns(_root);
            config.OutputLocation.Returns(".");
            var solution = Substitute.For<ISolutionConfig>();
            solution.GetApplicationConfig(Arg.Any<string>()).Returns((IApplicationConfig)null);
            solution.GetApplicationConfig(Fakes.ApplicationId).Returns(config);

            return new ValidateDriftRecipes(solution, metadata);
        }
    }
}
