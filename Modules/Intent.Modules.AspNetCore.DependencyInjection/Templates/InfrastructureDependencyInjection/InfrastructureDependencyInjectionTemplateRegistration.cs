using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.AspNetCore.DependencyInjection.Templates.InfrastructureDependencyInjection
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class InfrastructureDependencyInjectionTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => InfrastructureDependencyInjectionTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new InfrastructureDependencyInjectionTemplate(outputTarget, null);
        }
    }
}