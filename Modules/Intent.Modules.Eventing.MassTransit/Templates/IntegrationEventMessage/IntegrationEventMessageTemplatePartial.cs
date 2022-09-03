using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class IntegrationEventMessageTemplate : CSharpTemplateBase<MessageModel>
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.IntegrationEventMessage";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationEventMessageTemplate(IOutputTarget outputTarget, MessageModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name.RemoveSuffix("Event")}Event",
                @namespace: $"{Model.InternalElement.Package.Name.ToPascalCase()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}