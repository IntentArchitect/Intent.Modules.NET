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

                    // Emit a MudBlazor-styled body when MudBlazor is installed, otherwise the default Bootstrap body.
                    if (ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor"))
                    {
                        file.AddHtmlElement("MudPaper", mudPaper => mudPaper
                            .AddAttribute("Class", "pa-4 mb-4 ux-gradient-primary")
                            .AddAttribute("Elevation", "0")
                            .AddHtmlElement("MudText", text => text
                                .AddAttribute("Typo", "Typo.h4")
                                .AddAttribute("Class", "text-white font-weight-bold mb-2")
                                .AddHtmlElement("MudIcon", icon => icon
                                    .AddAttribute("Icon", "@Icons.Material.Filled.MarkEmailRead")
                                    .AddAttribute("Class", "mr-2"))
                                .WithText("Confirm email"))
                            .AddHtmlElement("MudText", text => text
                                .AddAttribute("Typo", "Typo.body1")
                                .AddAttribute("Class", "text-white opacity-90")
                                .WithText("We are verifying your email address and completing your account setup.")));

                        file.AddHtmlElement("MudGrid", grid => grid
                            .AddAttribute("Spacing", "3")
                            .AddHtmlElement("MudItem", item => item
                                .AddAttribute("xs", "12")
                                .AddAttribute("md", "8")
                                .AddAttribute("lg", "6")
                                .AddHtmlElement("MudCard", card => card
                                    .AddAttribute("Class", "ux-fade-in-up")
                                    .AddAttribute("Style", "animation-delay: 0.1s")
                                    .AddHtmlElement("MudCardContent", content => content
                                        .AddHtmlElement("MudText", text => text
                                            .AddAttribute("Typo", "Typo.h5")
                                            .WithText("Email confirmation status"))
                                        .AddHtmlElement("MudText", text => text
                                            .AddAttribute("Typo", "Typo.body2")
                                            .AddAttribute("Class", "mb-4")
                                            .WithText("The result of your email confirmation request is shown below."))
                                        .AddHtmlElement("StatusMessage", status => status
                                            .AddAttribute("Message", "@statusMessage"))
                                        .AddHtmlElement("MudStack", stack => stack
                                            .AddAttribute("Spacing", "1")
                                            .AddAttribute("Class", "mt-4")
                                            .AddHtmlElement("MudLink", link => link
                                                .AddAttribute("Href", "Account/Login")
                                                .WithText("Continue to log in")))))));
                    }
                    else
                    {
                        file.AddHtmlElement("h1", element => element.WithText("Confirm email"));
                        file.AddHtmlElement($"StatusMessage Message=\"@statusMessage\"");
                    }

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
            var config = RazorFile.GetConfig();
            // TEMP (verification): force full overwrite so the Software Factory reflects pure template
            // output rather than merging into the hand-authored file.
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