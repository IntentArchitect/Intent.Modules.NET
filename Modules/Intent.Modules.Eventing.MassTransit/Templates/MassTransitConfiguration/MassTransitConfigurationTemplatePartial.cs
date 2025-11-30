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
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Eventing.Contracts.Settings;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.CompositeMessageBusConfiguration;
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
        FulfillsRole("Eventing.MessageBusConfiguration");
        
        _messageBroker = GetMessageBroker();

        AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
        AddTypeSource(IntegrationCommandTemplate.TemplateId);

        _applicableMessages = GetApplicableMessages();

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Reflection")
            .AddUsing("MassTransit")
            .AddUsing("MassTransit.Configuration")
            .AddUsing("Microsoft.Extensions.Configuration")
            .OnBuild(file =>
            {
                AddNugetDependency(NugetPackages.MassTransitAbstractions(OutputTarget));
                AddNugetDependency(NugetPackages.MassTransit(OutputTarget));

                var messageBrokerDependency = _messageBroker.GetNugetDependency(OutputTarget);
                if (messageBrokerDependency is not null)
                {
                    AddNugetDependency(messageBrokerDependency);
                }

                _consumers = GetConsumers();
                _producers = GetProducers();
            })
            .AddClass($"MassTransitConfiguration", @class =>
            {
                @class.Static();
                @class.AddMethod(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "AddMassTransitConfiguration", method =>
                {
                    method.Static();
                    method.AddParameter(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "services", param => param.WithThisModifier());
                    method.AddParameter("IConfiguration", "configuration");

                    var requiresCompositeMessageBus = this.RequiresCompositeMessageBus();
                    if (requiresCompositeMessageBus)
                    {
                        method.AddParameter(this.GetMessageBrokerRegistryName(), "registry");
                        method.AddStatement($"services.AddScoped<{this.GetMassTransitMessageBusName()}>();");
                    }
                    else
                    {
                        var busInterface = this.GetBusInterfaceName();
            
                        method.AddStatement($"services.AddScoped<{this.GetMassTransitMessageBusName()}>();");
                        method.AddStatement($"services.AddScoped<{busInterface}>(provider => provider.GetRequiredService<{this.GetMassTransitMessageBusName()}>());");
                    }
                    
                    method.AddInvocationStatement("services.AddMassTransit", stmt => stmt
                        .AddArgument(GetConfigurationForAddMassTransit("configuration"))
                        .AddMetadata("configure-masstransit", true)
                        .SeparatedFromPrevious());
                        
                    if (requiresCompositeMessageBus && _producers?.Any() == true)
                    {
                        foreach (var producer in _producers)
                        {
                            method.AddStatement($"registry.Register<{producer.MessageTypeName}, {this.GetMassTransitMessageBusName()}>();");
                        }
                    }
                    
                    method.AddReturn("services");
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

    public override void AfterTemplateRegistration()
    {
        base.AfterTemplateRegistration();

        // this is to cater for the fact the MassTransit > 8.5.0 requires EF9.
        if (OutputTarget.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.EntityFrameworkCore"))
        {
            var efVersion = NugetRegistry.GetVersion("Microsoft.EntityFrameworkCore", OutputTarget.GetMaxNetAppVersion());
            if (efVersion != null && int.TryParse(efVersion.Version.First().ToString(), out int majorVersion) && majorVersion < 9)
            {
                NugetRegistry.Register(NugetPackages.MassTransitAbstractionsPackageName, v => new PackageVersion("8.4.1", true));
                NugetRegistry.Register(NugetPackages.MassTransitPackageName, v => new PackageVersion("8.4.1", true)
                    .WithNugetDependency(NugetPackages.MassTransitAbstractionsPackageName, "8.4.1")
                    .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                    .WithNugetDependency("Microsoft.Extensions.Diagnostics.HealthChecks", "8.0.0")
                    .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "8.0.0")
                    .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "8.0.0")
                    .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0"));
                NugetRegistry.Register(NugetPackages.MassTransitRabbitMQPackageName, v => new PackageVersion("8.4.1", true)
                    .WithNugetDependency("MassTransit", "8.4.1")
                    .WithNugetDependency("RabbitMQ.Client", "7.1.2"));
                NugetRegistry.Register(NugetPackages.MassTransitAzureServiceBusCorePackageName, v => new PackageVersion("8.4.1", true)
                    .WithNugetDependency("Azure.Identity", "1.13.2")
                    .WithNugetDependency("Azure.Messaging.ServiceBus", "7.19.0")
                    .WithNugetDependency("MassTransit", "8.4.1"));
                NugetRegistry.Register(NugetPackages.MassTransitAmazonSQSPackageName, v => new PackageVersion("8.4.1", true)
                    .WithNugetDependency("AWSSDK.SimpleNotificationService", "3.7.400.141")
                    .WithNugetDependency("AWSSDK.SQS", "3.7.400.141")
                    .WithNugetDependency("MassTransit", "8.4.1"));
                NugetRegistry.Register(NugetPackages.MassTransitEntityFrameworkCorePackageName, v => new PackageVersion("8.4.1", true)
                    .WithNugetDependency("MassTransit", "8.4.1")
                    .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "8.0.0")
                    .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "8.0.1"));
            }
        }
    }
    private MessageBrokerBase GetMessageBroker()
    {
        var messageBrokerSetting = this.ExecutionContext.Settings.GetMassTransitMessageBusSettings().MessagingServiceProvider().AsEnum();
        return messageBrokerSetting switch
        {
            MassTransitMessageBusSettings.MessagingServiceProviderOptionsEnum.InMemory => new InMemoryMessageBroker(this),
            MassTransitMessageBusSettings.MessagingServiceProviderOptionsEnum.Rabbitmq => new RabbitMqMessageBroker(this),
            MassTransitMessageBusSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus => new AzureServiceBusMessageBroker(this),
            MassTransitMessageBusSettings.MessagingServiceProviderOptionsEnum.AmazonSqs => new AmazonSqsMessageBroker(this),
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
            .Concat(GetDecorators().SelectMany(decorator => decorator.GetConsumerFactories() ?? []))
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
            .Concat(GetDecorators().SelectMany(decorator => decorator.GetProducerFactories() ?? []))
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
        var serviceIntegrationMessages = ExecutionContext.MetadataManager.GetAssociatedMessageModels(OutputTarget.Application);

        var filtered = legacySubscriptionMessages
            .Union(legacyPublishMessages)
            .Union(serviceIntegrationMessages)
            .FilterMessagesForThisMessageBroker(this, [MessageModelStereotypeExtensions.MessageTopologySettings.DefinitionId]);

        var result = filtered
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToList();
        return result;
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

        if (ExecutionContext.Settings.GetMassTransitMessageBusSettings().OutboxPattern().IsInMemory())
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

        if (ExecutionContext.Settings.GetMassTransitMessageBusSettings().OutboxPattern().IsInMemory())
        {
            yield return new CSharpStatement($"{factoryConfigVarName}.UseInMemoryOutbox(context);").AddMetadata("in-memory-outbox", true);
        }
        else if (ExecutionContext.Settings.GetMassTransitMessageBusSettings().OutboxPattern().IsEntityFramework() &&
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
        return ExecutionContext.Settings.GetMassTransitMessageBusSettings().RetryPolicy().AsEnum() switch
        {
            MassTransitMessageBusSettings.RetryPolicyOptionsEnum.RetryImmediate => GetCSharpRetryStatement("Immediate",
                ("int", "RetryLimit", "5")),
            MassTransitMessageBusSettings.RetryPolicyOptionsEnum.RetryInterval => GetCSharpRetryStatement("Interval",
                ("int", "RetryCount", "10"),
                ("TimeSpan", "Interval", "TimeSpan.FromSeconds(5)")),
            MassTransitMessageBusSettings.RetryPolicyOptionsEnum.RetryIncremental => GetCSharpRetryStatement("Incremental",
                ("int", "RetryLimit", "10"),
                ("TimeSpan", "InitialInterval", "TimeSpan.FromSeconds(5)"),
                ("TimeSpan", "IntervalIncrement", "TimeSpan.FromSeconds(5)")),
            MassTransitMessageBusSettings.RetryPolicyOptionsEnum.RetryExponential => GetCSharpRetryStatement("Exponential",
                ("int", "RetryLimit", "10"),
                ("TimeSpan", "MinInterval", "TimeSpan.FromSeconds(5)"),
                ("TimeSpan", "MaxInterval", "TimeSpan.FromMinutes(30)"),
                ("TimeSpan", "IntervalDelta", "TimeSpan.FromSeconds(5)")),
            MassTransitMessageBusSettings.RetryPolicyOptionsEnum.RetryNone => GetCSharpRetryStatement("None"),
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

        switch (ExecutionContext.Settings.GetMassTransitMessageBusSettings().RetryPolicy().AsEnum())
        {
            case MassTransitMessageBusSettings.RetryPolicyOptionsEnum.RetryImmediate:
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryImmediate",
                    new
                    {
                        RetryLimit = 5
                    }));
                break;
            case MassTransitMessageBusSettings.RetryPolicyOptionsEnum.RetryInterval:
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryInterval",
                    new
                    {
                        RetryCount = 10,
                        Interval = TimeSpan.FromSeconds(5)
                    }));
                break;
            case MassTransitMessageBusSettings.RetryPolicyOptionsEnum.RetryIncremental:
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("MassTransit:RetryIncremental",
                    new
                    {
                        RetryLimit = 10,
                        InitialInterval = TimeSpan.FromSeconds(5),
                        IntervalIncrement = TimeSpan.FromSeconds(5)
                    }));
                break;
            case MassTransitMessageBusSettings.RetryPolicyOptionsEnum.RetryExponential:
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
            case MassTransitMessageBusSettings.RetryPolicyOptionsEnum.RetryNone:
            default:
                break;
        }
    }

    public override void BeforeTemplateExecution()
    {
        var requiresCompositeMessageBus = this.RequiresCompositeMessageBus();
        if (!requiresCompositeMessageBus)
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest.ToRegister(
                    "AddMassTransitConfiguration",
                    ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));
        }

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