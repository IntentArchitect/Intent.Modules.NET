using System;
using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Authentication.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Client.RedirectToLogin
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RedirectToLoginTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Client.RedirectToLoginTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RedirectToLoginTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedirectToLoginTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"RedirectToLogin")
                .Configure(file =>
                {
                    file.AddInjectDirective("Microsoft.AspNetCore.Components.NavigationManager", "NavigationManager");

                    file.AddCodeBlock(code =>
                    {
                        code.AddMethod("void", "OnInitialized", onInitialized =>
                        {
                            onInitialized.Protected().Override();

                            // Route resolved from the modelled login page rather than hardcoded, so
                            // that every generated redirect shares one source.
                            onInitialized.AddStatement($"NavigationManager.NavigateTo(\"{this.GetLoginRoute()}?returnUrl=\" + Uri.EscapeDataString(NavigationManager.Uri), forceLoad: true);");
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