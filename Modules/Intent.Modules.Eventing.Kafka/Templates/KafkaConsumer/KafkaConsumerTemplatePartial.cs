using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Kafka.Templates.KafkaEventDispatcher;
using Intent.Modules.Eventing.Kafka.Templates.KafkaEventDispatcherInterface;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Kafka.Templates.KafkaConsumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class KafkaConsumerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Kafka.KafkaConsumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public KafkaConsumerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.ConfluentKafka(OutputTarget));
            AddNugetDependency(NugetPackages.ConfluentSchemaRegistrySerdesJson(OutputTarget));

            AddKnownType("Confluent.Kafka.IsolationLevel");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Confluent.Kafka")
                .AddUsing("Confluent.Kafka.SyncOverAsync")
                .AddUsing("Confluent.SchemaRegistry.Serdes")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddClass($"KafkaConsumer", @class =>
                {
                    @class.AddGenericParameter("T", out var t);
                    @class.AddGenericTypeConstraint(t, c => c.AddType("class"));
                    @class.ImplementsInterface(this.GetKafkaConsumerInterfaceName());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"ILogger<{@class.Name}<{t}>>", "logger", p => p.IntroduceReadonlyField());
                        ctor.AddParameter("IServiceProvider", "serviceProvider", p => p.IntroduceReadonlyField());
                        ctor.AddParameter("IConfiguration", "configuration", p => p.IntroduceReadonlyField());
                    });

                    @class.AddMethod("Task", "DoWork", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "stoppingToken");

                        method.AddStatement("var messageType = $\"{typeof(T).Namespace}.{typeof(T).Name}\";");

                        method.AddStatement(@"var consumerConfig = _configuration
                .GetSection($""Kafka:MessageTypes:{messageType}:ConsumerConfig"")
                .Get<ConsumerConfig>();", s => s.SeparatedFromPrevious());

                        method.AddStatement(@"_logger.LogInformation(consumerConfig != null
                ? ""Using message type specific configuration""
                : ""Using default configuration"");", s => s.SeparatedFromPrevious());

                        method.AddStatement(@"consumerConfig ??= _configuration
                .GetSection(""Kafka:DefaultConsumerConfig"")
                .Get<ConsumerConfig>();", s => s.SeparatedFromPrevious());

                        method.AddStatement("var topic = _configuration[$\"Kafka:MessageTypes:{messageType}:Topic\"] ?? typeof(T).Name;", s => s.SeparatedFromPrevious());

                        method.AddStatement("_logger.LogInformation($\"Topic: {topic}\");", s => s.SeparatedFromPrevious());

                        method.AddTryBlock(outerTry =>
                        {
                            outerTry.AddUsingBlock(
                                $"var consumer = new ConsumerBuilder<string, {t}>(consumerConfig).SetValueDeserializer(new JsonDeserializer<{t}>().AsSyncOverAsync()).Build()",
                                @using =>
                                {
                                    @using.AddStatement("consumer.Subscribe(topic);");

                                    @using
                                        .AddTryBlock(innerTry =>
                                        {
                                            innerTry.AddWhileStatement("!stoppingToken.IsCancellationRequested",
                                                @while =>
                                                {
                                                    @while.AddUsingBlock("var scope = _serviceProvider.CreateScope()",
                                                        usingScope =>
                                                        {
                                                            usingScope.AddStatement("var consumeResult = consumer.Consume(stoppingToken);");
                                                            usingScope.AddStatement($"var dispatcher = scope.ServiceProvider.GetRequiredService<{this.GetKafkaEventDispatcherInterfaceName()}<{t}>>();");

                                                            usingScope.AddTryBlock(@try =>
                                                            {
                                                                @try.AddStatement("await dispatcher.Dispatch(consumeResult.Message.Value, stoppingToken);", s => s.SeparatedFromPrevious());
                                                            });
                                                            usingScope.AddCatchBlock(@catch =>
                                                            {
                                                                @catch.WithExceptionType("Exception").WithParameterName("exception");
                                                                @catch.AddStatement("_logger.LogError(exception, \"Error processing incoming message\");");
                                                            });
                                                        });
                                                });
                                        })
                                        .AddCatchBlock(innerCatch =>
                                        {
                                            innerCatch.WithExceptionType("OperationCanceledException");
                                            innerCatch.AddStatement("// NOP");
                                        })
                                        .AddFinallyBlock(innerFinally =>
                                        {
                                            innerFinally.AddStatement("consumer.Close();");
                                        });
                                });
                        })
                            .AddCatchBlock(outerCatch =>
                            {
                                outerCatch.WithExceptionType("Exception").WithParameterName("exception");
                                outerCatch.AddStatement("_logger.LogError(exception, $\"Error creating consumer for {messageType}\");");
                            });
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Kafka:DefaultConsumerConfig",
                new
                {
                    GroupId = ExecutionContext.GetSolutionConfig().SolutionName,
                    AutoOffsetReset = "Earliest",
                    BootstrapServers = "localhost:9092"
                }));
            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.Kafka.Name, new Dictionary<string, string>
            {
                { Infrastructure.Kafka.Property.KafkaSettingsName, "DefaultConsumerConfig" },
                { Infrastructure.Kafka.Property.KafkaSettingsPath, "Kafka:DefaultConsumerConfig:BootstrapServers" }
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