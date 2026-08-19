using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Templates.Templates.Client;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the shared UxField component, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components/Account/Shared/UxField.razor.
    /// Bootstrap-only — the MudBlazor app uses MudTextField directly instead, so this component is
    /// never referenced (and was never generated) when MudBlazor is installed; no Mud variant needed.
    /// All state is declared in the inline @code block, so <see cref="BuildCodeBehind"/> is a no-op.
    /// </summary>
    internal static class UxFieldContent
    {
        public static string BuildRazorContent(RazorComponentTemplateBase<ComponentModel> template)
        {
            return """
                @* Form field: label above + filled shell with an optional leading icon.
                Bootstrap-free; mirrors the MudBlazor app's .login-input-* fields.
                ChildContent is the input (placed inside the shell); Validation renders below. *@
                <div class="ux-field">
                    @if (!string.IsNullOrEmpty(Label))
                    {
                        <label class="ux-field-label"
                            for="@For">
                            @Label
                        </label>
                    }
                    <div class="ux-field-shell">
                        @if (!string.IsNullOrEmpty(Icon))
                        {
                            <UxIcon Name="@Icon"
                                Class="ux-field-icon" />
                        }
                        @ChildContent
                    </div>
                </div>

                @code {
                    [Parameter] public string? Label { get; set; }
                    [Parameter] public string? Icon { get; set; }
                    [Parameter] public string? For { get; set; }
                    [Parameter] public RenderFragment? ChildContent { get; set; }
                }
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
        }
    }
}
