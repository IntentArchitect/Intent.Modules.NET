using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Output;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Module.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TemplateSuppressorExtension : FactoryExtensionBase
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;
        public override string Id => "Intent.ModularMonolith.Module.TemplateSuppressorExtension";

        public TemplateSuppressorExtension(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

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
            SortOutMessageSubscriptions(application);

            //Security
            DisableTemplate(application, "Intent.Application.Identity.ApplicationSecurityConfiguration");
            DisableTemplate(application, "Intent.Application.Identity.CurrentUserService");

        }

        private void SortOutMessageSubscriptions(IApplication application)
        {
            var externalAppIds = new HashSet<string>();

            var externalMessageTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.IntegrationCommand")
                .Concat(application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.IntegrationEventMessage"))
                .Where(x => IsSubscribed(x));

            foreach (var em in externalMessageTemplates)
            {

                em.CanRun = false;
                var element = ((ITemplateWithModel)em).Model;
                var appId = ((IElementWrapper)element).InternalElement.Package.ApplicationId;
                externalAppIds.Add(appId);
            }

            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.IntegrationEventDto");
            DisbaleSubscribedOnes(templates);
            templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.IntegrationEventEnum");
            DisbaleSubscribedOnes(templates);

            var applicationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.DependencyInjection.DependencyInjection");

            //This Logic is adding Project Dependencies to other Applications Contracts based on Subscriptions
            //It is assuming Project Structures are Relatively aligned with the application template
            foreach (var externalAppId in externalAppIds)
            {
                var appConfig = _configurationProvider.GetApplicationConfig(externalAppId);
                var currentProject = externalMessageTemplates.First().OutputTarget.GetProject();
                var thisModuleContactProject = Path.Combine(currentProject.RelativeLocation, $"{currentProject.Name}.csproj");
                var targetProject = @"..\..\" + thisModuleContactProject.Replace(application.Name, appConfig.Name).Replace(@"./Modules/", "");
                //Sanity Check if the project is not there don't add the reference
                if (System.IO.File.Exists(Path.Combine(currentProject.Location, targetProject)))
                {
                    ((CSharpTemplateBase<object>)applicationTemplate).AddAssemblyReference(new ProjectReference(targetProject));
                }
            }
        }

        private static bool IsSubscribed(ICSharpFileBuilderTemplate template)
        {
            return !template.Namespace.StartsWith(template.OutputTarget.ApplicationName());
        }

        private static void DisbaleSubscribedOnes(System.Collections.Generic.IEnumerable<ICSharpFileBuilderTemplate> templates)
        {
            foreach (var template in templates)
            {
                if (IsSubscribed(template))
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