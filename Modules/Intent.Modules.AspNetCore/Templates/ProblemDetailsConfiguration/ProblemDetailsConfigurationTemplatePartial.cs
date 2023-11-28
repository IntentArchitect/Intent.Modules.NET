using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
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
                .AddClass($"ProblemDetailsConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureProblemDetails", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        if (OutputTarget.GetProject().IsNetApp(6))
                        {
                            GetDotNet6Implementation(@class, method);
                        }
                        else
                        {
                            GetDotNet7AndAboveImplementation(method);
                        }
                    });
                });
        }

        private void GetDotNet6Implementation(CSharpClass @class, CSharpClassMethod method)
        {
            CSharpFile
                .AddUsing("System.Diagnostics")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("Microsoft.AspNetCore.Hosting")
                .AddUsing("Microsoft.AspNetCore.Diagnostics")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddUsing("System.Collections.Generic")
                ;
            @class.AddField(UseType("System.Text.Json.JsonSerializerOptions"), "DefaultOptions", field =>
            {
                field.PrivateReadOnly().Static();
                field.WithAssignment(new CSharpObjectInitializerBlock("new()")
                    .AddInitStatement("DefaultIgnoreCondition", $"{UseType("System.Text.Json.Serialization.JsonIgnoreCondition")}.Never")
                    .AddInitStatement("IgnoreReadOnlyFields", "false")
                    .AddInitStatement("IgnoreReadOnlyProperties", "false")
                    .AddInitStatement("IncludeFields", "false")
                    .AddInitStatement("WriteIndented", "false"));
            });
            method.AddInvocationStatement("services.AddExceptionHandler", addProblemDetails => addProblemDetails
                .AddArgument(new CSharpLambdaBlock("conf")
                    .WithExpressionBody(new CSharpLambdaBlock("conf.ExceptionHandler = context")
                        .AddStatements(@"
                        var details = new ProblemDetails
                        {
                            Status = context.Response.StatusCode,
                            Type = $""https://httpstatuses.io/{context.Response.StatusCode}"",
                            Title = ""Internal Server Error""
                        };
                        details.Extensions.TryAdd(""traceId"", Activity.Current?.Id ?? context.TraceIdentifier);
                        
                        var env = context.RequestServices.GetService<IWebHostEnvironment>()!;
                        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (env.IsDevelopment() && exceptionFeature is not null)
                        {
                            details.Detail = exceptionFeature.Error.ToString();
                        }")
                        .AddInvocationStatement("return context.Response.WriteAsJsonAsync", inv => inv
                            .AddArgument("details")
                            .AddArgument("DefaultOptions")
                            .AddArgument(@"contentType: ""application/problem+json"""))
                    )))
                .AddStatement("return services;");
        }

        // .NET 8 might look different: https://anthonygiretti.com/2023/06/14/asp-net-core-8-improved-exception-handling-with-iexceptionhandler/
        private void GetDotNet7AndAboveImplementation(CSharpClassMethod method)
        {
            CSharpFile
                .AddUsing("System.Diagnostics")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("Microsoft.AspNetCore.Hosting")
                .AddUsing("Microsoft.AspNetCore.Diagnostics")
                .AddUsing("System.Collections.Generic");
            method.AddInvocationStatement("services.AddProblemDetails", addProblemDetails => addProblemDetails
                    .AddArgument(new CSharpLambdaBlock("conf")
                        .WithExpressionBody(new CSharpLambdaBlock("conf.CustomizeProblemDetails = context")
                            .AddStatements(@"
                        context.ProblemDetails.Type = $""https://httpstatuses.io/{context.ProblemDetails.Status}"";
                
                        if (context.ProblemDetails.Status != 500) { return; }
                        context.ProblemDetails.Title = ""Internal Server Error"";
                        context.ProblemDetails.Extensions.TryAdd(""traceId"", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
                
                        var env = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>()!;
                        if (!env.IsDevelopment()) { return; }
                
                        var exceptionFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionFeature is null) { return; }
                        context.ProblemDetails.Detail = exceptionFeature.Error.ToString();")
                        )))
                .AddStatement("return services;");
        }

        public override bool CanRunTemplate()
        {
            return IsExceptionHandlerSupported(OutputTarget);
        }

        public static bool IsExceptionHandlerSupported(IOutputTarget outputTarget)
        {
            var proj = outputTarget.GetProject();
            return !proj.IsNetCore2App() &&
                   !proj.IsNetCore3App() &&
                   !proj.IsNetApp(4) &&
                   !proj.IsNetApp(5);
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate()) { return; }

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureProblemDetails")
                .HasDependency(this));

            ExecutionContext.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest
                .ToRegister("UseExceptionHandler")
                .WithPriority(-40));
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