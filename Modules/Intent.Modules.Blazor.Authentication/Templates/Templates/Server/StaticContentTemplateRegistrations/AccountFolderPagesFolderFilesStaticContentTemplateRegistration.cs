using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Blazor.Authentication.Api;
using Intent.Engine;
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
    public class AccountFolderPagesFolderFilesStaticContentTemplateRegistration : AuthStaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations.AccountFolderPagesFolderFilesStaticContentTemplateRegistration";

        public AccountFolderPagesFolderFilesStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "Components/Account";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => ReplacementsPrivate(outputTarget);

        [IntentIgnore]
        private static string GetAccountNamespaceRoot(IOutputTarget outputTarget)
        {
            return outputTarget.GetNamespace()
                .Replace("Components.Account.Pages.Manage", string.Empty)
                .Replace("Components.Account.Pages", string.Empty)
                .Replace("Pages.Account.Manage", string.Empty)
                .Replace("Pages.Account", string.Empty);
        }

        [IntentIgnore]
        private Dictionary<string, string> ReplacementsPrivate(IOutputTarget outputTarget)
        {
            var replacements = new Dictionary<string, string>();
            var accountNamespaceRoot = GetAccountNamespaceRoot(outputTarget);

            replacements.Add("Namespace", accountNamespaceRoot);

            if (!outputTarget.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity"))
            {
                replacements.Add("IdentityClass", "ApplicationUser");
                // JWT apps have no server-side user-data namespace, so _Imports must not emit a
                // dangling `@using …Data`. Non-Identity setups that still have a Data namespace keep it.
                var dataNamespace = $"{accountNamespaceRoot}Data";

                var securityType = outputTarget.ExecutionContext.MetadataManager.GetAuthenticationType(outputTarget.ExecutionContext.GetApplicationConfig().Id);
                var isJwt = securityType.IsJWT();

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

            if (auth.IsASPNETCoreIdentity())
            {
                if (!mudBlazorInstalled)
                {
                    RegisterAuthStaticContent(registry, application);
                    return;
                }

                RegisterAuthStaticContent(registry, application, ext => ext == ".cs");
                return;
            }

            if (auth.IsJWT() && !mudBlazorInstalled)
            {
                // Non-MudBlazor JWT: only the account-pages layout wiring (_Imports → @layout AccountLayout).
                // JWT's real pages are RazorBuilder templates; the Identity-only static pages stay out.
                RegisterAuthStaticContent(registry, application,
                    pathFilter: rel => rel == "_Imports.razor");
            }
        }
    }
}
