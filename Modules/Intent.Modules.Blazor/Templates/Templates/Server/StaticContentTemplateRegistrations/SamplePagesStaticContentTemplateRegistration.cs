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

        private IApplication _application;

        public override string ContentSubFolder => "SamplePages";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget)
        {
            var authInstalled = _application?.InstalledModules
                .Any(im => im.ModuleId == "Intent.Blazor.Authentication") == true;

            return new Dictionary<string, string>
            {
                ["ApplicationName"] = outputTarget.ApplicationName(),
                ["Namespace"] = outputTarget.GetNamespace(),
                // AppUserMenu (Account/Shared, shipped by the Auth module) is only referenced when ASP.NET
                // Identity auth is installed — the base Blazor module must not couple to Auth for other apps.
                ["AppUserMenu"] = authInstalled ? "<AppUserMenu />" : ""
            };
        }

        protected override OverwriteBehaviour GetDefaultOverrideBehaviour(IOutputTarget outputTarget)
        {
            return OverwriteBehaviour.OnceOff;
        }

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            _application = application;
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