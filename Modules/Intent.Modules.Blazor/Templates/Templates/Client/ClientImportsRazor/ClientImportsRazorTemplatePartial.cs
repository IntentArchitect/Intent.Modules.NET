using System;
using System.Linq;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.ClientImportsRazor
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientImportsRazorTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.ClientImportsRazorTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="ClientImportsRazorTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ClientImportsRazorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, "_Imports").Configure(file =>
            {
                file.AddUsing("System.Net.Http");
                file.AddUsing("System.Net.Http.Json");
                file.AddUsing("Microsoft.AspNetCore.Components.Forms");
                file.AddUsing("Microsoft.AspNetCore.Components.Routing");
                file.AddUsing("Microsoft.AspNetCore.Components.Web");
                file.AddUsing("Microsoft.AspNetCore.Components.Web.Virtualization");
                file.AddUsing("Microsoft.JSInterop");
                file.AddUsing("static Microsoft.AspNetCore.Components.Web.RenderMode");

                // Two-project (Auto/Wasm): the model-generated MainLayout lives in .Client/Components/Layout and
                // resolves the co-located atoms (NavLinks/ThemeToggle) natively, but also injects <AppUserMenu/>.
                // .Components.Layout (MainLayout's own namespace) is always populated for MudBlazor two-project
                // apps, so that using is gated on MudBlazor alone.
                if (ExecutionContext.GetInstalledModules().Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor"))
                {
                    file.AddUsing($"{OutputTarget.GetNamespace()}.Components.Layout");

                    // The no-op AppUserMenu scaffold ships to a SIBLING .Client/Layout folder (kept out of
                    // Components/Layout on purpose, so the server's real AppUserMenu atoms-import doesn't collide),
                    // but only when Authentication is installed (see
                    // UserMenuDefaultClientStaticContentTemplateRegistration.Register) - that's the only case
                    // <AppUserMenu/> is referenced at all. Without Authentication the .Layout namespace is never
                    // generated, so importing it unconditionally would be RZ... CS0234 (namespace doesn't exist).
                    if (ExecutionContext.GetInstalledModules().Any(m => m.ModuleId == "Intent.Blazor.Authentication"))
                    {
                        file.AddUsing($"{OutputTarget.GetNamespace()}.Layout");
                    }
                }
            });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer();
        }

        /// <inheritdoc />
        public override void AfterTemplateRegistration()
        {
            if (!CanRunTemplate()) return;
            base.AfterTemplateRegistration();
            OutputTarget.GetProject().AddProperty("NoDefaultLaunchSettingsFile", "true");
            OutputTarget.GetProject().AddProperty("StaticWebAssetProjectMode", "Default");
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public IRazorFile RazorFile { get; }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            return RazorFile.GetConfig();
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public override string TransformText() => RazorFile.ToString();
    }
}
