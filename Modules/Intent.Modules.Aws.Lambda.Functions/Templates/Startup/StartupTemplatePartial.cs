using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Templates.Startup
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class StartupTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.Lambda.Functions.StartupTemplate";

        private readonly List<ServiceConfigurationRequest> _serviceConfigurations = new();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public StartupTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AmazonLambdaCore(outputTarget));
            AddNugetDependency(NugetPackages.AmazonLambdaAPIGatewayEvents(outputTarget));
            AddNugetDependency(NugetPackages.AmazonLambdaSerializationSystemTextJson(outputTarget));
            AddNugetDependency(NugetPackages.AmazonLambdaAnnotations(outputTarget));
            AddNugetDependency(NugetPackages.AmazonLambdaLoggingAspNetCore(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsLogging(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsDependencyInjection(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationBinder(outputTarget));
            
            AddFrameworkDependency("Microsoft.AspNetCore.App");
            
            ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(HandleServiceConfigurationRequest);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddAssemblyAttribute("LambdaSerializer", attr => attr.AddArgument($"typeof(DefaultLambdaJsonSerializer)"))
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Amazon.Lambda.Serialization.SystemTextJson")
                .AddUsing("Amazon.Lambda.Core")
                .AddClass($"Startup", @class =>
                {
                    @class.AddAttribute(UseType("Amazon.Lambda.Annotations.LambdaStartup"));
                    @class.AddMethod("HostApplicationBuilder", "ConfigureHostBuilder", method =>
                    {
                        method.AddStatement("var hostBuilder = new HostApplicationBuilder();");
                        method.AddAssignmentStatement(
                            new CSharpVariableDeclaration("configuration").ToString(),
                            new CSharpStatement("hostBuilder.Configuration")
                                .AddInvocation("AddJsonFile", add => add.AddArgument(@"""appsettings.json""").AddArgument("optional", "true").OnNewLine())
                                .AddInvocation("AddEnvironmentVariables", add => add.OnNewLine())
                                .AddInvocation("Build", build => build.OnNewLine()));
                        method.AddStatement("hostBuilder.Logging.AddLambdaLogger();");
                    });
                })
                .AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var configMethod = @class.FindMethod("ConfigureHostBuilder")!;
                    configMethod.AddStatements(GetServiceConfigurationStatementList());

                    foreach (var request in GetRelevantServiceConfigurationRequests())
                    {
                        foreach (var templateDependency in request.TemplateDependencies)
                        {
                            var template = GetTemplate<IClassProvider>(templateDependency);
                            if (template != null)
                            {
                                AddUsing(template.Namespace);
                            }

                            AddTemplateDependency(templateDependency);
                        }

                        foreach (var @namespace in request.RequiredNamespaces)
                        {
                            AddUsing(@namespace);
                        }
                    }

                    configMethod.AddReturn("hostBuilder");
                });
        }

        public override void BeforeTemplateExecution()
        {
            this.ExecutionContext.EventDispatcher.Publish(new LaunchProfileRegistrationRequest
            {
                Name = "Mock Lambda Test Tool",
                CommandName = "Executable",
                CommandLineArgs = "--port 5050",
                ExecutablePath = """%USERPROFILE%\.dotnet\tools\dotnet-lambda-test-tool-8.0.exe""",
                EnvironmentVariables = new Dictionary<string, string>(),
                WorkingDirectory = "$(ProjectDir)$(OutputPath)"
            });
            
            Project.GetProject().AddProperty("AWSProjectType", "Lambda");
            Project.GetProject().AddProperty("PublishReadyToRun", "true");
        }
        
        private void HandleServiceConfigurationRequest(ServiceConfigurationRequest request)
        {
            _serviceConfigurations.Add(request);
        }

        private IEnumerable<ServiceConfigurationRequest> GetRelevantServiceConfigurationRequests()
        {
            return _serviceConfigurations
                .Where(p => !p.IsHandled)
                .OrderBy(o => o.Priority)
                .ToArray();
        }

        private List<CSharpStatement> GetServiceConfigurationStatementList()
        {
            var statementList = new List<CSharpStatement>();

            statementList.AddRange(GetRelevantServiceConfigurationRequests()
                .Select(s =>
                {
                    foreach (var dependency in s.TemplateDependencies)
                    {
                        var classProvider = GetTemplate<IClassProvider>(dependency);

                        AddTemplateDependency(dependency);
                        AddUsing(classProvider.Namespace);
                    }
                    return new CSharpStatement($"hostBuilder.Services.{s.ExtensionMethodName}({GetExtensionMethodParameterList(s)});");
                }));

            return statementList;
        }

        private static string GetExtensionMethodParameterList(ServiceConfigurationRequest request)
        {
            if (request.ExtensionMethodParameterList?.Any() != true)
            {
                return string.Empty;
            }

            var paramList = new List<string>();

            foreach (var param in request.ExtensionMethodParameterList)
            {
                switch (param)
                {
                    case ServiceConfigurationRequest.ParameterType.Configuration:
                        paramList.Add("configuration");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            paramName: nameof(request.ExtensionMethodParameterList),
                            actualValue: param,
                            message: "Type specified in parameter list is not known or supported");
                }
            }

            return string.Join(", ", paramList);
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