using System;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.ClientImportsRazor
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientImportsRazorTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.ClientImportsRazorTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="ClientImportsRazorTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ClientImportsRazorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, "_Imports").Configure(file =>
            {
                file.AddUsing("System.Net.Http");
                file.AddUsing("System.Net.Http.Json");
                file.AddUsing("Microsoft.AspNetCore.Components.Forms");
                file.AddUsing("Microsoft.AspNetCore.Components.Routing");
                file.AddUsing("Microsoft.AspNetCore.Components.Web");
                file.AddUsing("static Microsoft.AspNetCore.Components.Web.RenderMode");
                file.AddUsing("Microsoft.AspNetCore.Components.Web.Virtualization");
                file.AddUsing("Microsoft.JSInterop");
            });
        }

        /// <inheritdoc />
        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            OutputTarget.GetProject().AddProperty("NoDefaultLaunchSettingsFile", "true");
            OutputTarget.GetProject().AddProperty("StaticWebAssetProjectMode", "Default");
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