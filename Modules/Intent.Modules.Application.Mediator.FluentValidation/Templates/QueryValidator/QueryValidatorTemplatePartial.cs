using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.FluentValidation.Settings;
using Intent.Modules.Application.Mediator.Templates.QueryModels;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Modules.FluentValidation.Shared;
using Intent.Modules.FluentValidation.Shared.Templates.DtoValidator;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.FluentValidation.Templates.QueryValidator
{
    [IntentManaged(Mode.Ignore)]
    public class QueryValidatorTemplate : DtoValidatorTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Mediator.FluentValidation.QueryValidator";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public QueryValidatorTemplate(IOutputTarget outputTarget, QueryModel model)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: new DTOModel(model.InternalElement),
                toValidateTemplateId: QueryModelsTemplate.TemplateId,
                dtoTemplateId: TemplateRoles.Application.Contracts.Dto,
                dtoValidatorTemplateId: TemplateRoles.Application.Validation.Dto,
                modelParameterName: "query",
                validatorProviderInterfaceTemplateId: "Application.Common.ValidatorProviderInterface",
                uniqueConstraintValidationEnabled: outputTarget.ExecutionContext.Settings.GetFluentValidationApplicationLayer().UniqueConstraintValidation().IsDefaultEnabled(),
                repositoryInjectionEnabled: true,
                customValidationEnabled: true,
                sourceElementAdvancedMappings: null,
                additionalFolders: outputTarget.ExecutionContext.ConsolidateIntoSingleFile()
                    ? []
                    : new[] { model.GetConceptName() })
        {
            FulfillsRole(TemplateRoles.Application.Validation.Query);
            FulfillsRole("Application.Validation");
        }

        // With the setting on, CombineWithModelsExtension builds the validator into the query's own file.
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.ConsolidateIntoSingleFile();
        }

        public static void Configure(CSharpTemplateBase<QueryModel> queryTemplate)
        {
            queryTemplate.AddNugetDependency(NugetPackages.FluentValidation(queryTemplate.OutputTarget));
            queryTemplate.ConfigureForValidation(
                dtoModel: new DTOModel(queryTemplate.Model.InternalElement),
                toValidateTemplateId: QueryModelsTemplate.TemplateId,
                modelParameterName: "query",
                dtoTemplateId: TemplateRoles.Application.Contracts.Dto,
                dtoValidatorTemplateId: TemplateRoles.Application.Validation.Dto,
                validatorProviderInterfaceTemplateId: "Application.Common.ValidatorProviderInterface",
                uniqueConstraintValidationEnabled: queryTemplate.ExecutionContext.Settings.GetFluentValidationApplicationLayer().UniqueConstraintValidation().IsDefaultEnabled(),
                repositoryInjectionEnabled: true,
                customValidationEnabled: true,
                associationedElements: null);
        }
    }
}
