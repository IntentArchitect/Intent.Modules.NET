using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.VisualStudio.Projects.Api.CSharpProjectNETModelStereotypeExtensions.NETSettings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.Hangfire.Templates.HangfireDashboardAuthFilter
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class HangfireDashboardAuthFilterTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => HangfireDashboardAuthFilterTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new HangfireDashboardAuthFilterTemplate(outputTarget);
        }
    }
}