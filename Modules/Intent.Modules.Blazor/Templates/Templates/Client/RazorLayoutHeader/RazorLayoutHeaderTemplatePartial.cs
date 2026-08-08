using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using JetBrains.Annotations;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutHeader
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RazorLayoutHeaderTemplate : RazorTemplateBase<LayoutHeaderModel>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorLayoutHeaderTemplate";

        private RazorTemplateBase<LayoutModel>? _parentTemplate;

        /// <summary>
        /// Creates a new instance of <see cref="RazorLayoutHeaderTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RazorLayoutHeaderTemplate(IOutputTarget outputTarget, LayoutHeaderModel model) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"{Model.InternalElement.ParentElement.Name}{Model.Name}")
                .Configure(file =>
                {
                    if (ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor"))
                    {
                        file.AddHtmlElement("MudAppBar", appBar =>
                        {
                            appBar.AddHtmlElement("span", span =>
                            {
                                span.AddAttribute("onclick", "navDrawer.toggle()");
                                span.AddHtmlElement("MudIconButton", @icon =>
                                {
                                    @icon.AddAttribute("Icon", "@Icons.Material.Filled.Menu")
                                        .AddAttribute("Color", "Color.Inherit")
                                        .AddAttribute("Edge", "Edge.Start");
                                });
                            });

                            appBar.AddHtmlElement("MudButton", @button =>
                            {
                                button.AddAttribute("Href", "/")
                                    .AddAttribute("Class", "my-2 mr-2")
                                    .AddAttribute("Color", "Color.Inherit");

                                button.AddHtmlElement("MudText", text =>
                                {
                                    text.AddAttribute("Typo.h5");
                                    text.Text = ExecutionContext.GetApplicationConfig().Name;
                                });
                            });

                            appBar.AddHtmlElement("MudSpacer");
                            if (ExecutionContext.Settings.GetBlazor().EnableThemeToggle())
                            {
                                appBar.AddHtmlElement("ThemeToggle", toggle =>
                                {
                                //toggle.AddAttribute("OnToggle", "@(() => OnThemeToggle.InvokeAsync())");
                                });
                            }

                            if (Model.InternalElement.ParentElement.AsLayoutModel().ProfileMenu is not null)
                            {
                                appBar.AddHtmlElement("UserMenu");
                            }
                        });
                    }
                    else
                    {
                        file.AddHtmlElement("header", header =>
                        {
                            header.AddAttribute("class", "ux-app-header ux-gradient-primary d-flex align-items-center gap-2 px-3 py-2");

                            header.AddHtmlElement("button", button =>
                            {
                                button.AddAttribute("type", "button")
                                    .AddAttribute("class", "btn-icon text-white")
                                    .AddAttribute("onclick", "navDrawer.toggle()")
                                    .AddAttribute("aria-label", "Toggle navigation");

                                button.AddHtmlElement("span", span =>
                                {
                                    span.AddAttribute("class", "navbar-toggler-icon");
                                });
                            });

                            header.AddHtmlElement("a", link =>
                            {
                                link.AddAttribute("href", "/")
                                    .AddAttribute("class", "fw-bold fs-5 text-white text-decoration-none");
                                link.Text = ExecutionContext.GetApplicationConfig().Name;
                            });

                            header.AddHtmlElement("div", spacer =>
                            {
                                spacer.AddAttribute("class", "flex-grow-1");
                            });

                            if (ExecutionContext.Settings.GetBlazor().EnableThemeToggle())
                            {
                                header.AddHtmlElement("ThemeToggle");
                            }

                            if (Model.InternalElement.ParentElement.AsLayoutModel().ProfileMenu is not null)
                            {
                                header.AddHtmlElement("UserMenu");
                            }
                        });
                    }
                });
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public IRazorFile RazorFile { get; }

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            return new RazorFileConfig($"{Model.InternalElement.ParentElement.Name}{Model.Name}", string.Empty,
                fileName: $"{Model.InternalElement.ParentElement.Name}{Model.Name}",
                relativeLocation: GetRelativeLocation(),
                overwriteBehaviour: OverwriteBehaviour.OverwriteDisabled);
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
