using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.Kafka.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Kafka.Templates.KafkaConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class KafkaConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Kafka.KafkaConfiguration";
        private readonly Lazy<IReadOnlyCollection<MessageModel>> _subscribedMessageModels;
        private readonly Lazy<IReadOnlyCollection<MessageModel>> _publishedMessageModels;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public KafkaConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            _subscribedMessageModels = new Lazy<IReadOnlyCollection<MessageModel>>(() =>
            {
                var serviceDesignerMessages = ExecutionContext.MetadataManager
                    .Services(ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                    .SelectMany(x => x.IntegrationEventSubscriptions())
                    .Select(x => x.TypeReference.Element.AsMessageModel());

                var eventingDesignerMessages = ExecutionContext.MetadataManager
                    .Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
                    .SelectMany(x => x.SubscribedMessages())
                    .Select(x => x.TypeReference.Element.AsMessageModel());

                var messageModels = Enumerable.Empty<MessageModel>()
                    .Concat(serviceDesignerMessages)
                    .Concat(eventingDesignerMessages)
                    .OrderBy(x => x.Name)
                    .ToArray();

                return messageModels;
            });

            _publishedMessageModels = new Lazy<IReadOnlyCollection<MessageModel>>(() =>
            {
                var serviceDesignerMessages = ExecutionContext.MetadataManager
                    .GetExplicitlyPublishedMessageModels(OutputTarget.Application);

                var eventingDesignerMessages = ExecutionContext.MetadataManager
                    .Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
                    .SelectMany(x => x.PublishedMessages())
                    .Select(x => x.TypeReference.Element.AsMessageModel());

                var messageModels = Enumerable.Empty<MessageModel>()
                    .Concat(serviceDesignerMessages)
                    .Concat(eventingDesignerMessages)
                    .OrderBy(x => x.Name)
                    .ToArray();

                return messageModels;
            });

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Confluent.SchemaRegistry")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"KafkaConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("void", "AddKafkaConfiguration", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());

                        method.AddInvocationStatement("services.AddSingleton<ISchemaRegistryClient>", invocation =>
                        {
                            invocation.AddArgument(new CSharpLambdaBlock("serviceProvider"), statement =>
                            {
                                var block = statement as CSharpLambdaBlock;

                                block.AddStatement(@"var schemaRegistryConfig = serviceProvider
                    .GetRequiredService<IConfiguration>()
                    .GetSection(""Kafka:SchemaRegistryConfig"")
                    .Get<SchemaRegistryConfig>();");

                                block.AddStatement("return new CachedSchemaRegistryClient(schemaRegistryConfig);", s => s.SeparatedFromPrevious());
                            });
                        });
                        method.AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()}, {this.GetKafkaEventBusName()}>();");
                        method.AddStatement($"services.AddScoped(typeof({this.GetKafkaEventDispatcherInterfaceName()}<>), typeof({this.GetKafkaEventDispatcherName()}<>));");
                        method.AddStatement($"services.AddHostedService<{this.GetKafkaConsumerBackgroundServiceName()}>();");

                        foreach (var message in _subscribedMessageModels.Value)
                        {
                            var messageTypeName = this.GetIntegrationEventMessageName(message);

                            method.AddStatement($"services.AddTransient<{this.GetKafkaConsumerInterfaceName()}, {this.GetKafkaConsumerName()}<{messageTypeName}>>();");
                        }

                        foreach (var message in _publishedMessageModels.Value)
                        {
                            method.AddStatement($"services.AddSingleton(serviceProvider => {GetProducerFactoryStatement(message)});");
                        }
                    });

                    if (_publishedMessageModels.Value.Any())
                    {
                        var tValue = "TValue";

                        @class.AddMethod($"IKafkaProducer<{tValue}>", "CreateProducer", method =>
                        {
                            method.Private().Static();
                            method.AddGenericParameter("TKey", out var tKey);
                            method.AddGenericParameter(tValue, out _);
                            method.AddGenericTypeConstraint(tValue, c => c.AddType("class"));

                            method.AddParameter("IServiceProvider", "serviceProvider");
                            method.AddParameter($"Func<{tValue}, {tKey}>?", "keyProvider", p => p.WithDefaultValue("null"));

                            method.AddInvocationStatement($"return new KafkaProducer<{tKey}, {tValue}>", statement =>
                            {
                                statement.WithArgumentsOnNewLines();
                                statement.AddArgument(new CSharpArgument("serviceProvider.GetRequiredService<ISchemaRegistryClient>()"), a => a.WithName("schemaRegistryClient"));
                                statement.AddArgument(new CSharpArgument("keyProvider"), a => a.WithName("keyProvider"));
                                statement.AddArgument(new CSharpArgument("serviceProvider.GetRequiredService<IConfiguration>()"), a => a.WithName("configuration"));
                                statement.AddArgument(new CSharpArgument($"serviceProvider.GetRequiredService<{UseType("Microsoft.Extensions.Logging.ILogger")}<KafkaProducer<TKey, TValue>>>()"), a => a.WithName("logger"));
                            });
                        });
                    }
                });
        }

        private string GetProducerFactoryStatement(MessageModel message)
        {
            var messageTypeName = this.GetIntegrationEventMessageName(message);
            var keyProperty = message.Properties.SingleOrDefault(x => x.HasKey());

            if (keyProperty == null)
            {
                return $"CreateProducer<{UseType("Confluent.Kafka.Null")}, {messageTypeName}>(serviceProvider)";
            }

            var keyAccessExpression = $"message => message.{keyProperty.Name.ToPascalCase()}";
            var keyTypeName = GetTypeName(keyProperty);

            if (keyTypeName is "Guid" or "System.Guid")
            {
                return $"CreateProducer<string, {messageTypeName}>(serviceProvider, {keyAccessExpression}.ToString())";
            }

            return $"CreateProducer<{keyTypeName}, {messageTypeName}>(serviceProvider, {keyAccessExpression})";
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddKafkaConfiguration")
                .ForConcern("Infrastructure")
                .HasDependency(this));

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Kafka:SchemaRegistryConfig",
                new
                {
                    Url = "http://localhost:8081",
                    BasicAuthUserInfo = "key:secret"
                }));

            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.Kafka.Name, new Dictionary<string, string>
            {
                { Infrastructure.Kafka.Property.KafkaSchemaRegistryUrl, "Kafka:SchemaRegistryConfig:Url" }
            }));

            var messages = Enumerable.Empty<MessageModel>()
                .Union(_subscribedMessageModels.Value)
                .Union(_publishedMessageModels.Value)
                .Select(model => (
                    FullyQualifiedTypeName: GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, model),
                    Topic: GetMessageTopic(model)))
                .OrderBy(x => x.FullyQualifiedTypeName);

            foreach (var (fullyQualifiedTypeName, topic) in messages)
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest($"Kafka:MessageTypes:{fullyQualifiedTypeName}:Topic", topic));
            }
        }

        private static string GetMessageTopic(MessageModel messageModel)
        {
            var stack = new Stack<string>();
            var element = messageModel.InternalElement;

            while (true)
            {
                stack.Push(element.Name);

                if (element.ParentElement == null)
                {
                    stack.Push(element.Package.Name);
                    break;
                }

                element = element.ParentElement;
            }

            return string.Join('.', stack);
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