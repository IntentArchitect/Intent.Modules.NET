using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;

namespace Intent.Modules.Common.Drift
{
    /// <summary>
    /// One designer's ValidateDriftRecipes module task, for the task of the module that contributes the designer.
    /// Validates the designer's recipe file against the code, on demand: what each recipe finds and reads, and the recipe
    /// probe's warnings, as JSON (<see cref="RecipeValidation"/>), or <c>{ "errorMessage": "…" }</c>. Its one argument is
    /// <c>{ "applicationId": "…" }</c>, optionally with <c>"fact"</c>, <c>"element"</c> and <c>"file"</c> to look at one kind
    /// of fact, element or code file and list every reading there (<see cref="RecipeFilter"/>). It reads only the recipe
    /// file, the model and the code, nothing from a Software Factory run. The model is read through the metadata manager,
    /// which the reused module-task host may serve a few seconds stale after an earlier call; the recipe file and the code
    /// are read fresh.
    /// </summary>
    public static class RecipeValidationTask
    {
        private static readonly JsonSerializerOptions ArgumentOptions = new() { PropertyNameCaseInsensitive = true };

        public static string Execute(DriftDesigner designer, ISolutionConfig solution, IMetadataManager metadataManager, params string[] args)
        {
            try
            {
                var arguments = args.Length == 0 ? null : JsonSerializer.Deserialize<TaskArguments>(args[0], ArgumentOptions);
                var applicationId = arguments?.ApplicationId;
                if (string.IsNullOrWhiteSpace(applicationId))
                {
                    return Error("Expected one argument: { \"applicationId\": \"<the application's id>\" }.");
                }

                var filter = RecipeFilter.Of(arguments.Fact, arguments.Element, arguments.File);
                if (filter?.Fact != null && designer.Kinds.All(x => x.Key != filter.Fact))
                {
                    return Error($"No {designer.Name} fact is called {filter.Fact}. They are: {string.Join(", ", designer.Kinds.Select(x => x.Key))}.");
                }

                var config = solution.GetApplicationConfig(applicationId);
                if (config == null)
                {
                    return Error($"No application has the id {applicationId}.");
                }

                var recipePath = Path.Combine(config.DirectoryPath, "guardrails", $"{designer.Name.ToLowerInvariant()}.drift.json");
                var model = metadataManager.GetDesigner(applicationId, designer.DesignerId);
                return RecipeValidation.Run(new RecipeValidationInput
                {
                    Designer = designer.Name,
                    Kinds = designer.Kinds,
                    RecipePath = $"guardrails/{Path.GetFileName(recipePath)}",
                    RecipeJson = File.Exists(recipePath) ? File.ReadAllText(recipePath) : null,
                    Code = new CodeFiles(Path.GetFullPath(Path.Combine(config.DirectoryPath, config.OutputLocation))),
                    Facts = model == null ? Array.Empty<ModelFact>() : designer.ReadFacts(model, applicationId),
                    Filter = filter
                }).ToJson();
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException or JsonException)
            {
                return Error(e.Message);
            }
        }

        private static string Error(string message) => JsonSerializer.Serialize(new { errorMessage = message });

        private sealed class TaskArguments
        {
            public string ApplicationId { get; set; }
            public string Fact { get; set; }
            public string Element { get; set; }
            public string File { get; set; }
        }
    }
}
