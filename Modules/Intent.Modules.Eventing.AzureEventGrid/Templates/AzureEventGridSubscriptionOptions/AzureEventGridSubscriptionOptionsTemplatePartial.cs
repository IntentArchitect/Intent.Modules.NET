using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridSubscriptionOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class AzureEventGridSubscriptionOptionsTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Eventing.AzureEventGrid.AzureEventGridSubscriptionOptions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureEventGridSubscriptionOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AzureMessagingEventGrid(outputTarget));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AzureEventGridSubscriptionOptions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}