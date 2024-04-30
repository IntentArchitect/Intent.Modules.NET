using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.ExceptionFilter;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class ExceptionFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.AspNetCore.Controllers.ExceptionFilter";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public ExceptionFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddTypeSource("Application.ForbiddenAccessException");
        AddTypeSource("Domain.NotFoundException");

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Diagnostics")
            .AddUsing("Microsoft.AspNetCore.Mvc")
            .AddUsing("Microsoft.AspNetCore.Mvc.Filters")
            .AddClass($"ExceptionFilter", @class =>
            {
                @class.ImplementsInterface("Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter");
                @class.AddMethod("void", "OnException", method =>
                {
                    method.AddParameter("ExceptionContext", "context");
                    method.AddSwitchStatement("context.Exception",
                        stmt => stmt.AddMetadata("exception-switch", true));
                });
            })
            .AddClass("ProblemDetailsExtensions", @class =>
            {
                @class.Internal().Static();
                @class.AddMethod("IActionResult", "AddContextInformation", method =>
                {
                    method.Static();
                    method.AddParameter("ObjectResult", "objectResult", parm => parm.WithThisModifier());
                    method.AddParameter("ExceptionContext", "context");
                    method.AddIfStatement("objectResult.Value is not ProblemDetails problemDetails", stmt => stmt
                        .AddStatement("return objectResult;"));
                    method.AddStatements($@"
                        problemDetails.Extensions.Add(""traceId"", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
                        problemDetails.Type = ""https://httpstatuses.io/"" + (objectResult.StatusCode ?? problemDetails.Status);
                        return objectResult;");
                });
            })
            .OnBuild(InstallKnownExceptions);
    }

    private void InstallKnownExceptions(CSharpFile file)
    {
        var priClass = file.Classes.First(p => p.Name == "ExceptionFilter");
        var switchStatement = GetExceptionSwitchStatement(priClass);

        if (ExecutionContext
            .FindTemplateInstances<IClassProvider>(TemplateDependency.OnTemplate("Application.Validation"))
            .Concat(ExecutionContext
                .FindTemplateInstances<IClassProvider>(TemplateDependency.OnTemplate("Application.Validation.Dto")))
            .Any())
        {
            file.AddUsing("FluentValidation");
            switchStatement.AddCase("ValidationException exception", block => block
                .AddForEachStatement("error", "exception.Errors", stmt => stmt
                    .AddStatement("context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);"))
                .AddInvocationStatement("context.Result = new BadRequestObjectResult", invoke => invoke
                    .AddArgument("new ValidationProblemDetails(context.ModelState)")
                    .WithoutSemicolon())
                .AddInvocationStatement(".AddContextInformation", stmt => stmt.AddArgument("context"))
                .AddStatement("context.ExceptionHandled = true;")
                .WithBreak());
        }

        var forbidException = ExecutionContext.FindTemplateInstance(
            TemplateDependency.OnTemplate("Application.ForbiddenAccessException"));
        if (forbidException is not null)
        {
            switchStatement.AddCase(GetTypeName(forbidException),
                block => block.AddStatement("context.Result = new ForbidResult();")
                    .AddStatement("context.ExceptionHandled = true;")
                    .WithBreak());
        }

        switchStatement.AddCase(UseType("System.UnauthorizedAccessException"),
            block => block.AddStatement("context.Result = new UnauthorizedResult();")
                .AddStatement("context.ExceptionHandled = true;")
                .WithBreak());

        var notFoundException = ExecutionContext.FindTemplateInstance(
            TemplateDependency.OnTemplate("Domain.NotFoundException"));
        if (notFoundException is not null)
        {
            switchStatement.AddCase($"{GetTypeName(notFoundException)} exception", block => block
                .AddInvocationStatement("context.Result = new NotFoundObjectResult", invoke => invoke
                    .AddArgument(new CSharpObjectInitializerBlock("new ProblemDetails")
                        .AddInitStatement("Detail", "exception.Message"))
                    .WithoutSemicolon())
                .AddInvocationStatement(".AddContextInformation", stmt => stmt.AddArgument("context"))
                .AddStatement("context.ExceptionHandled = true;")
                .WithBreak());
        }
    }

    private static CSharpSwitchStatement GetExceptionSwitchStatement(CSharpClass priClass)
    {
        return (CSharpSwitchStatement)priClass
            .FindMethod("OnException")
            .FindStatement(p => p.HasMetadata("exception-switch"));
    }

    [IntentManaged(Mode.Fully)]
    public CSharpFile CSharpFile { get; }

    [IntentManaged(Mode.Fully)]
    protected override CSharpFileConfig DefineFileConfig()
    {
        return CSharpFile.GetConfig();
    }

    [IntentManaged(Mode.Fully)]
    public override string TransformText()
    {
        return CSharpFile.ToString();
    }
}