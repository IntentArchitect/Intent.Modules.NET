using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Kafka.Templates.KafkaEventBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class KafkaEventBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Kafka.KafkaEventBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public KafkaEventBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Confluent.SchemaRegistry")
                .AddClass($"KafkaEventBus", @class =>
                {
                    @class.ImplementsInterface(this.GetEventBusInterfaceName());
                    @class.AddField($"ConcurrentDictionary<Type, {this.GetKafkaProducerInterfaceName()}>", "_producersByMessageType", f => f
                        .PrivateReadOnly()
                        .WithAssignment(new CSharpStatement("new ConcurrentDictionary<Type, IKafkaProducer>()")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ISchemaRegistryClient", "schemaRegistryClient", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("T", out var t);
                        method.AddGenericTypeConstraint(t, c => c.AddType("class"));
                        method.AddParameter(t, "message");

                        method.AddStatement($"var producer = _producersByMessageType.GetOrAdd(typeof({t}), _ => new {this.GetKafkaProducerName()}<{t}>(_schemaRegistryClient));");
                        method.AddStatement("producer.EnqueueMessage(message);");
                    });

                    @class.AddMethod("void", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddOptionalCancellationTokenParameter(this);

                        method.AddStatement("var producers = _producersByMessageType.Values;");
                        method.AddForEachStatement("producer", "producers", @while =>
                        {
                            @while.AddStatement("await producer.FlushAsync(cancellationToken);");
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