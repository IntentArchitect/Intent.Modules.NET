using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using System.Linq;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the ForgotPasswordConfirmation page, seeded onto
    /// the modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static ForgotPasswordConfirmation.razor content files.
    /// </summary>
    internal static class ForgotPasswordConfirmationPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent()
                : BuildBootstrapContent();
        }

        private static string BuildMudBlazorContent()
        {
            return """
                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                    Elevation="0">
                    <MudText Typo="Typo.h4"
                        Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.ForwardToInbox"
                            Class="mr-2" />
                        Forgot password confirmation
                    </MudText>
                    <MudText Typo="Typo.body1"
                        Class="text-white opacity-90">
                        Check your inbox for the next steps to reset your password.
                    </MudText>
                </MudPaper>

                <MudCard Class="ux-fade-in-up auth-form-shell"
                    Style="animation-delay: 0.1s"
                    Outlined="true">
                    <MudCardContent>
                        <MudText Typo="Typo.h5"
                            Class="mb-3">
                            Email sent
                        </MudText>
                        <MudText Typo="Typo.body1">Please check your email to reset your password.</MudText>
                    </MudCardContent>
                </MudCard>
                """;
        }

        private static string BuildBootstrapContent()
        {
            return """
                <AccountHero Icon="mail-check"
                    Title="Check your email"
                    Subtitle="We've sent you a password reset link." />
                <div class="ux-form-narrow">
                    <section>
                        <p>Please check your email to reset your password.</p>
                    </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
        }
    }
}
