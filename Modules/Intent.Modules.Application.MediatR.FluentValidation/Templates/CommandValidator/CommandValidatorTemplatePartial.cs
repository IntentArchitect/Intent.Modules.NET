using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.FluentValidation.Settings;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Modules.FluentValidation.Shared;
using Intent.Modules.FluentValidation.Shared.Templates.DtoValidator;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator
{
    [IntentManaged(Mode.Ignore)]
    public class CommandValidatorTemplate : DtoValidatorTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.FluentValidation.CommandValidator";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommandValidatorTemplate(IOutputTarget outputTarget, CommandModel model, IEnumerable<IAssociationEnd> associationedElements)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: new DTOModel(model.InternalElement),
                toValidateTemplateId: CommandModelsTemplate.TemplateId,
                dtoTemplateId: TemplateRoles.Application.Contracts.Dto,
                dtoValidatorTemplateId: TemplateRoles.Application.Validation.Dto,
                modelParameterName: "command",
                validatorProviderInterfaceTemplateId: "Application.Common.ValidatorProviderInterface",
                uniqueConstraintValidationEnabled: outputTarget.ExecutionContext.Settings.GetFluentValidationApplicationLayer().UniqueConstraintValidation().IsDefaultEnabled(),
                repositoryInjectionEnabled: true,
                customValidationEnabled: true,
                associationedElements: associationedElements,
                additionalFolders: outputTarget.ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
                    ? Array.Empty<string>()
                    : new[] { model.GetConceptName() })
        {
            FulfillsRole(TemplateRoles.Application.Validation.Command);
        }

        public static void Configure(CSharpTemplateBase<CommandModel> template, IEnumerable<IAssociationEnd> associationedElements)
        {
            template.ConfigureForValidation(
                dtoModel: new DTOModel(template.Model.InternalElement),
                toValidateTemplateId: CommandModelsTemplate.TemplateId,
                modelParameterName: "command",
                dtoTemplateId: TemplateRoles.Application.Contracts.Dto,
                dtoValidatorTemplateId: TemplateRoles.Application.Validation.Dto,
                validatorProviderInterfaceTemplateId: "Application.Common.ValidatorProviderInterface",
                uniqueConstraintValidationEnabled: template.ExecutionContext.Settings.GetFluentValidationApplicationLayer().UniqueConstraintValidation().IsDefaultEnabled(),
                repositoryInjectionEnabled: true,
                customValidationEnabled: true,
                associationedElements: associationedElements);
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile();
        }
    }
}