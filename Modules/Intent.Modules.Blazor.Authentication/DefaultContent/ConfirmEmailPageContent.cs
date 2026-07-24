using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using System.Linq;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the ConfirmEmail page, seeded onto the modelled
    /// <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old dedicated ConfirmEmailTemplate/ConfirmEmailCodeBehindTemplate pair.
    /// </summary>
    internal static class ConfirmEmailPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var authServiceInterfaceTemplate = template.ExecutionContext.FindTemplateInstance(AuthServiceInterfaceTemplate.TemplateId);
            var authServiceInterfaceBuilder = authServiceInterfaceTemplate as ICSharpFileBuilderTemplate;

            var redirectManager = template.GetIdentityRedirectManagerTemplateName();
            var authService = template.GetAuthServiceInterfaceTemplateName();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(redirectManager, authService, authServiceInterfaceBuilder.Namespace ?? "")
                : BuildBootstrapContent(redirectManager, authService, authServiceInterfaceBuilder.Namespace ?? "");
        }

        private static string BuildMudBlazorContent(string redirectManager, string authService, string authServiceNamespace)
        {
            return $$"""
                @using {{authServiceNamespace}}
                @inject {{redirectManager}} RedirectManager
                @inject {{authService}} AuthService

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                          Elevation="0">
                    <MudText Typo="Typo.h4"
                             Class="text-white font-weight-bold mb-2">
                        <MudIcon Icon="@Icons.Material.Filled.MarkEmailRead"
                                 Class="mr-2" />
                        Confirm email
                    </MudText>
                    <MudText Typo="Typo.body1"
                             Class="text-white opacity-90">
                        We are verifying your email address and completing your account setup.
                    </MudText>
                </MudPaper>

                <MudGrid Spacing="3">
                    <MudItem xs="12"
                             md="8"
                             lg="6">
                        <MudCard Class="ux-fade-in-up"
                                 Style="animation-delay: 0.1s">
                            <MudCardContent>
                                <MudText Typo="Typo.h5">Email confirmation status</MudText>
                                <MudText Typo="Typo.body2"
                                         Class="mb-4">
                                    The result of your email confirmation request is shown below.
                                </MudText>
                                <StatusMessage Message="@statusMessage" />
                                <MudStack Spacing="1"
                                          Class="mt-4">
                                    <MudLink Href="Account/Login">Continue to log in</MudLink>
                                </MudStack>
                            </MudCardContent>
                        </MudCard>
                    </MudItem>
                </MudGrid>
                """;
        }

        private static string BuildBootstrapContent(string redirectManager, string authService, string authServiceNamespace)
        {
            return $$"""
                @using {{authServiceNamespace}}
                @inject {{redirectManager}} RedirectManager
                @inject {{authService}} AuthService

                <AccountHero Icon="check-circle"
                             Title="Confirm email"
                             Subtitle="Email confirmation status." />

                <div class="ux-form-narrow">
                    <section>
                        <StatusMessage Message="@statusMessage" />
                    </section>
                </div>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            code.AddField("string?", "statusMessage", c => c.Private());

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext?"), "HttpContext", httpContext => httpContext.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));

            code.AddProperty("string?", "UserId", httpContext => httpContext.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("string?", "Code", httpContext => httpContext.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();

            onInitializedAsync.AddIfStatement("UserId is null || Code is null", @if =>
            {
                @if.AddStatement("RedirectManager.RedirectTo(\"\");");
            });

            onInitializedAsync.AddStatement("statusMessage = await AuthService.ConfirmEmail(UserId, Code);");
        }
    }
}
