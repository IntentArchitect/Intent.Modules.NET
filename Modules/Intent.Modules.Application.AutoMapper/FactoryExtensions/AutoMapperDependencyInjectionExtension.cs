using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.AutoMapper.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AutoMapperDependencyInjectionExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.AutoMapper.AutoMapperDependencyInjectionExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.DependencyInjection);
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var method = file.Classes.First().FindMethod("AddApplication");
                if (method is null)
                {
                    return;
                }

                // It would be important to remove the existing AutoMapper package if it exists so that the correct version is installed.
                template.ExecutionContext.EventDispatcher.Publish(new RemoveNugetPackageEvent(NugetPackages.AutoMapperPackageName, template.OutputTarget));
                template.AddNugetDependency(NugetPackages.AutoMapper(template.OutputTarget));

                template.ExecutionContext.EventDispatcher.Publish(new RemoveNugetPackageEvent("AutoMapper.Extensions.Microsoft.DependencyInjection", template.OutputTarget));

                template.AddUsing("AutoMapper");

                method.AddInvocationStatement("services.AddAutoMapper", stmt =>
                {
                    if (!application.Settings.GetAutoMapperSettings().UsePreCommercialVersion())
                    {
                        (template as IntentTemplateBase)?.ApplyAppSetting("AutoMapper:LicenseKey", "<License key here>");

                        stmt.AddArgument(new CSharpLambdaBlock("cfg").AddStatement(@"cfg.LicenseKey = configuration[""AutoMapper:LicenseKey""] ?? string.Empty;"));
                    }
                    stmt.AddArgument("Assembly.GetExecutingAssembly()");
                });
            });
        }
    }
}