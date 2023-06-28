using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.MassTransit.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

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

        MessageHandlerModels = ExecutionContext.MetadataManager
            .Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
            .SelectMany(x => x.SubscribedMessages());
        
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Reflection")
            .AddUsing("MassTransit")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddClass($"MassTransitConfiguration", @class =>
            {
                @class.Static();
                @class.AddMethod("void", "AddMassTransitConfiguration", method =>
                {
                    method.Static();
                    method.AddParameter("IServiceCollection", "services", parm => parm.WithThisModifier());
                    method.AddParameter("IConfiguration", "configuration");
                    method.AddInvocationStatement("services.AddMassTransit", stmt => stmt
                        .AddArgument(GetConfigurationForAddMassTransit("configuration"))
                        .AddMetadata("configure-masstransit", true));
                });
                @class.AddMethod("void", "AddConsumers", method =>
                {
                    method.Private().Static();
                    method.AddParameter("IRegistrationConfigurator", "cfg", parm => parm.WithThisModifier());
                    method.AddStatements(GetConsumerStatements("cfg"));
                });
                AddNonDefaultEndpointConfigurationMethods(@class);
            });
    }

    private IEnumerable<MessageSubscribeAssocationTargetEndModel> MessageHandlerModels { get; }

    private void AddNonDefaultEndpointConfigurationMethods(CSharpClass @class)
    {
        if (!MessageHandlerModels.Any(HasMessageBrokerStereotype))
        {
            return;
        }

        @class.AddMethod("void", "ConfigureNonDefaultEndpoints", method =>
        {
            method.Private().Static();
            method.AddParameter("IServiceBusBusFactoryConfigurator", "cfg", parm => parm.WithThisModifier());
            method.AddParameter("IBusRegistrationContext", "context");
            
        });

        @class.AddMethod("void", "AddCustomConsumerEndpoint", method =>
        {
            method.Private().Static();
            method.AddGenericParameter("TConsumer", out var tConsumer);
            method.AddGenericTypeConstraint(tConsumer, c => c.AddType("class").AddType("IConsumer"));
            method.AddParameter("IServiceBusBusFactoryConfigurator", "cfg", parm => parm.WithThisModifier());
            method.AddParameter("IBusRegistrationContext", "context");
            method.AddParameter("string", "instanceId");
            method.AddParameter("Action<IServiceBusReceiveEndpointConfigurator>", "configuration");

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
    
    private IReadOnlyCollection<CSharpStatement> GetConsumerStatements(string configParamName)
    {
        var statements = new List<CSharpStatement>();
        foreach (var messageHandlerModel in MessageHandlerModels)
        {
            var messageName =
                this.GetIntegrationEventMessageName(messageHandlerModel.TypeReference.Element.AsMessageModel());
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace("_", "-").Replace(" ", "-")
                .Replace(".", "-");
            var consumerDefinitionType =
                $@"{this.GetIntegrationEventHandlerInterfaceName()}<{messageName}>, {messageName}";
            var consumerWrapperType = $@"{this.GetWrapperConsumerName()}<{consumerDefinitionType}>";

            // Until we can do single-line method chaining this will have to do for now...
            var addConsumer = $@"{configParamName}.AddConsumer<{consumerWrapperType}>"
                              + $@"(typeof({this.GetWrapperConsumerName()}Definition<{consumerDefinitionType}>))";

            if (HasMessageBrokerStereotype(messageHandlerModel))
            {
                addConsumer += $@".ExcludeFromConfigureEndpoints()";
            }
            else
            {
                addConsumer += $@".Endpoint(config => config.InstanceId = ""{sanitizedAppName}"")";
            }

            addConsumer += ";";
            statements.Add(addConsumer);
        }

        return statements;
    }

    private static bool HasMessageBrokerStereotype(MessageSubscribeAssocationTargetEndModel messageHandlerModel)
    {
        return messageHandlerModel.HasAzureServiceBusConsumerSettings() || messageHandlerModel.HasRabbitMQConsumerSettings();
    }

    private CSharpLambdaBlock GetConfigurationForAddMassTransit(string configurationVarName)
    {
        var block = new CSharpLambdaBlock("x")
            .AddStatement($"x.SetKebabCaseEndpointNameFormatter();")
            .AddStatement($"x.AddConsumers();");

        switch (ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum())
        {
            case EventingSettings.MessagingServiceProviderOptionsEnum.InMemory:
                block.AddInvocationStatement("x.UsingInMemory", memory => memory
                    .AddArgument(new CSharpLambdaBlock("(context, cfg)")
                        .AddStatement(GetMessageRetryStatement("cfg", configurationVarName))
                        .AddStatements(GetPostHostConfigurationStatements()))
                    .AddMetadata("message-broker", "memory"));
                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.Rabbitmq:
                block.AddInvocationStatement("x.UsingRabbitMq", rabbitMq => rabbitMq
                    .AddArgument(new CSharpLambdaBlock("(context, cfg)")
                        .AddStatement(GetMessageRetryStatement("cfg", configurationVarName))
                        .AddInvocationStatement("cfg.Host", host => host
                            .AddArgument(@"configuration[""RabbitMq:Host""]")
                            .AddArgument(@"configuration[""RabbitMq:VirtualHost""]")
                            .AddArgument(new CSharpLambdaBlock("host")
                                .AddStatement(@"host.Username(configuration[""RabbitMq:Username""]);")
                                .AddStatement(@"host.Password(configuration[""RabbitMq:Password""]);")))
                        .AddStatements(GetPostHostConfigurationStatements()))
                    .AddMetadata("message-broker", "rabbit-mq"));
                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus:
                block.AddInvocationStatement("x.UsingAzureServiceBus", azBus => azBus
                    .AddArgument(new CSharpLambdaBlock("(context, cfg)")
                        .AddStatement(GetMessageRetryStatement("cfg", configurationVarName))
                        .AddInvocationStatement("cfg.Host", host => host
                            .AddArgument(@"configuration[""AzureMessageBus:ConnectionString""]"))
                        .AddStatements(GetPostHostConfigurationStatements()))
                    .AddMetadata("message-broker", "azure-service-bus"));
                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.AmazonSqs:
                block.AddInvocationStatement("x.UsingAmazonSqs", sqs => sqs
                    .AddArgument(new CSharpLambdaBlock("(context, cfg)")
                        .AddStatement(GetMessageRetryStatement("cfg", configurationVarName))
                        .AddInvocationStatement("cfg.Host", host => host
                            .AddArgument(@"configuration[""AmazonSqs:Host""]")
                            .AddArgument(new CSharpLambdaBlock("host")
                                .AddStatement(@"host.AccessKey(configuration[""AmazonSqs:AccessKey""]);")
                                .AddStatement(@"host.SecretKey(configuration[""AmazonSqs:SecretKey""]);")))
                        .AddStatements(GetPostHostConfigurationStatements()))
                    .AddMetadata("message-broker", "amazon-sqs"));
                break;
            default:
                throw new InvalidOperationException(
                    $"Messaging Service Provider is set to a setting that is not supported: {ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum()}");
        }

        return block;
    }

    private IEnumerable<CSharpStatement> GetPostHostConfigurationStatements()
    {
        yield return new CSharpStatement("cfg.ConfigureEndpoints(context);").AddMetadata("configure-endpoints", true);
        if (MessageHandlerModels.Any(HasMessageBrokerStereotype))
        {
            yield return new CSharpStatement($@"cfg.ConfigureNonDefaultEndpoints(context);");
        }
    }

    private CSharpStatement GetMessageRetryStatement(string configParamName, string configurationVarName)
    {
        return new CSharpInvocationStatement($@"{configParamName}.UseMessageRetry")
            .AddArgument(new CSharpLambdaBlock("r").WithExpressionBody(new CSharpInvocationStatement("r.Interval")
                .WithoutSemicolon()
                .AddArgument($@"{configurationVarName}.GetValue<int?>(""MassTransit:Retry:RetryCount"") ?? 10")
                .AddArgument(
                    $@"{configurationVarName}.GetValue<TimeSpan?>(""MassTransit:Retry:Interval"") ?? TimeSpan.FromSeconds(30)")
                .WithArgumentsOnNewLines()));
    }

    public override void BeforeTemplateExecution()
    {
        ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest.ToRegister(
                "AddMassTransitConfiguration",
                ServiceConfigurationRequest.ParameterType.Configuration)
            .ForConcern("Infrastructure")
            .HasDependency(this));

        ExecutionContext.EventDispatcher.Publish(
            new AppSettingRegistrationRequest("MassTransit:Retry",
                new
                {
                    RetryCount = 10,
                    Interval = TimeSpan.FromSeconds(30)
                }));

        switch (ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum())
        {
            case EventingSettings.MessagingServiceProviderOptionsEnum.InMemory:
                // InMemory doesn't require appsettings
                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.Rabbitmq:
                AddNugetDependency(NuGetPackages.MassTransitRabbitMq);

                ExecutionContext.EventDispatcher.Publish(
                    new AppSettingRegistrationRequest("RabbitMq", new
                    {
                        Host = "localhost",
                        VirtualHost = "/",
                        Username = "guest",
                        Password = "guest"
                    }));

                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus:
                AddNugetDependency(NuGetPackages.MassTransitAzureServiceBusCore);

                ExecutionContext.EventDispatcher.Publish(
                    new AppSettingRegistrationRequest("AzureMessageBus", new
                    {
                        ConnectionString = "your connection string"
                    }));
                break;
            case EventingSettings.MessagingServiceProviderOptionsEnum.AmazonSqs:
                AddNugetDependency(NuGetPackages.MassTransitAmazonSqs);

                ExecutionContext.EventDispatcher.Publish(
                    new AppSettingRegistrationRequest("AmazonSqs", new
                    {
                        Host = "us-east-1",
                        AccessKey = "your-iam-access-key",
                        SecretKey = "your-iam-secret-key"
                    }));
                break;
            default:
                throw new InvalidOperationException(
                    $"Messaging Service Provider is set to a setting that is not supported: {ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum()}");
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