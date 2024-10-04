using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.FluentValidation.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FluentValidationBlazorInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.FluentValidation.FluentValidationBlazorInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;


        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Blazor.Client.DependencyInjection);
            if (template is null)
            {
                return;
            }

            template.AddNugetDependency(NugetPackages.FluentValidationDependencyInjectionExtensions(template.OutputTarget));
            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var mainMethod = @class.FindMethod(x => x.ReturnType == "IServiceCollection");
                if (mainMethod == null)
                    return;
                file.AddUsing("FluentValidation");
                mainMethod.AddStatement("services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);");
            });
        }
    }
}