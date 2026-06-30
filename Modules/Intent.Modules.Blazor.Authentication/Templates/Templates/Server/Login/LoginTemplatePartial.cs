using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRedirectManager;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.LoginCodeBehind;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.Login
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class LoginTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.LoginTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="LoginTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public LoginTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"Login")
                .Configure(file =>
                {
                    file.AddPageDirective($"/Account/Login");

                    file.AddInjectDirective(GetTypeName(AuthServiceInterfaceTemplate.TemplateId), "AuthService");
                    file.AddInjectDirective("Microsoft.AspNetCore.Components.NavigationManager", "NavigationManager");

                    file.AddHtmlElement("PageTitle", element => element.WithText($"Log in"));

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
                                    .AddAttribute("Icon", "@Icons.Material.Filled.LockOpen")
                                    .AddAttribute("Class", "mr-2"))
                                .WithText("Welcome back"))
                            .AddHtmlElement("MudText", text => text
                                .AddAttribute("Typo", "Typo.body1")
                                .AddAttribute("Class", "text-white opacity-90")
                                .WithText("Sign in with your local account to continue.")));

                        file.AddHtmlElement("MudGrid", grid =>
                        {
                            grid.AddAttribute("Spacing", "3");
                            grid.AddHtmlElement("MudItem", item => item
                                .AddAttribute("xs", "12")
                                .AddAttribute("md", "7")
                                .AddAttribute("lg", "6")
                                .AddHtmlElement("MudCard", card => card
                                    .AddAttribute("Class", "ux-fade-in-up")
                                    .AddAttribute("Style", "animation-delay: 0.1s")
                                    .AddHtmlElement("MudCardContent", content => content
                                        .AddHtmlElement("StatusMessage", status => status.AddAttribute("Message", "@errorMessage"))
                                        .AddHtmlElement("EditForm", form => form
                                            .AddAttribute("Model", "Input")
                                            .AddAttribute("FormName", "login")
                                            .AddAttribute("OnValidSubmit", "LoginUser")
                                            .AddAttribute("method", "post")
                                            .AddHtmlElement("DataAnnotationsValidator")
                                            .AddHtmlElement("MudGrid", formGrid => formGrid
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudText", t => t.AddAttribute("Typo", "Typo.h5").WithText("Use a local account to log in"))
                                                    .AddHtmlElement("MudText", t => t.AddAttribute("Typo", "Typo.body2").AddAttribute("Class", "mb-2").WithText("Enter your credentials below."))
                                                    .AddHtmlElement("ValidationSummary", v => v.AddClass("text-danger").AddAttribute("role", "alert")))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("div", field => field.AddClass("login-input-field")
                                                        .AddHtmlElement("label", l => l.AddClass("login-input-label").AddAttribute("for", "email").WithText("Email"))
                                                        .AddHtmlElement("div", shell => shell.AddClass("login-input-shell")
                                                            .AddHtmlElement("MudIcon", ic => ic.AddAttribute("Icon", "@Icons.Material.Filled.Email").AddAttribute("Class", "login-input-icon"))
                                                            .AddHtmlElement("InputText", it => it.AddAttribute("id", "email").AddClass("login-input-control").AddAttribute("@bind-Value", "Input.Email").AddAttribute("autocomplete", "username").AddAttribute("aria-required", "true").AddAttribute("placeholder", "name@example.com").AddAttribute("type", "email")))
                                                        .AddHtmlElement("ValidationMessage", v => v.AddClass("text-danger").AddAttribute("For", "() => Input.Email"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("div", field => field.AddClass("login-input-field")
                                                        .AddHtmlElement("label", l => l.AddClass("login-input-label").AddAttribute("for", "password").WithText("Password"))
                                                        .AddHtmlElement("div", shell => shell.AddClass("login-input-shell")
                                                            .AddHtmlElement("MudIcon", ic => ic.AddAttribute("Icon", "@Icons.Material.Filled.Lock").AddAttribute("Class", "login-input-icon"))
                                                            .AddHtmlElement("InputText", it => it.AddAttribute("id", "password").AddClass("login-input-control").AddAttribute("@bind-Value", "Input.Password").AddAttribute("autocomplete", "current-password").AddAttribute("aria-required", "true").AddAttribute("placeholder", "Enter your password").AddAttribute("type", "password")))
                                                        .AddHtmlElement("ValidationMessage", v => v.AddClass("text-danger").AddAttribute("For", "() => Input.Password"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("div", field => field.AddClass("login-checkbox-field")
                                                        .AddHtmlElement("InputCheckbox", cb => cb.AddAttribute("id", "rememberMe").AddClass("login-checkbox-control").AddAttribute("@bind-Value", "Input.RememberMe"))
                                                        .AddHtmlElement("label", l => l.AddClass("login-checkbox-label").AddAttribute("for", "rememberMe").WithText("Remember me"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudStack", s => s.AddAttribute("Row", "true").AddAttribute("Spacing", "2").AddAttribute("Justify", "Justify.FlexEnd").AddAttribute("AlignItems", "AlignItems.Center")
                                                        .AddHtmlElement("MudButton", b => b.AddAttribute("ButtonType", "ButtonType.Submit").AddAttribute("Color", "Color.Primary").AddAttribute("Variant", "Variant.Filled").AddAttribute("FullWidth", "true").AddAttribute("StartIcon", "@Icons.Material.Filled.Login").WithText("Log in"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudStack", s => s.AddAttribute("Spacing", "1")
                                                        .AddHtmlElement("MudLink", l => l.AddAttribute("Href", "Account/ForgotPassword").WithText("Forgot your password?"))
                                                        .AddHtmlElement("MudLink", l => l.AddAttribute("Href", "@(NavigationManager.GetUriWithQueryParameters(\"Account/Register\", new Dictionary<string, object?> { [\"ReturnUrl\"] = ReturnUrl }))").WithText("Register as a new user"))
                                                        .AddHtmlElement("MudLink", l => l.AddAttribute("Href", "Account/ResendEmailConfirmation").WithText("Resend email confirmation")))))))));

                            if (ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity())
                            {
                                grid.AddHtmlElement("MudItem", item => item
                                    .AddAttribute("xs", "12")
                                    .AddAttribute("md", "5")
                                    .AddAttribute("lg", "6")
                                    .AddHtmlElement("MudCard", card => card
                                        .AddAttribute("Class", "ux-fade-in-up")
                                        .AddAttribute("Style", "animation-delay: 0.2s")
                                        .AddHtmlElement("MudCardContent", content => content
                                            .AddHtmlElement("MudText", t => t.AddAttribute("Typo", "Typo.h6").WithText("Use another service to log in"))
                                            .AddHtmlElement("MudText", t => t.AddAttribute("Typo", "Typo.body2").AddAttribute("Class", "mb-4").WithText("Choose an external provider to authenticate."))
                                            .AddHtmlElement("ExternalLoginPicker"))));
                            }
                        });

                        file.AddHtmlElement("style", style => style.WithText(@"
    .login-input-field {
        display: flex;
        flex-direction: column;
        gap: var(--space-2);
    }

    .login-input-label,
    .login-checkbox-label {
        color: var(--text);
        font-size: var(--type-label-lg);
        font-weight: 500;
    }

    .login-input-shell {
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

    .login-input-shell:focus-within {
        border-color: var(--primary);
        box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
    }

    .login-input-icon {
        color: var(--text-muted);
        flex-shrink: 0;
    }

    .login-input-control {
        width: 100%;
        min-height: 42px;
        color: var(--text);
        background: transparent;
        border: none;
        outline: none;
    }

    .login-input-control::placeholder {
        color: var(--text-muted);
    }

    .login-checkbox-field {
        display: flex;
        align-items: center;
        gap: var(--space-2);
    }

    .login-checkbox-control {
        width: 1rem;
        height: 1rem;
        accent-color: var(--primary);
        flex-shrink: 0;
    }
"));
                    }
                    else
                    {

                        file.AddHtmlElement("AccountHero", hero => hero
                            .AddAttribute("Icon", "lock-open")
                            .AddAttribute("Title", "Welcome back")
                            .AddAttribute("Subtitle", "Sign in with your local account to continue."));

                        file.AddHtmlElement("div", grid =>
                        {
                            // Single-column login (no external-login picker → not Identity) gets the
                            // narrow 554px card; the two-column Identity login keeps the full grid.
                            grid.AddClass(ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity()
                                ? "ux-form-grid"
                                : "ux-form-grid ux-form-narrow");
                            grid.AddHtmlElement("div", element => element.AddClass("ux-form-col")
                                .AddHtmlElement("section", element => element
                                    .AddHtmlElement("StatusMessage", element => element.AddAttribute("Message", "@errorMessage"))
                                    .AddHtmlElement("EditForm", element => element.AddAttribute("Model", "Input").AddAttribute("FormName", "login").AddAttribute("OnValidSubmit", "LoginUser").AddAttribute("method", "post")
                                        .AddHtmlElement("DataAnnotationsValidator")
                                        .AddHtmlElement("div", element => element.AddClass("ux-section-head")
                                            .AddHtmlElement("h2", element => element.WithText("Use a local account to log in"))
                                            .AddHtmlElement("p", element => element.AddClass("ux-section-subtitle").WithText("Enter your credentials below."))
                                        )
                                        .AddHtmlElement("ValidationSummary", element => element.AddClass("text-danger").AddAttribute("role", "alert"))
                                        .AddHtmlElement("UxField", element => element.AddAttribute("Label", "Email").AddAttribute("Icon", "mail").AddAttribute("For", "email")
                                            .AddHtmlElement("InputText", element => element.AddAttribute("id", "email").AddClass("ux-input").AddAttribute("@bind-Value", "Input.Email").AddAttribute("autocomplete", "username").AddAttribute("aria-required", "true").AddAttribute("placeholder", "name@example.com"))
                                        )
                                        .AddHtmlElement("ValidationMessage", element => element.AddClass("text-danger").AddAttribute("For", "() => Input.Email"))
                                        .AddHtmlElement("UxField", element => element.AddAttribute("Label", "Password").AddAttribute("Icon", "lock").AddAttribute("For", "password")
                                            .AddHtmlElement("InputText", element => element.AddAttribute("id", "password").AddClass("ux-input").AddAttribute("type", "password").AddAttribute("@bind-Value", "Input.Password").AddAttribute("autocomplete", "current-password").AddAttribute("aria-required", "true").AddAttribute("placeholder", "Enter your password"))
                                        )
                                        .AddHtmlElement("ValidationMessage", element => element.AddClass("text-danger").AddAttribute("For", "() => Input.Password"))
                                        .AddHtmlElement("div", element => element.AddClass("form-check")
                                            .AddHtmlElement("InputCheckbox", element => element.AddAttribute("id", "rememberMe").AddClass("form-check-input").AddAttribute("@bind-Value", "Input.RememberMe"))
                                            .AddHtmlElement("label", element => element.AddClass("form-check-label").AddAttribute("for", "rememberMe").WithText("Remember me"))
                                        )
                                        .AddHtmlElement("button", element => element.AddClass("w-100 btn btn-primary").AddAttribute("type", "submit")
                                            .AddHtmlElement("UxIcon", element => element.AddAttribute("Name", "log-in"))
                                            .WithText("Log in")
                                        )
                                        .AddHtmlElement("div", element => element.AddClass("ux-account-links")
                                            .AddHtmlElement("a", element => element.AddAttribute("href", "Account/ForgotPassword").WithText("Forgot your password?"))
                                            .AddHtmlElement("a", element => element.AddAttribute("href", "@(NavigationManager.GetUriWithQueryParameters(\"Account/Register\", new Dictionary<string, object?> { [\"ReturnUrl\"] = ReturnUrl }))").WithText("Register as a new user"))
                                            .AddHtmlElement("a", element => element.AddAttribute("href", "Account/ResendEmailConfirmation").WithText("Resend email confirmation"))
                                        )
                                    )
                                )
                            );

                            if (ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity())
                            {
                                grid.AddHtmlElement("div", element => element.AddClass("ux-form-col")
                                    .AddHtmlElement("section", element => element
                                        .AddHtmlElement("div", element => element.AddClass("ux-section-head")
                                            .AddHtmlElement("h3", element => element.WithText("Use another service to log in"))
                                            .AddHtmlElement("p", element => element.AddClass("ux-section-subtitle").WithText("Choose an external provider to authenticate."))
                                        )
                                        .AddHtmlElement("ExternalLoginPicker")
                                    )
                                );
                            }
                        });

                    }

                    var code = GetCodeBehind();
                    code.AddField("string?", "errorMessage");

                    code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext"), "HttpContext", input =>
                    {
                        input.Private();
                        input.WithInitialValue("default!");
                        input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute"));
                    });
                    code.AddProperty("InputModel", "Input", input =>
                    {
                        input.Private();
                        input.WithInitialValue("default!");
                        input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
                    });
                    code.AddProperty("string?", "ReturnUrl", input =>
                    {
                        input.Private();
                        input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute"));
                    });


                    code.AddMethod("void", "OnInitialized", onInitialized =>
                    {
                        onInitialized.Protected().Override();

                        onInitialized.AddStatement("Input ??= new();");
                    });

                    code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "LoginUser", onValidSubmitAsync =>
                    {
                        onValidSubmitAsync.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute").RemoveSuffix("Attribute"));
                        onValidSubmitAsync.Async();

                        onValidSubmitAsync.AddStatement("await AuthService.Login(Input.Email, Input.Password, Input.RememberMe, ReturnUrl);");
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
                            email.AddAttribute("DataType(DataType.Password)");
                            email.WithInitialValue("\"\"");
                        });

                        inputModel.AddProperty("bool", "RememberMe", email =>
                        {
                            email.AddAttribute("Display(Name = \"Remember me?\")");
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

        // Code-behind plumbing (hand-added; Body=Merge region, survives module regen). Routes this
        // page's @code into the sibling LoginCodeBehindTemplate (.razor.cs) when present, falling
        // back to an inline @code block otherwise.
        private IBuildsCSharpMembers _codeBehind;

        public ICSharpFileBuilderTemplate CodeBehindTemplate { get; private set; }

        public override ICSharpCodeContext RootCodeContext => GetCodeBehind();

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            CodeBehindTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(LoginCodeBehindTemplate.TemplateId);
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