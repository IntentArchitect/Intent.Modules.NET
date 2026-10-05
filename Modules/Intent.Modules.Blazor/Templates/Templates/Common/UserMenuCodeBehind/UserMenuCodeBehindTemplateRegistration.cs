using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Common.UserMenuCodeBehind
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class UserMenuCodeBehindTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => UserMenuCodeBehindTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new UserMenuCodeBehindTemplate(outputTarget);
        }

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            // UserMenu.razor ships from MudBlazor's ThemeArtifacts unconditionally, and from this module's
            // ThemeToggle content only when Theme Toggle is enabled. The code-behind must follow the markup.
            var mudBlazorInstalled = application.InstalledModules
                .Any(module => module.ModuleId == "Intent.Blazor.Components.MudBlazor");

            if (!mudBlazorInstalled && !application.Settings.GetBlazor().EnableThemeToggle())
            {
                return;
            }

            base.Register(registry, application);
        }
    }
}
