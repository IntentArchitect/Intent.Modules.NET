using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.MultiTenancy.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenantStoreDbContext
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class MultiTenantStoreDbContextTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => MultiTenantStoreDbContextTemplate.TemplateId;

        [IntentManaged(Mode.Ignore)]
        public override void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (!application.Settings.GetMultitenancySettings().Store().IsEfcore())
            {
                AbortRegistration();
            }
            base.DoRegistration(registry, application);
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new MultiTenantStoreDbContextTemplate(outputTarget);
        }
    }
}