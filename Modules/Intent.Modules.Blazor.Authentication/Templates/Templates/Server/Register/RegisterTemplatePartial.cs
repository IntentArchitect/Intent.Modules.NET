using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.RegisterCodeBehind;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.Register
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RegisterTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.RegisterTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RegisterTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RegisterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"Register")
                .Configure(file =>
                {
                    file.AddPageDirective($"/Account/Register");

                    file.AddInjectDirective(GetTypeName(AuthServiceInterfaceTemplate.TemplateId), "AuthService");

                    file.AddHtmlElement("PageTitle", element => element.WithText($"Register"));

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
                                    .AddAttribute("Icon", "@Icons.Material.Filled.PersonAdd")
                                    .AddAttribute("Class", "mr-2"))
                                .WithText("Create your account"))
                            .AddHtmlElement("MudText", text => text
                                .AddAttribute("Typo", "Typo.body1")
                                .AddAttribute("Class", "text-white opacity-90")
                                .WithText("Register with your email address to continue.")));

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
                                        .AddHtmlElement("StatusMessage", status => status.AddAttribute("Message", "@Message"))
                                        .AddHtmlElement("EditForm", form => form
                                            .AddAttribute("Model", "Input")
                                            .AddAttribute("FormName", "register")
                                            .AddAttribute("OnValidSubmit", "RegisterUser")
                                            .AddAttribute("method", "post")
                                            .AddAttribute("asp-route-returnUrl", "@ReturnUrl")
                                            .AddHtmlElement("DataAnnotationsValidator")
                                            .AddHtmlElement("MudGrid", formGrid => formGrid
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudText", t => t.AddAttribute("Typo", "Typo.h5").WithText("Create a new account"))
                                                    .AddHtmlElement("MudText", t => t.AddAttribute("Typo", "Typo.body2").AddAttribute("Class", "mb-2").WithText("Enter your details below."))
                                                    .AddHtmlElement("ValidationSummary", v => v.AddClass("text-danger").AddAttribute("role", "alert")))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("div", field => field.AddClass("register-input-field")
                                                        .AddHtmlElement("label", l => l.AddClass("register-input-label").AddAttribute("for", "email").WithText("Email"))
                                                        .AddHtmlElement("div", shell => shell.AddClass("register-input-shell")
                                                            .AddHtmlElement("MudIcon", ic => ic.AddAttribute("Icon", "@Icons.Material.Filled.Email").AddAttribute("Class", "register-input-icon"))
                                                            .AddHtmlElement("InputText", it => it.AddAttribute("id", "email").AddClass("register-input-control").AddAttribute("@bind-Value", "Input.Email").AddAttribute("autocomplete", "username").AddAttribute("aria-required", "true").AddAttribute("placeholder", "name@example.com").AddAttribute("type", "email")))
                                                        .AddHtmlElement("ValidationMessage", v => v.AddClass("text-danger").AddAttribute("For", "() => Input.Email"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("div", field => field.AddClass("register-input-field")
                                                        .AddHtmlElement("label", l => l.AddClass("register-input-label").AddAttribute("for", "password").WithText("Password"))
                                                        .AddHtmlElement("div", shell => shell.AddClass("register-input-shell")
                                                            .AddHtmlElement("MudIcon", ic => ic.AddAttribute("Icon", "@Icons.Material.Filled.Lock").AddAttribute("Class", "register-input-icon"))
                                                            .AddHtmlElement("InputText", it => it.AddAttribute("id", "password").AddClass("register-input-control").AddAttribute("@bind-Value", "Input.Password").AddAttribute("autocomplete", "new-password").AddAttribute("aria-required", "true").AddAttribute("placeholder", "Enter your password").AddAttribute("type", "password")))
                                                        .AddHtmlElement("ValidationMessage", v => v.AddClass("text-danger").AddAttribute("For", "() => Input.Password"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("div", field => field.AddClass("register-input-field")
                                                        .AddHtmlElement("label", l => l.AddClass("register-input-label").AddAttribute("for", "confirm-password").WithText("Confirm Password"))
                                                        .AddHtmlElement("div", shell => shell.AddClass("register-input-shell")
                                                            .AddHtmlElement("MudIcon", ic => ic.AddAttribute("Icon", "@Icons.Material.Filled.LockReset").AddAttribute("Class", "register-input-icon"))
                                                            .AddHtmlElement("InputText", it => it.AddAttribute("id", "confirm-password").AddClass("register-input-control").AddAttribute("@bind-Value", "Input.ConfirmPassword").AddAttribute("autocomplete", "new-password").AddAttribute("aria-required", "true").AddAttribute("placeholder", "Confirm your password").AddAttribute("type", "password")))
                                                        .AddHtmlElement("ValidationMessage", v => v.AddClass("text-danger").AddAttribute("For", "() => Input.ConfirmPassword"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudStack", s => s.AddAttribute("Row", "true").AddAttribute("Spacing", "2").AddAttribute("Justify", "Justify.FlexEnd").AddAttribute("AlignItems", "AlignItems.Center")
                                                        .AddHtmlElement("MudButton", b => b.AddAttribute("ButtonType", "ButtonType.Submit").AddAttribute("Color", "Color.Primary").AddAttribute("Variant", "Variant.Filled").AddAttribute("FullWidth", "true").AddAttribute("StartIcon", "@Icons.Material.Filled.HowToReg").WithText("Register"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudStack", s => s.AddAttribute("Spacing", "1")
                                                        .AddHtmlElement("MudLink", l => l.AddAttribute("Href", "Account/Login").WithText("Already have an account? Log in")))))))));

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
    .register-input-field {
        display: flex;
        flex-direction: column;
        gap: var(--space-2);
    }

    .register-input-label {
        color: var(--text);
        font-size: var(--type-label-lg);
        font-weight: 500;
    }

    .register-input-shell {
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

    .register-input-shell:focus-within {
        border-color: var(--primary);
        box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
    }

    .register-input-icon {
        color: var(--text-muted);
        flex-shrink: 0;
    }

    .register-input-control {
        width: 100%;
        min-height: 42px;
        color: var(--text);
        background: transparent;
        border: none;
        outline: none;
    }

    .register-input-control::placeholder {
        color: var(--text-muted);
    }
"));
                    }
                    else
                    {

                    file.AddHtmlElement("h1", element => element.WithText("Register"));
                    file.AddHtmlElement($"div", element => element.AddClass("row")
                        .AddHtmlElement("div", element => element.AddClass("col-md-4")
                            .AddHtmlElement("section", element => element
                                .AddHtmlElement("StatusMessage", element => element.AddAttribute("Message", "@Message"))
                                .AddHtmlElement("EditForm", element => element.AddAttribute("Model", "Input").AddAttribute("FormName", "register").AddAttribute("OnValidSubmit", "RegisterUser").AddAttribute("method", "post").AddAttribute("asp-route-returnUrl", "@ReturnUrl")
                                    .AddHtmlElement("DataAnnotationsValidator")
                                    .AddHtmlElement("h2", element => element.WithText("Create a new account."))
                                    .AddHtmlElement("hr")
                                    .AddHtmlElement("ValidationSummary", element => element.AddClass("text-danger").AddAttribute("role", "alert"))
                                    .AddHtmlElement("div", element => element.AddClass("form-floating mb-3")
                                        .AddHtmlElement("InputText", element => element.AddClass("form-control").AddAttribute("@bind-Value", "Input.Email").AddAttribute("autocomplete", "username").AddAttribute("aria-required", "true").AddAttribute("placeholder", "name@example.com"))
                                        .AddHtmlElement("label", element => element.AddClass("form-label").AddAttribute("for", "email").WithText("Email"))
                                        .AddHtmlElement("ValidationMessage", element => element.AddClass("text-danger").AddAttribute("For", "() => Input.Email"))
                                    )
                                    .AddHtmlElement("div", element => element.AddClass("form-floating mb-3")
                                        .AddHtmlElement("InputText", element => element.AddClass("form-control").AddAttribute("@bind-Value", "Input.Password").AddAttribute("autocomplete", "current-password").AddAttribute("aria-required", "true").AddAttribute("placeholder", "password").AddAttribute("type", "password"))
                                        .AddHtmlElement("label", element => element.AddClass("form-label").AddAttribute("for", "password").WithText("Password"))
                                        .AddHtmlElement("ValidationMessage", element => element.AddClass("text-danger").AddAttribute("For", "() => Input.Password"))
                                    )
                                    .AddHtmlElement("div", element => element.AddClass("form-floating mb-3")
                                        .AddHtmlElement("InputText", element => element.AddClass("form-control").AddAttribute("@bind-Value", "Input.ConfirmPassword").AddAttribute("autocomplete", "current-password").AddAttribute("aria-required", "true").AddAttribute("placeholder", "password").AddAttribute("type", "password"))
                                        .AddHtmlElement("label", element => element.AddClass("form-label").AddAttribute("for", "confirm-password").WithText("Confirm Password"))
                                        .AddHtmlElement("ValidationMessage", element => element.AddClass("text-danger").AddAttribute("For", "() => Input.ConfirmPassword"))
                                    )
                                    .AddHtmlElement("div", element => element
                                        .AddHtmlElement("button", element => element.AddClass("w-100 btn btn-lg btn-primary").AddAttribute("type", "submit").WithText("Register"))
                                    )
                                )
                             )
                         )
                     );
                    if (ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity())
                    {
                        file.AddHtmlElement("div", element => element.AddClass("col-md-6 col-md-offset-2")
                                .AddHtmlElement("section", element => element
                                    .AddHtmlElement("h3", element => element.WithText("Use another service to log in."))
                                    .AddHtmlElement("hr")
                                    .AddHtmlElement("ExternalLoginPicker")
                                    )
                                );
                    }

                    }

                    var code = GetCodeBehind();
                    code.AddField($"IEnumerable<{code.Template.UseType("Microsoft.AspNetCore.Identity.IdentityError")}>?", "identityErrors");

                    code.AddProperty("InputModel", "Input", input =>
                    {
                        input.Private();
                        input.WithInitialValue("new()");
                        input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
                    });
                    code.AddProperty("string?", "ReturnUrl", input =>
                    {
                        input.Private();
                        input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute"));
                    });

                    code.AddProperty("string?", "Message", p => p.Private().WithoutSetter().Getter.WithExpressionImplementation("identityErrors is null ? null : $\"Error: {string.Join(\", \", identityErrors.Select(error => error.Description))}\""));

                    code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "RegisterUser", onValidSubmitAsync =>
                    {
                        onValidSubmitAsync.Async();

                        onValidSubmitAsync.AddStatement("await AuthService.Register(Input.Email, Input.Password, ReturnUrl);");
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
            var config = RazorFile.GetConfig();
            // TEMP (verification): force full overwrite so the Software Factory reflects pure template output.
            config.ConfigureRazorMerger(merger => merger.WithDefaultMode(Intent.RoslynWeaver.Attributes.Mode.Fully));
            return config;
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public override string TransformText() => RazorFile.ToString();

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.GetSettings().GetBlazor().Authentication().IsOidc();
        }

        // Code-behind plumbing (hand-added; Body=Merge region, survives module regen). Routes this
        // page's @code into the sibling RegisterCodeBehindTemplate (.razor.cs) when present,
        // falling back to an inline @code block otherwise.
        private IBuildsCSharpMembers _codeBehind;

        public ICSharpFileBuilderTemplate CodeBehindTemplate { get; private set; }

        public override ICSharpCodeContext RootCodeContext => GetCodeBehind();

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            CodeBehindTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(RegisterCodeBehindTemplate.TemplateId);
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