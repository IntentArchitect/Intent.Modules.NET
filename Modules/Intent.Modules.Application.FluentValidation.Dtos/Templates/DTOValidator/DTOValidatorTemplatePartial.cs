using System.Collections.Generic;
using Intent.Application.FluentValidation.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Dtos.Templates.DTOValidator
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class DTOValidatorTemplate : CSharpTemplateBase<DTOModel>
    {
        public const string TemplateId = "Intent.Application.FluentValidation.Dtos.DTOValidator";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DTOValidatorTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Validator",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetDtoModel()
        {
            return GetTypeName(DtoModelTemplate.TemplateId, Model);
        }

        private IEnumerable<string> GetCustomValidationMethods()
        {
            foreach (var property in Model.Fields)
            {
                if (property.HasValidations() && property.GetValidations().HasCustomValidation())
                {
                    yield return $@"
        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        private bool Validate{property.Name.ToPascalCase()}({GetDtoModel()} model, {GetTypeName(property)} value)
        {{
            throw new NotImplementedException(""Your custom validation rules here..."");
        }}";
                }
            }
        }
    }
}