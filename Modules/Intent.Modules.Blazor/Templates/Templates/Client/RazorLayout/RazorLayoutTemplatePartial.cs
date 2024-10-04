using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutCodeBehind;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Ignore, Comments = Mode.Fully)]
    public class RazorLayoutTemplate : RazorComponentTemplateBase<LayoutModel>
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorLayoutTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RazorLayoutTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public RazorLayoutTemplate(IOutputTarget outputTarget, LayoutModel model) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"{Model.Name}")
                .Configure(file =>
                {
                    file.AddInheritsDirective("LayoutComponentBase");

                    ComponentBuilderProvider.BuildComponent(Model.InternalElement, file);

                    if (file.ChildNodes.All(x => x is not IHtmlElement))
                    {
                        file.AddHtmlElement("div", div => div.WithText("@Body"));
                    }

                    var block = GetCodeBehind();
                    block.AddCodeBlockMembers(this, Model.InternalElement);
                });
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        public sealed override IRazorFile RazorFile { get; }

        protected override string CodeBehindTemplateId => RazorLayoutCodeBehindTemplate.TemplateId;

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