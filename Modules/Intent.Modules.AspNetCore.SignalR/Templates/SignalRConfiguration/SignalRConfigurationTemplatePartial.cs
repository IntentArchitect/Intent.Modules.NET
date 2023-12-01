using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.SignalR.Api;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.SignalR.Templates.SignalRConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SignalRConfigurationTemplate : CSharpTemplateBase<IList<SignalRHubModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.SignalR.SignalRConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SignalRConfigurationTemplate(IOutputTarget outputTarget, IList<SignalRHubModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddClass($"SignalRConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureSignalR", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        method.AddStatements(@"
                                services.AddSignalR();
                                services.RegisterServices();
                                return services;");
                    });
                    @class.AddMethod("void", "RegisterServices", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        foreach (var hubModel in Model)
                        {
                            method.AddStatement($"services.AddTransient<{this.GetHubServiceInterfaceName(hubModel)}, {this.GetHubServiceName(hubModel)}>();");
                        }
                    });
                    @class.AddMethod(UseType("Microsoft.AspNetCore.Routing.IEndpointRouteBuilder"),
                        "MapHubs", method =>
                        {
                            method.Static();
                            method.AddParameter("IEndpointRouteBuilder", "endpoints",
                                param => param.WithThisModifier());
                            foreach (var hubModel in Model)
                            {
                                method.AddStatement(
                                    $@"endpoints.MapHub<{this.GetHubName(hubModel)}>(""{hubModel.GetHubSettings().Route()}"");");
                            }

                            method.AddStatement("return endpoints;");
                        });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureSignalR")
                .HasDependency(this));

            var startup = ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            startup?.CSharpFile.AfterBuild(_ =>
            {
                startup.StartupFile.AddUseEndpointsStatement(ctx => $"{ctx.Endpoints}.MapHubs();");
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