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
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Consumers;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Producers;
using Intent.Modules.Modelers.Eventing;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class MassTransitConfigurationTemplate : CSharpTemplateBase<object, MassTransitConfigurationDecoratorContract>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Eventing.MassTransit.MassTransitConfiguration";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public MassTransitConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.MassTransit(OutputTarget));

        _messageBroker = GetMessageBroker();
        var messageBrokerDependency = _messageBroker.GetNugetDependency(OutputTarget);
        if (messageBrokerDependency is not null)
        {
            AddNugetDependency(messageBrokerDependency);
        }

        AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
        AddTypeSource(IntegrationCommandTemplate.TemplateId);

        _applicableMessages = GetApplicableMessages();

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Reflection")
            .AddUsing("MassTransit")
            .AddUsing("MassTransit.Configuration")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .OnBuild(file =>
            {
                _consumers = GetConsumers();
                _producers = GetProducers();
            })
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
    private IReadOnlyCollection<Consumer> _consumers;
    private IReadOnlyCollection<Producer> _producers;
    private readonly IReadOnlyList<MessageModel> _applicableMessages;

    private MessageBrokerBase GetMessageBroker()
    {
        var messageBrokerSetting = this.ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum();
        return messageBrokerSetting switch
        {
            EventingSettings.MessagingServiceProviderOptionsEnum.InMemory => new InMemoryMessageBroker(this),
            EventingSettings.MessagingServiceProviderOptionsEnum.Rabbitmq => new RabbitMqMessageBroker(this),
            EventingSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus => new AzureServiceBusMessageBroker(this),
            EventingSettings.MessagingServiceProviderOptionsEnum.AmazonSqs => new AmazonSqsMessageBroker(this),
            _ => throw new InvalidOperationException(
                $"Messaging Service Provider is set to a setting that is not supported: {messageBrokerSetting}")
        };
    }

    private IReadOnlyCollection<Consumer> GetConsumers()
    {
        var consumers = new IConsumerFactory[]
            {
                new LegacyEventingConsumerFactory(this),
                new ServiceIntegrationEventingConsumerFactory(this),
                new ServiceIntegrationCommandConsumerFactory(this),
            }
            .Concat(GetDecorators().SelectMany(decorator => decorator.GetConsumerFactories() ?? Enumerable.Empty<IConsumerFactory>()))
            .SelectMany(factory => factory.CreateConsumers())
            .ToArray();
        return consumers;
    }

    private IReadOnlyCollection<Producer> GetProducers()
    {
        var producers = new IProducerFactory[]
            {
                new ServiceIntegrationCommandSendProducerFactory(this)
            }
            .Concat(GetDecorators().SelectMany(decorator => decorator.GetProducerFactories() ?? Enumerable.Empty<IProducerFactory>()))
            .SelectMany(factory => factory.CreateProducers())
            .ToArray();
        return producers;
    }

    private IReadOnlyList<MessageModel> GetApplicableMessages()
    {
        var eventApplications = ExecutionContext.MetadataManager.Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels().ToList();
        var legacySubscriptionMessages = eventApplications.SelectMany(x => x.SubscribedMessages())
            .Select(x => x.TypeReference.Element.AsMessageModel());
        var legacyPublishMessages = eventApplications.SelectMany(x => x.PublishedMessages())
            .Select(x => x.TypeReference.Element.AsMessageModel());

        var serviceIntegrationMessages = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id)
            .GetMessageModels()
            .Where(p => p.InternalElement.AssociatedElements.Any());

        var filtered = legacySubscriptionMessages
            .Union(legacyPublishMessages)
            .Union(serviceIntegrationMessages);

        var result = filtered.ToList();
        return result;
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

        yield return new CSharpStatement($"{factoryConfigVarName}.ConfigureEndpoints(context);").AddMetadata("configure-endpoints", true).SeparatedFromPrevious();
        if (ShouldConfigureNonDefaultEndpoints())
        {
            yield return new CSharpStatement($@"{factoryConfigVarName}.ConfigureNonDefaultEndpoints(context);");
        }

        if (ExecutionContext.Settings.GetEventingSettings().OutboxPattern().IsInMemory())
        {
            yield return new CSharpStatement($"{factoryConfigVarName}.UseInMemoryOutbox(context);").AddMetadata("in-memory-outbox", true);
        }
        else if (ExecutionContext.Settings.GetEventingSettings().OutboxPattern().IsEntityFramework() &&
                 ExecutionContext.GetApplicationConfig().Modules.All(p => p.ModuleId != "Intent.Eventing.MassTransit.EntityFrameworkCore"))
        {
            Logging.Log.Warning("Please install Intent.Eventing.MassTransit.EntityFrameworkCore module for the Outbox pattern to persist to the database");
        }

        if (ShouldConfigureMessageTopologies())
        {
            yield return new CSharpStatement($"{factoryConfigVarName}.AddMessageTopologyConfiguration();");
        }

        if (ShouldAddReceiveEndpoints())
        {
            yield return new CSharpStatement($"{factoryConfigVarName}.AddReceiveEndpoints(context);");
        }

        if (_producers.Any())
        {
            yield return new CSharpStatement("EndpointConventionRegistration();");
        }
    }

    private bool ShouldConfigureMessageTopologies()
    {
        return _applicableMessages.Any(ShouldConfigureMessageTopology);
    }

    private bool ShouldConfigureMessageTopology(MessageModel message)
    {
        return HasMessageTopologySettings(message);
    }

    private static bool HasMessageTopologySettings(MessageModel message)
    {
        return message.HasMessageTopologySettings() && !string.IsNullOrWhiteSpace(message.GetMessageTopologySettings().EntityName());
    }

    private bool ShouldConfigureNonDefaultEndpoints()
    {
        return _consumers.Any(ShouldConfigureNonDefaultEndpoint);
    }

    private static bool ShouldConfigureNonDefaultEndpoint(Consumer consumer)
    {
        return consumer.Settings.RabbitMqConsumerSettings is not null ||
               consumer.Settings.AzureConsumerSettings is not null;
    }

    private bool ShouldAddReceiveEndpoints()
    {
        return _consumers.Any(ShouldAddReceiveEndpoint);
    }

    private static bool ShouldAddReceiveEndpoint(Consumer consumer)
    {
        return consumer.IsSpecificMessageConsumer;
    }

    private void AddMessageTopologyConfiguration(CSharpClass @class)
    {
        if (!ShouldConfigureMessageTopologies())
        {
            return;
        }

        @class.AddMethod("void", "AddMessageTopologyConfiguration", method =>
        {
            method.Private().Static();
            method.AddParameter(_messageBroker.GetMessageBrokerBusFactoryConfiguratorName(), "cfg", param => param.WithThisModifier());

            foreach (var messageModel in _applicableMessages)
            {
                if (HasMessageTopologySettings(messageModel))
                {
                    method.AddStatement(
                        $@"cfg.Message<{GetTypeName(IntegrationEventMessageTemplate.TemplateId, messageModel)}>(x => x.SetEntityName(""{messageModel.GetMessageTopologySettings().EntityName()}""));");
                }
            }
        });
    }

    private void AddConsumers(CSharpClass @class)
    {
        @class.AddMethod("void", "AddConsumers", method =>
        {
            method.Private().Static();
            method.AddParameter("IRegistrationConfigurator", "cfg", param => param.WithThisModifier());
            method.AddStatements(_consumers.Select(consumer => GetAddConsumerStatement("cfg", consumer)));
        });
    }

    private CSharpStatement GetAddConsumerStatement(string configParamName, Consumer consumer)
    {
        var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace("_", "-").Replace(" ", "-")
            .Replace(".", "-");
        // Until we can do single-line method chaining this will have to do for now...
        var addConsumer = $"{configParamName}.AddConsumer<{consumer.ConsumerTypeName}>"
                          + $"(typeof({consumer.ConsumerDefinitionTypeName}))";

        if (ShouldConfigureNonDefaultEndpoint(consumer))
        {
            addConsumer += $@".ExcludeFromConfigureEndpoints()";
        }
        else
        {
            if (consumer.IsSpecificMessageConsumer)
            {
                addConsumer += $@".ExcludeFromConfigureEndpoints()";
            }
            else
            {
                addConsumer += $@".Endpoint(config => config.InstanceId = ""{sanitizedAppName}"")";
            }
        }

        addConsumer += ";";
        return addConsumer;
    }

    private void AddReceivedEndpointsForCommandSubscriptions(CSharpClass @class)
    {
        if (!ShouldAddReceiveEndpoints())
        {
            return;
        }

        @class.AddMethod("void", "AddReceiveEndpoints", method =>
        {
            method.Private().Static();
            method.AddParameter(_messageBroker.GetMessageBrokerBusFactoryConfiguratorName(), "cfg", param => param.WithThisModifier());
            method.AddParameter("IBusRegistrationContext", "context");

            foreach (var consumerGroup in _consumers.Where(ShouldAddReceiveEndpoint).GroupBy(key =>
                     {
                         var destinationAddress = key.DestinationAddress;
                         if (string.IsNullOrWhiteSpace(destinationAddress))
                         {
                             destinationAddress = $"{key.Message.MessageTypeFullName.ToKebabCase()}";
                         }

                         return destinationAddress;
                     }))
            {
                method.AddInvocationStatement("cfg.ReceiveEndpoint", caller =>
                {
                    caller.AddArgument($@"""{consumerGroup.Key}""");
                    var lambda = new CSharpLambdaBlock("e");
                    caller.AddArgument(lambda);

                    lambda.AddStatement("e.ConfigureConsumeTopology = false;");

                    lambda.AddStatements(consumerGroup.Select(consumer => $"e.Consumer<{consumer.ConsumerTypeName}>(context);"));
                });
            }
        });
    }

    private void AddEndpointConventionRegistrations(CSharpClass @class)
    {
        if (!_producers.Any())
        {
            return;
        }

        @class.AddMethod("void", "EndpointConventionRegistration", method =>
        {
            method.Private().Static();

            method.AddStatements(_producers.Select(producer =>
                $@"EndpointConvention.Map<{producer.MessageTypeName}>(new Uri(""{producer.Urn}""));"));
        });
    }

    private void AddNonDefaultEndpointConfigurationMethods(CSharpClass @class)
    {
        if (!ShouldConfigureNonDefaultEndpoints())
        {
            return;
        }

        bool hasGeneratedConfigStatements = false;

        @class.AddMethod("void", "ConfigureNonDefaultEndpoints", method =>
        {
            method.Private().Static();
            method.AddParameter(_messageBroker.GetMessageBrokerBusFactoryConfiguratorName(), "cfg", param => param.WithThisModifier());
            method.AddParameter("IBusRegistrationContext", "context");

            foreach (var consumer in _consumers.Where(ShouldConfigureNonDefaultEndpoint))
            {
                var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace("_", "-").Replace(" ", "-")
                    .Replace(".", "-");

                var statements = _messageBroker.GetCustomConfigurationStatements(consumer, sanitizedAppName).ToList();
                hasGeneratedConfigStatements |= statements.Count != 0;
                method.AddStatements(statements);
            }
        });

        if (!hasGeneratedConfigStatements)
        {
            return;
        }

        foreach (var helperMethod in _messageBroker.GetCustomConfigurationHelperMethods(@class))
        {
            @class.Methods.Add(helperMethod);
        }
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
                .AddArgument(new CSharpLambdaBlock("r").WithExpressionBody(retry))
                .SeparatedFromPrevious();
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