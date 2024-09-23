using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.ModelDefinition;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.FluentValidation.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.FluentValidation.Templates.ModelDefinitionValidator
{
    [IntentManaged(Mode.Merge, Signature = Mode.Ignore)]
    public partial class ModelDefinitionValidatorTemplate : CSharpTemplateBase<ModelDefinitionModel>, IFluentValidationTemplate
    {
        public const string TemplateId = "Intent.Blazor.FluentValidation.ModelDefinitionValidator";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModelDefinitionValidatorTemplate(IOutputTarget outputTarget, ModelDefinitionModel model) : base(TemplateId, outputTarget, model)
        {
            var letsPretendYouAreADto = new DTOModel(model.InternalElement, model.InternalElement.SpecializationType);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath());
            this.ConfigureForValidation(
                dtoModel: model.InternalElement,
                configureClassValidations: [],
                configureFieldValidations: [
                    (methodChain, field) => methodChain.AddCustomValidations(this, field),
                    (methodChain, field) => methodChain.AddMaxLengthValidatorsFromMappedDomain(model.InternalElement, field)
                ]);
        }

        public string ToValidateTemplateId => ModelDefinitionTemplate.TemplateId;
        public string DtoTemplateId => TemplateRoles.Blazor.HttpClient.Contracts.Dto;
        public string ValidatorProviderTemplateId => "Blazor.Client.Validation.ValidatorProviderInterface";

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}