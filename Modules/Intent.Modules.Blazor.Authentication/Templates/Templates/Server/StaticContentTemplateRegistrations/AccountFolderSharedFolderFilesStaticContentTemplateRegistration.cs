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
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations
{
    // Signature = Mode.Merge so the Software Factory preserves the hand-set base class
    // (AuthStaticContentTemplateRegistration, which provides RegisterAuthStaticContent) on regeneration.
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class AccountFolderSharedFolderFilesStaticContentTemplateRegistration : AuthStaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations.AccountFolderSharedFolderFilesStaticContentTemplateRegistration";

        public AccountFolderSharedFolderFilesStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "Components/Account/Shared";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => ReplacementsPrivate(outputTarget);

        [IntentIgnore]
        private static string GetAccountNamespaceRoot(IOutputTarget outputTarget)
        {
            return outputTarget.GetNamespace()
                .Replace("Components.Account.Shared", string.Empty)
                .Replace("Pages.Account.Shared", string.Empty);
        }

        [IntentIgnore]
        private Dictionary<string, string> ReplacementsPrivate(IOutputTarget outputTarget)
        {
            var replacements = new Dictionary<string, string>();
            var accountNamespaceRoot = GetAccountNamespaceRoot(outputTarget);

            replacements.Add("Namespace", accountNamespaceRoot);

            // The shared layout lives in the server project for InteractiveServer, but in the .Client
            // project for InteractiveAuto / InteractiveWebAssembly. Emit the render-mode-correct layout
            // namespace so these components' @layout / @using resolve (otherwise Auto/Wasm hit CS0234).
            var layoutRoot = accountNamespaceRoot;
            replacements.Add("LayoutNamespace", outputTarget.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer()
                ? layoutRoot
                : $"{layoutRoot.TrimEnd('.')}.Client.");

            if (!outputTarget.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity"))
            {
                replacements.Add("IdentityClass", "ApplicationUser");
                // JWT/SSO apps have no server-side user-data namespace, so _Imports must not emit a
                // dangling `@using …Data`. Only Built-in Login (ASP.NET Identity) generates that namespace.
                var dataNamespace = $"{accountNamespaceRoot}Data";

                var securityType = outputTarget.ExecutionContext.MetadataManager.GetAuthenticationType(outputTarget.ExecutionContext.GetApplicationConfig().Id);
                var hasDataNamespace = securityType.IsBuiltInLoginASPNETIdentity();

                replacements.Add("NamespaceData", hasDataNamespace ? $"@using {dataNamespace}" : "");
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

            if (auth.IsBuiltInLoginASPNETIdentity())
            {
                if (!mudBlazorInstalled)
                {
                    RegisterAuthStaticContent(registry, application);
                    return;
                }

                // MudBlazor ships the shared .razor markup; this registration ships the .cs code-behind.
                RegisterAuthStaticContent(registry, application, ext => ext == ".cs");
                return;
            }

            if (auth.IsBearerTokenJWT())
            {
                // JWT ships only the Identity-free account shell — the SignInManager-dependent
                // ExternalLoginPicker/ManageNavMenu and the Manage components stay Identity-only.
                if (!mudBlazorInstalled)
                {
                    // Non-MudBlazor: the shared UX primitives + layout css/wiring (their code-behind included).
                    RegisterAuthStaticContent(registry, application,
                        pathFilter: rel => rel is "AccountHero.razor" or "UxField.razor" or "UxIcon.razor"
                        or "StatusMessage.razor" or "StatusMessage.razor.cs" or "AccountLayout.razor.css" or "_Imports.razor");
                    return;
                }

                // MudBlazor ships the shell .razor itself; here we add only StatusMessage's code-behind.
                RegisterAuthStaticContent(registry, application,
                    pathFilter: rel => rel == "StatusMessage.razor.cs");
            }
        }
    }
}
