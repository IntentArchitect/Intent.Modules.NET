using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.MudBlazorUxThemeSyncSkill
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class MudBlazorUxThemeSyncSkillTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => MudBlazorUxThemeSyncSkillTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new MudBlazorUxThemeSyncSkillTemplate(outputTarget);
        }
    }
}