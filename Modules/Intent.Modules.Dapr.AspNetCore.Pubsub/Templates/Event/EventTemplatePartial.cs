using System.Collections.Generic;
using Intent.Dapr.AspNetCore.Pubsub.Api;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.Event
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class EventTemplate : CSharpTemplateBase<MessageModel>
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Pubsub.Event";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EventTemplate(IOutputTarget outputTarget, MessageModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MediatR);

            AddTypeSource("Application.Contract.Dto", "List<{0}>");
            AddTypeSource("Domain.Enum", "List<{0}>");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string PubsubName()
        {
            var pubsub = Model.GetDaprSettings()?.PubsubName();

            return !string.IsNullOrWhiteSpace(pubsub)
                ? $"\"{pubsub}\""
                : $"\"pubsub\"";
        }

        private string TopicName()
        {
            var topic = Model.GetDaprSettings()?.TopicName();

            return !string.IsNullOrWhiteSpace(topic)
                ? $"\"{topic}\""
                : $"nameof({ClassName})";
        }
    }
}