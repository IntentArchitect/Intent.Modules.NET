using System;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Module.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TemplateSuppressorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModularMonolith.Module.TemplateSuppressorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            //ASP Net Core
            DisableTemplate(application, IAppStartupTemplate.RoleName);
            DisableTemplate(application, "Intent.AspNetCore.Program");
            //ASP Net Core Versioning
            DisableTemplate(application, "Intent.AspNetCore.Versioning.ApiVersionSwaggerGenOptions");
            //ASP Net Core Controllers
            DisableTemplate(application, "Intent.AspNetCore.Controllers.ExceptionFilter");            
            //Integration Subscriptions
            DisableMessageSubscriptions(application);

            //Security
            DisableTemplate(application, "Intent.Application.Identity.ApplicationSecurityConfiguration");
            DisableTemplate(application, "Intent.Application.Identity.CurrentUserService");
            
        }

        private void DisableMessageSubscriptions(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.IntegrationCommand");
            DisbaleSubscribedOnes(templates);
            templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.IntegrationEventMessage");
            DisbaleSubscribedOnes(templates);
            templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.IntegrationEventDto");
            DisbaleSubscribedOnes(templates);
            templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.IntegrationEventEnum");
            DisbaleSubscribedOnes(templates);
        }

        private static void DisbaleSubscribedOnes(System.Collections.Generic.IEnumerable<ICSharpFileBuilderTemplate> templates)
        {
            foreach (var template in templates)
            {
                if (!template.Namespace.StartsWith(template.OutputTarget.ApplicationName()))
                {
                    template.CanRun = false;
                }
            }
        }

        private void DisableTemplate(IApplication application, string templateId)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(templateId);

            if (template is not null)
            {
                template.CanRun = false;
            }

        }
    }
}