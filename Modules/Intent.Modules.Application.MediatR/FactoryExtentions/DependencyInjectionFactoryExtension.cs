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

namespace Intent.Modules.Application.MediatR.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DependencyInjectionFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.DependencyInjectionFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            RegisterServices(application);
        }

        private static void RegisterServices(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Application.DependencyInjection);
            if (template == null)
            {
                return;
            }
            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Microsoft.Extensions.DependencyInjection");

                var method = file.Classes.First().FindMethod("AddApplication");
                method.AddInvocationStatement("services.AddMediatR", invocation =>
                {
                    invocation.AddArgument(new CSharpLambdaBlock("cfg"), a =>
                    {
                        var options = (CSharpLambdaBlock)a;
                        options.AddStatement("cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());");
                    });
                });
            });

        }
    }
}