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

namespace Intent.Modules.Bugsnag.Templates.BugsnagConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class BugsnagConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Bugsnag.BugsnagConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BugsnagConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.BugsnagAspNetCore(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Bugsnag.AspNet.Core")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"BugsnagConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureBugsnag", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddInvocationStatement("services.AddBugsnag", inv => inv
                            .AddArgument(new CSharpLambdaBlock("cfg")
                                .AddStatement(@"cfg.ApiKey = configuration[""Bugsnag:ApiKey""];")
                                .AddStatement(@"cfg.AppVersion = configuration[""Bugsnag:AppVersion""];")
                                .AddIfStatement(@"!string.IsNullOrWhiteSpace(configuration[""Bugsnag:ReleaseStage""])", ifStmt => ifStmt
                                    .AddStatement(@"cfg.ReleaseStage = configuration[""Bugsnag:ReleaseStage""];"))));

                        method.AddStatement("return services;");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            this.ApplyAppSetting("Bugsnag", new
            {
                ApiKey = "",
                AppVersion = "1.0.0",
                ReleaseStage = ""
            });

            this.ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureBugsnag", ServiceConfigurationRequest.ParameterType.Configuration)
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