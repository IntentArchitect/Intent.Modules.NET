using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

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
                    m.AddTryBlock(tryBlock => { tryBlock.AddStatement("return await operation();"); });
                    m.AddCatchBlock("Exception", "ex", catchBlock => { catchBlock.AddStatement("return HandleException(ex);"); });
                });

                cls.AddMethod("IHttpResult", "HandleException", m =>
                {
                    m.Private().Static();
                    m.AddParameter("Exception", "exception");
                    m.AddSwitchStatement("exception", sw =>
                    {
                        sw.AddCase("ValidationException validationEx", cs =>
                        {
                            cs.AddStatement("var errors = new List<ValidationError>();");
                            cs.AddForEachStatement("error", "validationEx.Errors",
                                loop => { loop.AddStatement("errors.Add(new ValidationError(error.PropertyName, error.ErrorMessage, error.ErrorCode));"); });
                            cs.AddStatement(
                                "return HttpResults.BadRequest(new ResponseErrors(\"ValidationError\", \"One or more validation errors occurred\", 400, errors));");
                        });
                        sw.AddCase("NotFoundException notFoundEx",
                            cs =>
                            {
                                cs.AddStatement(
                                    @"return HttpResults.NotFound(new ResponseDetail(""NotFound"", ""Resource not found"", 404, notFoundEx.Message ?? ""The requested resource was not found""));");
                            });
                        sw.AddCase("ForbiddenAccessException forbiddenEx",
                            cs =>
                            {
                                cs.AddStatement(
                                    @"return HttpResults.NewResult(HttpStatusCode.Forbidden, new ResponseDetail(""Forbidden"", ""Access forbidden"", 403, forbiddenEx.Message ?? ""You do not have permission to access this resource""));");
                            });
                        sw.AddCase("UnauthorizedAccessException unauthorizedEx",
                            cs =>
                            {
                                cs.AddStatement(
                                    @"return HttpResults.NewResult(HttpStatusCode.Unauthorized, new ResponseDetail(""Unauthorized"", ""Authentication required"", 401, unauthorizedEx.Message ?? ""Authentication is required to access this resource""));");
                            });
                        sw.AddDefault(cs =>
                        {
                            cs.AddStatement(
                                @"return HttpResults.NewResult(HttpStatusCode.InternalServerError, new ResponseDetail(""InternalServerError"", ""An error occurred while processing your request"", 500, ""Please try again later or contact support if the problem persists""));");
                        });
                    });
                });

                cls.AddNestedRecord("ResponseDetail", rec=>
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
                cls.AddNestedRecord("ResponseErrors", rec=>
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
                cls.AddNestedRecord("ValidationError", rec=>
                {
                    rec.Private();
                    rec.AddPrimaryConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "Property");
                        ctor.AddParameter("string", "Message");
                        ctor.AddParameter("string", "Code");
                    });
                });
            });
    }

    [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

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