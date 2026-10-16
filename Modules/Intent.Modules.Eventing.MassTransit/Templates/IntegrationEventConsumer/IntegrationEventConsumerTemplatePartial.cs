using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventConsumer;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class IntegrationEventConsumerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Eventing.MassTransit.IntegrationEventConsumer";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public IntegrationEventConsumerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        var tHandler = new CSharpGenericParameter("THandler");

        ConsumerHelper.AddConsumerDependencies(this);
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath());
        ConsumerHelper.AddConsumerClass(this,
            baseName: "IntegrationEvent",
            configureClass: (@class, tMessage) =>
            {
                @class.GenericParameters.Insert(0, tHandler);
                @class.AddGenericTypeConstraint(tHandler, c => c.AddType($"{this.GetIntegrationEventHandlerInterfaceName()}<{tMessage}>"));
            },
            configureConsumeMethod: (@class, method, tMessage) =>
            {
                method.AddStatement($"var handler = _serviceProvider.GetRequiredService<{tHandler}>();",
                    stmt => stmt.AddMetadata("handler", "instantiate"));
                method.AddStatement($"await handler.HandleAsync(context.Message, context.CancellationToken);",
                    stmt => stmt.AddMetadata("handler", "execute"));
            },
            applyStandardUnitOfWorkLogic: true);
        ConsumerHelper.AddConsumerDefinitionClass(this,
            baseName: "IntegrationEvent");

        // Runs after every OnBuild (default priority 0) so the consume body is complete (unit of work, flush) when wrapped.
        CSharpFile.AfterBuild(_ => ClaimMediatorFlushOwner(), 500);
    }

    private const string MediatorFlushOwnerTemplateId = "Intent.Application.Mediator.MessageBusFlushOwner";

    /// <summary>
    /// Mediator dispatch (D18): the consumer is the outermost owner of the scoped message bus buffer. It claims
    /// <c>MessageBusFlushOwner</c> before handing the message to the integration event handler, so the
    /// <c>MessageBusPublishBehaviour</c> of any command the handler sends sees a nested owner and does not flush
    /// the bus ahead of the consumer's own unit of work. The consumer's existing flush, which already runs
    /// after the transaction completes, is left exactly where it is.
    /// <para>
    /// Referenced by template id only (no project reference on Intent.Application.Mediator), and only emitted when
    /// that owner is generated for this application, so MediatR and Wolverine dispatch output is unchanged.
    /// </para>
    /// </summary>
    private void ClaimMediatorFlushOwner()
    {
        // Under the EF outbox the flush lives in the DbContext's SaveChanges and the Mediator pipeline's flush item
        // is stripped by MessageBusInteropExtension, so there is no nested flush to guard against.
        if (ExecutionContext.Settings.GetMassTransitMessageBusSettings().OutboxPattern().IsEntityFramework())
        {
            return;
        }

        // FindTemplateInstance alone is not enough: the owner template of this always-installed module resolves
        // its type name even when its own CanRunTemplate() skips it (no message bus role resolved).
        var ownerTemplate = ExecutionContext.FindTemplateInstance<IIntentTemplate>(MediatorFlushOwnerTemplateId);
        if (ownerTemplate == null || !ownerTemplate.CanRunTemplate() || !TryGetTypeName(MediatorFlushOwnerTemplateId, out var ownerTypeName))
        {
            return;
        }

        var consume = CSharpFile.Classes.First().FindMethod("Consume");
        if (consume == null)
        {
            return;
        }

        var statements = consume.Statements.ToList();
        var instantiateIndex = statements.FindIndex(stmt => stmt.TryGetMetadata("handler", out string value) && value == "instantiate");
        if (instantiateIndex < 0)
        {
            return;
        }

        var body = statements.Skip(instantiateIndex).ToList();
        foreach (var statement in body)
        {
            consume.Statements.Remove(statement);
        }

        // A consumer always runs in its own DI scope, so it is the outermost owner and the Enter() result is not needed.
        consume.AddStatement($"var flushOwner = _serviceProvider.GetRequiredService<{ownerTypeName}>();");
        consume.AddStatement("flushOwner.Enter();");
        consume.AddTryBlock(tryBlock => tryBlock.AddStatements(body));
        consume.AddFinallyBlock(finallyBlock => finallyBlock.AddStatement("flushOwner.Exit();"));
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