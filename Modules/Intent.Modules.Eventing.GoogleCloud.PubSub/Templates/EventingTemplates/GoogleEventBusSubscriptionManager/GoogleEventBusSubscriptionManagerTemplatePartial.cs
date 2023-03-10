using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.EventingTemplates.GoogleEventBusSubscriptionManager
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class GoogleEventBusSubscriptionManagerTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Eventing.GoogleCloud.PubSub.EventingTemplates.GoogleEventBusSubscriptionManager";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GoogleEventBusSubscriptionManagerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.GoogleCloudPubSubV1);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"GoogleEventBusSubscriptionManager",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}