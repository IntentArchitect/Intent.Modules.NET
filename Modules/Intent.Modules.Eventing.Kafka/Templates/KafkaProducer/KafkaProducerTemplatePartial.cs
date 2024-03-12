using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
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
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("System.Net")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Confluent.Kafka")
                .AddUsing("Confluent.Kafka.SyncOverAsync")
                .AddUsing("Confluent.SchemaRegistry")
                .AddUsing("Confluent.SchemaRegistry.Serdes")
                .AddClass($"KafkaProducer", @class =>
                {
                    var kafkaProducerInterfaceName = this.GetKafkaProducerInterfaceName();

                    @class.AddGenericParameter("T", out var t);
                    @class.ImplementsInterface(kafkaProducerInterfaceName);
                    @class.AddGenericTypeConstraint(t, c => c.AddType("class"));
                    @class.AddField($"ConcurrentQueue<{t}>", "_messages", f => f.PrivateReadOnly().WithAssignment(new CSharpStatement("new ConcurrentQueue<T>()")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ISchemaRegistryClient", "schemaRegistryClient", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("void", "EnqueueMessage", method =>
                    {
                        method.IsExplicitImplementationFor(kafkaProducerInterfaceName);
                        method.AddParameter("object", "message");
                        method.WithExpressionBody($"EnqueueMessage(({t})message)");
                    });

                    @class.AddMethod("void", "EnqueueMessage", method =>
                    {
                        method.AddParameter(t, "message");
                        method.WithExpressionBody("_messages.Enqueue(message)");
                    });

                    @class.AddMethod("void", "FlushAsync", method =>
                    {
                        method.Async();
                        method.AddOptionalCancellationTokenParameter(this);

                        method.AddStatement("""
                                            // TODO JL: Pull from config
                                            var config = new ProducerConfig
                                            {
                                                BootstrapServers = "localhost:61294",
                                                ClientId = Dns.GetHostName(),
                                            };
                                            """);

                        method.AddStatement("// TODO JL: One should be able to choose a key, possibly a stereotype on a field on messages");
                        method.AddUsingBlock(
                            $"""
                            var producer = new ProducerBuilder<Null, {t}>(config)
                                .SetValueSerializer(new JsonSerializer<{t}>(_schemaRegistryClient).AsSyncOverAsync())
                                .Build()
                            """,
                            @using =>
                            {
                                @using.AddWhileStatement("_messages.TryDequeue(out var message)", @while =>
                                {
                                    @while.AddInvocationStatement("await producer.ProduceAsync", invocation =>
                                    {
                                        invocation.WithArgumentsOnNewLines();
                                        invocation.AddArgument($"topic: typeof({t}).Name");
                                        invocation.AddObjectInitializerBlock($"message: new Message<Null, {t}>", block =>
                                        {
                                            block.BeforeSeparator = CSharpCodeSeparatorType.NewLine;
                                            block.AfterSeparator = CSharpCodeSeparatorType.None;
                                            block.AddInitStatement("Value", "message");
                                        });
                                        invocation.AddArgument("cancellationToken: cancellationToken");
                                    });
                                });

                                @using.AddStatement("producer.Flush(cancellationToken);");
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