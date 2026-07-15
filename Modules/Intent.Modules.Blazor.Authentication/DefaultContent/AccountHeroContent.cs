using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the shared AccountHero component, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components/Account/Shared/AccountHero.razor.
    /// Bootstrap-only — the MudBlazor app uses MudPaper/MudIcon directly instead, so this component
    /// is never referenced (and was never generated) when MudBlazor is installed; no Mud variant needed.
    /// All state is declared in the inline @code block, so <see cref="BuildCodeBehind"/> is a no-op.
    /// </summary>
    internal static class AccountHeroContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            return """
                <div class="account-hero ux-fade-in @(Variant == "danger" ? "account-hero-danger" : null)">
                    <span class="account-hero-badge">
                        <UxIcon Name="@Icon" />
                    </span>
                    <div class="account-hero-text">
                        <h1 class="account-hero-title">@Title</h1>
                        @if (!string.IsNullOrEmpty(Subtitle))
                        {
                            <p class="account-hero-subtitle">@Subtitle</p>
                        }
                    </div>
                </div>

                @code {
                    [Parameter] public string Icon { get; set; } = "lock";
                    [Parameter] public string Title { get; set; } = "";
                    [Parameter] public string? Subtitle { get; set; }
                    [Parameter] public string? Variant { get; set; }
                }
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
        }
    }
}
