using Intent.Engine;
using Intent.Modules.AzureFunctions.Settings;
using Intent.Modules.AzureFunctions.Templates.Isolated.GlobalExceptionMiddleware;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static Intent.Modules.Common.CSharp.AppStartup.IAppStartupFile;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.Isolated.Program
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.Isolated.Program";

        private readonly Lazy<bool> _hasMultipleInstances;
        private readonly List<ContainerRegistrationRequest> _containerRegistrationRequests = new();
        private readonly List<ServiceConfigurationRequest> _serviceConfigurationRequests = new();
        private readonly CSharpLambdaBlock _configureServicesBlock;
        private readonly IServiceConfigurationContext _serviceConfigurationContext;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            _hasMultipleInstances = new Lazy<bool>(() => ExecutionContext.FindTemplateInstances<ITemplate>(TemplateId).Count() > 1);

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


            var isBuilt = false;
            OnEmitOrPublished<ServiceConfigurationRequest>(request =>
            {
                if (isBuilt)
                {
                    return;
                }

                _serviceConfigurationRequests.Add(request);
            });

            OnEmitOrPublished<ContainerRegistrationRequest>(request =>
            {
                if (isBuilt)
                {
                    return;
                }

                _containerRegistrationRequests.Add(request);
            });

            var configStatements = new CSharpLambdaBlock("(ctx, services)");
            _configureServicesBlock = configStatements;
            _serviceConfigurationContext = new ServiceConfigurationContext("configuration", "services");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Azure.Functions.Worker")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("System.Configuration")
                .AddUsing("System.Linq")
                .AddTopLevelStatements(tls =>
                {


                    configStatements.AddStatement("var configuration = ctx.Configuration;")
                                .AddStatement("services.AddApplicationInsightsTelemetryWorkerService();")
                                .AddStatement("services.ConfigureFunctionsApplicationInsights();")
                                .AddInvocationStatement($"services.Configure<{UseType("Microsoft.Extensions.Logging.LoggerFilterOptions")}>", conf => conf
                                    .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                    {
                                        arg.AddStatement(
                                            "// The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override.");
                                        arg.AddStatement(
                                            "// Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs");
                                        arg.AddStatement(
                                            """const string applicationInsightsLoggerProvider = "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider";""");
                                        arg.AddStatement("var toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName == applicationInsightsLoggerProvider);");
                                        arg.AddIfStatement("toRemove is not null", fi =>
                                        {
                                            fi.AddStatement("options.Rules.Remove(toRemove);");
                                        });
                                    }));
                    InitializeMissingServiceConfigMetadata(configStatements);
                    var globalExceptionConfigStatement = new CSharpLambdaBlock("(ctx, builder)");

                    if (ExecutionContext.Settings.GetAzureFunctionsSettings().UseGlobalExceptionMiddleware())
                    {
                        configStatements.AddStatement($"services.AddSingleton<{GetTypeName(GlobalExceptionMiddlewareTemplate.TemplateId)}>();");
                        globalExceptionConfigStatement.AddStatement($"builder.UseMiddleware<GlobalExceptionMiddleware>();");
                    }

                    var hostConfigStatement = new CSharpStatement("new HostBuilder()")
                        .AddInvocation("ConfigureFunctionsWebApplication", i => i
                            .OnNewLine()
                            .AddArgument(globalExceptionConfigStatement)
                            )
                        .AddInvocation("ConfigureServices", cs => cs
                            .OnNewLine()
                            .AddArgument(configStatements)
                        )
                        .AddInvocation("Build", i => i.OnNewLine());

                    tls.AddStatement(new CSharpAssignmentStatement("var host", hostConfigStatement));
                    tls.AddStatement("host.Run();", s => s.SeparatedFromPrevious());

                    foreach (var request in _serviceConfigurationRequests)
                    {
                        ProcessServiceConfigurationRequest(request);
                    }

                    foreach (var request in _containerRegistrationRequests)
                    {
                        ProcessContainerRegistrationRequest(request);
                    }

                    isBuilt = true;

                    // Our previous subscriptions are configured to only process if "isBuilt" is false,
                    // and we make new subscriptions below. This is because other templates which are
                    // also subscribed may have been (by chance) instantiated after this class,
                    // resulting in their handlers being registered after and thus would only get to
                    // handle their subscriptions after this class which will read them all as
                    // unhandled. So we make new subscriptions which we know will process requests
                    // after the other handlers.
                    OnEmitOrPublished<ServiceConfigurationRequest>(ProcessServiceConfigurationRequest);
                    OnEmitOrPublished<ContainerRegistrationRequest>(ProcessContainerRegistrationRequest);
                });

            if (ExecutionContext.GetSettings().GetAzureFunctionsSettings().UseGlobalExceptionMiddleware())
            {
                CSharpFile.AddUsing(this.GetNamespace());
            }
        }

        private void InitializeMissingServiceConfigMetadata(CSharpLambdaBlock configStatements)
        {
            foreach (var statement in configStatements.Statements.Where(s => !s.HasMetadata("startup-statement-type")))
            {
                statement
                    .AddMetadata("startup-statement-type", StatementType.ConfigureServices)
                    .AddMetadata("startup-statement-priority", 0);
            }
        }

        public override bool CanRunTemplate()
        {
            return AzureFunctionsHelper.GetAzureFunctionsProcessType(OutputTarget) == AzureFunctionsHelper.AzureFunctionsProcessType.Isolated;
        }

        public override void AfterTemplateRegistration()
        {
            if (CanRunTemplate())
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("FUNCTIONS_WORKER_RUNTIME", "dotnet-isolated"));
            }
        }

        public override void BeforeTemplateExecution()
        {
            OutputTarget.GetProject().AddFrameworkDependency("Microsoft.AspNetCore.App");
        }

        private bool IsApplicable(
            IEnumerable<ITemplateDependency> templateDependencies,
            [NotNullWhen(true)] out List<(ITemplateDependency Dependency, IClassProvider ClassProvider)>? resolvedDependencies)
        {
            resolvedDependencies = [];

            foreach (var dependency in templateDependencies)
            {
                var classProvider = ExecutionContext.FindTemplateInstance<IClassProvider>(dependency);

                if (_hasMultipleInstances.Value &&
                    !ExecutionContext.IsAccessibleTo(classProvider!, OutputTarget))
                {
                    resolvedDependencies = null;
                    return false;
                }

                resolvedDependencies.Add((dependency, classProvider)!);
            }

            return true;
        }


        private void ProcessServiceConfigurationRequest(ServiceConfigurationRequest request)
        {
            if (request.IsHandled ||
                !IsApplicable(request.TemplateDependencies, out var resolvedDependencies))
            {
                return;
            }

            foreach (var (dependency, classProvider) in resolvedDependencies)
            {
                AddTemplateDependency(dependency);
                AddUsing(classProvider.Namespace);
            }

            foreach (var @namespace in request.RequiredNamespaces)
            {
                AddUsing(@namespace);
            }

            AddServiceConfiguration<CSharpStatement>(ctx =>
            {
                var parameterList = new List<string>();

                foreach (var parameter in request.ExtensionMethodParameterList)
                {
                    switch (parameter)
                    {
                        case ServiceConfigurationRequest.ParameterType.Configuration:
                            parameterList.Add(ctx.Configuration);
                            break;
                        default:
                            //If its not a "magic string" parameter , use what they gave us.
                            parameterList.Add(parameter);
                            break;
                    }
                }

                return $"{ctx.Services}.{request.ExtensionMethodName}({string.Join(", ", parameterList)});";
            }, priority: request.Priority);
        }

        private void ProcessContainerRegistrationRequest(ContainerRegistrationRequest request)
        {
            if (request.IsHandled ||
                !IsApplicable(request.TemplateDependencies, out var resolvedDependencies))
            {
                return;
            }

            foreach (var (dependency, classProvider) in resolvedDependencies)
            {
                AddTemplateDependency(dependency);
                AddUsing(classProvider.Namespace);
            }

            foreach (var @namespace in request.RequiredNamespaces)
            {
                AddUsing(@namespace);
            }

            AddContainerRegistration<CSharpStatement>(ctx =>
            {
                return (request.ConcreteType.StartsWith("typeof("), request.InterfaceType != null) switch
                {
                    (false, false) => $"{ctx.Services}.{RegistrationType(request)}<{UseType(request.ConcreteType)}>();",
                    (false, true) => $"{ctx.Services}.{RegistrationType(request)}<{UseType(request.InterfaceType!)}, {UseType(request.ConcreteType)}>();",
                    (true, false) => $"{ctx.Services}.{RegistrationType(request)}({UseTypeOf(request.ConcreteType)});",
                    (true, true) => $"{ctx.Services}.{RegistrationType(request)}({UseTypeOf(request.InterfaceType!)}, {UseTypeOf(request.ConcreteType)});"
                };

                string UseTypeOf(string type)
                {
                    var typeName = type.Substring("typeof(".Length, type.Length - "typeof()".Length);
                    return $"typeof({UseType(typeName)})";
                }

                static string RegistrationType(ContainerRegistrationRequest registration)
                {
                    return registration.Lifetime switch
                    {
                        ContainerRegistrationRequest.LifeTime.Singleton => "AddSingleton",
                        ContainerRegistrationRequest.LifeTime.PerServiceCall => "AddScoped",
                        _ => "AddTransient"
                    };
                }
            }, priority: request.Priority);
        }

        public void ConfigureServices(Action<IHasCSharpStatements, IServiceConfigurationContext> configure)
        {
            if (_configureServicesBlock == null)
            {
                /*
                switch (_usesMinimalHostingModel, _usesTopLevelStatements)
                {
                    case (false, false) or (false, true):
                        {
                            _configureServicesBlock = _cSharpFile.Classes.Single().Methods.First(x => x.Name == "ConfigureServices");
                            break;
                        }
                    case (true, false):
                        {
                            var block = new ServiceRegistrationStatementBlock();
                            _configureServicesBlock = block;
                            _cSharpFile.Classes.Single().Methods.First(x => x.Name == "Main").AddStatement(block);
                            break;

                        }
                    case (true, true):
                        {
                            var block = new ServiceRegistrationStatementBlock();
                            _configureServicesBlock = block;
                            _cSharpFile.TopLevelStatements.AddStatement(block);
                            break;
                        }
                }*/
            }

            configure(_configureServicesBlock, _serviceConfigurationContext);

        }

        public void AddServiceConfiguration<TStatement>(
            Func<IServiceConfigurationContext, TStatement> create,
            ConfigureServiceStatement<TStatement>? configure = null,
            int? priority = null) where TStatement : CSharpStatement
        {
            ConfigureServices((statements, context) =>
            {
                var statement = create(context);
                Insert(statements, statement, StatementType.ConfigureServices, priority);
                configure?.Invoke(statement, context);
            });
        }

        private static void Insert(
            IHasCSharpStatements hasCSharpStatements,
            CSharpStatement statement,
            StatementType type,
            int? priority)
        {
            priority ??= int.MaxValue;

            statement
                .AddMetadata("startup-statement-type", type)
                .AddMetadata("startup-statement-priority", priority);

            var insertBelow = hasCSharpStatements.Statements
                .Where(x => x.TryGetMetadata<StatementType>("startup-statement-type", out var metadataType) &&
                            metadataType == type)
                .LastOrDefault(x => (int)x.GetMetadata("startup-statement-priority") <= priority);

            if (insertBelow == null && type == StatementType.ContainerRegistration)
            {
                insertBelow = hasCSharpStatements.Statements
                    .LastOrDefault(x => x.TryGetMetadata<StatementType>("startup-statement-type", out var metadataType) &&
                                        metadataType == StatementType.ConfigureServices);
            }

            if (insertBelow == null)
            {
                hasCSharpStatements.InsertStatement(0, statement);
                return;
            }

            insertBelow.InsertBelow(statement);

            if (insertBelow.HasMetadata("startup-statement-requires-line-after"))
            {
                statement.SeparatedFromPrevious();
            }
        }

        public void AddContainerRegistration<TStatement>(
            Func<IServiceConfigurationContext, TStatement> create,
            ConfigureServiceStatement<TStatement>? configure = null,
            int? priority = null) where TStatement : CSharpStatement
        {
            ConfigureServices((statements, context) =>
            {
                var statement = create(context);
                Insert(statements, statement, StatementType.ContainerRegistration, priority);
                configure?.Invoke(statement, context);
            });
        }

        private enum StatementType
        {
            ConfigureServices,
            ContainerRegistration,
            AppConfiguration
        }
        private record ServiceConfigurationContext(string Configuration, string Services) : IServiceConfigurationContext;

        /*
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
        }*/

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