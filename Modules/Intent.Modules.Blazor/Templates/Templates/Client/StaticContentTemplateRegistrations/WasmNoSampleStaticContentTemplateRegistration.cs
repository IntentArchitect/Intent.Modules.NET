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

namespace Intent.Modules.Blazor.Templates.Templates.Client.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class WasmNoSampleStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Templates.Templates.Client.StaticContentTemplateRegistrations.WasmNoSampleStaticContentTemplateRegistration";

        public WasmNoSampleStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "WasmNoSamplePages";


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
            // This condition is mirrored by TemplateHelper.ShipsAppCss, which AppRazorTemplate uses
            // to decide whether to emit the app.css <link>. Change one and change the other, or the
            // link and the file drift apart again.
            if (TemplateHelper.ComponentLibraryInstalled(application))
                return;
            if (!application.GetSettings().GetBlazor().RenderMode().IsInteractiveWebAssembly())
                return;

            base.Register(registry, application);
        }

    }
}