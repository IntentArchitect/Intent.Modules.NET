using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Drift;
using Intent.Modules.Services.Guardrails.Contracts.Drift;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.ModuleTask", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Tasks
{
    /// <summary>
    /// Validates the Services drift recipe file against the code, on demand. The validation is the drift module's
    /// (<see cref="RecipeValidationTask"/>, which documents the argument and the JSON): this task supplies the Services
    /// designer (<see cref="ServicesDrift"/>).
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class ValidateDriftRecipes : IModuleTask
    {
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
            return RecipeValidationTask.Execute(ServicesDrift.Designer, _solution, _metadataManager, args);
        }
    }
}
