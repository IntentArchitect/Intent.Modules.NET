using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AzureFunctions.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.Isolated.GlobalExceptionMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GlobalExceptionMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.Isolated.GlobalExceptionMiddlewareTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GlobalExceptionMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Azure.Functions.Worker")
                .AddUsing("Microsoft.Azure.Functions.Worker.Http")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System")
                .AddUsing("System.Net")
                .AddUsing("System.Text.Json")
                .AddUsing("System.ComponentModel.DataAnnotations")
                .AddClass($"GlobalExceptionMiddleware", @class =>
                {
                    @class.ImplementsInterface(UseType("Microsoft.Azure.Functions.Worker.Middleware.IFunctionsWorkerMiddleware"));
                    @class.AddMethod("Task", "Invoke", invoke =>
                    {
                        invoke.Async();
                        invoke.AddParameter("FunctionContext", "context");
                        invoke.AddParameter("FunctionExecutionDelegate", "next");
                        invoke
                        .AddTryBlock(@try => @try.AddStatement("await next(context);"))
                        .AddCatchBlock("ValidationException", "ve", @catch => @catch.AddStatement("await WriteJsonError(context, HttpStatusCode.BadRequest, ve.ValidationResult?.ErrorMessage ?? ve.Message);"))
                        .AddCatchBlock($"{GetTypeName("Intent.Entities.NotFoundException")}", "nf", @catch => @catch.AddStatement("await WriteJsonError(context, HttpStatusCode.NotFound, nf.Message);"))
                        .AddCatchBlock("JsonException", "je", @catch => @catch.AddStatement("await WriteJsonError(context, HttpStatusCode.BadRequest, je.Message);"))
                        .AddCatchBlock("FormatException", "fe", @catch => @catch.AddStatement("await WriteJsonError(context, HttpStatusCode.BadRequest, fe.Message);"))
                        .AddCatchBlock("Exception", "ex", @catch => @catch.AddStatement("await WriteJsonError(context, HttpStatusCode.InternalServerError, \"Internal server error!!!!\");"));
                    });

                    @class.AddMethod("Task", "WriteJsonError", writeJsonError =>
                    {
                        writeJsonError.Async().Static().Private();

                        writeJsonError.AddParameter("FunctionContext", "context");
                        writeJsonError.AddParameter("HttpStatusCode", "statusCode");
                        writeJsonError.AddParameter("string", "message");

                        writeJsonError.AddAssignmentStatement("var req", new CSharpStatement("await context.GetHttpRequestDataAsync();"));

                        writeJsonError.AddIfStatement("req is null", @if => @if.AddReturn(""));

                        writeJsonError.AddAssignmentStatement("var response", new CSharpStatement("req.CreateResponse(statusCode);"));
                        writeJsonError.AddAssignmentStatement("var error", new CSharpStatement("new { error = message };"));

                        writeJsonError.AddStatement("await response.WriteAsJsonAsync(error);");
                        writeJsonError.AddStatement("context.GetInvocationResult().Value = response;");
                    });
                });
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

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.Settings.GetAzureFunctionsSettings().UseGlobalExceptionMiddleware();
        }
    }
}