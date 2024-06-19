using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Blazor.HttpClients.Templates.DtoContract;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.FluentValidation.Shared.Templates.DtoValidator;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Dtos.FluentValidation.Templates.DtoValidator
{
    [IntentManaged(Mode.Ignore)]
    public class DtoValidatorTemplate : DtoValidatorTemplateBase
    {
        public const string TemplateId = "Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator";

        public DtoValidatorTemplate(IOutputTarget outputTarget, DTOModel model, IEnumerable<IAssociationEnd> associationedElements)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: model,
                toValidateTemplateId: DtoContractTemplate.TemplateId,
                dtoTemplateId: DtoContractTemplate.TemplateId,
                dtoValidatorTemplateId: TemplateId,
                modelParameterName: "model",
                @namespace: ExtensionMethods.GetPackageBasedNamespace(model, outputTarget),
                relativeLocation: ExtensionMethods.GetPackageBasedRelativeLocation(model, outputTarget),
                validatorProviderInterfaceTemplateId: "Blazor.Client.Validation.ValidatorProviderInterface",
                uniqueConstraintValidationEnabled: true,
                repositoryInjectionEnabled: false,
                customValidationEnabled: false,
                associationedElements: associationedElements)
        {
        }
    }
}