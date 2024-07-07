using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutCodeBehind
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RazorLayoutCodeBehindTemplate : CSharpTemplateBase<LayoutModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorLayoutCodeBehindTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RazorLayoutCodeBehindTemplate(IOutputTarget outputTarget, LayoutModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .WithFileExtension("razor.cs")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.Partial();
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