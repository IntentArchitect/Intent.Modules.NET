using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventInterface;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventBusImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EventBusImplementationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Pubsub.EventBusImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EventBusImplementationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.DaprClient(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Dapr.Client")
                .AddClass("EventBusImplementation", @class => @class
                    .ImplementsInterface(this.GetEventBusInterfaceName())
                    .AddField($"ConcurrentQueue<{this.GetEventInterfaceName()}>", "_events", f => f.PrivateReadOnly())
                    .AddConstructor(constructor => constructor
                        .AddParameter("DaprClient", "dapr", p => p.IntroduceReadonlyField())
                        .AddStatement($"_events = new ConcurrentQueue<{this.GetEventInterfaceName()}>();")
                    )
                    .AddMethod("void", "Publish", method => method
                        .AddGenericParameter("T")
                        .AddParameter("T", "@event")
                        .AddGenericTypeConstraint("T", constraint => constraint
                            .AddType("class")
                            .AddType(this.GetEventInterfaceName())
                        )
                        .AddStatement("_events.Enqueue(@event);")
                    )
                    .AddMethod("Task", "FlushAllAsync", method => method
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatementBlock("while (_events.TryDequeue(out var @event))", block => block
                            .AddStatements(new[]
                            {
                                "// We need to make sure that we pass the concrete type to PublishEventAsync,",
                                "// which can be accomplished by casting the event to dynamic. This ensures",
                                "// that all event fields are properly serialized.",
                                "await _dapr.PublishEventAsync(@event.PubsubName, @event.TopicName, (object)@event, cancellationToken);"
                            })
                        )
                    )
                );
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(ClassName)
                .WithPerServiceCallLifeTime()
                .ForInterface(this.GetEventBusInterfaceName())
                .WithPriority(4)
                .ForConcern("Infrastructure")
                .HasDependency(this)
                .HasDependency(ExecutionContext.FindTemplateInstance<ITemplate>(EventInterfaceTemplate.TemplateId)));
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