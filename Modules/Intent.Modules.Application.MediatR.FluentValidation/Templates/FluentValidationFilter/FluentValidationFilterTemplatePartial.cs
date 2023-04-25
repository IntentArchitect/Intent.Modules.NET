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

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.FluentValidationFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class FluentValidationFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.FluentValidation.FluentValidationFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FluentValidationFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("FluentValidationFilter")
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddUsing("Microsoft.AspNetCore.Mvc.Filters")
                .AddClass($"FluentValidationFilter", @class =>
                {
                    @class.ImplementsInterface("IExceptionFilter");
                    @class.AddMethod("void", "OnException", method =>
                    {
                        method.AddParameter("ExceptionContext", "context");
                        method.AddIfStatement("context.Exception is ValidationException ex", ifStmt => ifStmt
                            .AddForEachStatement("error", "ex.Errors", forStmt => forStmt
                                .AddStatement(
                                    "context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);"))
                            .AddStatement(
                                "context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));")
                            .SeparatedFromPrevious());
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