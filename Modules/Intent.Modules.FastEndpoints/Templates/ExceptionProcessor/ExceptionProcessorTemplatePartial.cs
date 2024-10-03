using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Templates.ExceptionProcessor
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ExceptionProcessorTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.FastEndpoints.ExceptionProcessorTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ExceptionProcessorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddKnownType("FastEndpoints.Mode");
            AddKnownType("FastEndpoints.ProblemDetails");
            AddKnownType("FastEndpoints.ForbiddenAccessException");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Diagnostics")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("FastEndpoints")
                .AddUsing("FluentValidation")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Mode = Intent.RoslynWeaver.Attributes.Mode")
                .AddClass($"ExceptionProcessor", @class =>
                {
                    @class.ImplementsInterface("IGlobalPostProcessor");
                    @class.AddMethod("Task", "PostProcessAsync", method =>
                    {
                        method.AddParameter("IPostProcessorContext", "context");
                        method.AddParameter("CancellationToken", "ct");
                        method.Async();

                        method.AddIfStatement("!context.HasExceptionOccurred || context.HttpContext.ResponseStarted()", stmt => stmt.AddStatement("return;"));

                        method.AddSwitchStatement("context.ExceptionDispatchInfo.SourceException", switchStatement =>
                        {
                            switchStatement.AddCase("ValidationException exception", block =>
                            {
                                block.AddStatement("context.MarkExceptionAsHandled();");
                                block.AddStatement("await context.HttpContext.Response.SendResultAsync(new ProblemDetails(exception.Errors.ToList(), context.HttpContext.Request.Path, Activity.Current?.Id ?? context.HttpContext.TraceIdentifier, StatusCodes.Status400BadRequest));");
                                block.WithBreak();
                            });

                            var forbidException = ExecutionContext.FindTemplateInstance(TemplateDependency.OnTemplate("Application.ForbiddenAccessException"));
                            if (forbidException is not null)
                            {
                                switchStatement.AddCase(GetTypeName(forbidException), block =>
                                {
                                    block.AddStatement("context.MarkExceptionAsHandled();");
                                    block.AddStatement("await context.HttpContext.Response.SendResultAsync(Results.Forbid());");
                                    block.WithBreak();
                                });
                            }

                            switchStatement.AddCase("UnauthorizedAccessException", block =>
                            {
                                block.AddStatement("context.MarkExceptionAsHandled();");
                                block.AddStatement("await context.HttpContext.Response.SendResultAsync(Results.Unauthorized());");
                                block.WithBreak();
                            });

                            var notFoundException = ExecutionContext.FindTemplateInstance(TemplateDependency.OnTemplate("Domain.NotFoundException"));
                            if (notFoundException is not null)
                            {
                                switchStatement.AddCase($"{GetTypeName(notFoundException)} exception", block =>
                                {
                                    block.AddStatement("context.MarkExceptionAsHandled();");
                                    block.AddStatement("context.HttpContext.Response.HttpContext.MarkResponseStart();");
                                    block.AddStatement(
                                        "await context.HttpContext.Response.SendResultAsync(Results.NotFound(new { Detail = exception.Message, TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier }));");
                                    block.WithBreak();
                                });
                            }

                            switchStatement.AddDefault(block =>
                            {
                                block.AddStatement("context.ExceptionDispatchInfo.Throw();");
                                block.WithBreak();
                            });
                        });
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
    }
}