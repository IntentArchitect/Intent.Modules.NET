using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.AI.Blazor.Templates.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class MudBlazorTemplateThemeCSSStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.AI.Blazor.Templates.StaticContentTemplateRegistrations.MudBlazorTemplateThemeCSSStaticContentTemplateRegistration";

        public MudBlazorTemplateThemeCSSStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "MudBlazor";


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
            if (!application.GetInstalledModules().Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor"))
                return;

            base.Register(registry, application);
        }
    }
}