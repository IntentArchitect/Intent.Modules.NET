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

namespace Intent.Modules.AspNetCore.Controllers.Templates.ExceptionFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ExceptionFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.ExceptionFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ExceptionFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Diagnostics")
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddUsing("Microsoft.AspNetCore.Mvc.Filters")
                .AddClass($"ExceptionFilter", @class =>
                {
                    @class.ImplementsInterface("IExceptionFilter");
                    @class.AddMethod("void", "OnException", method =>
                    {
                        method.AddParameter("ExceptionContext", "context");
                        method.AddSwitchStatement("context.Exception",
                            stmt => stmt.AddMetadata("exception-switch", true));
                    });
                    @class.AddMethod("IActionResult", "BuildResult", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("ExceptionContext", "context");
                        method.AddParameter("Func<ProblemDetails, ObjectResult>", "objectResultFactory");
                        method.AddParameter("ProblemDetails", "problemDetails");
                        method.AddStatements(@"
var result = objectResultFactory(problemDetails);
problemDetails.Extensions.Add(""traceId"", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
problemDetails.Type = $""https://httpstatuses.io/{result.StatusCode}"";
return result;");
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