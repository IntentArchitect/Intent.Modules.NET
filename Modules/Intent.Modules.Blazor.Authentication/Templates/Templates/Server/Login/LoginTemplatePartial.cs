using System;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRedirectManager;
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

                    file.AddUsing("System.ComponentModel.DataAnnotations");
                    file.AddUsing("Microsoft.AspNetCore.Authentication");
                    file.AddUsing("Microsoft.AspNetCore.Identity");
                    file.AddInjectDirective(GetTypeName(AuthServiceInterfaceTemplate.TemplateId), "AuthService");
                    file.AddInjectDirective("Microsoft.AspNetCore.Components.NavigationManager", "NavigationManager");

                    file.AddHtmlElement("PageTitle", element => element.WithText($"Log in"));

                    file.AddHtmlElement("h1", element => element.WithText("Log in"));
                    file.AddHtmlElement($"div", element => element.AddClass("row")
                        .AddHtmlElement("div", element => element.AddClass("col-md-4")
                            .AddHtmlElement("section", element => element
                                .AddHtmlElement("StatusMessage", element => element.AddAttribute("Message", "@errorMessage"))
                                .AddHtmlElement("EditForm", element => element.AddAttribute("Model", "Input").AddAttribute("FormName", "login").AddAttribute("OnValidSubmit", "LoginUser").AddAttribute("method", "post")
                                    .AddHtmlElement("DataAnnotationsValidator")
                                    .AddHtmlElement("h2", element => element.WithText("Use a local account to log in."))
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
                                        .AddHtmlElement("label", element => element.AddClass("form-label")
                                            .AddHtmlElement("InputCheckbox", element => element.AddClass("darker-border-checkbox form-check-input").AddAttribute("@bind-Value", "Input.RememberMe").WithText("Remember me"))
                                            )
                                    )
                                    .AddHtmlElement("div", element => element
                                        .AddHtmlElement("button", element => element.AddClass("w-100 btn btn-lg btn-primary").AddAttribute("type", "submit").WithText("Login"))
                                    )
                                    .AddHtmlElement("div", element => element
                                        .AddHtmlElement("p", element => element
                                            .AddHtmlElement("a", element => element.AddAttribute("href", "Account/ForgotPassword").WithText("Forgot your password?"))
                                            )
                                    )
                                    .AddHtmlElement("div", element => element
                                        .AddHtmlElement("p", element => element
                                            .AddHtmlElement("a", element => element.AddAttribute("href", "@(NavigationManager.GetUriWithQueryParameters(\"Account/Register\", new Dictionary<string, object?> { [\"ReturnUrl\"] = ReturnUrl }))").WithText("Register as a new user"))
                                            )
                                    )
                                    .AddHtmlElement("div", element => element
                                        .AddHtmlElement("p", element => element
                                            .AddHtmlElement("a", element => element.AddAttribute("href", "Account/ResendEmailConfirmation").WithText("Resend email confirmation"))
                                            )
                                    )
                                )
                             )
                         )
                     );
                    if (ExecutionContext.GetSettings().GetAuthenticationType().Authentication().IsAspnetcoreIdentity())
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
                        code.AddField("string?", "errorMessage");

                        code.AddProperty("HttpContext", "HttpContext", input =>
                        {
                            input.Private();
                            input.WithInitialValue("default!");
                            input.AddAttribute("CascadingParameter");
                        });
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

                        code.AddMethod("Task", "OnInitializedAsync", onValidSubmitAsync =>
                        {
                            onValidSubmitAsync.Protected().Async().Override();

                            onValidSubmitAsync.AddIfStatement("HttpMethods.IsGet(HttpContext.Request.Method)", @if =>
                            {
                                if (ExecutionContext.GetSettings().GetAuthenticationType().Authentication().IsAspnetcoreIdentity())
                                {
                                    @if.AddStatement("await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);");
                                }
                                else
                                {
                                    @if.AddStatement("await HttpContext.SignOutAsync();");
                                }
                            });
                        });

                        code.AddMethod("Task", "LoginUser", onValidSubmitAsync =>
                        {
                            onValidSubmitAsync.Async();

                            onValidSubmitAsync.AddStatement("await AuthService.Login(Input.Email, Input.Password, Input.RememberMe, ReturnUrl);");
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
                                email.AddAttribute("DataType(DataType.Password)");
                                email.WithInitialValue("\"\"");
                            });

                            inputModel.AddProperty("bool", "RememberMe", email =>
                            {
                                email.AddAttribute("Display(Name = \"Remember me?\")");
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
    }
}