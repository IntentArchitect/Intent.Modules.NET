using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class UserMenuDefaultStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations.UserMenuDefaultStaticContentTemplateRegistration";

        public UserMenuDefaultStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "UserMenuDefault";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>();

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            // Single-project (InteractiveServer) only: this no-op scaffold ships beside MainLayout in
            // Components/Layout so the model-generated Mud layout's injected <AppUserMenu/> resolves. Two-project
            // render modes (InteractiveAuto / InteractiveWebAssembly) are handled by
            // UserMenuDefaultClientStaticContentTemplateRegistration, which ships the flat Layout/ variant to the
            // .Client (beside the .Client MainLayout) and keeps the shared-atoms namespace (Components/Layout)
            // free of an AppUserMenu so the server static-SSR shells don't collide.
            if (!application.GetSettings().GetBlazor().RenderMode().IsInteractiveServer())
            {
                return;
            }

            // ASP.NET Core Identity: the Authentication module ships the real AppUserMenu to
            // Components/Account/Shared and the server _Imports surfaces it to MainLayout, so this no-op scaffold
            // must NOT also ship — it would make <AppUserMenu/> ambiguous in MainLayout and ManageLayout (RZ9985).
            // JWT/OIDC (Auth installed but ships no real menu) and non-Auth still get the scaffold so
            // <AppUserMenu/> resolves. The "Authentication Type" setting lives on the base Blazor settings group
            // (id from Intent.Blazor.Authentication's BlazorSettingsExtensions.Authentication()), so it is
            // readable here WITHOUT a reference to the Authentication module.
            var authenticationType = application.GetSettings().GetBlazor().GetSetting("5ec4a775-6208-405b-b66f-0dd5c6e591bb")?.Value;
            if (authenticationType == "aspnetcore-identity")
            {
                return;
            }

            base.Register(registry, application);
        }
    }
}
