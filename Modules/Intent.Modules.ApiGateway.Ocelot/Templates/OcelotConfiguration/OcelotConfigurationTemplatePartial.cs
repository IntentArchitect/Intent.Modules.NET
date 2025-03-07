using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Intent.Modules.ApiGateway.Ocelot.Templates.OcelotConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OcelotConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ApiGateway.Ocelot.OcelotConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OcelotConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.Ocelot(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Ocelot.DependencyInjection")
                .AddClass($"OcelotConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IConfigurationBuilder", "ConfigureOcelot", method =>
                    {
                        method.Static();
                        method.AddParameter("IConfigurationBuilder", "configuration", param => param.WithThisModifier());
                        method.AddStatement("""configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);""");
                        method.AddStatement("return configuration;");
                    });
                    @class.AddMethod("IServiceCollection", "ConfigureOcelot", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement(@"services.AddOcelot(configuration);");
                        method.AddStatement("return services;");
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureOcelot", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));

            var programTemplate = ExecutionContext.FindTemplateInstance<IProgramTemplate>("App.Program");
            if (programTemplate == null)
            {
                return;
            }

            var startupFileTemplate = ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            if (startupFileTemplate == null)
            {
                return;
            }

            programTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing(this.Namespace);
                file.AddUsing("Microsoft.Extensions.Configuration");

                if (programTemplate.ProgramFile.UsesMinimalHostingModel)
                {
                    programTemplate.ProgramFile.AddHostBuilderConfigurationStatement("builder.Configuration.ConfigureOcelot();");
                }
                else
                {
                    programTemplate.ProgramFile.ConfigureAppConfiguration(false, (statements, parameters) =>
                    {
                        var ctx = parameters.FirstOrDefault(p => p.Contains("config", StringComparison.OrdinalIgnoreCase));
                        if (ctx is not null)
                        {
                            statements.AddStatement($"{ctx}.ConfigureOcelot();");
                        }
                    });
                }
            }, 1);

            startupFileTemplate.CSharpFile.AfterBuild(file =>
            {
                startupFileTemplate.StartupFile.ConfigureApp((statements, context) =>
                {
                    startupFileTemplate.AddUsing("Ocelot.Middleware");

                    var statement = statements.FindStatement(x => x.GetText("").Contains("app.Run"));
                    if (statement is not null)
                    {
                        statement.InsertAbove($"await {context.App}.UseOcelot();");
                    }
                    else
                    {
                        statements.AddStatement($"{context.App}.UseOcelot().Wait();");
                    }
                });
                startupFileTemplate.StartupFile.AddUseEndpointsStatement(ctx => "// Needed for Ocelot");
            }, 9999);
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