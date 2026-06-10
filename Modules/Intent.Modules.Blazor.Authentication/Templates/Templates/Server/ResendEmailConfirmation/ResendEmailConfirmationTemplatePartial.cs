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

                    // When MudBlazor is installed the page body is provided by the hand-authored
                    // MudBlazor markup (preserved on merge); only emit the default Bootstrap body otherwise.
                    if (!ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor"))
                    {
                    file.AddHtmlElement("h1", element => element.WithText("Resend email confirmation"));
                    file.AddHtmlElement("h2", element => element.WithText("Enter your email."));
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
                                .AddHtmlElement("button", element => element.AddClass("w-100 btn btn-lg btn-primary").AddAttribute("type", "submit").WithText("Resend"))
                                )
                             )
                         )
                     );

                    }

                    var code = GetCodeBehind();
                    code.AddField("string?", "message");

                    code.AddProperty("InputModel", "Input", input =>
                    {
                        input.Private();
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