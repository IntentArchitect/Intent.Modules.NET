using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ExceptionFilterExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.ExceptionFilterExtension";

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
                        .FindStatement(s => s.HasMetadata("configure-services-controllers-generic")) is not CSharpInvocationStatement controllersStatement)
                {
                    return;
                }

                CSharpLambdaBlock lambda;
                if (!controllersStatement.Statements.Any())
                {
                    lambda = new CSharpLambdaBlock("opt");
                    controllersStatement.AddArgument(lambda);
                }
                else
                {
                    lambda = controllersStatement.Statements.First() as CSharpLambdaBlock;
                }

                lambda.AddStatement($"opt.Filters.Add<{template.GetExceptionFilterName()}>();");
            }, 10);
        }
    }
}