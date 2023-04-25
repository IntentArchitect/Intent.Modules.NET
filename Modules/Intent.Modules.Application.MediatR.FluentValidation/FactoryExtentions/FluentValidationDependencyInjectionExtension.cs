using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.MediatR.FluentValidation.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FluentValidationDependencyInjectionExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.FluentValidation.FluentValidationDependencyInjectionExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallStartupControllerFilter(application);
        }
        
        private static void InstallStartupControllerFilter(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.AspNetCore.Startup"));

            template?.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                if (priClass.FindMethod("ConfigureServices")
                        .FindStatement(s => s.HasMetadata("configure-services-controllers-generic")) is not CSharpInvocationStatement method)
                {
                    return;
                }

                CSharpLambdaBlock lambda;
                if (!method.Statements.Any())
                {
                    lambda = new CSharpLambdaBlock("opt");
                    method.AddArgument(lambda);
                }
                else
                {
                    lambda = method.Statements.First() as CSharpLambdaBlock;
                }

                lambda.AddStatement($"opt.Filters.Add<{template.GetFluentValidationFilterName()}>();");
            }, 10);
        }
    }
}