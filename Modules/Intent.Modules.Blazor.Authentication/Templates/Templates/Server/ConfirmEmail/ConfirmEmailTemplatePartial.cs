using System;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
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

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ConfirmEmail
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ConfirmEmailTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.ConfirmEmailTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="ConfirmEmailTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ConfirmEmailTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"ConfirmEmail")
                .Configure(file =>
                {
                    file.AddPageDirective($"/Account/ConfirmEmail");
                    file.AddInjectDirective(GetTypeName(IdentityRedirectManagerTemplate.TemplateId), "RedirectManager");
                    file.AddInjectDirective(GetTypeName(AuthServiceInterfaceTemplate.TemplateId), "AuthService");
                    file.AddHtmlElement("PageTitle", element => element.WithText($"Confirm email"));
                    file.AddHtmlElement("h1", element => element.WithText("Confirm email"));
                    file.AddHtmlElement($"StatusMessage Message=\"@statusMessage\"");

                    file.AddCodeBlock(code =>
                    {
                        code.AddField("string?", "statusMessage", c => c.Private());

                        code.AddProperty("HttpContext", "HttpContext", httpContext => httpContext.WithInitialValue("default!").AddAttribute("CascadingParameter"));

                        code.AddProperty("string?", "UserId", httpContext => httpContext.AddAttribute("SupplyParameterFromQuery"));
                        code.AddProperty("string?", "Code", httpContext => httpContext.AddAttribute("SupplyParameterFromQuery"));

                        code.AddMethod("Task", "OnInitializedAsync", onInitializedAsync =>
                        {
                            onInitializedAsync.Async().Protected().Override();

                            onInitializedAsync.AddIfStatement("UserId is null || Code is null", @if =>
                            {
                                @if.AddStatement("RedirectManager.RedirectTo(\"\");");
                            });

                            onInitializedAsync.AddStatement("statusMessage = await AuthService.ConfirmEmail(UserId, Code);");
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
            return base.CanRunTemplate() && !ExecutionContext.GetSettings().GetAuthenticationType().Authentication().IsOidc();
        }
    }
}