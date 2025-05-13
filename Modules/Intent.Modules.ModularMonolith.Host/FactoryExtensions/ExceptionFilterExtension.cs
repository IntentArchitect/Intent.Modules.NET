using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.ModularMonolith.Host;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Host.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ExceptionFilterExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModularMonolith.Host.ExceptionFilterExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;


        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            var filterTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.Controllers.ExceptionFilter");
            filterTemplate?.CSharpFile.OnBuild(file =>
            {
                filterTemplate.AddNugetDependency(NugetPackages.FluentValidation(filterTemplate.OutputTarget));
                InstalValidationExceptionIfMissing(file);
            }, 1000);
        }

        private void InstalValidationExceptionIfMissing(CSharpFile file)
        {
            var priClass = file.Classes.First(p => p.Name == "ExceptionFilter");
            var switchStatement = GetExceptionSwitchStatement(priClass);

            file.AddUsing("FluentValidation");
            if (switchStatement.FindStatement(s => s.GetText("").StartsWith("case ValidationException")) != null)
            {
                return;
            }
            switchStatement.AddCase("ValidationException exception", block => block
                .AddForEachStatement("error", "exception.Errors", stmt => stmt
                    .AddStatement("context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);").SeparatedFromNext())
                .AddInvocationStatement("context.Result = new BadRequestObjectResult", invoke => invoke
                    .AddArgument("new ValidationProblemDetails(context.ModelState)")
                    .WithoutSemicolon())
                .AddInvocationStatement(".AddContextInformation", stmt => stmt.AddArgument("context"))
                .AddStatement("context.ExceptionHandled = true;")
                .WithBreak());
        }

        private static CSharpSwitchStatement GetExceptionSwitchStatement(CSharpClass priClass)
        {
            return (CSharpSwitchStatement)priClass
                .FindMethod("OnException")
                .FindStatement(p => p.HasMetadata("exception-switch"));
        }

    }
}