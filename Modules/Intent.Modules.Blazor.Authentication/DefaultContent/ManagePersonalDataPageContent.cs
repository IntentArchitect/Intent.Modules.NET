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
    /// Default (first-generation only) content for the Manage/PersonalData page, seeded onto the
    /// modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/PersonalData pair.
    /// </summary>
    internal static class ManagePersonalDataPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var userAccessor = template.GetIdentityUserAccessorTemplateName();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(userAccessor)
                : BuildBootstrapContent(userAccessor);
        }

        public static string? BuildStyleContent(RazorComponentTemplate template)
        {
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");
            return isMudBlazor ? MudBlazorStyle : null;
        }

        private const string MudBlazorStyle = """
            .auth-form-shell {
                max-width: 720px;
                box-shadow: var(--shadow-2);
                border-radius: var(--radius-xl);
            }

            .personal-data-actions {
                display: flex;
                flex-wrap: wrap;
                gap: var(--space-3);
                align-items: center;
            }
            """;

        private static string BuildMudBlazorContent(string userAccessor)
        {
            return $$"""
                @using Microsoft.AspNetCore.Authorization
                @inject {{userAccessor}} UserAccessor

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                    Elevation="0">
                    <MudText Typo="Typo.h4"
                        Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.FolderShared"
                            Class="mr-2" />
                        Personal data
                    </MudText>
                    <MudText Typo="Typo.body1"
                        Class="text-white opacity-90">
                        Review, download, or permanently delete the personal data linked to your account.
                    </MudText>
                </MudPaper>

                <StatusMessage />

                <MudCard Class="ux-fade-in-up auth-form-shell"
                    Style="animation-delay: 0.1s"
                    Outlined="true">
                    <MudCardContent>
                        <MudText Typo="Typo.h5"
                            Class="mb-3">
                            Personal data
                        </MudText>
                        <MudText Typo="Typo.body1"
                            Class="mb-3">
                            Your account contains personal data that you have given us. This page allows you to download or delete that data.
                        </MudText>
                        <MudAlert Severity="Severity.Warning"
                            Class="mb-4">
                            Deleting this data will permanently remove your account, and this cannot be recovered.
                        </MudAlert>

                        <div class="personal-data-actions">
                            <form action="Account/Manage/DownloadPersonalData"
                                method="post">
                                <AntiforgeryToken />
                                <MudButton ButtonType="ButtonType.Submit"
                                    Variant="Variant.Filled"
                                    Color="Color.Primary"
                                    StartIcon="@Icons.Material.Filled.Download">
                                    Download
                                </MudButton>
                            </form>
                            <MudButton Href="Account/Manage/DeletePersonalData"
                                Variant="Variant.Filled"
                                Color="Color.Error"
                                StartIcon="@Icons.Material.Filled.Delete">
                                Delete
                            </MudButton>
                        </div>
                    </MudCardContent>
                </MudCard>

                """;
        }

        private static string BuildBootstrapContent(string userAccessor)
        {
            return $$"""
                @inject {{userAccessor}} UserAccessor

                <StatusMessage />
                <div class="ux-section-head">
                    <h3>Personal data</h3>
                    <p class="ux-section-subtitle">Your account contains personal data that you have given us. This page lets you download or delete that data.</p>
                </div>
                <div class="ux-callout ux-callout-warning"
                    role="alert">
                    <span class="ux-callout-icon"><UxIcon Name="alert" /></span>
                    <div class="ux-callout-body">
                        <strong>Deleting this data will permanently remove your account, and this cannot be recovered.</strong>
                    </div>
                </div>
                <div class="ux-button-row">
                    <form action="Account/Manage/DownloadPersonalData"
                        method="post">
                        <AntiforgeryToken />
                        <button class="btn btn-primary"
                            type="submit">
                            <UxIcon Name="download" />
                            Download
                        </button>
                    </form>
                    <a href="Account/Manage/DeletePersonalData"
                        class="btn btn-outline-danger">
                        <UxIcon Name="trash" />
                        Delete
                    </a>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext?"), "HttpContext", p => p.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();
            onInitializedAsync.AddStatement("_ = await UserAccessor.GetRequiredUserAsync(HttpContext);");
        }
    }
}
