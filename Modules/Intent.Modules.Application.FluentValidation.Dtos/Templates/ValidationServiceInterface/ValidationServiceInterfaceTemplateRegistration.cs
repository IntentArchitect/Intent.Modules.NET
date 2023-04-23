using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.FluentValidation.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Dtos.Templates.ValidationServiceInterface
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ValidationServiceInterfaceTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => ValidationServiceInterfaceTemplate.TemplateId;


        public override void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (!application.MetadataManager.Services(application).GetDTOModels()
                    .Any(x => !x.HasMapFromDomainMapping() && ValidationRulesExtensions.GetValidationRules(x.Fields).Any()))
            {
                AbortRegistration(); // Need cleaner, more obvious way, to do this
                return;
            }
            base.DoRegistration(registry, application);
        }

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new ValidationServiceInterfaceTemplate(outputTarget);
        }
    }
}