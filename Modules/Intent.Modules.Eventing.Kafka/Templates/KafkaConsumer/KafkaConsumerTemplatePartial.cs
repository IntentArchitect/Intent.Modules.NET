using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
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
            AddNugetDependency(NugetPackages.ConfluentKafka);
            AddNugetDependency(NugetPackages.ConfluentSchemaRegistrySerdesJson);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Confluent.Kafka")
                .AddUsing("Confluent.Kafka.SyncOverAsync")
                .AddUsing("Confluent.SchemaRegistry.Serdes")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"KafkaConsumer", @class =>
                {
                    @class.AddGenericParameter("T", out var t);
                    @class.AddGenericTypeConstraint(t, c => c.AddType("class"));
                    @class.ImplementsInterface(this.GetKafkaConsumerInterfaceName());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IServiceProvider", "serviceProvider", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("Task", "DoWork", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "stoppingToken");

                        method.AddStatement($$"""
                                            var consumerConfig = new ConsumerConfig
                                            {
                                                GroupId = "kafka-dotnet-getting-started",
                                                AutoOffsetReset = AutoOffsetReset.Earliest,
                                                BootstrapServers = "localhost:61294"
                                            };
                                            
                                            var topic = typeof({{t}}).Name; // TODO JL: This should come from configuration
                                            """);

                        method.AddUsingBlock(
                            $"var consumer = new ConsumerBuilder<string, {t}>(consumerConfig).SetValueDeserializer(new JsonDeserializer<{t}>().AsSyncOverAsync()).Build()",
                            @using =>
                            {
                                @using.AddStatement("consumer.Subscribe(topic);");

                                @using.AddTryBlock(@try =>
                                {
                                    @try.AddWhileStatement("!stoppingToken.IsCancellationRequested", @while =>
                                    {
                                        @while.AddUsingBlock("var scope = _serviceProvider.CreateScope()", usingScope =>
                                        {
                                            usingScope.AddStatement("var consumeResult = consumer.Consume(stoppingToken);");
                                            usingScope.AddStatement($"var handler = scope.ServiceProvider.GetRequiredService<{this.GetIntegrationEventHandlerInterfaceName()}<{t}>>();");

                                            usingScope.AddTryBlock(tryHandle =>
                                            {
                                                tryHandle.AddStatement("await handler.HandleAsync(consumeResult.Message.Value, stoppingToken);");
                                            });
                                            usingScope.AddCatchBlock(@catch =>
                                            {
                                                @catch.AddStatement("// TODO JL: Proper error handling");
                                            });
                                        });
                                    });
                                });

                                @using.AddCatchBlock(@catch => @catch
                                    .WithExceptionType("OperationCanceledException")
                                    .AddStatement("// NOP"));
                                @using.AddFinallyBlock(@finally => @finally.AddStatement("consumer.Close();"));
                            });
                    });
                });
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