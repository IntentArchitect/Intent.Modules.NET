using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.AspNetCore.Templates.Program;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.AspNetCore.Templates;

using static IAppStartupFile;

internal class AppStartupFile : IAppStartupFile
{
    private readonly CSharpTemplateBase<object> _template;
    private readonly bool _usesMinimalHostingModel;
    private readonly bool _usesTopLevelStatements;
    private readonly IServiceConfigurationContext _serviceConfigurationContext;
    private readonly IAppConfigurationContext _appConfigurationContext;
    private readonly IEndpointConfigurationContext _endpointConfigurationContext;
    private readonly CSharpFile _cSharpFile;
    private readonly List<ContainerRegistrationRequest> _containerRegistrationRequests = new();
    private readonly List<ServiceConfigurationRequest> _serviceConfigurationRequests = new();
    private readonly List<ApplicationBuilderRegistrationRequest> _applicationBuilderRegistrationRequests = new();

    public AppStartupFile(CSharpTemplateBase<object> template)
    {
        if (template is not ICSharpFileBuilderTemplate fileBuilderTemplate)
        {
            throw new Exception($"template must implement {nameof(ICSharpFileBuilderTemplate)}");
        }

        _template = template;
        _template.FulfillsRole(IAppStartupTemplate.RoleName);
        _cSharpFile = fileBuilderTemplate.CSharpFile;

        if (template is ProgramTemplate)
        {
            var netSettings = template.OutputTarget.GetProject()?.InternalElement?.AsCSharpProjectNETModel()?.GetNETSettings();
            _usesMinimalHostingModel = netSettings?.UseMinimalHostingModel() == true;
            _usesTopLevelStatements = netSettings?.UseTopLevelStatements() == true;
        }

        if (_usesMinimalHostingModel)
        {
            _serviceConfigurationContext = new ServiceConfigurationContext("builder.Configuration", "builder.Services");
            _appConfigurationContext = new AppConfigurationContext("builder.Configuration", "app", "app.Environment");
            _endpointConfigurationContext = new EndpointConfigurationContext(
                Configuration: _appConfigurationContext.Configuration,
                App: _appConfigurationContext.App,
                Env: _appConfigurationContext.Env,
                Endpoints: "app");
        }
        else
        {
            _serviceConfigurationContext = new ServiceConfigurationContext("Configuration", "services");
            _appConfigurationContext = new AppConfigurationContext("Configuration", "app", "env");
            _endpointConfigurationContext = new EndpointConfigurationContext(
                Configuration: _appConfigurationContext.Configuration,
                App: _appConfigurationContext.App,
                Env: _appConfigurationContext.Env,
                Endpoints: "endpoints");
        }

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

        template.ExecutionContext.EventDispatcher.Subscribe<ApplicationBuilderRegistrationRequest>(request =>
        {
            if (isBuilt)
            {
                return;
            }

            _applicationBuilderRegistrationRequests.Add(request);
        });

        _cSharpFile
            .OnBuild(_ =>
            {
                ConfigureServices((statements, _) =>
                {
                    if (_usesMinimalHostingModel)
                    {
                        statements
                            .FindStatement(x => x.HasMetadata("is-builder-statement"))
                            .AddMetadata("startup-statement-type", StatementType.ConfigureServices)
                            .AddMetadata("startup-statement-priority", int.MinValue)
                            .AddMetadata("startup-statement-requires-line-after", true);

                        statements
                            .FindStatement(x => x.HasMetadata("is-add-services-to-container-comment"))
                            .AddMetadata("startup-statement-type", StatementType.ContainerRegistration)
                            .AddMetadata("startup-statement-priority", int.MinValue)
                            .AddMetadata("startup-statement-requires-line-after", true);
                    }
                });

                ConfigureApp((statements, _) =>
                {
                    if (_usesMinimalHostingModel)
                    {
                        statements
                            .FindStatement(x => x.HasMetadata("is-configure-request-pipeline-comment"))
                            .AddMetadata("startup-statement-type", StatementType.AppConfiguration)
                            .AddMetadata("startup-statement-priority", int.MinValue);
                    }
                    else
                    {
                        statements
                            .FindStatement(x => x.ToString()!.Contains(".IsDevelopment()"))
                            .AddMetadata("startup-statement-type", StatementType.AppConfiguration)
                            .AddMetadata("startup-statement-priority", int.MinValue);
                    }
                });

                _applicationBuilderRegistrationRequests.Add(ApplicationBuilderRegistrationRequest.ToRegister("UseHttpsRedirection").WithPriority(-30));
                _applicationBuilderRegistrationRequests.Add(ApplicationBuilderRegistrationRequest.ToRegister("UseRouting").WithPriority(-20));
                _applicationBuilderRegistrationRequests.Add(ApplicationBuilderRegistrationRequest.ToRegister("UseAuthorization").WithPriority(-5));

                // To match previous behaviour (and avoid changes for upgrading clients) where would always be generated
                AddAppConfigurationLambdaInternal(
                    "UseEndpoints",
                    new[] { "endpoints" },
                    (_, lambdaStatement, _) =>
                    {
                        // Would previously create an empty line in the block, so we replicate that, but remove the empty line
                        // if anything else gets added.
                        var cSharpStatement = new CSharpStatement(string.Empty).AddMetadata("is-startup-file-placeholder", true);
                        lambdaStatement.AddStatement(cSharpStatement);

                        _cSharpFile.AfterBuild(_ =>
                        {
                            if (lambdaStatement.Statements.Any(x => !Equals(x, cSharpStatement)))
                            {
                                cSharpStatement.Remove();
                            }

                            // For simplicity while things are in progress we keep all the endpoint statements in their own
                            // block and now at the last moment we move them out:
                            if (_usesMinimalHostingModel)
                            {
                                var lambdaParent = ((CSharpStatement)lambdaStatement.Parent);

                                var statements = lambdaStatement.Statements.ToArray();
                                var targetBlock = lambdaParent.Parent;
                                var index = targetBlock.Statements.IndexOf(lambdaParent);
                                targetBlock.InsertStatements(index, statements);
                                lambdaParent.Remove();

                            }
                        }, int.MaxValue);
                    },
                    priority: 0);

                foreach (var request in _serviceConfigurationRequests)
                {
                    ProcessServiceConfigurationRequest(request);
                }

                foreach (var request in _containerRegistrationRequests)
                {
                    ProcessContainerRegistrationRequest(request);
                }

                foreach (var request in _applicationBuilderRegistrationRequests)
                {
                    ProcessApplicationConfigurationRequest(request);
                }

                isBuilt = true;

                // Our previous subscriptions are configured to only process if "isBuilt" is false
                // and we make new subscriptions below. This is because other templates which are
                // also subscribed may have been (by chance) instantiated after this class,
                // resulting in their handlers being registered after and thus would only get to
                // handle their subscriptions after this class which will read them all as
                // unhandled. So we make new subscriptions which we know will process requests
                // after the other handlers.
                template.ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(ProcessServiceConfigurationRequest);
                template.ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(ProcessContainerRegistrationRequest);
                template.ExecutionContext.EventDispatcher.Subscribe<ApplicationBuilderRegistrationRequest>(ProcessApplicationConfigurationRequest);
            }, int.MinValue + 1);
    }

    public IAppStartupFile ConfigureServices(Action<IHasCSharpStatements, IServiceConfigurationContext> configure)
    {
        IHasCSharpStatements block = (_usesMinimalHostingModel, _usesTopLevelStatements) switch
        {
            (false, false) or (false, true) => _cSharpFile.Classes.Single().Methods.First(x => x.Name == "ConfigureServices"),
            (true, false) => _cSharpFile.Classes.Single().Methods.First(x => x.Name == "Main"),
            (true, true) => _cSharpFile.TopLevelStatements
        };

        configure(block, _serviceConfigurationContext);

        return this;
    }

    public IAppStartupFile ConfigureApp(Action<IHasCSharpStatements, IAppConfigurationContext> configure)
    {
        IHasCSharpStatements block = (_usesMinimalHostingModel, _usesTopLevelStatements) switch
        {
            (false, false) or (false, true) => _cSharpFile.Classes.Single().Methods.First(x => x.Name == "Configure"),
            (true, false) => _cSharpFile.Classes.Single().Methods.First(x => x.Name == "Main"),
            (true, true) => _cSharpFile.TopLevelStatements
        };

        configure(block, _appConfigurationContext);

        return this;
    }

    public IAppStartupFile ConfigureEndpoints(Action<IHasCSharpStatements, IEndpointConfigurationContext> configure)
    {
        ConfigureApp((statements, _) =>
        {
            var useEndPointsStatement = (CSharpInvocationStatement)statements
                .FindStatement(x => x.TryGetMetadata<string>("lambda-registration-for", out var name) &&
                                    name == "UseEndpoints");
            var block = useEndPointsStatement?.Statements.OfType<StartupLambda>().First();

            configure(block, _endpointConfigurationContext);
        });

        return this;
    }

    public IAppStartupFile AddMethod(string returnType, string name, Action<IStartupMethod> configure, int? priority)
    {
        StartupMethod.CreateOn(_template, returnType, name, configure, priority);
        return this;
    }

    public IAppStartupFile AddServiceConfiguration<TStatement>(
        Func<IServiceConfigurationContext, TStatement> create,
        ConfigureServiceStatement<TStatement> configure = null,
        int? priority = null) where TStatement : CSharpStatement
    {
        var statement = create(_serviceConfigurationContext);
        Insert(statement, StatementType.ConfigureServices, priority);
        configure?.Invoke(statement, _serviceConfigurationContext);

        return this;
    }

    public IAppStartupFile AddServiceConfigurationLambda(
        string methodName,
        IEnumerable<string> parameters,
        ConfigureServiceLambda configure = null,
        int? priority = null)
    {
        ConfigureServices((statements, _) =>
        {
            var invocationStatement = (CSharpInvocationStatement)statements.Statements.FirstOrDefault(x =>
                x.TryGetMetadata<string>("lambda-registration-for", out var name) &&
                name == methodName);
            var startupLambda = invocationStatement?.Statements.OfType<StartupLambda>().First();

            if (startupLambda == null)
            {
                startupLambda = new StartupLambda(parameters);

                invocationStatement = new CSharpInvocationStatement($"{_serviceConfigurationContext.Services}.{methodName}");
                invocationStatement.AddMetadata("lambda-registration-for", methodName);
                invocationStatement.AddArgument(startupLambda);

                Insert(invocationStatement, StatementType.ConfigureServices, priority);
            }
            else
            {
                startupLambda.AddParameters(parameters.Skip(startupLambda.Parameters.Count));
            }

            configure?.Invoke(
                statement: invocationStatement,
                lambdaStatement: startupLambda,
                context: new ServiceConfigurationLambdaContext(
                    Configuration: _serviceConfigurationContext.Configuration,
                    Services: _serviceConfigurationContext.Services,
                    Parameters: startupLambda.Parameters));
        });

        return this;
    }

    public IAppStartupFile AddContainerRegistration<TStatement>(
        Func<IServiceConfigurationContext, TStatement> create,
        ConfigureServiceStatement<TStatement> configure = null,
        int? priority = null) where TStatement : CSharpStatement
    {
        var statement = create(_serviceConfigurationContext);
        Insert(statement, StatementType.ContainerRegistration, priority);
        configure?.Invoke(statement, _serviceConfigurationContext);

        return this;
    }

    public IAppStartupFile AddContainerRegistrationLambda(
        string methodName,
        IEnumerable<string> parameters,
        ConfigureServiceLambda configure = null,
        int? priority = null)
    {

        ConfigureServices((statements, _) =>
        {
            var invocationStatement = (CSharpInvocationStatement)statements.Statements.FirstOrDefault(x =>
                x.TryGetMetadata<string>("lambda-registration-for", out var name) &&
                name == methodName);
            var startupLambda = invocationStatement?.Statements.OfType<StartupLambda>().First();

            if (startupLambda == null)
            {
                startupLambda = new StartupLambda(parameters);

                invocationStatement = new CSharpInvocationStatement($"{_serviceConfigurationContext.Services}.{methodName}");
                invocationStatement.AddMetadata("lambda-registration-for", methodName);
                invocationStatement.AddArgument(startupLambda);

                Insert(invocationStatement, StatementType.ContainerRegistration, priority);
            }
            else
            {
                startupLambda.AddParameters(parameters.Skip(startupLambda.Parameters.Count));
            }

            configure?.Invoke(
                statement: invocationStatement,
                lambdaStatement: startupLambda,
                context: new ServiceConfigurationLambdaContext(
                    Configuration: _serviceConfigurationContext.Configuration,
                    Services: _serviceConfigurationContext.Services,
                    Parameters: startupLambda.Parameters));
        });

        return this;
    }

    public IAppStartupFile AddAppConfiguration<TStatement>(
        Func<IAppConfigurationContext, TStatement> create,
        ConfigureAppStatement<TStatement> configure = null,
        int? priority = null) where TStatement : CSharpStatement
    {
        var statement = create(_appConfigurationContext);
        Insert(statement, StatementType.AppConfiguration, priority);
        configure?.Invoke(statement, _appConfigurationContext);

        return this;
    }

    public IAppStartupFile AddAppConfigurationLambda(
        string methodName,
        IEnumerable<string> parameters,
        ConfigureAppLambda configure = null,
        int? priority = null)
    {
        if (methodName == "UseEndpoints")
        {
            throw new InvalidOperationException($"For \"UseEndpoints\", use the {nameof(IAppStartupFile.AddUseEndpointsStatement)} method instead.");
        }

        return AddAppConfigurationLambdaInternal(methodName, parameters, configure, priority);
    }

    private IAppStartupFile AddAppConfigurationLambdaInternal(
        string methodName,
        IEnumerable<string> parameters,
        ConfigureAppLambda configure,
        int? priority)
    {
        ConfigureApp((statements, _) =>
        {
            var invocationStatement = (CSharpInvocationStatement)statements.Statements.FirstOrDefault(x =>
                x.TryGetMetadata<string>("lambda-registration-for", out var name) &&
                name == methodName);
            var startupLambda = invocationStatement?.Statements.OfType<StartupLambda>().First();

            if (startupLambda == null)
            {
                startupLambda = new StartupLambda(parameters);

                invocationStatement = new CSharpInvocationStatement($"{_appConfigurationContext.App}.{methodName}");
                invocationStatement.AddMetadata("lambda-registration-for", methodName);
                invocationStatement.AddArgument(startupLambda);

                Insert(invocationStatement, StatementType.AppConfiguration, priority);
            }
            else
            {
                startupLambda.AddParameters(parameters.Skip(startupLambda.Parameters.Count));
            }

            configure?.Invoke(
                statement: invocationStatement,
                lambdaStatement: startupLambda,
                context: new AppConfigurationLambdaContext(
                    Configuration: _appConfigurationContext.Configuration,
                    App: _appConfigurationContext.App,
                    Env: _appConfigurationContext.Env,
                    Parameters: startupLambda.Parameters));
        });

        return this;
    }

    public IAppStartupFile AddUseEndpointsStatement<TStatement>(
        Func<IEndpointConfigurationContext, TStatement> create,
        ConfigureEndpointStatement<TStatement> configure = null,
        int? priority = null) where TStatement : CSharpStatement
    {
        priority ??= int.MaxValue;

        var statement = create(_endpointConfigurationContext);

        statement.AddMetadata("startup-statement-priority", priority);

        AddAppConfigurationLambdaInternal(
            methodName: "UseEndpoints",
            parameters: new[] { "endpoints" },
            configure: (_, lambdaStatement, _) =>
            {
                var insertBelow = lambdaStatement.Statements
                    .LastOrDefault(x => x.TryGetMetadata<int>("startup-statement-priority", out var metadataPriority)
                        ? metadataPriority <= priority
                        : 0 <= priority);

                if (insertBelow == null)
                {
                    lambdaStatement.InsertStatement(0, statement);
                    return;
                }

                insertBelow.InsertBelow(statement);

                if (insertBelow.HasMetadata("startup-statement-requires-line-after"))
                {
                    statement.SeparatedFromPrevious();
                }
            },
            priority);

        configure?.Invoke(statement, _endpointConfigurationContext);
        return this;
    }

    private void Insert(CSharpStatement statement, StatementType type, int? priority)
    {
        IHasCSharpStatements hasCSharpStatements;
        priority ??= int.MaxValue;

        if (_usesMinimalHostingModel)
        {
            hasCSharpStatements = _usesTopLevelStatements
                ? _cSharpFile.TopLevelStatements
                : _cSharpFile.Classes.First().Methods.First(x => x.Name == "Main");
        }
        else
        {
            var @class = _cSharpFile.Classes.First();
            hasCSharpStatements = @class.Methods.Single(x => x.Name == type switch
            {
                StatementType.ConfigureServices => "ConfigureServices",
                StatementType.ContainerRegistration => "ConfigureServices",
                StatementType.AppConfiguration => "Configure",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            });
        }

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
            var classProvider = _template.GetTemplate<IClassProvider>(dependency);

            _template.AddTemplateDependency(dependency);
            _template.AddUsing(classProvider.Namespace);
        }

        foreach (var @namespace in request.RequiredNamespaces)
        {
            _template.AddUsing(@namespace);
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
                        throw new ArgumentOutOfRangeException(
                            paramName: nameof(request.ExtensionMethodParameterList),
                            actualValue: parameter,
                            message: "Type specified in parameter list is not known or supported");
                }
            }

            return $"{ctx.Services}.{request.ExtensionMethodName}({string.Join(", ", parameterList)});";
        }, priority: request.Priority);
    }

    private void ProcessContainerRegistrationRequest(ContainerRegistrationRequest request)
    {
        if (request.IsHandled)
        {
            return;
        }

        foreach (var dependency in request.TemplateDependencies)
        {
            var classProvider = _template.GetTemplate<IClassProvider>(dependency);

            _template.AddTemplateDependency(dependency);
            _template.AddUsing(classProvider.Namespace);
        }

        foreach (var @namespace in request.RequiredNamespaces)
        {
            _template.AddUsing(@namespace);
        }

        AddContainerRegistration<CSharpStatement>(ctx =>
        {
            return (request.ConcreteType.StartsWith("typeof("), request.InterfaceType != null) switch
            {
                (false, false) => $"{ctx.Services}.{RegistrationType(request)}<{_template.UseType(request.ConcreteType)}>();",
                (false, true) => $"{ctx.Services}.{RegistrationType(request)}<{_template.UseType(request.InterfaceType)}, {_template.UseType(request.ConcreteType)}>();",
                (true, false) => $"{ctx.Services}.{RegistrationType(request)}({UseTypeOf(request.ConcreteType)});",
                (true, true) => $"{ctx.Services}.{RegistrationType(request)}({UseTypeOf(request.InterfaceType)}, {UseTypeOf(request.ConcreteType)});"
            };

            string UseTypeOf(string type)
            {
                var typeName = type.Substring("typeof(".Length, type.Length - "typeof()".Length);
                return $"typeof({_template.UseType(typeName)})";
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

    private void ProcessApplicationConfigurationRequest(ApplicationBuilderRegistrationRequest request)
    {
        foreach (var dependency in request.TemplateDependencies)
        {
            var classProvider = _template.GetTemplate<IClassProvider>(dependency);

            _template.AddTemplateDependency(dependency);
            _template.AddUsing(classProvider.Namespace);
        }

        foreach (var @namespace in request.RequiredNamespaces)
        {
            _template.AddUsing(@namespace);
        }

        AddAppConfiguration<CSharpStatement>(ctx =>
        {
            var parameterList = new List<string>();

            foreach (var parameter in request.ExtensionMethodParameterList)
            {
                switch (parameter)
                {
                    case ApplicationBuilderRegistrationRequest.ParameterType.Configuration:
                        parameterList.Add("Configuration");
                        break;
                    case ApplicationBuilderRegistrationRequest.ParameterType.HostEnvironment:
                    case ApplicationBuilderRegistrationRequest.ParameterType.WebHostEnvironment:
                        parameterList.Add("env");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            paramName: nameof(request.ExtensionMethodParameterList),
                            actualValue: parameter,
                            message: "Type specified in parameter list is not known or supported");
                }
            }

            return $"{ctx.App}.{request.ExtensionMethodName}({string.Join(", ", parameterList)});";
        }, priority: request.Priority);
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

    private record EndpointConfigurationContext(string Configuration, string App, string Env, string Endpoints) : IEndpointConfigurationContext;

    private record ServiceConfigurationLambdaContext(string Configuration, string Services, IReadOnlyList<string> Parameters) : IServiceConfigurationLambdaContext;

    private record AppConfigurationLambdaContext(string Configuration, string App, string Env, IReadOnlyList<string> Parameters) : IAppConfigurationLambdaContext;

    private record ServiceConfigurationContext(string Configuration, string Services) : IServiceConfigurationContext;

    private record AppConfigurationContext(string Configuration, string App, string Env) : IAppConfigurationContext;

    private enum StatementType
    {
        ConfigureServices,
        ContainerRegistration,
        AppConfiguration
    }
}