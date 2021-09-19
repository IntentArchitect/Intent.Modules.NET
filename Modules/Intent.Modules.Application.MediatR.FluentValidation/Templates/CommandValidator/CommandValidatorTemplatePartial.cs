using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.MediatR.FluentValidation.Api;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class CommandValidatorTemplate : CSharpTemplateBase<CommandModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.FluentValidation.CommandValidator";

        public CommandValidatorTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.FluentValidation);
        }

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
                if (property.HasStringValidation() && property.GetStringValidation().HasCustomValidation())
                {
                    yield return $@"
        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        private bool Validate{property.Name}({GetCommandModel()} command, {GetTypeName(property)} value)
        {{
            throw new NotImplementedException(""Your custom validation rules here..."");
        }}";
                }
            }
        }
    }
}