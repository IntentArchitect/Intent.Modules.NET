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
    /// Default (first-generation only) content for the InvalidPasswordReset page, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static InvalidPasswordReset.razor content files.
    /// </summary>
    internal static class InvalidPasswordResetPageContent
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
                <MudCard Class="ux-fade-in-up auth-form-shell" Style="animation-delay: 0.1s" Outlined="true">
                    <MudCardContent>
                        <MudText Typo="Typo.h5" Color="Color.Error" Class="mb-3">Invalid reset link</MudText>
                        <MudText Typo="Typo.body1">The password reset link is invalid.</MudText>
                    </MudCardContent>
                </MudCard>
                """;
        }

        private static string BuildBootstrapContent()
        {
            return """
                <div class="ux-form-narrow">
                    <section>
                        <h2 class="text-danger">Invalid password reset</h2>
                        <p>The password reset link is invalid.</p>
                    </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
        }
    }
}
