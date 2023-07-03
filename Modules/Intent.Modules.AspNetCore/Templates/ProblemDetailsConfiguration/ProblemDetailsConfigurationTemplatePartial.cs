using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Templates.ProblemDetailsConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ProblemDetailsConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.ProblemDetailsConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProblemDetailsConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("Microsoft.AspNetCore.Hosting")
                .AddUsing("Microsoft.AspNetCore.Diagnostics")
                .AddClass($"ProblemDetailsConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureProblemDetails", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        method.AddInvocationStatement("services.AddProblemDetails", addProblemDetails => addProblemDetails
                                .AddArgument(new CSharpLambdaBlock("conf")
                                    .WithExpressionBody(new CSharpLambdaBlock("conf.CustomizeProblemDetails = context")
                                        .AddStatements(@"
                        context.ProblemDetails.Type = $""https://httpstatuses.io/{context.ProblemDetails.Status}"";
                
                        if (context.ProblemDetails.Status != 500) { return; }
                        context.ProblemDetails.Title = ""Internal Server Error"";
                
                        var env = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>()!;
                        if (!env.IsDevelopment()) { return; }
                
                        var exceptionFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionFeature is null) { return; }
                        context.ProblemDetails.Detail = exceptionFeature.Error.ToString();")
                                    )))
                            .AddStatement("return services;");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureProblemDetails")
                .HasDependency(this));
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