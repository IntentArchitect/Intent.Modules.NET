using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.WrapperConsumer;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class WrapperConsumerTemplate : CSharpTemplateBase<object, ConsumerDecorator>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Eventing.MassTransit.WrapperConsumer";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public WrapperConsumerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NuGetPackages.MassTransitAbstractions);
        AddTypeSource(IntegrationEventMessageTemplate.TemplateId);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Threading.Tasks")
            .AddUsing("MassTransit")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddClass($"WrapperConsumer", @class =>
            {
                @class.AddGenericParameter("THandler", out var tHandler);
                @class.AddGenericParameter("TMessage", out var tMessage);
                @class.ImplementsInterface($"IConsumer<{tMessage}>");
                @class.AddGenericTypeConstraint(tMessage, g => g.AddType("class"));
                @class.AddGenericTypeConstraint(tHandler,
                    g => g.AddType($"{this.GetIntegrationEventHandlerInterfaceName()}<{tMessage}>"));

                @class.AddConstructor(ctor =>
                {
                    ctor.AddParameter("IServiceProvider", "serviceProvider", parm => parm.IntroduceReadonlyField());
                });

                @class.AddMethod("Task", "Consume", method =>
                {
                    method.Async();
                    method.AddParameter($"ConsumeContext<{tMessage}>", "context");
                    method.AddStatement(
                        $"var eventBus = _serviceProvider.GetService<{this.GetMassTransitEventBusName()}>(){(UseExplicitNullSymbol ? "!" : string.Empty)};");
                    method.AddStatement($"eventBus.Current = context;");
                    method.AddStatement(
                        $"var handler = _serviceProvider.GetService<{tHandler}>(){(UseExplicitNullSymbol ? "!" : string.Empty)};",
                        stmt => stmt.AddMetadata("handler", "instantiate"));
                    method.AddStatement($"await handler.HandleAsync(context.Message, context.CancellationToken);",
                        stmt => stmt.AddMetadata("handler", "execute"));
                    method.AddStatement($"await eventBus.FlushAllAsync(context.CancellationToken);",
                        stmt => stmt.AddMetadata("event-bus-flush", true));
                });
            })
            .AddClass($"WrapperConsumerDefinition", @class =>
            {
                @class.AddGenericParameter("THandler", out var tHandler);
                @class.AddGenericParameter("TMessage", out var tMessage);
                @class.WithBaseType($"ConsumerDefinition<WrapperConsumer<{tHandler}, {tMessage}>>");
                @class.AddGenericTypeConstraint(tMessage, g => g.AddType("class"));
                @class.AddGenericTypeConstraint(tHandler, g => g.AddType($"{this.GetIntegrationEventHandlerInterfaceName()}<{tMessage}>"));
                @class.AddConstructor(ctor =>
                {
                    ctor.AddParameter("IServiceProvider", "serviceProvider", parm => parm.IntroduceReadonlyField());
                });
                @class.AddMethod("void", "ConfigureConsumer", method =>
                {
                    method.Protected().Override();
                    method.AddParameter("IReceiveEndpointConfigurator", "endpointConfigurator");
                    method.AddParameter($"IConsumerConfigurator<WrapperConsumer<{tHandler}, {tMessage}>>", "consumerConfigurator");
                });
            });
    }

    private bool UseExplicitNullSymbol => Project.GetProject().NullableEnabled;

    [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

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