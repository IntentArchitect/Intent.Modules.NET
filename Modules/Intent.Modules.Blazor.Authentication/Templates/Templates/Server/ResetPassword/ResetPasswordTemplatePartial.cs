using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ResetPasswordCodeBehind;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ResetPassword
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ResetPasswordTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.ResetPasswordTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="ResetPasswordTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ResetPasswordTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"ResetPassword")
                .Configure(file =>
                {
                    file.AddPageDirective($"/Account/ResetPassword");

                    file.AddInjectDirective(GetTypeName(AuthServiceInterfaceTemplate.TemplateId), "AuthService");
                    file.AddInjectDirective(GetTypeName(IdentityRedirectManager.IdentityRedirectManagerTemplate.TemplateId), "RedirectManager");

                    file.AddHtmlElement("PageTitle", element => element.WithText($"Reset password"));

                    // Emit a MudBlazor-styled body when MudBlazor is installed, otherwise the default Bootstrap body.
                    if (ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor"))
                    {
                        file.AddHtmlElement("MudPaper", paper => paper
                            .AddAttribute("Class", "pa-4 mb-4 ux-gradient-primary")
                            .AddAttribute("Elevation", "0")
                            .AddHtmlElement("MudText", text => text
                                .AddAttribute("Typo", "Typo.h4")
                                .AddAttribute("Class", "text-white font-weight-bold mb-2")
                                .AddHtmlElement("MudIcon", icon => icon
                                    .AddAttribute("Icon", "@Icons.Material.Filled.Password")
                                    .AddAttribute("Class", "mr-2"))
                                .WithText("Reset password"))
                            .AddHtmlElement("MudText", text => text
                                .AddAttribute("Typo", "Typo.body1")
                                .AddAttribute("Class", "text-white opacity-90")
                                .WithText("Enter your email address and choose a new password.")));

                        file.AddHtmlElement("MudGrid", grid => grid
                            .AddAttribute("Spacing", "3")
                            .AddHtmlElement("MudItem", item => item
                                .AddAttribute("xs", "12")
                                .AddAttribute("md", "7")
                                .AddAttribute("lg", "6")
                                .AddHtmlElement("MudCard", card => card
                                    .AddAttribute("Class", "ux-fade-in-up")
                                    .AddAttribute("Style", "animation-delay: 0.1s")
                                    .AddHtmlElement("MudCardContent", content => content
                                        .AddHtmlElement("StatusMessage", status => status.AddAttribute("Message", "@Message"))
                                        .AddHtmlElement("EditForm", form => form
                                            .AddAttribute("Model", "Input")
                                            .AddAttribute("FormName", "reset-password")
                                            .AddAttribute("OnValidSubmit", "OnValidSubmitAsync")
                                            .AddAttribute("method", "post")
                                            .AddHtmlElement("DataAnnotationsValidator")
                                            .AddHtmlElement("MudGrid", formGrid => formGrid
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudText", t => t.AddAttribute("Typo", "Typo.h5").WithText("Reset your password"))
                                                    .AddHtmlElement("MudText", t => t.AddAttribute("Typo", "Typo.body2").AddAttribute("Class", "mb-2").WithText("Enter your email address and your new password below."))
                                                    .AddHtmlElement("ValidationSummary", v => v.AddClass("text-danger").AddAttribute("role", "alert")))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("div", field => field.AddClass("reset-password-input-field")
                                                        .AddHtmlElement("label", l => l.AddClass("reset-password-input-label").AddAttribute("for", "email").WithText("Email"))
                                                        .AddHtmlElement("div", shell => shell.AddClass("reset-password-input-shell")
                                                            .AddHtmlElement("MudIcon", ic => ic.AddAttribute("Icon", "@Icons.Material.Filled.Email").AddAttribute("Class", "reset-password-input-icon"))
                                                            .AddHtmlElement("InputText", it => it.AddAttribute("id", "email").AddClass("reset-password-input-control").AddAttribute("@bind-Value", "Input.Email").AddAttribute("autocomplete", "username").AddAttribute("aria-required", "true").AddAttribute("placeholder", "name@example.com").AddAttribute("type", "email")))
                                                        .AddHtmlElement("ValidationMessage", v => v.AddClass("text-danger").AddAttribute("For", "() => Input.Email"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("div", field => field.AddClass("reset-password-input-field")
                                                        .AddHtmlElement("label", l => l.AddClass("reset-password-input-label").AddAttribute("for", "password").WithText("Password"))
                                                        .AddHtmlElement("div", shell => shell.AddClass("reset-password-input-shell")
                                                            .AddHtmlElement("MudIcon", ic => ic.AddAttribute("Icon", "@Icons.Material.Filled.Lock").AddAttribute("Class", "reset-password-input-icon"))
                                                            .AddHtmlElement("InputText", it => it.AddAttribute("id", "password").AddClass("reset-password-input-control").AddAttribute("@bind-Value", "Input.Password").AddAttribute("autocomplete", "new-password").AddAttribute("aria-required", "true").AddAttribute("placeholder", "Enter your new password").AddAttribute("type", "password")))
                                                        .AddHtmlElement("ValidationMessage", v => v.AddClass("text-danger").AddAttribute("For", "() => Input.Password"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("div", field => field.AddClass("reset-password-input-field")
                                                        .AddHtmlElement("label", l => l.AddClass("reset-password-input-label").AddAttribute("for", "confirm-password").WithText("Confirm password"))
                                                        .AddHtmlElement("div", shell => shell.AddClass("reset-password-input-shell")
                                                            .AddHtmlElement("MudIcon", ic => ic.AddAttribute("Icon", "@Icons.Material.Filled.LockReset").AddAttribute("Class", "reset-password-input-icon"))
                                                            .AddHtmlElement("InputText", it => it.AddAttribute("id", "confirm-password").AddClass("reset-password-input-control").AddAttribute("@bind-Value", "Input.ConfirmPassword").AddAttribute("autocomplete", "new-password").AddAttribute("aria-required", "true").AddAttribute("placeholder", "Confirm your new password").AddAttribute("type", "password")))
                                                        .AddHtmlElement("ValidationMessage", v => v.AddClass("text-danger").AddAttribute("For", "() => Input.ConfirmPassword"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("InputText", it => it.AddAttribute("id", "code").AddClass("d-none").AddAttribute("@bind-Value", "Input.Code").AddAttribute("type", "hidden")))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudStack", s => s.AddAttribute("Row", "true").AddAttribute("Spacing", "2").AddAttribute("Justify", "Justify.FlexEnd").AddAttribute("AlignItems", "AlignItems.Center")
                                                        .AddHtmlElement("MudButton", b => b.AddAttribute("ButtonType", "ButtonType.Submit").AddAttribute("Color", "Color.Primary").AddAttribute("Variant", "Variant.Filled").AddAttribute("FullWidth", "true").AddAttribute("StartIcon", "@Icons.Material.Filled.SaveAs").WithText("Reset password"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudStack", s => s.AddAttribute("Spacing", "1")
                                                        .AddHtmlElement("MudLink", l => l.AddAttribute("Href", "Account/Login").WithText("Back to log in"))))))))));

                        file.AddHtmlElement("style", style => style.WithText(@"
    .reset-password-input-field {
        display: flex;
        flex-direction: column;
        gap: var(--space-2);
    }

    .reset-password-input-label {
        color: var(--text);
        font-size: var(--type-label-lg);
        font-weight: 500;
    }

    .reset-password-input-shell {
        display: flex;
        align-items: center;
        gap: var(--space-2);
        min-height: 44px;
        padding: 0 0.875rem;
        background: var(--surface-2);
        border: 1px solid var(--border);
        border-radius: var(--radius-sm);
        box-shadow: var(--shadow-1);
    }

    .reset-password-input-shell:focus-within {
        border-color: var(--primary);
        box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
    }

    .reset-password-input-icon {
        color: var(--text-muted);
        flex-shrink: 0;
    }

    .reset-password-input-control {
        width: 100%;
        min-height: 42px;
        color: var(--text);
        background: transparent;
        border: none;
        outline: none;
    }

    .reset-password-input-control::placeholder {
        color: var(--text-muted);
    }
"));
                    }
                    else
                    {
                        file.AddHtmlElement("h1", element => element.WithText("Reset password"));
                        file.AddHtmlElement("h2", element => element.WithText("Reset your password"));
                        file.AddHtmlElement($"hr");
                        file.AddHtmlElement($"div", element => element.AddClass("row")
                            .AddHtmlElement("div", element => element.AddClass("col-md-4")
                                .AddHtmlElement("EditForm", element => element.AddAttribute("Model", "Input").AddAttribute("FormName", "resend-email-confirmation").AddAttribute("OnValidSubmit", "OnValidSubmitAsync").AddAttribute("method", "post")
                                    .AddHtmlElement("DataAnnotationsValidator")
                                    .AddHtmlElement("ValidationSummary", element => element.AddClass("text-danger").AddAttribute("role", "alert"))
                                    .AddHtmlElement("div", element => element.AddClass("form-floating mb-3")
                                        .AddHtmlElement("InputText", element => element.AddClass("form-control").AddAttribute("@bind-Value", "Input.Email").AddAttribute("autocomplete", "username").AddAttribute("aria-required", "true").AddAttribute("placeholder", "name@example.com"))
                                        .AddHtmlElement("label", element => element.AddClass("form-label").AddAttribute("for", "email").WithText("Email"))
                                        .AddHtmlElement("ValidationMessage", element => element.AddClass("text-danger").AddAttribute("For", "() => Input.Email"))
                                    .AddHtmlElement("div", element => element.AddClass("form-floating mb-3")
                                        .AddHtmlElement("InputText", element => element.AddClass("form-control").AddAttribute("type", "password").AddAttribute("@bind-Value", "Input.Password").AddAttribute("autocomplete", "new-password").AddAttribute("aria-required", "true").AddAttribute("placeholder", "Please enter your password."))
                                        .AddHtmlElement("label", element => element.AddClass("form-label").AddAttribute("for", "password").WithText("Password"))
                                        .AddHtmlElement("ValidationMessage", element => element.AddClass("text-danger").AddAttribute("For", "() => Input.Password"))
                                    .AddHtmlElement("div", element => element.AddClass("form-floating mb-3")
                                        .AddHtmlElement("InputText", element => element.AddClass("form-control").AddAttribute("type", "password").AddAttribute("@bind-Value", "Input.ConfirmPassword").AddAttribute("autocomplete", "new-password").AddAttribute("aria-required", "true").AddAttribute("placeholder", "Please confirm your password."))
                                        .AddHtmlElement("label", element => element.AddClass("form-label").AddAttribute("for", "confirm-password").WithText("Confirm password"))
                                        .AddHtmlElement("ValidationMessage", element => element.AddClass("text-danger").AddAttribute("For", "() => Input.ConfirmPassword"))
                                    .AddHtmlElement("button", element => element.AddClass("w-100 btn btn-lg btn-primary").AddAttribute("type", "submit").WithText("Resend"))
                                    )
                                 )
                             )
                         )));

                    }

                    var code = GetCodeBehind();
                    code.AddField($"IEnumerable<{code.Template.UseType("Microsoft.AspNetCore.Identity.IdentityError")}>?", "identityErrors");

                    code.AddProperty("InputModel", "Input", input =>
                    {
                        input.Private();
                        input.WithInitialValue("new()");
                        input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
                    });

                    code.AddProperty("string?", "Code", input =>
                    {
                        input.Private();
                        input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute"));
                    });

                    code.AddProperty("string?", "Message", p => p.Private().WithoutSetter().Getter.WithExpressionImplementation("identityErrors is null ? null : $\"Error: {string.Join(\", \", identityErrors.Select(error => error.Description))}\""));

                    code.AddMethod("void", "OnInitialized", onValidSubmitAsync =>
                    {
                        onValidSubmitAsync.Protected().Override();

                        onValidSubmitAsync.AddIfStatement("Code is null", @if =>
                        {
                            @if.AddStatement("RedirectManager.RedirectTo(\"Account/ResetPasswordConfirmation\");");
                        });

                        onValidSubmitAsync.AddStatement($"Input.Code = {code.Template.UseType("System.Text.Encoding")}.UTF8.GetString({code.Template.UseType("Microsoft.AspNetCore.WebUtilities.WebEncoders")}.Base64UrlDecode(Code));");
                    });

                    code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
                    {
                        onValidSubmitAsync.Private().Async();

                        onValidSubmitAsync.AddStatement("await AuthService.ResetPassword(Input.Email, Input.Code, Input.Password);");
                    });

                    code.AddClass("InputModel", inputModel =>
                    {
                        inputModel.Private().Sealed();

                        inputModel.AddProperty("string", "Email", email =>
                        {
                            email.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                            email.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.EmailAddressAttribute").RemoveSuffix("Attribute"));
                            email.WithInitialValue("\"\"");
                        });

                        inputModel.AddProperty("string", "Password", email =>
                        {
                            email.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                            email.AddAttribute("StringLength(100, ErrorMessage = \"The {0} must be at least {2} and at max {1} characters long.\", MinimumLength = 6)");
                            email.AddAttribute("DataType(DataType.Password)");
                            email.WithInitialValue("\"\"");
                        });

                        inputModel.AddProperty("string", "ConfirmPassword", email =>
                        {
                            email.AddAttribute("DataType(DataType.Password)");
                            email.AddAttribute("Display(Name = \"Confirm password\")");
                            email.AddAttribute("Compare(\"Password\", ErrorMessage = \"The password and confirmation password do not match.\")");
                            email.WithInitialValue("\"\"");
                        });

                        inputModel.AddProperty("string", "Code", email =>
                        {
                            email.AddAttribute(code.Template.UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute"));
                            email.WithInitialValue("\"\"");
                        });
                    });
                });
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public IRazorFile RazorFile { get; }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            return RazorFile.GetConfig();
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public override string TransformText() => RazorFile.ToString();

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.GetSettings().GetBlazor().Authentication().IsOidc();
        }

        // Code-behind plumbing (hand-added; Body=Merge region, survives module regen). Routes this
        // page's @code into the sibling ResetPasswordCodeBehindTemplate (.razor.cs) when present,
        // falling back to an inline @code block otherwise.
        private IBuildsCSharpMembers _codeBehind;

        public ICSharpFileBuilderTemplate CodeBehindTemplate { get; private set; }

        public override ICSharpCodeContext RootCodeContext => GetCodeBehind();

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            CodeBehindTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(ResetPasswordCodeBehindTemplate.TemplateId);
        }

        private IBuildsCSharpMembers GetCodeBehind()
        {
            if (_codeBehind != null)
            {
                return _codeBehind;
            }

            if (CodeBehindTemplate != null)
            {
                _codeBehind = CodeBehindTemplate.CSharpFile.Classes.First();
            }
            else
            {
                RazorFile.AddCodeBlock(x => _codeBehind = x);
            }

            return _codeBehind;
        }
    }
}