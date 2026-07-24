using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutFooterCodeBehind
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RazorLayoutFooterCodeBehindTemplate : CSharpTemplateBase<LayoutFooterModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorLayoutFooterCodeBehindTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RazorLayoutFooterCodeBehindTemplate(IOutputTarget outputTarget, LayoutFooterModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace([.. Model.InternalElement.ParentElement.AsLayoutModel().GetParentFolderNames()]), this.GetRelativeLocation())
                .AddClass($"{Model.InternalElement.ParentElement.Name}{Model.Name}", @class =>
                {
                    @class.Partial();
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.InternalElement.ParentElement.Name}{Model.Name}",
                @namespace: this.GetNamespace([.. Model.InternalElement.ParentElement.AsLayoutModel().GetParentFolderNames()]),
                relativeLocation: GetRelativeLocation(),
                fileExtension: "razor.cs",
                fileName: $"{Model.InternalElement.ParentElement.Name}{Model.Name}",
                overwriteBehaviour: OverwriteBehaviour.OverwriteDisabled);
        }

        private string GetRelativeLocation()
        {
            var path = string.Join("/", Model.InternalElement.ParentElement.AsLayoutModel().GetParentFolderNames());
            return path;
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}