using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Authentication.DefaultContent;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Templates;
using Intent.Modules.Blazor.Templates.Templates.Client;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentStyle;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerComponent;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerComponentCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Server.ServerImportsRazor;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
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
        private static readonly Dictionary<string, (Func<RazorComponentTemplateBase<ComponentModel>, string> BuildRazorContent, Action<IBuildsCSharpMembers> BuildCodeBehind, Func<RazorComponentTemplateBase<ComponentModel>, string?>? BuildStyleContent)> PageContentByPageId = new()
        {
            ["identity-login"] = (LoginPageContent.BuildRazorContent, LoginPageContent.BuildCodeBehind, LoginPageContent.BuildStyleContent),
            ["jwt-login"] = (LoginPageContent.BuildRazorContent, LoginPageContent.BuildCodeBehind, LoginPageContent.BuildStyleContent),
            ["oidc-login"] = (LoginPageContent.BuildRazorContent, LoginPageContent.BuildCodeBehind, LoginPageContent.BuildStyleContent),
            ["identity-register"] = (RegisterPageContent.BuildRazorContent, RegisterPageContent.BuildCodeBehind, RegisterPageContent.BuildStyleContent),
            ["jwt-register"] = (RegisterPageContent.BuildRazorContent, RegisterPageContent.BuildCodeBehind, RegisterPageContent.BuildStyleContent),
            ["identity-forgot-password"] = (ForgotPasswordPageContent.BuildRazorContent, ForgotPasswordPageContent.BuildCodeBehind, ForgotPasswordPageContent.BuildStyleContent),
            ["jwt-forgot-password"] = (ForgotPasswordPageContent.BuildRazorContent, ForgotPasswordPageContent.BuildCodeBehind, ForgotPasswordPageContent.BuildStyleContent),
            ["identity-reset-password"] = (ResetPasswordPageContent.BuildRazorContent, ResetPasswordPageContent.BuildCodeBehind, ResetPasswordPageContent.BuildStyleContent),
            ["jwt-reset-password"] = (ResetPasswordPageContent.BuildRazorContent, ResetPasswordPageContent.BuildCodeBehind, ResetPasswordPageContent.BuildStyleContent),
            ["identity-confirm-email"] = (ConfirmEmailPageContent.BuildRazorContent, ConfirmEmailPageContent.BuildCodeBehind, null),
            ["jwt-confirm-email"] = (ConfirmEmailPageContent.BuildRazorContent, ConfirmEmailPageContent.BuildCodeBehind, null),
            ["identity-resend-email-confirmation"] = (ResendEmailConfirmationPageContent.BuildRazorContent, ResendEmailConfirmationPageContent.BuildCodeBehind, ResendEmailConfirmationPageContent.BuildStyleContent),
            ["jwt-resend-email-confirmation"] = (ResendEmailConfirmationPageContent.BuildRazorContent, ResendEmailConfirmationPageContent.BuildCodeBehind, ResendEmailConfirmationPageContent.BuildStyleContent),

            // Informational/simple Account pages: the stereotype's page-tagging script only ever
            // creates these under ASP.NET Core Identity mode, so their page ids carry no mode prefix.
            ["access-denied"] = (AccessDeniedPageContent.BuildRazorContent, AccessDeniedPageContent.BuildCodeBehind, null),
            ["forgot-password-confirmation"] = (ForgotPasswordConfirmationPageContent.BuildRazorContent, ForgotPasswordConfirmationPageContent.BuildCodeBehind, null),
            ["invalid-password-reset"] = (InvalidPasswordResetPageContent.BuildRazorContent, InvalidPasswordResetPageContent.BuildCodeBehind, null),
            ["invalid-user"] = (InvalidUserPageContent.BuildRazorContent, InvalidUserPageContent.BuildCodeBehind, null),
            ["lockout"] = (LockoutPageContent.BuildRazorContent, LockoutPageContent.BuildCodeBehind, null),
            ["register-confirmation"] = (RegisterConfirmationPageContent.BuildRazorContent, RegisterConfirmationPageContent.BuildCodeBehind, null),
            ["reset-password-confirmation"] = (ResetPasswordConfirmationPageContent.BuildRazorContent, ResetPasswordConfirmationPageContent.BuildCodeBehind, null),

            // Auth-flow Account pages: also Identity-only, so no mode prefix on their page ids.
            ["confirm-email-change"] = (ConfirmEmailChangePageContent.BuildRazorContent, ConfirmEmailChangePageContent.BuildCodeBehind, ConfirmEmailChangePageContent.BuildStyleContent),
            ["external-login"] = (ExternalLoginPageContent.BuildRazorContent, ExternalLoginPageContent.BuildCodeBehind, ExternalLoginPageContent.BuildStyleContent),
            ["login-with-2fa"] = (LoginWith2faPageContent.BuildRazorContent, LoginWith2faPageContent.BuildCodeBehind, LoginWith2faPageContent.BuildStyleContent),
            ["login-with-recovery-code"] = (LoginWithRecoveryCodePageContent.BuildRazorContent, LoginWithRecoveryCodePageContent.BuildCodeBehind, LoginWithRecoveryCodePageContent.BuildStyleContent),

            // Manage/* Account pages: also Identity-only, so no mode prefix on their page ids.
            ["manage"] = (ManageIndexPageContent.BuildRazorContent, ManageIndexPageContent.BuildCodeBehind, ManageIndexPageContent.BuildStyleContent),
            ["manage-email"] = (ManageEmailPageContent.BuildRazorContent, ManageEmailPageContent.BuildCodeBehind, ManageEmailPageContent.BuildStyleContent),
            ["manage-change-password"] = (ManageChangePasswordPageContent.BuildRazorContent, ManageChangePasswordPageContent.BuildCodeBehind, ManageChangePasswordPageContent.BuildStyleContent),
            ["manage-set-password"] = (ManageSetPasswordPageContent.BuildRazorContent, ManageSetPasswordPageContent.BuildCodeBehind, ManageSetPasswordPageContent.BuildStyleContent),
            ["manage-two-factor-authentication"] = (ManageTwoFactorAuthenticationPageContent.BuildRazorContent, ManageTwoFactorAuthenticationPageContent.BuildCodeBehind, ManageTwoFactorAuthenticationPageContent.BuildStyleContent),
            ["manage-disable-2fa"] = (ManageDisable2faPageContent.BuildRazorContent, ManageDisable2faPageContent.BuildCodeBehind, ManageDisable2faPageContent.BuildStyleContent),
            ["manage-enable-authenticator"] = (ManageEnableAuthenticatorPageContent.BuildRazorContent, ManageEnableAuthenticatorPageContent.BuildCodeBehind, ManageEnableAuthenticatorPageContent.BuildStyleContent),
            ["manage-reset-authenticator"] = (ManageResetAuthenticatorPageContent.BuildRazorContent, ManageResetAuthenticatorPageContent.BuildCodeBehind, ManageResetAuthenticatorPageContent.BuildStyleContent),
            ["manage-generate-recovery-codes"] = (ManageGenerateRecoveryCodesPageContent.BuildRazorContent, ManageGenerateRecoveryCodesPageContent.BuildCodeBehind, ManageGenerateRecoveryCodesPageContent.BuildStyleContent),
            ["manage-personal-data"] = (ManagePersonalDataPageContent.BuildRazorContent, ManagePersonalDataPageContent.BuildCodeBehind, ManagePersonalDataPageContent.BuildStyleContent),
            ["manage-delete-personal-data"] = (ManageDeletePersonalDataPageContent.BuildRazorContent, ManageDeletePersonalDataPageContent.BuildCodeBehind, ManageDeletePersonalDataPageContent.BuildStyleContent),
            ["manage-external-logins"] = (ManageExternalLoginsPageContent.BuildRazorContent, ManageExternalLoginsPageContent.BuildCodeBehind, ManageExternalLoginsPageContent.BuildStyleContent),

            // Shared (non-Page) components: tagged by the stereotype's shared() helper, so they carry
            // no Page stereotype and RazorComponentTemplate skips the @page/<PageTitle> injection for
            // them. "account-layout" remains on its dedicated template pair, but now follows the same
            // default-content builder pattern as these component seeds.
            ["status-message"] = (StatusMessageContent.BuildRazorContent, StatusMessageContent.BuildCodeBehind, null),

            ["manage-nav-menu"] = (ManageNavMenuContent.BuildRazorContent, ManageNavMenuContent.BuildCodeBehind, ManageNavMenuContent.BuildStyleContent),
            ["external-login-picker"] = (ExternalLoginPickerContent.BuildRazorContent, ExternalLoginPickerContent.BuildCodeBehind, ExternalLoginPickerContent.BuildStyleContent),
            ["show-recovery-codes"] = (ShowRecoveryCodesContent.BuildRazorContent, ShowRecoveryCodesContent.BuildCodeBehind, ShowRecoveryCodesContent.BuildStyleContent),
            ["app-user-menu"] = (AppUserMenuContent.BuildRazorContent, AppUserMenuContent.BuildCodeBehind, AppUserMenuContent.BuildStyleContent),
            ["account-hero"] = (AccountHeroContent.BuildRazorContent, AccountHeroContent.BuildCodeBehind, null),
            ["ux-field"] = (UxFieldContent.BuildRazorContent, UxFieldContent.BuildCodeBehind, null),
            ["ux-icon"] = (UxIconContent.BuildRazorContent, UxIconContent.BuildCodeBehind, null),
        };

        public override string Id => "Intent.Blazor.Authentication.AuthPageDefaultContentFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var pageTemplates = application.FindTemplateInstances<RazorComponentTemplateBase<ComponentModel>>(RazorComponentTemplate.TemplateId)
                .Concat(application.FindTemplateInstances<RazorComponentTemplateBase<ComponentModel>>(RazorServerComponentTemplate.TemplateId))
                .ToList();
            var codeBehindTemplates = application.FindTemplateInstances<CSharpTemplateBase<ComponentModel>>(RazorComponentCodeBehindTemplate.TemplateId)
                .Concat(application.FindTemplateInstances<CSharpTemplateBase<ComponentModel>>(RazorServerComponentCodeBehindTemplate.TemplateId))
                .ToList();
            var styleTemplates = application.FindTemplateInstances<RazorComponentStyleTemplate>(RazorComponentStyleTemplate.TemplateId).ToList();

            // Non-Identity mode: pages reference the generated ApplicationUser class by its bare name
            // (e.g. "SignInManager<ApplicationUser>") in .razor markup. RazorComponentTemplate's
            // TransformText() never consults a template's CSharpFile.Usings when rendering that markup,
            // so the only way to make the type resolve is via the project-wide _Imports.razor.
            if (!application.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity"))
            {
                var applicationUserTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(ApplicationUserTemplate.TemplateId);
                var imports = application.FindTemplateInstance<IRazorFileTemplate>(ServerImportsRazorTemplate.TemplateId);

                if (applicationUserTemplate is not null)
                {
                    imports?.RazorFile.AddUsing(applicationUserTemplate.Namespace);
                }
            }

            foreach (var pageTemplate in pageTemplates)
            {
                if (!pageTemplate.Model.InternalElement.Metadata.TryGetValue(PageIdMetadataKey, out var pageId) ||
                    !PageContentByPageId.TryGetValue(pageId, out var content))
                {
                    // Unrecognised/unmapped page id: leave the generic template's default output as-is.
                    continue;
                }

                ((IAuthPageRazorTemplate)pageTemplate).DefaultContentOverride = content.BuildRazorContent(pageTemplate);


                var codeBehindTemplate = codeBehindTemplates.FirstOrDefault(
                    x => x.Model.InternalElement.Id == pageTemplate.Model.InternalElement.Id);

                ((ICSharpFileBuilderTemplate?)codeBehindTemplate)?.CSharpFile.AfterBuild(file => content.BuildCodeBehind(file.Classes.Single()), ExtensionPriority);

                if (content.BuildStyleContent is null)
                {
                    continue;
                }

                var styleTemplate = styleTemplates.FirstOrDefault(
                    x => x.Model.InternalElement.Id == pageTemplate.Model.InternalElement.Id);

                var styleContent = content.BuildStyleContent(pageTemplate);
                if (styleTemplate is not null && !string.IsNullOrEmpty(styleContent))
                {
                    styleTemplate.StyleContentOverride = styleContent;
                }
            }
        }
    }
}
