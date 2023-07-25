using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Intent.Modules.AspNetCore.HealthChecks.Templates.HealthChecksConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HealthChecksConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.HealthChecks.HealthChecksConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HealthChecksConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("HealthChecks.UI.Client")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.AspNetCore.Diagnostics.HealthChecks")
                .AddUsing("Microsoft.AspNetCore.Routing")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"HealthChecksConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureHealthChecks", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement("var hcBuilder = services.AddHealthChecks();", stmt => stmt.AddMetadata("add-health-checks", true));
                        method.AddStatement("return services;", stmt => stmt.SeparatedFromPrevious());
                    });
                    @class.AddMethod("IEndpointRouteBuilder", "MapDefaultHealthChecks", method =>
                    {
                        method.Static();
                        method.AddParameter("IEndpointRouteBuilder", "endpoints", param => param.WithThisModifier());
                        method.AddInvocationStatement("endpoints.MapHealthChecks", mapHealthCheck =>
                        {
                            mapHealthCheck.AddArgument(@"""/hc""");
                            mapHealthCheck.AddArgument(new CSharpObjectInitializerBlock("new HealthCheckOptions")
                                .AddInitStatement("Predicate", "_ => true")
                                .AddInitStatement("ResponseWriter", "UIResponseWriter.WriteHealthCheckUIResponse"));
                        });
                        method.AddStatement("return endpoints;");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureHealthChecks", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));

            var startup = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("App.Startup"));
            startup?.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                ((IHasCSharpStatements)priClass.FindMethod("Configure")
                        ?.FindStatement(s => s.GetText("").Contains("UseEndpoints")))
                    ?.InsertStatement(0, $"endpoints.MapDefaultHealthChecks();");
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