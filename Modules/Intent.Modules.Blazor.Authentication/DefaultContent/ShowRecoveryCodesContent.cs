using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using System.Linq;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the shared ShowRecoveryCodes component, seeded
    /// onto the modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Shared/ShowRecoveryCodes pair.
    /// Identity-only (the stereotype's page-tagging script never creates it under JWT/OIDC).
    /// </summary>
    internal static class ShowRecoveryCodesContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent()
                : BuildBootstrapContent();
        }

        public static string BuildStyleContent(RazorComponentTemplate template)
        {
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");
            return isMudBlazor ? MudBlazorStyle : BootstrapStyle;
        }

        private const string MudBlazorStyle = """
            .recovery-codes-container {
                display: flex;
                flex-wrap: wrap;
                gap: var(--space-2);
            }

            .recovery-codes-container ::deep .recovery-code {
                display: inline-block;
                padding: var(--space-1) var(--space-2);
                color: var(--text);
                background: var(--surface-2);
                border: var(--border-width) solid var(--border);
                border-radius: var(--radius-sm);
                font-family: var(--font-mono);
                font-size: var(--type-body-sm);
            }
            """;

        private const string BootstrapStyle = """
            .row {
                display: flex;
                flex-wrap: wrap;
                gap: var(--space-6);
                padding: var(--space-4);
            }

            .row ::deep .col-md-12 {
                flex: 1 1 100%;
                max-width: 100%;
            }

            .row ::deep .recovery-code {
                display: inline-block;
                margin: 0 var(--space-2) var(--space-2) 0;
                padding: var(--space-1) var(--space-2);
                color: var(--text);
                background: var(--surface-2);
                border: var(--border-width) solid var(--border);
                border-radius: var(--radius-sm);
            }
            """;

        private static string BuildMudBlazorContent()
        {
            return """
                <StatusMessage Message="@StatusMessage" />
                <MudText Typo="Typo.h5"
                    Class="mb-3">
                    Recovery codes
                </MudText>
                <MudAlert Severity="Severity.Warning"
                    Class="mb-3">
                    <MudText Typo="Typo.body1"><strong>Put these codes in a safe place.</strong></MudText>
                    <MudText Typo="Typo.body1">If you lose your device and don't have the recovery codes you will lose access to your account.</MudText>
                </MudAlert>
                <div class="recovery-codes-container">
                    @foreach (var recoveryCode in RecoveryCodes)
                    {
                        <code class="recovery-code">@recoveryCode</code>
                    }
                </div>
                """;
        }

        private static string BuildBootstrapContent()
        {
            return """
                <StatusMessage Message="@StatusMessage" />
                <h3>Recovery codes</h3>
                <div class="alert alert-warning"
                    role="alert">
                    <p>
                        <strong>Put these codes in a safe place.</strong>
                    </p>
                    <p>
                        If you lose your device and don't have the recovery codes you will lose access to your account.
                    </p>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        @foreach (var recoveryCode in RecoveryCodes)
                        {
                            <div>
                                <code class="recovery-code">@recoveryCode</code>
                            </div>
                        }
                    </div>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddProperty("string[]", "RecoveryCodes", p =>
            {
                p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.ParameterAttribute").RemoveSuffix("Attribute"));
                p.WithInitialValue("[]");
            });

            code.AddProperty("string?", "StatusMessage", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.ParameterAttribute").RemoveSuffix("Attribute")));
        }
    }
}
