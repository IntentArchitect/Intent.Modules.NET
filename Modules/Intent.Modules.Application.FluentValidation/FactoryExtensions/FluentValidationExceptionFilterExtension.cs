using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class FluentValidationExceptionFilterExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Application.FluentValidation.FluentValidationExceptionFilterExtension";

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

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
            file.AddUsing("FluentValidation");
            var priClass = file.Classes.First();
            var switchStatement = (CSharpSwitchStatement)priClass.FindMethod("OnException")
                .FindStatement(p => p.HasMetadata("exception-switch"));
            if (switchStatement == null)
            {
                return;
            }

            switchStatement.AddCase("ValidationException" + " ex", block => block
                .AddForEachStatement("error", "ex.Errors", stmt => stmt
                    .AddStatement("context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);"))
                .AddInvocationStatement("context.Result = BuildResult", inv => inv
                    .AddArgument("context")
                    .AddArgument("problemDetails => new BadRequestObjectResult(problemDetails)")
                    .AddArgument("new ValidationProblemDetails(context.ModelState)")
                    .WithArgumentsOnNewLines())
                .WithBreak());
        });
    }
}