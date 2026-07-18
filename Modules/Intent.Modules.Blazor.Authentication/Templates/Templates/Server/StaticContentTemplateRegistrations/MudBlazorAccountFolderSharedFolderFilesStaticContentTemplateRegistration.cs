using System.Collections.Generic;
using System.Linq;
using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Api;
using Intent.Modules.Blazor.Authentication.FactoryExtensions;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class MudBlazorAccountFolderSharedFolderFilesStaticContentTemplateRegistration : AuthStaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations.MudBlazorAccountFolderSharedFolderFilesStaticContentTemplateRegistration";

        public MudBlazorAccountFolderSharedFolderFilesStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "ComponentsMudBlazor/Account/Shared";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => ReplacementsPrivate(outputTarget);

        [IntentIgnore]
        private Dictionary<string, string> ReplacementsPrivate(IOutputTarget outputTarget)
        {
            var replacements = new Dictionary<string, string>();

            replacements.Add("Namespace", outputTarget.GetNamespace().Replace("Components.Account.Shared", ""));

            // The shared layout lives in the server project for InteractiveServer, but in the .Client
            // project for InteractiveAuto / InteractiveWebAssembly. Emit the render-mode-correct layout
            // namespace so these components' @layout / @using resolve (otherwise Auto/Wasm hit CS0234).
            var layoutRoot = outputTarget.GetNamespace().Replace("Components.Account.Shared", "");
            replacements.Add("LayoutNamespace", outputTarget.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer()
                ? layoutRoot
                : $"{layoutRoot.TrimEnd('.')}.Client.");

            if (!outputTarget.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity"))
            {
                replacements.Add("IdentityClass", "ApplicationUser");
                // JWT apps have no server-side user-data namespace, so _Imports must not emit a
                // dangling `@using …Data` (a compile error). Non-Identity setups that still have a
                // Data namespace keep the using.
                var dataNamespace = $"{outputTarget.GetNamespace().Replace("Components.Account.Shared", "")}Data";

                var securityType = outputTarget.ExecutionContext.MetadataManager.GetAuthenticationType(outputTarget.ExecutionContext.GetApplicationConfig().Id);
                var isJwt = securityType.IsBearerTokenJWT();

                replacements.Add("NamespaceData", isJwt ? "" : $"@using {dataNamespace}");
                replacements.Add("IdentityClassNamespace", dataNamespace);
            }
            else
            {
                var startup = outputTarget.ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
                var identityClass = IdentityHelperExtensions.GetIdentityUserClassTuple(startup);
                replacements.Add("IdentityClass", identityClass.Name);
                replacements.Add("NamespaceData", $"@using {identityClass.Namespace}");
                replacements.Add("IdentityClassNamespace", identityClass.Namespace);
            }

            return replacements;
        }

        [IntentIgnore]
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            var auth = application.MetadataManager.GetAuthenticationType(application.Id);
            var mudBlazorInstalled = application.InstalledModules.Any(im => im.ModuleId == "Intent.Blazor.Components.MudBlazor");
            if (!mudBlazorInstalled)
            {
                return;
            }

            if (auth.IsBuiltInLoginASPNETIdentity())
            {
                // Identity: the full shared set (Manage layouts, ExternalLoginPicker, recovery codes,
                // AppUserMenu, StatusMessage, AccountLayout skin, _Imports…).
                RegisterAuthStaticContent(registry, application);
            }
            else if (auth.IsBearerTokenJWT())
            {
                // JWT: only the mode-independent account shell — the AccountLayout skin, StatusMessage
                // (used by the RazorBuilder login/register), and the _Imports wiring. The Identity-only
                // shared components (ManageLayout/NavMenu, ExternalLoginPicker, ShowRecoveryCodes,
                // AppUserMenu) are not relevant to JWT and stay out.
                RegisterAuthStaticContent(registry, application,
                    pathFilter: rel => rel is "AccountLayout.razor.css" or "StatusMessage.razor" or "_Imports.razor");
            }
        }
    }
}
