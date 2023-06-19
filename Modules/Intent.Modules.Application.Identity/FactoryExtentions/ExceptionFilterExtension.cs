using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates.ForbiddenAccessException;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Identity.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ExceptionFilterExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Identity.ExceptionFilterExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallExceptionFilter(application);
        }

        private static void InstallExceptionFilter(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate("Distribution.ExceptionFilter"));

            template?.CSharpFile.OnBuild(file =>
            {
                var priClass = file.Classes.First();
                var switchStatement = (CSharpSwitchStatement)priClass.FindMethod("OnException")
                    .FindStatement(p => p.HasMetadata("exception-switch"));
                if (switchStatement == null)
                {
                    return;
                }

                switchStatement.AddCase(template.GetTypeName(ForbiddenAccessExceptionTemplate.TemplateId), block => block
                    .AddStatement("context.Result = new ForbidResult();")
                    .WithBreak());
            });
        }
    }
}