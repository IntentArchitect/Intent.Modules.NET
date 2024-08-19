using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Application.FluentValidation.Settings;
using Intent.Modules.Constants;
using Intent.Modules.FluentValidation.Shared.Templates.DtoValidator;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Dtos.Templates.DTOValidator
{
    [IntentManaged(Mode.Ignore)]
    public class DTOValidatorTemplate : DtoValidatorTemplateBase
    {
        public const string TemplateId = "Intent.Application.FluentValidation.Dtos.DTOValidator";

        public DTOValidatorTemplate(IOutputTarget outputTarget, DTOModel model, IEnumerable<IAssociationEnd> associationedElements)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: model,
                toValidateTemplateId: DtoModelTemplate.TemplateId,
                dtoTemplateId: TemplateRoles.Application.Contracts.Dto,
                dtoValidatorTemplateId: TemplateRoles.Application.Validation.Dto,
                modelParameterName: "model",
                validatorProviderInterfaceTemplateId: "Application.Common.ValidatorProviderInterface",
                uniqueConstraintValidationEnabled: outputTarget.ExecutionContext.Settings.GetFluentValidationApplicationLayer().UniqueConstraintValidation().IsDefaultEnabled(),
                repositoryInjectionEnabled: true,
                associationedElements: associationedElements,
                customValidationEnabled: true)
        {
        }
    }
}