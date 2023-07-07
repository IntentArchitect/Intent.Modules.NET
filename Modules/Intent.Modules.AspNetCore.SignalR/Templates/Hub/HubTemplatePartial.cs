using System;
using System.Collections.Generic;
using Intent.AspNetCore.SignalR.Api;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.SignalR.Templates.Hub
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HubTemplate : CSharpTemplateBase<SignalRHubModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.SignalR.Hub";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HubTemplate(IOutputTarget outputTarget, SignalRHubModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name.RemoveSuffix("Hub")}Hub", @class =>
                {
                    @class.WithBaseType(UseType("Microsoft.AspNetCore.SignalR.Hub"));
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}