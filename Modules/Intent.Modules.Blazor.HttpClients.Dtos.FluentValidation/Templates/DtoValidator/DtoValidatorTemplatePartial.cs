using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Blazor.HttpClients.Templates.DtoContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.FluentValidation.Shared;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.FluentValidation.Shared.SimpleValidationRulesExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Dtos.FluentValidation.Templates.DtoValidator
{
    [IntentManaged(Mode.Fully)]
    public partial class DtoValidatorTemplate : CSharpTemplateBase<DTOModel>, IFluentValidationTemplate
    {
        public const string TemplateId = "Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DtoValidatorTemplate(IOutputTarget outputTarget, DTOModel model)
            : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(@namespace: ExtensionMethods.GetPackageBasedNamespace(model, outputTarget),
                relativeLocation: ExtensionMethods.GetPackageBasedRelativeLocation(model, outputTarget));

            this.ConfigureForValidation(
                dtoModel: model,
                configureValidations: [
                    //(methodChain, field) => methodChain.AddCustomValidations(this, field, DtoContractTemplate.TemplateId),
                    (methodChain, field) => methodChain.AddValidatorsFromMappedDomain(Model, field)
                ]);
        }

        public string ToValidateTemplateId => DtoContractTemplate.TemplateId;
        public string DtoTemplateId => DtoContractTemplate.TemplateId;
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