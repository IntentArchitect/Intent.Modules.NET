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

namespace Intent.Modules.Blazor.Templates.Templates.Static.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class SamplePagesStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Templates.Templates.Static.StaticContentTemplateRegistrations.SamplePagesStaticContentTemplateRegistration";

        public SamplePagesStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "SamplePages";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
            ["ApplicationName"] = outputTarget.ApplicationName(),
            ["Namespace"] = outputTarget.GetNamespace()

        };

        protected override OverwriteBehaviour GetDefaultOverrideBehaviour(IOutputTarget outputTarget)
        {
            return OverwriteBehaviour.OnceOff;
        }

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (!application.GetSettings().GetBlazor().IncludeSamplePages() && NoComponentLibraryInstalled(application))
            {
                return;
            }
            base.Register(registry, application);
        }

        private bool NoComponentLibraryInstalled(IApplication application)
        {
#warning consolidate this with other one
            return application.InstalledModules.Any(x => x.ModuleId == "Intent.Modelers.UI.Core");//This is base component library
        }
    }
}