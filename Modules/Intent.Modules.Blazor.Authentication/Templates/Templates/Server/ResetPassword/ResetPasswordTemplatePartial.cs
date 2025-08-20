using System;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
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

                    file.AddUsing("System.ComponentModel.DataAnnotations");
                    file.AddUsing("Microsoft.AspNetCore.WebUtilities");
                    file.AddUsing("System.Text");
                    file.AddInjectDirective(GetTypeName(AuthServiceInterfaceTemplate.TemplateId), "AuthService");
                    file.AddInjectDirective(GetTypeName(IdentityRedirectManager.IdentityRedirectManagerTemplate.TemplateId), "RedirectManager");

                    file.AddHtmlElement("PageTitle", element => element.WithText($"Reset password"));
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

                    file.AddCodeBlock(code =>
                    {
                        code.AddField("IEnumerable<IdentityError>?", "identityErrors");

                        code.AddProperty("InputModel", "Input", input =>
                        {
                            input.Private();
                            input.WithInitialValue("new()");
                            input.AddAttribute("SupplyParameterFromForm");
                        });

                        code.AddProperty("string?", "Code", input =>
                        {
                            input.Private();
                            input.AddAttribute("SupplyParameterFromQuery");
                        });

                        code.AddProperty("string?", "Message", p => p.Private().WithoutSetter().Getter.WithExpressionImplementation("identityErrors is null ? null : $\"Error: {string.Join(\", \", identityErrors.Select(error => error.Description))}\""));

                        code.AddMethod("void", "OnInitialized", onValidSubmitAsync =>
                        {
                            onValidSubmitAsync.Protected().Override();

                            onValidSubmitAsync.AddIfStatement("Code is null", @if =>
                            {
                                @if.AddStatement("RedirectManager.RedirectTo(\"Account/ResetPasswordConfirmation\");");
                            });

                            onValidSubmitAsync.AddStatement("Input.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));");
                        });

                        code.AddMethod("Task", "OnValidSubmitAsync", onValidSubmitAsync =>
                        {
                            onValidSubmitAsync.Private().Async();

                            onValidSubmitAsync.AddStatement("await AuthService.ResetPassword(Input.Email, Input.Code, Input.Password);");
                        });

                        code.AddClass("InputModel", inputModel =>
                        {
                            inputModel.Private().Sealed();

                            inputModel.AddProperty("string", "Email", email =>
                            {
                                email.AddAttribute("Required");
                                email.AddAttribute("EmailAddress");
                                email.WithInitialValue("\"\"");
                            });

                            inputModel.AddProperty("string", "Password", email =>
                            {
                                email.AddAttribute("Required");
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
                                email.AddAttribute("Required");
                                email.WithInitialValue("\"\"");
                            });
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
    }
}