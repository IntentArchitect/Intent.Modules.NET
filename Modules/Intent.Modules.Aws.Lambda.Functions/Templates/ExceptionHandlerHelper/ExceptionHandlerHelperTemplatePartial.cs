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

namespace Intent.Modules.Aws.Lambda.Functions.Templates.ExceptionHandlerHelper;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class ExceptionHandlerHelperTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Aws.Lambda.Functions.ExceptionHandlerHelper";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public ExceptionHandlerHelperTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddTypeSource("Application.ForbiddenAccessException");
        AddTypeSource("Domain.NotFoundException");

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Collections.Generic")
            .AddUsing("System.Net")
            .AddUsing("System.Threading.Tasks")
            .AddUsing("Amazon.Lambda.Annotations.APIGateway")
            .AddUsing("Microsoft.Extensions.Logging")
            .AddClass($"ExceptionHandlerHelper", cls =>
            {
                cls.Static();

                cls.AddMethod("Task<IHttpResult>", "ExecuteAsync", m =>
                {
                    m.Async();
                    m.Static();
                    m.AddParameter("Func<Task<IHttpResult>>", "operation");
                    m.AddParameter("ILogger", "logger");
                    m.AddTryBlock(tryBlock => { tryBlock.AddStatement("return await operation();"); });
                    m.AddCatchBlock("Exception", "ex", catchBlock =>
                    {
                        catchBlock.AddStatement(@"logger.LogError(ex, ""Unhandled exception occurred: {Message}"", ex.Message);");
                        catchBlock.AddStatement("return HandleException(ex);");
                    });
                });

                cls.AddMethod("IHttpResult", "HandleException", m =>
                {
                    m.Private().Static();
                    m.AddParameter("Exception", "exception");
                    m.AddSwitchStatement("exception",
                        stmt => stmt.AddMetadata("exception-switch", true));
                });

                cls.AddNestedRecord("ResponseDetail", rec =>
                {
                    rec.Private();
                    rec.AddPrimaryConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "Type");
                        ctor.AddParameter("string", "Title");
                        ctor.AddParameter("int", "Status");
                        ctor.AddParameter("object", "Detail");
                    });
                });
                cls.AddNestedRecord("ResponseErrors", rec =>
                {
                    rec.Private();
                    rec.AddPrimaryConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "Type");
                        ctor.AddParameter("string", "Title");
                        ctor.AddParameter("int", "Status");
                        ctor.AddParameter("List<ValidationError>", "Errors");
                    });
                });
                cls.AddNestedRecord("ValidationError", rec =>
                {
                    rec.Private();
                    rec.AddPrimaryConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "Property");
                        ctor.AddParameter("string", "Message");
                        ctor.AddParameter("string", "Code");
                    });
                });
            })
            .OnBuild(InstallKnownExceptions);
    }

    private void InstallKnownExceptions(CSharpFile file)
    {
        var priClass = file.Classes.First(p => p.Name == "ExceptionHandlerHelper");
        var switchStatement = GetExceptionSwitchStatement(priClass);

        if (ExecutionContext
            .FindTemplateInstances<IClassProvider>(TemplateDependency.OnTemplate("Application.Validation"))
            .Concat(ExecutionContext
                .FindTemplateInstances<IClassProvider>(TemplateDependency.OnTemplate("Application.Validation.Dto")))
            .Any())
        {
            file.AddUsing("FluentValidation");
            switchStatement.AddCase("ValidationException validationEx", block => block
                .AddStatement("var errors = new List<ValidationError>();")
                .AddForEachStatement("error", "validationEx.Errors",
                    loop => loop.AddStatement("errors.Add(new ValidationError(error.PropertyName, error.ErrorMessage, error.ErrorCode));"))
                .AddReturn(@"HttpResults.BadRequest(new ResponseErrors(""ValidationError"", ""One or more validation errors occurred"", 400, errors))"));
        }

        var forbidException = ExecutionContext.FindTemplateInstance(
            TemplateDependency.OnTemplate("Application.ForbiddenAccessException"));
        if (forbidException is not null)
        {
            switchStatement.AddCase($"{GetTypeName(forbidException)} forbiddenEx", block => block
                .AddReturn(@"HttpResults.NewResult(HttpStatusCode.Forbidden, new ResponseDetail(""Forbidden"", ""Access forbidden"", 403, forbiddenEx.Message ?? ""You do not have permission to access this resource""))"));
        }

        switchStatement.AddCase(UseType("System.UnauthorizedAccessException") + " unauthorizedEx",
            block => block.AddReturn(@"HttpResults.NewResult(HttpStatusCode.Unauthorized, new ResponseDetail(""Unauthorized"", ""Authentication required"", 401, unauthorizedEx.Message ?? ""Authentication is required to access this resource""))"));

        var notFoundException = ExecutionContext.FindTemplateInstance(
            TemplateDependency.OnTemplate("Domain.NotFoundException"));
        if (notFoundException is not null)
        {
            switchStatement.AddCase($"{GetTypeName(notFoundException)} notFoundEx", block => block
                .AddReturn(@"HttpResults.NotFound(new ResponseDetail(""NotFound"", ""Resource not found"", 404, notFoundEx.Message ?? ""The requested resource was not found""))"));
        }

        switchStatement.AddDefault(block =>
        {
            block.AddReturn(@"HttpResults.NewResult(HttpStatusCode.InternalServerError, new ResponseDetail(""InternalServerError"", ""An error occurred while processing your request"", 500, ""Please try again later or contact support if the problem persists""))");
        });
    }

    private static CSharpSwitchStatement GetExceptionSwitchStatement(CSharpClass priClass)
    {
        return (CSharpSwitchStatement)priClass
            .FindMethod("HandleException")
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