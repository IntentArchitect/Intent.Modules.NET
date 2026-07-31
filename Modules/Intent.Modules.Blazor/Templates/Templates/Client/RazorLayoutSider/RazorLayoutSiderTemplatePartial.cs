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

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutSider
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RazorLayoutSiderTemplate : RazorTemplateBase<LayoutSiderModel>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorLayoutSiderTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RazorLayoutSiderTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RazorLayoutSiderTemplate(IOutputTarget outputTarget, LayoutSiderModel model) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"{Model.InternalElement.ParentElement.Name}{Model.Name}")
                .Configure(file =>
                {
                    // TODO: This should be moved into Mudblazor module to seperate completely
                    // right now, dependency between the two
                    file.AddHtmlElement("nav", drawer =>
                    {
                        drawer.AddAttribute("class", "ux-nav-drawer");
                        //drawer.AddAttribute("Open", "DrawerOpen")
                        //.AddAttribute("OpenChanged", "DrawerOpenChanged")
                        //.AddAttribute("ClipMode", "DrawerClipMode.Always");

                        drawer.AddHtmlElement("MudNavMenu", menu =>
                        {
                            menu.AddHtmlElement("MudNavLink", link =>
                            {
                                link.AddAttribute("Href", "/")
                                    .AddAttribute("Match", "NavLinkMatch.All")
                                    .WithText("Home");
                            });
                        });
                    });
                });
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public IRazorFile RazorFile { get; }

        /// <inheritdoc />
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
        [IntentManaged(Mode.Fully)]
        public override string TransformText() => RazorFile.ToString();
    }
}
