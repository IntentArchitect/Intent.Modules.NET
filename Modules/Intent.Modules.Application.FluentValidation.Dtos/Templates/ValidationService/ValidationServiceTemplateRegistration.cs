using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.FluentValidation.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Constants;
using Intent.Modules.FluentValidation.Shared;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Dtos.Templates.ValidationService
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ValidationServiceTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => ValidationServiceTemplate.TemplateId;

        public override void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (!application.MetadataManager.Services(application).GetDTOModels()
                    .Any(x => !x.HasMapFromDomainMapping() && ValidationRulesExtensions.HasValidationRules(
                        dtoModel: x,
                        dtoTemplateId: TemplateRoles.Application.Contracts.Dto,
                        dtoValidatorTemplateId: TemplateRoles.Application.Validation.Dto,
                        uniqueConstraintValidationEnabled: application.Settings.GetFluentValidationApplicationLayer().UniqueConstraintValidation().IsDefaultEnabled(),
                        customValidationEnabled: true,
                        associationedElements: x.InternalElement?.AssociatedElements)))
            {
                AbortRegistration(); // Need cleaner, more obvious way, to do this
                return;
            }
            base.DoRegistration(registry, application);
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new ValidationServiceTemplate(outputTarget);
        }
    }
}