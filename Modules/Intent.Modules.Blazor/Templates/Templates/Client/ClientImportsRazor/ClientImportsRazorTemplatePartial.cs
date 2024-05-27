using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.ClientImportsRazor
{
    [IntentManaged(Mode.Merge)]
    public partial class ClientImportsRazorTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Client.ClientImportsRazorTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ClientImportsRazorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = new RazorFile(this).Configure(file =>
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

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            OutputTarget.GetProject().AddProperty("NoDefaultLaunchSettingsFile", "true");
            OutputTarget.GetProject().AddProperty("StaticWebAssetProjectMode", "Default");
        }

        public RazorFile RazorFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            return new RazorFileConfig(
                className: $"_Imports",
                @namespace: this.GetNamespace(),
                relativeLocation: this.GetFolderPath()
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return RazorFile.Build().ToString();
        }
    }
}