using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Mapperly;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NugetInstallFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Dtos.Mapperly.NuGetInstallFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            /*We always install the nuget package when the module is installed so the AI driven implementation instead of DomainInteractions works*/
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.DependencyInjection);
            if (template is null)
            {
                return;
            }

            template.ExecutionContext.EventDispatcher.Publish(
                new RemoveNugetPackageEvent(NugetPackages.RiokMapperlyPackageName, template.OutputTarget));

            template.AddNugetDependency(NugetPackages.RiokMapperly(template.OutputTarget));
        }
    }
}