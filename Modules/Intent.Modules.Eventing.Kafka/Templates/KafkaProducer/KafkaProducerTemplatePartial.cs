using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Kafka.Templates.KafkaProducer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class KafkaProducerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Kafka.KafkaProducer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public KafkaProducerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Confluent.Kafka")
                .AddUsing("Confluent.Kafka.SyncOverAsync")
                .AddUsing("Confluent.SchemaRegistry")
                .AddUsing("Confluent.SchemaRegistry.Serdes")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddClass($"KafkaProducer", @class =>
                {
                    var kafkaProducerInterfaceName = this.GetKafkaProducerInterfaceName();

                    @class.Sealed();
                    @class.AddGenericParameter("TKey", out var tKey);
                    @class.AddGenericParameter("TValue", out var tValue);
                    @class.AddGenericTypeConstraint(tValue, c => c.AddType("class"));
                    @class.ImplementsInterface($"{kafkaProducerInterfaceName}<{tValue}>");
                    @class.ImplementsInterface("IDisposable");

                    @class.AddField($"IProducer<{tKey}, {tValue}>", "_producer", f => f.PrivateReadOnly());
                    @class.AddField("string", "_topic", f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ISchemaRegistryClient", "schemaRegistryClient");
                        ctor.AddParameter($"Func<{tValue}, {tKey}>?", "keyProvider", p => p.IntroduceReadonlyField());
                        ctor.AddParameter("IConfiguration", "configuration");
                        ctor.AddParameter($"ILogger<{@class.Name}<{tKey}, {tValue}>>", "logger");

                        ctor.AddStatement("var messageType = $\"{typeof(TValue).Namespace}.{typeof(TValue).Name}\";");

                        ctor.AddStatement(@"var producerConfig = configuration
                .GetSection($""Kafka:MessageTypes:{messageType}:ProducerConfig"")
                .Get<ProducerConfig>();");

                        ctor.AddStatement(@"logger.LogInformation(producerConfig != null
                ? ""Using message type specific configuration""
                : ""Using default configuration"");", s => s.SeparatedFromPrevious());

                        ctor.AddStatement(@"producerConfig ??= configuration
                .GetSection(""Kafka:DefaultProducerConfig"")
                .Get<ProducerConfig>();", s => s.SeparatedFromPrevious());

                        ctor.AddStatement("_topic = configuration[$\"Kafka:MessageTypes:{messageType}:Topic\"] ?? typeof(TValue).Name;", s => s.SeparatedFromPrevious());
                        ctor.AddStatement($@"_producer = new ProducerBuilder<{tKey}, {tValue}>(producerConfig)
                .SetValueSerializer(new JsonSerializer<{tValue}>(schemaRegistryClient).AsSyncOverAsync())
                .Build();", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod("void", "Produce", method =>
                    {
                        method.Async();
                        method.AddParameter($"ConcurrentQueue<{tValue}>", "messageQueue");
                        method.AddOptionalCancellationTokenParameter();

                        method.AddWhileStatement("!cancellationToken.IsCancellationRequested && messageQueue.TryDequeue(out var message)", @while =>
                        {
                            @while.AddInvocationStatement("await _producer.ProduceAsync", invocation =>
                            {
                                invocation.WithArgumentsOnNewLines();
                                invocation.AddArgument("topic: _topic");
                                invocation.AddObjectInitializerBlock($"message: new Message<{tKey}, {tValue}>", block =>
                                {
                                    block.BeforeSeparator = CSharpCodeSeparatorType.NewLine;
                                    block.AfterSeparator = CSharpCodeSeparatorType.None;
                                    block.AddInitStatement("Key", "_keyProvider != null ? _keyProvider(message) : default!");
                                    block.AddInitStatement("Value", "message");
                                });
                                invocation.AddArgument("cancellationToken: cancellationToken");
                            });
                        });

                        method.AddStatement("_producer.Flush(cancellationToken);");
                    });

                    @class.AddMethod("void", "Dispose", method =>
                    {
                        method.AddStatement("_producer.Dispose();");
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Kafka:DefaultProducerConfig",
                new
                {
                    BootstrapServers = "localhost:9092",
                    ClientId = ExecutionContext.GetApplicationConfig().Name
                }));
            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.Kafka.Name, new Dictionary<string, string>
            {
                { Infrastructure.Kafka.Property.KafkaSettingsName, "DefaultProducerConfig" },
                { Infrastructure.Kafka.Property.KafkaSettingsPath, "Kafka:DefaultProducerConfig:BootstrapServers" }
            }));

            base.AfterTemplateRegistration();
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