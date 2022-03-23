using System.Collections.Generic;
using System.Linq;
using Intent.Application.MediatR.FluentValidation.Api;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.QueryValidator
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class QueryValidatorTemplate : CSharpTemplateBase<QueryModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.FluentValidation.QueryValidator";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public QueryValidatorTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
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

        private string GetQueryModel()
        {
            return GetTypeName(QueryModelsTemplate.TemplateId, Model);
        }

        private IEnumerable<string> GetCustomValidationMethods()
        {
            foreach (var property in Model.Properties)
            {
                if (property.HasValidations() && property.GetValidations().HasCustomValidation())
                {
                    yield return $@"
        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        private bool Validate{property.Name}({GetQueryModel()} command, {GetTypeName(property)} value)
        {{
            throw new NotImplementedException(""Your custom validation rules here..."");
        }}";
                }
            }
        }
    }
}