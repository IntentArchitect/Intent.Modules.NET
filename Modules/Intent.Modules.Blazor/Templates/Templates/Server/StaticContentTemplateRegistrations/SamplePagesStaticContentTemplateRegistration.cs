using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Server.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class SamplePagesStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Templates.Templates.Server.StaticContentTemplateRegistrations.SamplePagesStaticContentTemplateRegistration";

        public SamplePagesStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "SamplePages";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget)
        {
            return new Dictionary<string, string>
            {
                ["ApplicationName"] = outputTarget.ApplicationName(),
                ["Namespace"] = outputTarget.GetNamespace(),
                // The top-row user menu is always rendered. AppUserMenu is shipped by the Authentication
                // module (real account actions) or, when Auth isn't installed, by the base module as a no-op
                // scaffold (see the AppUserMenu static-content registrations) — so the reference always
                // resolves and this token is unconditional.
                ["AppUserMenu"] = "<AppUserMenu />"
            };
        }

        protected override OverwriteBehaviour GetDefaultOverrideBehaviour(IOutputTarget outputTarget)
        {
            return OverwriteBehaviour.OnceOff;
        }

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (TemplateHelper.ComponentLibraryInstalled(application))
                return;
            if (!application.GetSettings().GetBlazor().RenderMode().IsInteractiveServer())
                return;
            if (!application.GetSettings().GetBlazor().IncludeSamplePages())
                return;

            base.Register(registry, application);
        }
    }
}