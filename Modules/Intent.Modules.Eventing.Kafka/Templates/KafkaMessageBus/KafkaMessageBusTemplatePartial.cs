using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Kafka.Templates.KafkaMessageBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class KafkaMessageBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Kafka.KafkaMessageBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public KafkaMessageBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusImplementation);
            
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"KafkaMessageBus", @class =>
                {
                    @class.ImplementsInterface(this.GetBusInterfaceName());
                    @class.AddField($"ConcurrentDictionary<Type, IProducer>", "_producersByMessageType", f => f
                        .PrivateReadOnly()
                        .WithAssignment(new CSharpStatement("new ConcurrentDictionary<Type, IProducer>()")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IServiceProvider", "serviceProvider", p => p.IntroduceReadonlyField());
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage);
                        method.AddGenericTypeConstraint(TMessage, c => c.AddType("class"));
                        method.AddParameter(TMessage, "message");

                        method.AddStatement($"var producer = _producersByMessageType.GetOrAdd(typeof({TMessage}), _ => new Producer<{TMessage}>(_serviceProvider.GetRequiredService<{this.GetKafkaProducerInterfaceName()}<{TMessage}>>()));");
                        method.AddStatement("producer.EnqueueMessage(message);");
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage);
                        method.AddGenericTypeConstraint(TMessage, c => c.AddType("class"));
                        method.AddParameter(TMessage, "message");
                        method.AddParameter("IDictionary<string, object>", "additionalData");

                        method.AddStatement("throw new NotSupportedException(\"Additional data is not supported in Kafka event publishing.\");");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage);
                        method.AddGenericTypeConstraint(TMessage, c => c.AddType("class"));
                        method.AddParameter(TMessage, "message");

                        method.AddStatement("Publish(message);");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage);
                        method.AddGenericTypeConstraint(TMessage, c => c.AddType("class"));
                        method.AddParameter(TMessage, "message");
                        method.AddParameter("IDictionary<string, object>", "additionalData");

                        method.AddStatement("throw new NotSupportedException(\"Additional data is not supported in Kafka event publishing.\");");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddOptionalCancellationTokenParameter();

                        method.AddStatement("var producers = _producersByMessageType.Values;");
                        method.AddForEachStatement("producer", "producers", @while =>
                        {
                            @while.AddStatement("await producer.FlushAsync(cancellationToken);");
                        });
                    });

                    @class.AddNestedClass("Producer", producerClass =>
                    {
                        producerClass.Private();
                        producerClass.ImplementsInterface("IProducer");

                        producerClass.AddGenericParameter("T", out var t);
                        producerClass.AddGenericTypeConstraint(t, c => c.AddType("class"));
                        producerClass.AddField($"ConcurrentQueue<{t}>", "_messageQueue", f => f
                            .PrivateReadOnly()
                            .WithAssignment(new CSharpStatement($"new ConcurrentQueue<{t}>()")));

                        producerClass.AddConstructor(ctor =>
                        {
                            ctor.AddParameter($"IKafkaProducer<{t}>", "kafkaProducer", p => p.IntroduceReadonlyField());
                        });

                        producerClass.AddMethod("void", "EnqueueMessage", method =>
                        {
                            method.AddParameter("object", "message");
                            method.WithExpressionBody($"_messageQueue.Enqueue(({t})message)");
                        });

                        producerClass.AddMethod("Task", "FlushAsync", method =>
                        {
                            method.Async();
                            method.AddParameter("CancellationToken", "cancellationToken");
                            method.WithExpressionBody("await _kafkaProducer.Produce(_messageQueue, cancellationToken)");
                        });
                    });

                    @class.AddNestedInterface("IProducer", producerInterface =>
                    {
                        producerInterface.Private();
                        producerInterface.AddMethod("void", "EnqueueMessage", m => m.AddParameter("object", "message"));
                        producerInterface.AddMethod("Task", "FlushAsync", m => m.AddParameter("CancellationToken", "cancellationToken"));
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