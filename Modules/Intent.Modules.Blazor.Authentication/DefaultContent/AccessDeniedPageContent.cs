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
    /// Default (first-generation only) content for the AccessDenied page, seeded onto the modelled
    /// <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static AccessDenied.razor content files.
    /// </summary>
    internal static class AccessDeniedPageContent
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
                <MudCard Class="ux-fade-in-up auth-form-shell"
                    Style="animation-delay: 0.1s"
                    Outlined="true">
                    <MudCardContent>
                        <MudText Typo="Typo.h5"
                            Color="Color.Error"
                            Class="mb-3">
                            Access denied
                        </MudText>
                        <MudText Typo="Typo.body1">You do not have access to this resource.</MudText>
                    </MudCardContent>
                </MudCard>
                """;
        }

        private static string BuildBootstrapContent()
        {
            return """
                <div class="ux-form-narrow">
                    <section>
                        <h2 class="text-danger">Access denied</h2>
                        <p>You do not have access to this resource.</p>
                    </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
        }
    }
}
