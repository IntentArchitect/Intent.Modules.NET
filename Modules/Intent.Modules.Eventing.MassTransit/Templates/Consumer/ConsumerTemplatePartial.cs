using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Templates.EventMessage;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.Consumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ConsumerTemplate : CSharpTemplateBase<MessageHandlerModel>
    {
        public const string TemplateId = "Intent.Modules.Eventing.MassTransit.Consumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ConsumerTemplate(IOutputTarget outputTarget, MessageHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MassTransitAbstractions);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.TypeReference.Element.Name.ToPascalCase()}Consumer",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetMessageName()
        {
            return GetTypeName(Model.TypeReference);
        }
    }
}