using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRedirectManager;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ResendEmailConfirmationCodeBehind;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ResendEmailConfirmation
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ResendEmailConfirmationTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.ResendEmailConfirmationTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="ResendEmailConfirmationTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ResendEmailConfirmationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"ResendEmailConfirmation")
                .Configure(file =>
                {
                    file.AddPageDirective($"/Account/ResendEmailConfirmation");

                    file.AddInjectDirective(GetTypeName(AuthServiceInterfaceTemplate.TemplateId), "AuthService");

                    file.AddHtmlElement("PageTitle", element => element.WithText($"Resend email confirmation"));

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
                                    .AddAttribute("Icon", "@Icons.Material.Filled.Mail")
                                    .AddAttribute("Class", "mr-2"))
                                .WithText("Resend email confirmation"))
                            .AddHtmlElement("MudText", text => text
                                .AddAttribute("Typo", "Typo.body1")
                                .AddAttribute("Class", "text-white opacity-90")
                                .WithText("Enter your email address and we will send you another confirmation email.")));

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
                                        .AddHtmlElement("StatusMessage", status => status.AddAttribute("Message", "@message"))
                                        .AddHtmlElement("EditForm", form => form
                                            .AddAttribute("Model", "Input")
                                            .AddAttribute("FormName", "resend-email-confirmation")
                                            .AddAttribute("OnValidSubmit", "OnValidSubmitAsync")
                                            .AddAttribute("method", "post")
                                            .AddHtmlElement("DataAnnotationsValidator")
                                            .AddHtmlElement("MudGrid", formGrid => formGrid
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudText", t => t.AddAttribute("Typo", "Typo.h5").WithText("Send another confirmation email"))
                                                    .AddHtmlElement("MudText", t => t.AddAttribute("Typo", "Typo.body2").AddAttribute("Class", "mb-2").WithText("Enter the email address you used when registering."))
                                                    .AddHtmlElement("ValidationSummary", v => v.AddClass("text-danger").AddAttribute("role", "alert")))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("div", field => field.AddClass("resend-email-input-field")
                                                        .AddHtmlElement("label", l => l.AddClass("resend-email-input-label").AddAttribute("for", "email").WithText("Email"))
                                                        .AddHtmlElement("div", shell => shell.AddClass("resend-email-input-shell")
                                                            .AddHtmlElement("MudIcon", ic => ic.AddAttribute("Icon", "@Icons.Material.Filled.Email").AddAttribute("Class", "resend-email-input-icon"))
                                                            .AddHtmlElement("InputText", it => it.AddAttribute("id", "email").AddClass("resend-email-input-control").AddAttribute("@bind-Value", "Input.Email").AddAttribute("autocomplete", "username").AddAttribute("aria-required", "true").AddAttribute("placeholder", "name@example.com").AddAttribute("type", "email")))
                                                        .AddHtmlElement("ValidationMessage", v => v.AddClass("text-danger").AddAttribute("For", "() => Input.Email"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudStack", s => s.AddAttribute("Row", "true").AddAttribute("Spacing", "2").AddAttribute("Justify", "Justify.FlexEnd").AddAttribute("AlignItems", "AlignItems.Center")
                                                        .AddHtmlElement("MudButton", b => b.AddAttribute("ButtonType", "ButtonType.Submit").AddAttribute("Color", "Color.Primary").AddAttribute("Variant", "Variant.Filled").AddAttribute("FullWidth", "true").AddAttribute("StartIcon", "@Icons.Material.Filled.Send").WithText("Resend"))))
                                                .AddHtmlElement("MudItem", i => i
                                                    .AddAttribute("xs", "12")
                                                    .AddHtmlElement("MudStack", s => s.AddAttribute("Spacing", "1")
                                                        .AddHtmlElement("MudLink", l => l.AddAttribute("Href", "Account/Login").WithText("Back to log in"))))))))));

                        file.AddHtmlElement("style", style => style.WithText(@"
    .resend-email-input-field {
        display: flex;
        flex-direction: column;
        gap: var(--space-2);
    }

    .resend-email-input-label {
        color: var(--text);
        font-size: var(--type-label-lg);
        font-weight: 500;
    }

    .resend-email-input-shell {
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

    .resend-email-input-shell:focus-within {
        border-color: var(--primary);
        box-shadow: 0 0 0 3px var(--primary-subtle), 0 0 12px var(--primary-glow);
    }

    .resend-email-input-icon {
        color: var(--text-muted);
        flex-shrink: 0;
    }

    .resend-email-input-control {
        width: 100%;
        min-height: 42px;
        color: var(--text);
        background: transparent;
        border: none;
        outline: none;
    }

    .resend-email-input-control::placeholder {
        color: var(--text-muted);
    }
"));
                    }
                    else
                    {
                        file.AddHtmlElement("AccountHero", hero => hero
                            .AddAttribute("Icon", "mail")
                            .AddAttribute("Title", "Resend email confirmation")
                            .AddAttribute("Subtitle", "Enter your email to receive a new confirmation link."));

                        file.AddHtmlElement("div", element => element.AddClass("ux-form-narrow")
                            .AddHtmlElement("section", element => element
                                .AddHtmlElement("div", sh => sh.AddClass("ux-section-head")
                                    .AddHtmlElement("h2", h => h.WithText("Send another confirmation email"))
                                    .AddHtmlElement("p", p => p.AddClass("ux-section-subtitle").WithText("Enter the email address you used when registering.")))
                                .AddHtmlElement("EditForm", element => element.AddAttribute("Model", "Input").AddAttribute("FormName", "resend-email-confirmation").AddAttribute("OnValidSubmit", "OnValidSubmitAsync").AddAttribute("method", "post")
                                    .AddHtmlElement("DataAnnotationsValidator")
                                    .AddHtmlElement("ValidationSummary", element => element.AddClass("text-danger").AddAttribute("role", "alert"))
                                    .AddHtmlElement("UxField", element => element.AddAttribute("Label", "Email").AddAttribute("Icon", "mail").AddAttribute("For", "email")
                                        .AddHtmlElement("InputText", element => element.AddAttribute("id", "email").AddClass("ux-input").AddAttribute("@bind-Value", "Input.Email").AddAttribute("autocomplete", "username").AddAttribute("aria-required", "true").AddAttribute("placeholder", "name@example.com"))
                                    )
                                    .AddHtmlElement("ValidationMessage", element => element.AddClass("text-danger").AddAttribute("For", "() => Input.Email"))
                                    .AddHtmlElement("button", element => element.AddClass("w-100 btn btn-primary").AddAttribute("type", "submit")
                                        .AddHtmlElement("UxIcon", element => element.AddAttribute("Name", "mail"))
                                        .WithText("Resend")
                                    )
                                    .AddHtmlElement("div", element => element.AddClass("ux-account-links")
                                        .AddHtmlElement("a", a => a.AddAttribute("href", "Account/Login").WithText("Back to log in")))
                                )
                            )
                        );

                    }

                    var code = GetCodeBehind();
                    code.AddField("string?", "message");

                    code.AddProperty("InputModel", "Input", input =>
                    {
                        input.Private();
                        // If you are getting a build warning (BL0008) here, the simple standard MSFT solution is applied in the static content files instead — a C# weaver issue prevents this initializer (new() -> default!) from migrating cleanly for pre-existing codebases.
                        input.WithInitialValue("new()");
                        input.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromFormAttribute").RemoveSuffix("Attribute"));
                    });

                    code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnValidSubmitAsync", onValidSubmitAsync =>
                    {
                        onValidSubmitAsync.Private().Async();

                        onValidSubmitAsync.AddStatement("await AuthService.ResendEmailConfirmation(Input.Email);");
                        onValidSubmitAsync.AddStatement("message = \"Verification email sent. Please check your email.\";");
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
        // page's @code into the sibling ResendEmailConfirmationCodeBehindTemplate (.razor.cs) when
        // present, falling back to an inline @code block otherwise.
        private IBuildsCSharpMembers _codeBehind;

        public ICSharpFileBuilderTemplate CodeBehindTemplate { get; private set; }

        public override ICSharpCodeContext RootCodeContext => GetCodeBehind();

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            CodeBehindTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(ResendEmailConfirmationCodeBehindTemplate.TemplateId);
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