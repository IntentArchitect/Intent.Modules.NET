using System;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRedirectManager;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ForgotPassword
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ForgotPasswordTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.ForgotPasswordTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="ForgotPasswordTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ForgotPasswordTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"ForgotPassword")
                .Configure(file =>
                {
                    file.AddPageDirective($"/Account/ForgotPassword");
                    file.AddUsing("System.ComponentModel.DataAnnotations");
                    file.AddInjectDirective(GetTypeName(AuthServiceInterfaceTemplate.TemplateId), "AuthService");

                    file.AddHtmlElement("PageTitle", element => element.WithText($"Forgot your password?"));
                    file.AddHtmlElement("h1", element => element.WithText("Forgot your password?"));
                    file.AddHtmlElement($"hr");
                    file.AddHtmlElement($"div", element => element.AddClass("row")
                        .AddHtmlElement("div", element => element.AddClass("col-md-4")
                            .AddHtmlElement("EditForm", element => element.AddAttribute("Model", "Input").AddAttribute("FormName", "forgot-password").AddAttribute("OnValidSubmit", "OnValidSubmitAsync").AddAttribute("method", "post")
                                .AddHtmlElement("DataAnnotationsValidator")
                                .AddHtmlElement("ValidationSummary", element => element.AddClass("text-danger").AddAttribute("role", "alert"))
                                .AddHtmlElement("div", element => element.AddClass("form-floating mb-3")
                                    .AddHtmlElement("InputText", element => element.AddClass("form-control").AddAttribute("@bind-Value", "Input.Email").AddAttribute("autocomplete", "username").AddAttribute("aria-required", "true").AddAttribute("placeholder", "name@example.com"))
                                    .AddHtmlElement("label", element => element.AddClass("form-label").AddAttribute("for", "email").WithText("Email"))
                                    .AddHtmlElement("ValidationMessage", element => element.AddClass("text-danger").AddAttribute("For", "() => Input.Email"))
                                .AddHtmlElement("button", element => element.AddClass("w-100 btn btn-lg btn-primary").AddAttribute("type", "submit").WithText("Reset password"))
                                )
                             )
                         )
                     );

                    file.AddCodeBlock(code =>
                    {
                        code.AddProperty("InputModel", "Input", input =>
                        {
                            input.Private();
                            input.WithInitialValue("new()");
                            input.AddAttribute("SupplyParameterFromForm");
                        });

                        code.AddMethod("Task", "OnValidSubmitAsync", onValidSubmitAsync =>
                        {
                            onValidSubmitAsync.Private().Async();

                            onValidSubmitAsync.AddStatement("await AuthService.ForgotPassword(Input.Email);");
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