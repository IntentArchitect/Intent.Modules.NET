using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.DefaultContent;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AuthPageDefaultContentFactoryExtension : FactoryExtensionBase
    {
        // Extension priority band (AGENTS.md): factory extensions from other modules run at 500.
        private const int ExtensionPriority = 500;

        // Element metadata key stamped by the "Security Type" stereotype's page-tagging script onto
        // the modelled Login/Register/etc. Component/Page elements it creates.
        private const string PageIdMetadataKey = "blazor-auth-page-id";

        // Known auth page ids → their (now-removed) dedicated template pair's default content,
        // mechanically ported into DefaultContent/*PageContent. An unrecognised/unmapped page id is
        // left alone — the page just gets RazorComponentTemplate's generic (empty) default output.
        //
        // The "Security Type" stereotype's page-tagging script prefixes each of these six ids per
        // Authentication mode (e.g. "identity-login", "jwt-login", "oidc-login") because their
        // generated content genuinely differs per mode — today that difference is handled by each
        // *PageContent class branching internally (e.g. LoginPageContent checks IsAspnetcoreIdentity()),
        // so every mode-specific id for the same page still maps to the same content builder below.
        // This mapping only needs to change if/when that in-content branching is split into separate
        // content builders per mode.
        private static readonly Dictionary<string, (Func<RazorComponentTemplate, string> BuildRazorContent, Action<IBuildsCSharpMembers> BuildCodeBehind)> PageContentByPageId = new()
        {
            ["identity-login"] = (LoginPageContent.BuildRazorContent, LoginPageContent.BuildCodeBehind),
            ["jwt-login"] = (LoginPageContent.BuildRazorContent, LoginPageContent.BuildCodeBehind),
            ["oidc-login"] = (LoginPageContent.BuildRazorContent, LoginPageContent.BuildCodeBehind),
            ["identity-register"] = (RegisterPageContent.BuildRazorContent, RegisterPageContent.BuildCodeBehind),
            ["jwt-register"] = (RegisterPageContent.BuildRazorContent, RegisterPageContent.BuildCodeBehind),
            ["identity-forgot-password"] = (ForgotPasswordPageContent.BuildRazorContent, ForgotPasswordPageContent.BuildCodeBehind),
            ["jwt-forgot-password"] = (ForgotPasswordPageContent.BuildRazorContent, ForgotPasswordPageContent.BuildCodeBehind),
            ["identity-reset-password"] = (ResetPasswordPageContent.BuildRazorContent, ResetPasswordPageContent.BuildCodeBehind),
            ["jwt-reset-password"] = (ResetPasswordPageContent.BuildRazorContent, ResetPasswordPageContent.BuildCodeBehind),
            ["identity-confirm-email"] = (ConfirmEmailPageContent.BuildRazorContent, ConfirmEmailPageContent.BuildCodeBehind),
            ["jwt-confirm-email"] = (ConfirmEmailPageContent.BuildRazorContent, ConfirmEmailPageContent.BuildCodeBehind),
            ["identity-resend-email-confirmation"] = (ResendEmailConfirmationPageContent.BuildRazorContent, ResendEmailConfirmationPageContent.BuildCodeBehind),
            ["jwt-resend-email-confirmation"] = (ResendEmailConfirmationPageContent.BuildRazorContent, ResendEmailConfirmationPageContent.BuildCodeBehind),

            // Informational/simple Account pages: the stereotype's page-tagging script only ever
            // creates these under ASP.NET Core Identity mode, so their page ids carry no mode prefix.
            ["access-denied"] = (AccessDeniedPageContent.BuildRazorContent, AccessDeniedPageContent.BuildCodeBehind),
            ["forgot-password-confirmation"] = (ForgotPasswordConfirmationPageContent.BuildRazorContent, ForgotPasswordConfirmationPageContent.BuildCodeBehind),
            ["invalid-password-reset"] = (InvalidPasswordResetPageContent.BuildRazorContent, InvalidPasswordResetPageContent.BuildCodeBehind),
            ["invalid-user"] = (InvalidUserPageContent.BuildRazorContent, InvalidUserPageContent.BuildCodeBehind),
            ["lockout"] = (LockoutPageContent.BuildRazorContent, LockoutPageContent.BuildCodeBehind),
            ["register-confirmation"] = (RegisterConfirmationPageContent.BuildRazorContent, RegisterConfirmationPageContent.BuildCodeBehind),
            ["reset-password-confirmation"] = (ResetPasswordConfirmationPageContent.BuildRazorContent, ResetPasswordConfirmationPageContent.BuildCodeBehind),

            // Auth-flow Account pages: also Identity-only, so no mode prefix on their page ids.
            ["confirm-email-change"] = (ConfirmEmailChangePageContent.BuildRazorContent, ConfirmEmailChangePageContent.BuildCodeBehind),
            ["external-login"] = (ExternalLoginPageContent.BuildRazorContent, ExternalLoginPageContent.BuildCodeBehind),
            ["login-with-2fa"] = (LoginWith2faPageContent.BuildRazorContent, LoginWith2faPageContent.BuildCodeBehind),
            ["login-with-recovery-code"] = (LoginWithRecoveryCodePageContent.BuildRazorContent, LoginWithRecoveryCodePageContent.BuildCodeBehind),

            // Manage/* Account pages: also Identity-only, so no mode prefix on their page ids.
            ["manage"] = (ManageIndexPageContent.BuildRazorContent, ManageIndexPageContent.BuildCodeBehind),
            ["manage-email"] = (ManageEmailPageContent.BuildRazorContent, ManageEmailPageContent.BuildCodeBehind),
            ["manage-change-password"] = (ManageChangePasswordPageContent.BuildRazorContent, ManageChangePasswordPageContent.BuildCodeBehind),
            ["manage-set-password"] = (ManageSetPasswordPageContent.BuildRazorContent, ManageSetPasswordPageContent.BuildCodeBehind),
            ["manage-two-factor-authentication"] = (ManageTwoFactorAuthenticationPageContent.BuildRazorContent, ManageTwoFactorAuthenticationPageContent.BuildCodeBehind),
            ["manage-disable-2fa"] = (ManageDisable2faPageContent.BuildRazorContent, ManageDisable2faPageContent.BuildCodeBehind),
            ["manage-enable-authenticator"] = (ManageEnableAuthenticatorPageContent.BuildRazorContent, ManageEnableAuthenticatorPageContent.BuildCodeBehind),
            ["manage-reset-authenticator"] = (ManageResetAuthenticatorPageContent.BuildRazorContent, ManageResetAuthenticatorPageContent.BuildCodeBehind),
            ["manage-generate-recovery-codes"] = (ManageGenerateRecoveryCodesPageContent.BuildRazorContent, ManageGenerateRecoveryCodesPageContent.BuildCodeBehind),
            ["manage-personal-data"] = (ManagePersonalDataPageContent.BuildRazorContent, ManagePersonalDataPageContent.BuildCodeBehind),
            ["manage-delete-personal-data"] = (ManageDeletePersonalDataPageContent.BuildRazorContent, ManageDeletePersonalDataPageContent.BuildCodeBehind),
            ["manage-external-logins"] = (ManageExternalLoginsPageContent.BuildRazorContent, ManageExternalLoginsPageContent.BuildCodeBehind),

            // Shared (non-Page) components: tagged by the stereotype's shared() helper, so they carry
            // no Page stereotype and RazorComponentTemplate skips the @page/<PageTitle> injection for
            // them. "account-layout" remains on its dedicated template pair, but now follows the same
            // default-content builder pattern as these component seeds.
            ["status-message"] = (StatusMessageContent.BuildRazorContent, StatusMessageContent.BuildCodeBehind),

            ["manage-nav-menu"] = (ManageNavMenuContent.BuildRazorContent, ManageNavMenuContent.BuildCodeBehind),
            ["external-login-picker"] = (ExternalLoginPickerContent.BuildRazorContent, ExternalLoginPickerContent.BuildCodeBehind),
            ["show-recovery-codes"] = (ShowRecoveryCodesContent.BuildRazorContent, ShowRecoveryCodesContent.BuildCodeBehind),
            ["app-user-menu"] = (AppUserMenuContent.BuildRazorContent, AppUserMenuContent.BuildCodeBehind),
            ["account-hero"] = (AccountHeroContent.BuildRazorContent, AccountHeroContent.BuildCodeBehind),
            ["ux-field"] = (UxFieldContent.BuildRazorContent, UxFieldContent.BuildCodeBehind),
            ["ux-icon"] = (UxIconContent.BuildRazorContent, UxIconContent.BuildCodeBehind),
        };

        public override string Id => "Intent.Blazor.Authentication.AuthPageDefaultContentFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var pageTemplates = application.FindTemplateInstances<RazorComponentTemplate>(RazorComponentTemplate.TemplateId).ToList();
            var codeBehindTemplates = application.FindTemplateInstances<RazorComponentCodeBehindTemplate>(RazorComponentCodeBehindTemplate.TemplateId).ToList();

            foreach (var pageTemplate in pageTemplates)
            {
                if (!pageTemplate.Model.InternalElement.Metadata.TryGetValue(PageIdMetadataKey, out var pageId) ||
                    !PageContentByPageId.TryGetValue(pageId, out var content))
                {
                    // Unrecognised/unmapped page id: leave the generic template's default output as-is.
                    continue;
                }

                pageTemplate.DefaultContentOverride = content.BuildRazorContent(pageTemplate);


                var codeBehindTemplate = codeBehindTemplates.FirstOrDefault(
                    x => x.Model.InternalElement.Id == pageTemplate.Model.InternalElement.Id);

                codeBehindTemplate?.CSharpFile.AfterBuild(file => content.BuildCodeBehind(file.Classes.Single()), ExtensionPriority);
            }
        }
    }
}
