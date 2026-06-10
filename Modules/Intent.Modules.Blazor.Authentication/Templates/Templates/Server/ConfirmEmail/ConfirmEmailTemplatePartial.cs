using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ConfirmEmailCodeBehind;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRedirectManager;
using Intent.Modules.Blazor.Settings;
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

                    var code = GetCodeBehind();
                    code.AddField("string?", "statusMessage", c => c.Private());

                    code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext"), "HttpContext", httpContext => httpContext.WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));

                    code.AddProperty("string?", "UserId", httpContext => httpContext.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));
                    code.AddProperty("string?", "Code", httpContext => httpContext.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute").RemoveSuffix("Attribute")));

                    code.AddMethod(code.Template.UseType("System.Threading.Tasks.Task"), "OnInitializedAsync", onInitializedAsync =>
                    {
                        onInitializedAsync.Async().Protected().Override();

                        onInitializedAsync.AddIfStatement("UserId is null || Code is null", @if =>
                        {
                            @if.AddStatement("RedirectManager.RedirectTo(\"\");");
                        });

                        onInitializedAsync.AddStatement("statusMessage = await AuthService.ConfirmEmail(UserId, Code);");
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
        // page's @code into the sibling ConfirmEmailCodeBehindTemplate (.razor.cs) when present,
        // falling back to an inline @code block otherwise.
        private IBuildsCSharpMembers _codeBehind;

        public ICSharpFileBuilderTemplate CodeBehindTemplate { get; private set; }

        public override ICSharpCodeContext RootCodeContext => GetCodeBehind();

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            CodeBehindTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(ConfirmEmailCodeBehindTemplate.TemplateId);
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