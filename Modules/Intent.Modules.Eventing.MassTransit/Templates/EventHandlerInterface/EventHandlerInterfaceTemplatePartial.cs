using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Templates.EventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.EventHandlerInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class EventHandlerInterfaceTemplate : CSharpTemplateBase<MessageHandlerModel>
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.EventHandlerInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EventHandlerInterfaceTemplate(IOutputTarget outputTarget, MessageHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(EventMessageTemplate.TemplateId);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.TypeReference.Element.Name.ToPascalCase()}EventHandler",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetMessageName()
        {
            return GetTypeName(Model.TypeReference);
        }
    }
}