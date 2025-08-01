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
using System;

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

                    file.AddUsing("System.ComponentModel.DataAnnotations");
                    file.AddUsing("Microsoft.AspNetCore.Authentication");
                    file.AddUsing("Microsoft.AspNetCore.Identity");
                    file.AddInjectDirective(GetTypeName(AuthServiceInterfaceTemplate.TemplateId), "AuthService");

                    file.AddHtmlElement("PageTitle", element => element.WithText($"Register"));

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

                    file.AddCodeBlock(code =>
                    {
                        code.AddField("IEnumerable<IdentityError>?", "identityErrors");

                        code.AddProperty("InputModel", "Input", input =>
                        {
                            input.Private();
                            input.WithInitialValue("new()");
                            input.AddAttribute("SupplyParameterFromForm");
                        });
                        code.AddProperty("string?", "ReturnUrl", input =>
                        {
                            input.Private();
                            input.AddAttribute("SupplyParameterFromQuery");
                        });

                        code.AddProperty("string?", "Message", p => p.Private().WithoutSetter().Getter.WithExpressionImplementation("identityErrors is null ? null : $\"Error: {string.Join(\", \", identityErrors.Select(error => error.Description))}\""));

                        code.AddMethod("Task", "RegisterUser", onValidSubmitAsync =>
                        {
                            onValidSubmitAsync.Async();

                            onValidSubmitAsync.AddStatement("await AuthService.Register(Input.Email, Input.Password, ReturnUrl);");
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