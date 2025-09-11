#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Aws.Lambda.Functions.Templates.Startup;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Aws.Lambda.Functions.Templates;

using static IAppStartupFile;

internal class AppStartupFile : IAppStartupFile
{
    private readonly StartupTemplate _template;
    private readonly IServiceConfigurationContext _serviceConfigurationContext;
    private readonly CSharpFile _cSharpFile;
    private readonly List<ContainerRegistrationRequest> _containerRegistrationRequests = new();
    private readonly List<ServiceConfigurationRequest> _serviceConfigurationRequests = new();
    private IHasCSharpStatements? _configureServicesBlock;

    public AppStartupFile(StartupTemplate template)
    {
        _template = template;
        _template.FulfillsRole(IAppStartupTemplate.RoleName);
        _cSharpFile = _template.CSharpFile;

        // AWS Lambda uses HostApplicationBuilder pattern
        _serviceConfigurationContext = new ServiceConfigurationContext("configuration", "hostBuilder.Services");

        var isBuilt = false;
        template.ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(request =>
        {
            if (isBuilt)
            {
                return;
            }

            _serviceConfigurationRequests.Add(request);
        });

        template.ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(request =>
        {
            if (isBuilt)
            {
                return;
            }

            _containerRegistrationRequests.Add(request);
        });

        _cSharpFile
            .OnBuild(_ =>
            {
                ConfigureServices((statements, _) =>
                {
                    // The AWS Lambda startup uses a ConfigureHostBuilder method
                    // Service configuration happens within this method
                });

                foreach (var request in _serviceConfigurationRequests)
                {
                    ProcessServiceConfigurationRequest(request);
                }

                foreach (var request in _containerRegistrationRequests)
                {
                    ProcessContainerRegistrationRequest(request);
                }

                isBuilt = true;

                // Create new subscriptions for requests that come after build
                template.ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(ProcessServiceConfigurationRequest);
                template.ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(ProcessContainerRegistrationRequest);
            }, int.MinValue + 1);
    }

    public IAppStartupFile ConfigureServices(Action<IHasCSharpStatements, IServiceConfigurationContext> configure)
    {
        if (_configureServicesBlock == null)
        {
            // AWS Lambda uses the ConfigureHostBuilder method for service configuration
            _configureServicesBlock = _cSharpFile.Classes.Single().Methods.First(x => x.Name == "ConfigureHostBuilder");
        }

        configure(_configureServicesBlock, _serviceConfigurationContext);

        return this;
    }

    public IAppStartupFile ConfigureApp(Action<IHasCSharpStatements, IAppConfigurationContext> configure)
    {
        // Instead of breaking, we'll just ignore attempts to add app configuration.
        // AWS Lambda Functions do not support application pipeline configuration (app.Use* methods).
        return this;
    }

    public IAppStartupFile ConfigureEndpoints(Action<IHasCSharpStatements?, IEndpointConfigurationContext> configure)
    {
        // Instead of breaking, we'll just ignore attempts to add app configuration.
        // AWS Lambda Functions do not support application pipeline configuration (app.Use* methods).
        return this;
    }

    public IAppStartupFile AddMethod(string returnType, string name, Action<IStartupMethod> configure, int? priority)
    {
        
        return this;
    }

    public IAppStartupFile ExposeProgramClass()
    {
        // AWS Lambda doesn't use Program class pattern, this is a no-op
        return this;
    }

    public IAppStartupFile AddServiceConfiguration<TStatement>(
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

        return this;
    }

    public IAppStartupFile AddServiceConfigurationLambda(
        string methodName,
        IEnumerable<string> parameters,
        ConfigureServiceLambda? configure = null,
        int? priority = null)
    {
        ConfigureServices((statements, context) =>
        {
            var invocationStatement = (CSharpInvocationStatement?)statements.Statements.FirstOrDefault(x =>
                x.TryGetMetadata<string>("lambda-registration-for", out var name) &&
                name == methodName);
            var startupLambda = invocationStatement?.Statements.OfType<StartupLambda>().First();

            if (startupLambda == null)
            {
                startupLambda = new StartupLambda(parameters);

                invocationStatement = new CSharpInvocationStatement($"{context.Services}.{methodName}");
                invocationStatement.AddMetadata("lambda-registration-for", methodName);
                invocationStatement.AddArgument(startupLambda);

                Insert(statements, invocationStatement, StatementType.ConfigureServices, priority);
            }
            else
            {
                startupLambda.AddParameters(parameters.Skip(startupLambda.Parameters.Count));
            }

            configure?.Invoke(
                statement: invocationStatement,
                lambdaStatement: startupLambda,
                context: new ServiceConfigurationLambdaContext(
                    Configuration: context.Configuration,
                    Services: context.Services,
                    Parameters: startupLambda.Parameters));
        });

        return this;
    }

    public IAppStartupFile AddContainerRegistration<TStatement>(
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

        return this;
    }

    public IAppStartupFile AddContainerRegistrationLambda(
        string methodName,
        IEnumerable<string> parameters,
        ConfigureServiceLambda? configure = null,
        int? priority = null)
    {
        ConfigureServices((statements, context) =>
        {
            var invocationStatement = (CSharpInvocationStatement?)statements.Statements.FirstOrDefault(x =>
                x.TryGetMetadata<string>("lambda-registration-for", out var name) &&
                name == methodName);
            var startupLambda = invocationStatement?.Statements.OfType<StartupLambda>().First();

            if (startupLambda == null)
            {
                startupLambda = new StartupLambda(parameters);

                invocationStatement = new CSharpInvocationStatement($"{context.Services}.{methodName}");
                invocationStatement.AddMetadata("lambda-registration-for", methodName);
                invocationStatement.AddArgument(startupLambda);

                Insert(statements, invocationStatement, StatementType.ContainerRegistration, priority);
            }
            else
            {
                startupLambda.AddParameters(parameters.Skip(startupLambda.Parameters.Count));
            }

            configure?.Invoke(
                statement: invocationStatement,
                lambdaStatement: startupLambda,
                context: new ServiceConfigurationLambdaContext(
                    Configuration: context.Configuration,
                    Services: context.Services,
                    Parameters: startupLambda.Parameters));
        });

        return this;
    }

    public IAppStartupFile AddAppConfiguration<TStatement>(
        Func<IAppConfigurationContext, TStatement> create,
        ConfigureAppStatement<TStatement>? configure = null,
        int? priority = null) where TStatement : CSharpStatement
    {
        // Instead of breaking, we'll just ignore attempts to add app configuration.
        // AWS Lambda Functions do not support application pipeline configuration (app.Use* methods).
        return this;
    }

    public IAppStartupFile AddAppConfigurationLambda(
        string methodName,
        IEnumerable<string> parameters,
        ConfigureAppLambda? configure = null,
        int? priority = null)
    {
        // Instead of breaking, we'll just ignore attempts to add app configuration.
        // AWS Lambda Functions do not support application pipeline configuration (app.Use* methods).
        return this;
    }

    public IAppStartupFile AddUseEndpointsStatement<TStatement>(
        Func<IEndpointConfigurationContext, TStatement> create,
        ConfigureEndpointStatement<TStatement>? configure = null,
        int? priority = null) where TStatement : CSharpStatement
    {
        // Instead of breaking, we'll just ignore attempts to add app configuration.
        // AWS Lambda Functions do not support application pipeline configuration (app.Use* methods).
        return this;
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

    private void ProcessServiceConfigurationRequest(ServiceConfigurationRequest request)
    {
        if (request.IsHandled)
        {
            return;
        }

        foreach (var dependency in request.TemplateDependencies)
        {
            var template = _template.GetTemplate<IClassProvider>(dependency);
            if (template != null)
            {
                _template.AddUsing(template.Namespace);
            }
            _template.AddTemplateDependency(dependency);
        }

        foreach (var @namespace in request.RequiredNamespaces)
        {
            _template.AddUsing(@namespace);
        }

        AddServiceConfiguration<CSharpStatement>(ctx =>
        {
            var parameters = request.ExtensionMethodParameterList?.Any() == true
                ? GetExtensionMethodParameterList(request)
                : string.Empty;

            return new CSharpStatement($"{ctx.Services}.{request.ExtensionMethodName}({parameters});");
        }, priority: request.Priority);

        request.MarkAsHandled();
    }

    private void ProcessContainerRegistrationRequest(ContainerRegistrationRequest request)
    {
        if (request.IsHandled)
        {
            return;
        }

        foreach (var dependency in request.TemplateDependencies)
        {
            var template = _template.GetTemplate<IClassProvider>(dependency);
            if (template != null)
            {
                _template.AddUsing(template.Namespace);
            }
            _template.AddTemplateDependency(dependency);
        }

        foreach (var @namespace in request.RequiredNamespaces)
        {
            _template.AddUsing(@namespace);
        }

        AddContainerRegistration<CSharpStatement>(ctx =>
        {
            string UseTypeOf(string type)
            {
                return type.Contains('<') || type.Contains("[]") ? $"typeof({type})" : $"typeof({type})";
            }

            static string RegistrationType(ContainerRegistrationRequest registration)
            {
                return registration.InterfaceType != null
                    ? $"{registration.ConcreteType}, {registration.InterfaceType}"
                    : registration.ConcreteType;
            }

            return new CSharpStatement($"{ctx.Services}.Add{request.Lifetime}({UseTypeOf(RegistrationType(request))});");
        }, priority: request.Priority);

        request.MarkAsHandled();
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

    private class StartupLambda : CSharpLambdaBlock
    {
        private readonly List<string> _parameters = new();

        public StartupLambda(IEnumerable<string> parameters) : base(string.Empty)
        {
            AddParameters(parameters);
        }

        public IReadOnlyList<string> Parameters => _parameters;

        public void AddParameters(IEnumerable<string> parameters)
        {
            _parameters.AddRange(parameters);

            Text = Parameters.Count switch
            {
                < 1 => "()",
                1 => Parameters[0],
                _ => $"({string.Join(", ", Parameters)})"
            };
        }
    }

    private record ServiceConfigurationLambdaContext(string Configuration, string Services, IReadOnlyList<string> Parameters) : IServiceConfigurationLambdaContext;

    private record ServiceConfigurationContext(string Configuration, string Services) : IServiceConfigurationContext;

    private enum StatementType
    {
        ConfigureServices,
        ContainerRegistration
    }
}
