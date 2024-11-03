using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Intent.Modules.AzureFunctions.Templates.Isolated.Program
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.Isolated.Program";

        private readonly IList<ServiceConfigurationRequest> _serviceConfigurations = [];

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(HandleServiceConfigurationRequest);

            if (CanRunTemplate())
            {
                AddNugetDependency(NugetPackages.MicrosoftApplicationInsightsWorkerService(outputTarget));
                AddNugetDependency(NugetPackages.MicrosoftAzureFunctionsWorkerApplicationInsights(outputTarget));
                
                AddNugetDependency(NugetPackages.MicrosoftAzureFunctionsWorker(outputTarget));
                AddNugetDependency(NugetPackages.MicrosoftAzureFunctionsWorkerSdk(outputTarget));
                AddNugetDependency(NugetPackages.MicrosoftAzureFunctionsWorkerExtensionsHttp(outputTarget));
                AddNugetDependency(NugetPackages.MicrosoftAzureFunctionsWorkerExtensionsHttpAspNetCore(outputTarget));
                
                // Remove InProcess nuget dependencies
                ExecutionContext.EventDispatcher.Publish(new RemoveNugetPackageEvent(NugetPackages.MicrosoftNETSdkFunctionsPackageName, outputTarget));
                ExecutionContext.EventDispatcher.Publish(new RemoveNugetPackageEvent(NugetPackages.MicrosoftAzureFunctionsExtensionsPackageName, outputTarget));
            }

            var configStatements = new CSharpLambdaBlock("(ctx, services)");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Azure.Functions.Worker")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("System.Configuration")
                .AddTopLevelStatements(tls =>
                {
                    var hostConfigStatement = new CSharpStatement("new HostBuilder()")
                        .AddInvocation("ConfigureFunctionsWebApplication", i => i.OnNewLine())
                        .AddInvocation("ConfigureServices", cs => cs
                            .OnNewLine()
                            .AddArgument(configStatements
                                .AddStatement("var configuration = ctx.Configuration;")
                                .AddStatement("services.AddApplicationInsightsTelemetryWorkerService();")
                                .AddStatement("services.ConfigureFunctionsApplicationInsights();")
                            )
                        )
                        .AddInvocation("Build", i => i.OnNewLine());

                    tls.AddStatement(new CSharpAssignmentStatement("var host", hostConfigStatement));
                    tls.AddStatement("host.Run();", s => s.SeparatedFromPrevious());
                })
                .AfterBuild(file =>
                {
                    configStatements.AddStatements(GetServiceConfigurationStatementList());

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
                });
        }

        public override bool CanRunTemplate()
        {
            return AzureFunctionsHelper.GetAzureFunctionsProcessType(OutputTarget) == AzureFunctionsHelper.AzureFunctionsProcessType.Isolated;
        }
        
        public override void BeforeTemplateExecution()
        {
            OutputTarget.AddFrameworkDependency("Microsoft.AspNetCore.App");
        }

        private void HandleServiceConfigurationRequest(ServiceConfigurationRequest request)
        {
            _serviceConfigurations.Add(request);
        }

        private List<ServiceConfigurationRequest> GetRelevantServiceConfigurationRequests()
        {
            return _serviceConfigurations
                .Where(p => !p.IsHandled)
                .OrderBy(o => o.Priority)
                .ToList();
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
                    return new CSharpStatement($"services.{s.ExtensionMethodName}({GetExtensionMethodParameterList(s)});");
                }));

            return statementList;
        }

        private string GetExtensionMethodParameterList(ServiceConfigurationRequest request)
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