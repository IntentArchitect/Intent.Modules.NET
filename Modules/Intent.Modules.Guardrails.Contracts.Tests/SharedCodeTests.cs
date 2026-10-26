using System;
using System.IO;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The drift engine is copied into both Guardrails modules rather than shared through a third one. These tests
    /// keep the copies in step: they may differ only in their namespace.
    /// </summary>
    public class SharedCodeTests
    {
        [Theory]
        [InlineData("Drift/CodeIndex.cs")]
        [InlineData("Drift/FactKinds.cs")]
        [InlineData("Drift/RecipeEngine.cs")]
        [InlineData("Drift/StructureComparer.cs")]
        [InlineData("Drift/DriftReport.cs")]
        [InlineData("Drift/RecipeCheck.cs")]
        [InlineData("Templates/ModelSyncGuidance.cs")]
        public void The_copies_in_both_modules_are_identical_apart_from_the_namespace(string relativePath)
        {
            var domain = File.ReadAllText(Path.Combine(ModulesFolder(), "Intent.Modules.Domain.Guardrails.Contracts", relativePath));
            var services = File.ReadAllText(Path.Combine(ModulesFolder(), "Intent.Modules.Services.Guardrails.Contracts", relativePath));

            services.Replace("Intent.Modules.Services.Guardrails.Contracts", "Intent.Modules.Domain.Guardrails.Contracts").ShouldBe(domain);
        }

        private static string ModulesFolder()
        {
            for (var folder = new DirectoryInfo(AppContext.BaseDirectory); folder != null; folder = folder.Parent)
            {
                if (Directory.Exists(Path.Combine(folder.FullName, "Intent.Modules.Domain.Guardrails.Contracts")))
                {
                    return folder.FullName;
                }
            }

            throw new InvalidOperationException("The Modules folder wasn't found above the test output.");
        }
    }
}
