using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.ApiGateway.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
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
                .AddUsing("Ocelot.Configuration.File")
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
                        method.AddStatement("services.AddOcelot(configuration);");
                        method.AddStatement("ConfigureDownstreamHostAndPortsPlaceholders(services, configuration);");
                        method.AddStatement("return services;");
                    });
                    @class.AddNestedClass("GlobalHosts", nested =>
                    {
                        nested.WithBaseType("Dictionary<string, Uri>");
                    });
                    @class.AddMethod("void", "ConfigureDownstreamHostAndPortsPlaceholders", method =>
                    {
                        method.Static();
                        method.Private();
                        method.AddParameter("IServiceCollection", "services");
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddInvocationStatement("services.PostConfigure<FileConfiguration>", inv => inv
                            .AddArgument(new CSharpLambdaBlock("fileConfiguration")
                                .AddStatement(@"var globalHosts = configuration.GetSection(""Ocelot:Hosts"").Get<GlobalHosts>();")
                                .AddForEachStatement("route", "fileConfiguration.Routes", fe => fe
                                    .AddStatement("ConfigureRoute(route, globalHosts);"))));
                    });
                    @class.AddMethod("void", "ConfigureRoute", method =>
                    {
                        method.Static();
                        method.Private();
                        method.AddParameter("FileRoute", "route");
                        method.AddParameter("GlobalHosts" + (OutputTarget.GetProject().NullableEnabled ? "?" : ""), "globalHosts");
                        method.AddForEachStatement("hostAndPort", "route.DownstreamHostAndPorts", fe =>
                        {
                            fe.AddStatements("""
                                             var host = hostAndPort.Host;
                                             if (!host.StartsWith('{') || !host.EndsWith('}'))
                                             {
                                                 continue;
                                             }
                                             var placeHolder = host.TrimStart('{').TrimEnd('}');
                                             if (globalHosts?.TryGetValue(placeHolder, out var uri) != true || uri == null)
                                             {
                                                 continue;
                                             }
                                             route.DownstreamScheme = uri.Scheme;
                                             hostAndPort.Host = uri.Host;
                                             hostAndPort.Port = uri.Port;
                                             """);
                        });
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            var apiGatewayModels = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id).GetApiGatewayRouteModels();
            var downstreamServiceNames = apiGatewayModels
                .Select(s => s.DownstreamEndpoints().FirstOrDefault())
                .Where(p => p is not null)
                .OfType<DownstreamEndModel>()
                .Select(s => s.Element as IElement)
                .OfType<IElement>()
                .Select(s => s.Package.Name)
                .Distinct();
            foreach (var serviceName in downstreamServiceNames)
            {
                this.ApplyAppSetting($"Ocelot:Hosts:{serviceName}", "Enter URL here");
            }
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

            if (programTemplate.ProgramFile.UsesMinimalHostingModel &&
                programTemplate.ExecutionContext.FindTemplateInstances<ITemplate>(TemplateRoles.Distribution.WebApi.Controller).Any())
            {
                throw new Exception("Using ASP.NET Core Web API Controller with Minimal Hosting Model is not supported. Please ensure that Minimal Hosting Model is disabled. If you need this capability please contact support at support@intentarchitect.com.");
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
                    if (statement is not null &&
                        startupFileTemplate.Project.GetProject().InternalElement?.AsCSharpProjectNETModel()?.GetNETSettings().UseTopLevelStatements() == true)
                    {
                        statement.InsertAbove($"await {context.App}.UseOcelot();");
                    }
                    else if (statement is not null)
                    {
                        statement.InsertAbove($"{context.App}.UseOcelot().Wait();");
                    }
                    else
                    {
                        statements.AddStatement($"{context.App}.UseOcelot().Wait();");
                    }
                });
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