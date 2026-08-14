using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using System.Linq;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the InvalidUser page, seeded onto the modelled
    /// <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static InvalidUser.razor content files.
    /// </summary>
    internal static class InvalidUserPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplateBase<ComponentModel> template)
        {
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent()
                : BuildBootstrapContent();
        }

        private static string BuildMudBlazorContent()
        {
            return """
                <StatusMessage />

                <MudCard Class="ux-fade-in-up auth-form-shell"
                    Style="animation-delay: 0.1s"
                    Outlined="true">
                    <MudCardContent>
                        <MudText Typo="Typo.h5"
                            Color="Color.Error"
                            Class="mb-3">
                            Invalid user
                        </MudText>
                        <MudText Typo="Typo.body1">The requested user could not be found or is no longer available.</MudText>
                    </MudCardContent>
                </MudCard>
                """;
        }

        private static string BuildBootstrapContent()
        {
            return """
                <div class="ux-form-narrow">
                    <StatusMessage />
                    <section>
                        <h2 class="text-danger">Invalid user</h2>
                        <p>The requested user could not be found or is no longer available.</p>
                    </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
        }
    }
}
