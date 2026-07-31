using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutProfile
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RazorLayoutProfileTemplate : RazorTemplateBase<LayoutProfileMenuModel>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorLayoutProfileTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RazorLayoutProfileTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RazorLayoutProfileTemplate(IOutputTarget outputTarget, LayoutProfileMenuModel model) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"{Model.InternalElement.ParentElement.Name}{Model.Name}")
                .Configure(file =>
                {
                    file.AddHtmlElement("UserMenu");
                });
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public IRazorFile RazorFile { get; }

        [IntentManaged(Mode.Ignore)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            var config = RazorFile.GetConfig();

            return new RazorFileConfig($"{Model.InternalElement.ParentElement.Name}{Model.Name}", string.Empty,
                fileName: $"{Model.InternalElement.ParentElement.Name}{Model.Name}",
                relativeLocation: GetRelativeLocation(),
                overwriteBehaviour: Intent.Templates.OverwriteBehaviour.OverwriteDisabled);
        }

        private string GetRelativeLocation()
        {
            var path = string.Join("/", Model.InternalElement.ParentElement.AsLayoutModel().GetParentFolderNames());
            return path;
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        public override string TransformText() => RazorFile.ToString();
    }
}
