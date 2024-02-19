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