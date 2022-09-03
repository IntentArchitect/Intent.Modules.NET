using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventHandlerInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class IntegrationEventHandlerInterfaceTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.IntegrationEventHandlerInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationEventHandlerInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IIntegrationEventHandler",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}