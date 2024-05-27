using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.ClientImportsRazor;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Server.ServerImportsRazor
{
    [IntentManaged(Mode.Merge)]
    public partial class ServerImportsRazorTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Server.ServerImportsRazorTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServerImportsRazorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
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
                file.AddUsing("Microsoft.AspNetCore.Http");
                file.AddUsing(GetTemplate<IRazorFileTemplate>(ClientImportsRazorTemplate.TemplateId).Namespace);
            });
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