using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Shared.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Solace.Templates.IntegrationEventHandler
{
    [IntentManaged(Mode.Ignore)]
    public partial class IntegrationEventHandlerTemplate : IntegrationEventHandlerTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Eventing.Solace.IntegrationEventHandler";

        public IntegrationEventHandlerTemplate(IOutputTarget outputTarget, IntegrationEventHandlerModel model)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: model)
        {
        }
    }
}