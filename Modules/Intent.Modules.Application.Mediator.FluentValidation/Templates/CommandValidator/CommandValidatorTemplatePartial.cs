using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.FluentValidation.Settings;
using Intent.Modules.Application.Mediator.Templates.CommandModels;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Modules.FluentValidation.Shared;
using Intent.Modules.FluentValidation.Shared.Templates.DtoValidator;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.FluentValidation.Templates.CommandValidator
{
    [IntentManaged(Mode.Ignore)]
    public class CommandValidatorTemplate : DtoValidatorTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Mediator.FluentValidation.CommandValidator";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommandValidatorTemplate(IOutputTarget outputTarget, CommandModel model)
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
                sourceElementAdvancedMappings: null,
                additionalFolders: outputTarget.ExecutionContext.ConsolidateIntoSingleFile()
                    ? []
                    : new[] { model.GetConceptName() })
        {
            FulfillsRole(TemplateRoles.Application.Validation.Command);
            FulfillsRole("Application.Validation");
        }

        // With the setting on, CombineWithModelsExtension builds the validator into the command's own file.
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.ConsolidateIntoSingleFile();
        }

        public static void Configure(CSharpTemplateBase<CommandModel> commandTemplate)
        {
            commandTemplate.AddNugetDependency(NugetPackages.FluentValidation(commandTemplate.OutputTarget));
            commandTemplate.ConfigureForValidation(
                dtoModel: new DTOModel(commandTemplate.Model.InternalElement),
                toValidateTemplateId: CommandModelsTemplate.TemplateId,
                modelParameterName: "command",
                dtoTemplateId: TemplateRoles.Application.Contracts.Dto,
                dtoValidatorTemplateId: TemplateRoles.Application.Validation.Dto,
                validatorProviderInterfaceTemplateId: "Application.Common.ValidatorProviderInterface",
                uniqueConstraintValidationEnabled: commandTemplate.ExecutionContext.Settings.GetFluentValidationApplicationLayer().UniqueConstraintValidation().IsDefaultEnabled(),
                repositoryInjectionEnabled: true,
                customValidationEnabled: true,
                associationedElements: null);
        }
    }
}
