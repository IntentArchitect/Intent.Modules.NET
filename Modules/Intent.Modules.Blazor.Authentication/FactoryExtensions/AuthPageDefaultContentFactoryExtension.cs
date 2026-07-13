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
