using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Services.Guardrails.Contracts.Drift;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.ModuleTask", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Tasks
{
    /// <summary>
    /// Validates the Services drift recipe file against the code, on demand: what each recipe finds and reads, and the recipe
    /// probe's warnings, as JSON (<see cref="RecipeValidation"/>), or <c>{ "errorMessage": "…" }</c>. Its one argument is
    /// <c>{ "applicationId": "…" }</c>, optionally with <c>"fact"</c>, <c>"element"</c> and <c>"file"</c> to look at one kind
    /// of fact, element or code file and list every reading there (<see cref="RecipeFilter"/>). It reads only the recipe
    /// file, the model and the code, nothing from a Software
    /// Factory run. The model is read through the metadata manager, which the reused module-task host may serve a few
    /// seconds stale after an earlier call; the recipe file and the code are read fresh.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class ValidateDriftRecipes : IModuleTask
    {
        [IntentManaged(Mode.Ignore)]
        private const string DesignerName = "Services";

        [IntentManaged(Mode.Ignore)]
        private static readonly JsonSerializerOptions ArgumentOptions = new() { PropertyNameCaseInsensitive = true };

        [IntentManaged(Mode.Ignore)]
        private readonly ISolutionConfig _solution;

        [IntentManaged(Mode.Ignore)]
        private readonly IMetadataManager _metadataManager;

        [IntentManaged(Mode.Merge)]
        public ValidateDriftRecipes(ISolutionConfig solution, IMetadataManager metadataManager)
        {
            _solution = solution;
            _metadataManager = metadataManager;
        }

        public string TaskTypeId => "Intent.Services.Guardrails.Contracts.ValidateDriftRecipes";
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string TaskTypeName => "Validate drift recipes";
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public int Order => 0;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string Execute(params string[] args)
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
                if (filter?.Fact != null && FactKinds.For(DesignerName).All(x => x.Key != filter.Fact))
                {
                    return Error($"No {DesignerName} fact is called {filter.Fact}. They are: {string.Join(", ", FactKinds.For(DesignerName).Select(x => x.Key))}.");
                }

                var config = _solution.GetApplicationConfig(applicationId);
                if (config == null)
                {
                    return Error($"No application has the id {applicationId}.");
                }

                var recipePath = Path.Combine(config.DirectoryPath, "guardrails", $"{DesignerName.ToLowerInvariant()}.drift.json");
                var designer = _metadataManager.GetDesigner(applicationId, ApiMetadataDesignerExtensions.ServicesDesignerId);
                return RecipeValidation.Run(new RecipeValidationInput
                {
                    Designer = DesignerName,
                    RecipePath = $"guardrails/{Path.GetFileName(recipePath)}",
                    RecipeJson = File.Exists(recipePath) ? File.ReadAllText(recipePath) : null,
                    Code = new CodeFiles(Path.GetFullPath(Path.Combine(config.DirectoryPath, config.OutputLocation))),
                    Facts = designer == null ? Array.Empty<ModelFact>() : DriftModel.Read(designer, applicationId),
                    Filter = filter
                }).ToJson();
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException or JsonException)
            {
                return Error(e.Message);
            }
        }

        [IntentManaged(Mode.Ignore)]
        private static string Error(string message) => JsonSerializer.Serialize(new { errorMessage = message });

        [IntentManaged(Mode.Ignore)]
        private sealed class TaskArguments
        {
            public string ApplicationId { get; set; }
            public string Fact { get; set; }
            public string Element { get; set; }
            public string File { get; set; }
        }
    }
}
