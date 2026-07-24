using Intent.Modules.Blazor.Authentication.FactoryExtensions;
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
    /// Default (first-generation only) content for the Manage/Email page, seeded onto the modelled
    /// <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Pages/Manage/Email pair.
    /// </summary>
    internal static class ManageEmailPageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var identityClass = template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(template) :
                "ApplicationUser";
            var userAccessor = template.GetIdentityUserAccessorTemplateName();
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent(identityClass, userAccessor)
                : BuildBootstrapContent(identityClass, userAccessor);
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

            .auth-form-shell ::deep .mud-input-control-input-container,
            .auth-form-shell ::deep .mud-input-slot {
            background: var(--surface-2);
            }

            .auth-form-shell ::deep .mud-input-outlined-border {
            border-color: var(--border);
            }

            .auth-form-shell ::deep .mud-input-label {
            color: var(--text-muted);
            }

            .auth-form-shell ::deep .mud-input-root.mud-input-outlined.mud-input-adorned-start:hover .mud-input-outlined-border,
            .auth-form-shell ::deep .mud-input-root.mud-input-outlined.mud-input-adorned-start.mud-input-focused .mud-input-outlined-border {
            border-color: var(--primary);
            }

            .auth-form-shell ::deep .mud-input-root.mud-input-outlined.mud-input-focused {
            box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
            border-radius: var(--radius-sm);
            }
            """;

        private static string BuildMudBlazorContent(string identityClass, string userAccessor)
        {
            return $$"""
                @using System.Text
                @using System.Text.Encodings.Web
                @using Microsoft.AspNetCore.Identity
                @using Microsoft.AspNetCore.WebUtilities

                @inject UserManager<{{identityClass}}> UserManager
                @inject IEmailSender<{{identityClass}}> EmailSender
                @inject {{userAccessor}} UserAccessor
                @inject NavigationManager NavigationManager

                <MudPaper Class="pa-4 mb-4 ux-gradient-primary"
                Elevation="0">
                <MudText Typo="Typo.h4"
                Class="text-white font-weight-bold mb-2">
                <MudIcon Icon="@Icons.Material.Filled.AlternateEmail"
                Class="mr-2" />
                Manage email
                </MudText>
                <MudText Typo="Typo.body1"
                Class="text-white opacity-90">
                Review your current email address and request an email change.
                </MudText>
                </MudPaper>

                <StatusMessage Message="@message" />

                <MudCard Class="ux-fade-in-up auth-form-shell"
                Style="animation-delay: 0.1s"
                Outlined="true">
                <MudCardContent>
                <form @onsubmit="OnSendEmailVerificationAsync"
                @formname="send-verification"
                id="send-verification-form"
                method="post">
                <AntiforgeryToken />
                </form>
                <EditForm Model="Input"
                FormName="change-email"
                OnValidSubmit="OnValidSubmitAsync"
                method="post">
                <DataAnnotationsValidator />
                <ValidationSummary class="text-danger"
                role="alert" />

                <MudGrid>
                <MudItem xs="12">
                <MudText Typo="Typo.h5">Email settings</MudText>
                <MudText Typo="Typo.body2"
                Class="mb-4">
                Update the email address associated with your account.
                </MudText>
                </MudItem>
                <MudItem xs="12">
                <MudTextField T="string"
                Value="@email"
                Label="Email"
                Variant="Variant.Outlined"
                Adornment="Adornment.Start"
                AdornmentIcon="@Icons.Material.Filled.Email"
                Disabled="true" />
                @if (isEmailConfirmed)
                {
                <MudChip T="string"
                Color="Color.Success"
                Variant="Variant.Outlined"
                Class="mt-2">
                Verified
                </MudChip>
                }
                else
                {
                <MudButton ButtonType="ButtonType.Submit"
                Variant="Variant.Text"
                Color="Color.Primary"
                StartIcon="@Icons.Material.Filled.MarkEmailRead"
                form="send-verification-form"
                Class="mt-2">
                Send verification email
                </MudButton>
                }
                </MudItem>
                <MudItem xs="12">
                <MudTextField T="string"
                @bind-Value="Input.NewEmail"
                Label="New email"
                Placeholder="Please enter new email."
                Variant="Variant.Outlined"
                Adornment="Adornment.Start"
                AdornmentIcon="@Icons.Material.Filled.ForwardToInbox"
                Immediate="true"
                For="@(() => Input.NewEmail)" />
                <ValidationMessage For="() => Input.NewEmail"
                class="text-danger" />
                </MudItem>
                <MudItem xs="12">
                <MudStack Row="true"
                Justify="Justify.FlexEnd">
                <MudButton ButtonType="ButtonType.Submit"
                Variant="Variant.Filled"
                Color="Color.Primary"
                StartIcon="@Icons.Material.Filled.SaveAs">
                Change email
                </MudButton>
                </MudStack>
                </MudItem>
                </MudGrid>
                </EditForm>
                </MudCardContent>
                </MudCard>
                """;
        }

        private static string BuildBootstrapContent(string identityClass, string userAccessor)
        {
            return $$"""
                @using System.Text
                @using System.Text.Encodings.Web
                @using Microsoft.AspNetCore.Identity
                @using Microsoft.AspNetCore.WebUtilities

                @inject UserManager<{{identityClass}}> UserManager
                @inject IEmailSender<{{identityClass}}> EmailSender
                @inject {{userAccessor}} UserAccessor
                @inject NavigationManager NavigationManager

                <div class="ux-section-head">
                <h3>Email settings</h3>
                <p class="ux-section-subtitle">Update the email address associated with your account.</p>
                </div>

                <StatusMessage Message="@message"/>
                <form @onsubmit="OnSendEmailVerificationAsync"
                @formname="send-verification"
                id="send-verification-form"
                method="post">
                <AntiforgeryToken />
                </form>
                <EditForm Model="Input"
                FormName="change-email"
                OnValidSubmit="OnValidSubmitAsync"
                method="post">
                <DataAnnotationsValidator />
                <ValidationSummary class="text-danger"
                role="alert" />
                @if (isEmailConfirmed)
                {
                <UxField Label="Email"
                Icon="mail"
                For="email">
                <input id="email"
                type="text"
                value="@email"
                class="ux-input"
                placeholder="Your email"
                disabled />
                <span class="ux-field-suffix text-success"
                title="Confirmed">
                ✓
                </span>
                </UxField>
                }
                else
                {
                <UxField Label="Email"
                Icon="mail"
                For="email">
                <input id="email"
                type="text"
                value="@email"
                class="ux-input"
                placeholder="Your email"
                disabled />
                </UxField>
                <button type="submit"
                class="btn btn-outline-primary ux-inline-action"
                form="send-verification-form">
                <UxIcon Name="mail-check" /> Send verification email
                </button>
                }
                <UxField Label="New email"
                Icon="mail"
                For="new-email">
                <InputText id="new-email"
                @bind-Value="Input.NewEmail"
                class="ux-input"
                autocomplete="email"
                aria-required="true"
                placeholder="Enter a new email" />
                </UxField>
                <ValidationMessage For="() => Input.NewEmail"
                class="text-danger" />
                <button type="submit"
                class="btn btn-primary">
                <UxIcon Name="mail" />
                Change email
                </button>
                </EditForm>
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            var identityClass = code.Template.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity") ?
                IdentityHelperExtensions.GetIdentityUserClass(code.Template) :
                "ApplicationUser";

            code.AddField("string?", "message");
            code.AddField(identityClass, "user", f => f.WithAssignment(new CSharpStatement("default!")));
            code.AddField("string?", "email");
            code.AddField("bool", "isEmailConfirmed");

            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext"), "HttpContext", p => p.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("InputModel", "Input", p =>
            {
                p.Private();
                p.WithInitialValue("default!");
                p.AddAttribute($"{code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute")}(FormName = \"change-email\")");
            });

            // either get the existing method or add one
            ICSharpClassMethodDeclaration onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            if (onInitializedAsync is null)
            {
                code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync");
                onInitializedAsync = (code as ICSharpClass)?.Methods.FirstOrDefault(m => m.Name == "OnInitializedAsync");
            }

            onInitializedAsync.Async().Protected().Override();
            onInitializedAsync.AddStatement("Input ??= new();");
            onInitializedAsync.AddAssignmentStatement("user", new CSharpStatement("await UserAccessor.GetRequiredUserAsync(HttpContext);"));
            onInitializedAsync.AddAssignmentStatement("email", new CSharpStatement("await UserManager.GetEmailAsync(user);"));
            onInitializedAsync.AddAssignmentStatement("isEmailConfirmed", new CSharpStatement("await UserManager.IsEmailConfirmedAsync(user);"));
            onInitializedAsync.AddStatement("Input.NewEmail ??= email;");

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
            {
                onValidSubmitAsync.Private().Async();

                onValidSubmitAsync.AddIfStatement("Input.NewEmail is null || Input.NewEmail == email", @if =>
                {
                    @if.AddStatement("message = \"Your email is unchanged.\";");
                    @if.AddStatement("return;");
                });

                onValidSubmitAsync.AddAssignmentStatement("var userId", new CSharpStatement("await UserManager.GetUserIdAsync(user);"));
                onValidSubmitAsync.AddAssignmentStatement("var code", new CSharpStatement("await UserManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);"));
                onValidSubmitAsync.AddAssignmentStatement("code", new CSharpStatement($"{code.Template.UseType("Microsoft.AspNetCore.WebUtilities.WebEncoders")}.Base64UrlEncode({code.Template.UseType("System.Text.Encoding")}.UTF8.GetBytes(code));"));
                onValidSubmitAsync.AddAssignmentStatement("var callbackUrl", new CSharpStatement("NavigationManager.GetUriWithQueryParameters(NavigationManager.ToAbsoluteUri(\"Account/ConfirmEmailChange\").AbsoluteUri, new Dictionary<string, object?> { [\"userId\"] = userId, [\"email\"] = Input.NewEmail, [\"code\"] = code });"));
                onValidSubmitAsync.AddStatement($"await EmailSender.SendConfirmationLinkAsync(user, Input.NewEmail, {code.Template.UseType("System.Text.Encodings.Web.HtmlEncoder")}.Default.Encode(callbackUrl));");
                onValidSubmitAsync.AddStatement("message = \"Confirmation link to change email sent. Please check your email.\";");
            });

            code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnSendEmailVerificationAsync", onSendEmailVerificationAsync =>
            {
                onSendEmailVerificationAsync.Private().Async();

                onSendEmailVerificationAsync.AddIfStatement("email is null", @if =>
                {
                    @if.AddStatement("return;");
                });

                onSendEmailVerificationAsync.AddAssignmentStatement("var userId", new CSharpStatement("await UserManager.GetUserIdAsync(user);"));
                onSendEmailVerificationAsync.AddAssignmentStatement("var code", new CSharpStatement("await UserManager.GenerateEmailConfirmationTokenAsync(user);"));
                onSendEmailVerificationAsync.AddAssignmentStatement("code", new CSharpStatement($"{code.Template.UseType("Microsoft.AspNetCore.WebUtilities.WebEncoders")}.Base64UrlEncode({code.Template.UseType("System.Text.Encoding")}.UTF8.GetBytes(code));"));
                onSendEmailVerificationAsync.AddAssignmentStatement("var callbackUrl", new CSharpStatement("NavigationManager.GetUriWithQueryParameters(NavigationManager.ToAbsoluteUri(\"Account/ConfirmEmail\").AbsoluteUri, new Dictionary<string, object?> { [\"userId\"] = userId, [\"code\"] = code });"));
                onSendEmailVerificationAsync.AddStatement($"await EmailSender.SendConfirmationLinkAsync(user, email, {code.Template.UseType("System.Text.Encodings.Web.HtmlEncoder")}.Default.Encode(callbackUrl));");
                onSendEmailVerificationAsync.AddStatement("message = \"Verification email sent. Please check your email.\";");
            });

            code.AddClass("InputModel", inputModel =>
            {
                inputModel.Private().Sealed();

                inputModel.AddProperty("string?", "NewEmail", p =>
                {
                    p.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                    p.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.EmailAddressAttribute").RemoveSuffix("Attribute"));
                    p.AddAttribute("Display(Name = \"New email\")");
                });
            });
        }
    }
}
