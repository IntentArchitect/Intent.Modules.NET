using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Exceptions.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Exceptions.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class ExceptionFilterExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Application.Exceptions.ExceptionFilterExtension";

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
            var priClass = file.Classes.First();
            var switchStatement = (CSharpSwitchStatement)priClass.FindMethod("OnException")
                .FindStatement(p => p.HasMetadata("exception-switch"));
            if (switchStatement == null)
            {
                return;
            }

            switchStatement.AddCase(template.GetNotFoundExceptionName() + " ex", block => block
                .AddInvocationStatement("context.Result = BuildResult", inv => inv
                    .AddArgument("context")
                    .AddArgument("problemDetails => new NotFoundObjectResult(problemDetails)")
                    .AddArgument(new CSharpObjectInitializerBlock("new ProblemDetails")
                        .AddInitStatement("Detail", "ex.Message"))
                    .WithArgumentsOnNewLines())
                .WithBreak());
        });
    }
}