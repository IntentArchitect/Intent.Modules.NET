using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.MassTransit.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class MassTransitConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Eventing.MassTransit.MassTransitConfiguration";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public MassTransitConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NuGetPackages.MassTransit);

        _messageBroker = MessageBrokerFactory.GetMessageBroker(this);
        var messageBrokerDependency = _messageBroker.GetNugetDependency();
        if (messageBrokerDependency is not null)
        {
            AddNugetDependency(messageBrokerDependency);
        }

        AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
        AddTypeSource(IntegrationCommandTemplate.TemplateId);


        _eventApplications = ExecutionContext.MetadataManager.Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels().ToList();
        _messagesWithSettings = GetMessagesWithSettings();
        _eventSubscriptions = GetEventSubscriptions();
        _commandSubscriptions = GetIntegrationEventHandlerCommandSubscriptions();
        _commandSendDispatches = GetIntegrationCommandSendDispatches();

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Reflection")
            .AddUsing("MassTransit")
            .AddUsing("MassTransit.Configuration")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddClass($"MassTransitConfiguration", @class =>
            {
                @class.Static();
                @class.AddMethod("void", "AddMassTransitConfiguration", method =>
                {
                    method.Static();
                    method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                    method.AddParameter("IConfiguration", "configuration");
                    method.AddStatements(GetContainerRegistrationStatements());
                    method.AddInvocationStatement("services.AddMassTransit", stmt => stmt
                        .AddArgument(GetConfigurationForAddMassTransit("configuration"))
                        .AddMetadata("configure-masstransit", true)
                        .SeparatedFromPrevious());
                });
                AddMessageTopologyConfiguration(@class);
                AddConsumers(@class);
                AddReceivedEndpointsForCommandSubscriptions(@class);
                AddEndpointConventionRegistrations(@class);
                AddNonDefaultEndpointConfigurationMethods(@class);
            });
    }


    private readonly MessageBrokerBase _messageBroker;
    private readonly IReadOnlyList<ApplicationModel> _eventApplications;
    private readonly IReadOnlyList<MessageModel> _messagesWithSettings;
    private readonly IReadOnlyList<Subscription> _eventSubscriptions;
    private readonly IReadOnlyList<SendIntegrationCommandTargetEndModel> _commandSendDispatches;
    private readonly IReadOnlyList<SubscribeIntegrationCommandTargetEndModel> _commandSubscriptions;

    private IReadOnlyList<MessageModel> GetMessagesWithSettings()
    {
        return _eventApplications.SelectMany(x => x.SubscribedMessages())
            .Select(x => x.TypeReference.Element.AsMessageModel())
            .Union(_eventApplications.SelectMany(x => x.PublishedMessages())
                .Select(x => x.TypeReference.Element.AsMessageModel()))
            .Where(p => p.HasMessageTopologySettings() && !string.IsNullOrWhiteSpace(p.GetMessageTopologySettings().EntityName()))
            .ToList();
    }

    private IReadOnlyList<Subscription> GetEventSubscriptions()
    {
        return _eventApplications
            .SelectMany(x => x.SubscribedMessages())
            .Select(s => new Subscription(s.TypeReference.Element.AsMessageModel(), s.GetAzureServiceBusConsumerSettings(), s.GetRabbitMQConsumerSettings()))
            .Concat(
                ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                    .SelectMany(x => x.IntegrationEventSubscriptions())
                    .Select(s => new Subscription(s.TypeReference.Element.AsMessageModel(), s.GetAzureServiceBusConsumerSettings(), s.GetRabbitMQConsumerSettings()))
            )
            .ToList();
    }
    
    private IReadOnlyList<SubscribeIntegrationCommandTargetEndModel> GetIntegrationEventHandlerCommandSubscriptions()
    {
        return ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
            .SelectMany(x => x.IntegrationCommandSubscriptions())
            .ToList();
    }

    private IReadOnlyList<SendIntegrationCommandTargetEndModel> GetIntegrationCommandSendDispatches()
    {
        return ExecutionContext.MetadataManager.GetExplicitlySentIntegrationCommandDispatches(ExecutionContext.GetApplicationConfig().Id).ToList();
    }

    private IEnumerable<CSharpStatement> GetContainerRegistrationStatements()
    {
        var statements = new List<CSharpStatement>();

        statements.Add($@"services.AddScoped<{this.GetMassTransitEventBusName()}>();");
        statements.Add($@"services.AddScoped<{this.GetEventBusInterfaceName()}>(provider => provider.GetRequiredService<{this.GetMassTransitEventBusName()}>());");

        return statements;
    }

    private CSharpLambdaBlock GetConfigurationForAddMassTransit(string configurationVarName)
    {
        var block = new CSharpLambdaBlock("x")
            .AddStatement($"x.SetKebabCaseEndpointNameFormatter();")
            .AddStatement($"x.AddConsumers();");

        block.AddStatement(_messageBroker.AddMessageBrokerConfiguration(
            busRegistrationVarName: "x",
            factoryConfigVarName: "cfg",
            moreConfiguration: GetPostHostConfigurationStatements("cfg", configurationVarName)));

        if (ExecutionContext.Settings.GetEventingSettings().OutboxPattern().IsInMemory())
        {
            block.AddStatement("x.AddInMemoryInboxOutbox();");
        }

        return block;
    }

    private IEnumerable<CSharpStatement> GetPostHostConfigurationStatements(string factoryConfigVarName, string configurationVarName)
    {
        yield return GetMessageRetryStatement(factoryConfigVarName, configurationVarName);
        
        yield return new CSharpStatement($"{factoryConfigVarName}.ConfigureEndpoints(context);").AddMetadata("configure-endpoints", true);
        if (_eventSubscriptions.Any(_messageBroker.HasMessageBrokerStereotype))
        {
            yield return new CSharpStatement($@"{factoryConfigVarName}.ConfigureNonDefaultEndpoints(context);");
        }

        if (ExecutionContext.Settings.GetEventingSettings().OutboxPattern().IsInMemory())
        {
            yield return new CSharpStatement($"{factoryConfigVarName}.UseInMemoryOutbox(context);");
        }
        else if (ExecutionContext.Settings.GetEventingSettings().OutboxPattern().IsEntityFramework() &&
                 ExecutionContext.GetApplicationConfig().Modules.All(p => p.ModuleId != "Intent.Eventing.MassTransit.EntityFrameworkCore"))
        {
            Logging.Log.Warning("Please install Intent.Eventing.MassTransit.EntityFrameworkCore module for the Outbox pattern to persist to the database");
        }

        if (_messagesWithSettings.Any())
        {
            yield return new CSharpStatement($"{factoryConfigVarName}.AddMessageTopologyConfiguration();");
        }

        if (_commandSubscriptions.Any())
        {
            yield return new CSharpStatement($"{factoryConfigVarName}.AddReceiveEndpoints(context);");
        }

        if (_commandSendDispatches.Any())
        {
            yield return new CSharpStatement("EndpointConventionRegistration();");
        }
    }

    private void AddMessageTopologyConfiguration(CSharpClass @class)
    {
        if (!_messagesWithSettings.Any())
        {
            return;
        }

        @class.AddMethod("void", "AddMessageTopologyConfiguration", method =>
        {
            method.Private().Static();
            method.AddParameter(_messageBroker.GetMessageBrokerBusFactoryConfiguratorName(), "cfg", param => param.WithThisModifier());
            method.AddStatements(_messagesWithSettings.Select(messageModel =>
                $@"cfg.Message<{GetTypeName(IntegrationEventMessageTemplate.TemplateId, messageModel)}>(x => x.SetEntityName(""{messageModel.GetMessageTopologySettings().EntityName()}""));"));
        });
    }

    private void AddConsumers(CSharpClass @class)
    {
        @class.AddMethod("void", "AddConsumers", method =>
        {
            method.Private().Static();
            method.AddParameter("IRegistrationConfigurator", "cfg", param => param.WithThisModifier());
            foreach (var subscription in _eventSubscriptions)
            {
                method.AddStatement(GetAddConsumerStatement("cfg", subscription.Message, _messageBroker.HasMessageBrokerStereotype(subscription)));
            }

            foreach (var subscription in _commandSubscriptions)
            {
                method.AddStatement(GetAddConsumerStatement("cfg", subscription.TypeReference.Element.AsIntegrationCommandModel()));
            }
        });
    }

    private CSharpStatement GetAddConsumerStatement(string configParamName, MessageModel message, bool excludeFromConfigureEndpoints = false)
    {
        var messageName =
            this.GetIntegrationEventMessageName(message);
        var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace("_", "-").Replace(" ", "-")
            .Replace(".", "-");
        var consumerDefinitionType =
            $@"{this.GetIntegrationEventHandlerInterfaceName()}<{messageName}>, {messageName}";
        var consumerWrapperType = $@"{this.GetIntegrationEventConsumerName()}<{consumerDefinitionType}>";

        // Until we can do single-line method chaining this will have to do for now...
        var addConsumer = $@"{configParamName}.AddConsumer<{consumerWrapperType}>"
                          + $@"(typeof({this.GetIntegrationEventConsumerName()}Definition<{consumerDefinitionType}>))";

        if (excludeFromConfigureEndpoints)
        {
            addConsumer += $@".ExcludeFromConfigureEndpoints()";
        }
        else
        {
            addConsumer += $@".Endpoint(config => config.InstanceId = ""{sanitizedAppName}"")";
        }

        addConsumer += ";";
        return addConsumer;
    }

    private CSharpStatement GetAddConsumerStatement(string configParamName, IntegrationCommandModel integrationCommandModel)
    {
        var commandName = this.GetIntegrationCommandName(integrationCommandModel);
        var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace("_", "-").Replace(" ", "-")
            .Replace(".", "-");
        var consumerDefinitionType =
            $@"{this.GetIntegrationEventHandlerInterfaceName()}<{commandName}>, {commandName}";
        var consumerType = $@"{this.GetIntegrationEventConsumerName()}<{consumerDefinitionType}>";

        return
            $@"{configParamName}.AddConsumer<{consumerType}>(typeof({this.GetIntegrationEventConsumerName()}Definition<{consumerDefinitionType}>)).Endpoint(config => {{ config.InstanceId = ""{sanitizedAppName}""; config.ConfigureConsumeTopology = false; }});";
    }

    private void AddReceivedEndpointsForCommandSubscriptions(CSharpClass @class)
    {
        if (!_commandSubscriptions.Any())
        {
            return;
        }

        @class.AddMethod("void", "AddReceiveEndpoints", method =>
        {
            method.Private().Static();
            method.AddParameter(_messageBroker.GetMessageBrokerBusFactoryConfiguratorName(), "cfg", param => param.WithThisModifier());
            method.AddParameter("IBusRegistrationContext", "context");

            foreach (var subscription in _commandSubscriptions.GroupBy(key =>
                     {
                         var model = key.TypeReference.Element.AsIntegrationCommandModel();
                         var destinationAddress = key.GetCommandConsumption().QueueName();
                         if (string.IsNullOrWhiteSpace(destinationAddress))
                         {
                             destinationAddress = $"{this.GetFullyQualifiedTypeName(model.InternalElement).ToKebabCase()}";
                         }

                         return destinationAddress;
                     }))
            {
                method.AddInvocationStatement("cfg.ReceiveEndpoint", caller =>
                {
                    caller.AddArgument($@"""{subscription.Key}""");
                    var lambda = new CSharpLambdaBlock("e");
                    caller.AddArgument(lambda);

                    lambda.AddStatement("e.ConfigureConsumeTopology = false;");

                    foreach (var subscriber in subscription)
                    {
                        var model = subscriber.TypeReference.Element.AsIntegrationCommandModel();
                        lambda.AddStatement(
                            $"e.Consumer<{this.GetIntegrationEventConsumerName()}<IIntegrationEventHandler<{this.GetIntegrationCommandName(model)}>, {this.GetIntegrationCommandName(model)}>>(context);");
                    }
                });
            }
        });
    }

    private void AddEndpointConventionRegistrations(CSharpClass @class)
    {
        if (!_commandSendDispatches.Any())
        {
            return;
        }

        @class.AddMethod("void", "EndpointConventionRegistration", method =>
        {
            method.Private().Static();

            foreach (var dispatchModel in _commandSendDispatches)
            {
                var model = dispatchModel.TypeReference.Element.AsIntegrationCommandModel();
                var queueName = dispatchModel.GetCommandDistribution().DestinationQueueName();
                if (string.IsNullOrWhiteSpace(queueName))
                {
                    queueName = $"{this.GetFullyQualifiedTypeName(model.InternalElement).ToKebabCase()}";
                }

                method.AddStatement($@"EndpointConvention.Map<{GetTypeName(IntegrationCommandTemplate.TemplateId, model)}>(new Uri(""queue:{queueName}""));");
            }
        });
    }

    private void AddNonDefaultEndpointConfigurationMethods(CSharpClass @class)
    {
        if (!_eventSubscriptions.Any(_messageBroker.HasMessageBrokerStereotype))
        {
            return;
        }

        @class.AddMethod("void", "ConfigureNonDefaultEndpoints", method =>
        {
            method.Private().Static();
            method.AddParameter(_messageBroker.GetMessageBrokerBusFactoryConfiguratorName(), "cfg", parm => parm.WithThisModifier());
            method.AddParameter("IBusRegistrationContext", "context");

            foreach (var subscription in _eventSubscriptions)
            {
                var messageName =
                    this.GetIntegrationEventMessageName(subscription.Message);
                var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace("_", "-").Replace(" ", "-")
                    .Replace(".", "-");
                var consumerDefinitionType =
                    $@"{this.GetIntegrationEventHandlerInterfaceName()}<{messageName}>, {messageName}";
                var consumerWrapperType = $@"{this.GetIntegrationEventConsumerName()}<{consumerDefinitionType}>";

                method.AddInvocationStatement($"cfg.AddCustomConsumerEndpoint<{consumerWrapperType}>", inv => inv
                    .AddArgument("context")
                    .AddArgument($@"""{sanitizedAppName}""")
                    .AddArgument(new CSharpLambdaBlock("endpoint")
                        .AddStatements(_messageBroker.AddBespokeConsumerConfigurationStatements("endpoint", subscription))
                    )
                    .WithArgumentsOnNewLines());
            }
        });

        @class.AddMethod("void", "AddCustomConsumerEndpoint", method =>
        {
            method.Private().Static();
            method.AddGenericParameter("TConsumer", out var tConsumer);
            method.AddGenericTypeConstraint(tConsumer, c => c.AddType("class").AddType("IConsumer"));
            method.AddParameter(_messageBroker.GetMessageBrokerBusFactoryConfiguratorName(), "cfg", param => param.WithThisModifier());
            method.AddParameter("IBusRegistrationContext", "context");
            method.AddParameter("string", "instanceId");
            method.AddParameter($"Action<{_messageBroker.GetMessageBrokerReceiveEndpointConfiguratorName()}>", "configuration");

            method.AddInvocationStatement($"cfg.ReceiveEndpoint", stmt => stmt
                .AddArgument(new CSharpInvocationStatement($"new ConsumerEndpointDefinition<{tConsumer}>")
                    .WithoutSemicolon()
                    .AddArgument(new CSharpObjectInitializerBlock($@"new EndpointSettings<IEndpointDefinition<{tConsumer}>>")
                        .AddInitStatement("InstanceId", "instanceId")))
                .AddArgument("KebabCaseEndpointNameFormatter.Instance")
                .AddArgument(new CSharpLambdaBlock("endpoint")
                    .AddStatement("configuration.Invoke(endpoint);")
                    .AddStatement($"endpoint.ConfigureConsumer<{tConsumer}>(context);"))
                .WithArgumentsOnNewLines());
        });
    }

    private CSharpStatement GetMessageRetryStatement(string configParamName, string configurationVarName)
    {
        return ExecutionContext.Settings.GetEventingSettings().RetryPolicy().AsEnum() switch
        {
            EventingSettings.RetryPolicyOptionsEnum.RetryImmediate => GetCSharpRetryStatement("Immediate",
                ("int", "RetryLimit", "5")),
            EventingSettings.RetryPolicyOptionsEnum.RetryInterval => GetCSharpRetryStatement("Interval",
                ("int", "RetryCount", "10"),
                ("TimeSpan", "Interval", "TimeSpan.FromSeconds(5)")),
            EventingSettings.RetryPolicyOptionsEnum.RetryIncremental => GetCSharpRetryStatement("Incremental",
                ("int", "RetryLimit", "10"),
                ("TimeSpan", "InitialInterval", "TimeSpan.FromSeconds(5)"),
                ("TimeSpan", "IntervalIncrement", "TimeSpan.FromSeconds(5)")),
            EventingSettings.RetryPolicyOptionsEnum.RetryExponential => GetCSharpRetryStatement("Exponential",
                ("int", "RetryLimit", "10"),
                ("TimeSpan", "MinInterval", "TimeSpan.FromSeconds(5)"),
                ("TimeSpan", "MaxInterval", "TimeSpan.FromMinutes(30)"),
                ("TimeSpan", "IntervalDelta", "TimeSpan.FromSeconds(5)")),
            EventingSettings.RetryPolicyOptionsEnum.RetryNone => GetCSharpRetryStatement("None"),
            _ => throw new ArgumentOutOfRangeException()
        };

        CSharpStatement GetCSharpRetryStatement(string methodName, params (string Type, string Name, string DefaultValue)[] args)
        {
            var retry = new CSharpInvocationStatement($"r.{methodName}")
                .WithoutSemicolon()
                .WithArgumentsOnNewLines();

            foreach (var arg in args)
            {
                retry.AddArgument($@"{configurationVarName}.GetValue<{arg.Type}?>(""MassTransit:Retry{methodName}:{arg.Name}"") ?? {arg.DefaultValue}");
            }

            return new CSharpInvocationStatement($@"{configParamName}.UseMessageRetry")
                .AddArgument(new CSharpLambdaBlock("r").WithExpressionBody(retry));
        }
    }

    private void PublishRetryPoliciesAppSettings()
    {
        // Justification: No, there aren't any articles that I could find on the internet
        // that would provide as good defaults, but based on the use cases for each retry
        // policy, I've put together some values that make sense.

        switch (ExecutionContext.Settings.GetEventingSettings().RetryPolicy().AsEnum())
        {
            case EventingSettings.RetryPolicyOptionsEnum.RetryImmediate:
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryImmediate",
                    new
                    {
                        RetryLimit = 5
                    }));
                break;
            case EventingSettings.RetryPolicyOptionsEnum.RetryInterval:
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryInterval",
                    new
                    {
                        RetryCount = 10,
                        Interval = TimeSpan.FromSeconds(5)
                    }));
                break;
            case EventingSettings.RetryPolicyOptionsEnum.RetryIncremental:
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryIncremental",
                    new
                    {
                        RetryLimit = 10,
                        InitialInterval = TimeSpan.FromSeconds(5),
                        IntervalIncrement = TimeSpan.FromSeconds(5)
                    }));
                break;
            case EventingSettings.RetryPolicyOptionsEnum.RetryExponential:
                // I used the MassTransit algo to work out this one.
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryExponential",
                    new
                    {
                        RetryLimit = 10,
                        MinInterval = TimeSpan.FromSeconds(5),
                        MaxInterval = TimeSpan.FromMinutes(30),
                        IntervalDelta = TimeSpan.FromSeconds(5)
                    }));
                break;
            case EventingSettings.RetryPolicyOptionsEnum.RetryNone:
            default:
                break;
        }
    }

    public override void BeforeTemplateExecution()
    {
        ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest.ToRegister(
                "AddMassTransitConfiguration",
                ServiceConfigurationRequest.ParameterType.Configuration)
            .ForConcern("Infrastructure")
            .HasDependency(this));

        PublishRetryPoliciesAppSettings();

        var settings = _messageBroker.GetAppSettings();
        if (settings is not null)
        {
            ExecutionContext.EventDispatcher.Publish(settings);
        }
    }

    [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

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