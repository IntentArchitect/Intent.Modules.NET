using System.Collections.Generic;
using System.Linq;
using Intent.Application.FluentValidation.Api;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.FluentValidation.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class CommandValidatorTemplate : CSharpTemplateBase<CommandModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.FluentValidation.CommandValidator";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommandValidatorTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.FluentValidation);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Validator",
                @namespace: $"{this.GetNamespace(additionalFolders: Model.GetConceptName())}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.GetConceptName())}");
        }

        public bool HasValidations()
        {
            return this.GetValidationRules(Model.Properties).Any();
        }

        private string GetCommandModel()
        {
            return GetTypeName(CommandModelsTemplate.TemplateId, Model);
        }

        private IEnumerable<string> GetCustomValidationMethods()
        {
            foreach (var property in Model.Properties)
            {
                if (property.HasValidations() && property.GetValidations().HasCustomValidation())
                {
                    AddUsing("System.Threading.Tasks");
                    yield return $@"
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private async Task<bool> Validate{property.Name}Async({GetCommandModel()} command, {GetTypeName(property)} value, CancellationToken cancellationToken)
        {{
            throw new NotImplementedException(""Your custom validation rules here..."");
        }}";
                }
            }
        }
    }
}