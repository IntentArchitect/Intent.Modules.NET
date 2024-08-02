using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FluentValidationDependencyInjectionExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.FluentValidation.FluentValidationDependencyInjectionExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.DependencyInjection);
            if (template == null)
            {
                return;
            }
            template.AddNugetDependency(NugetPackages.FluentValidationDependencyInjectionExtensions(template.OutputTarget));
            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("FluentValidation");
                var @class = file.Classes.First();
                @class.FindMethod("AddApplication").InsertStatement(0, "services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);");
            });
        }
    }
}