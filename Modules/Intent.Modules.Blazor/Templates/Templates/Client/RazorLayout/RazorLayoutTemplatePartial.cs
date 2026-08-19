using System;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutCodeBehind;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
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
                        file.AddHtmlElement("div", div =>
                        {
                            div.AddAttribute("class", "ux-app-shell");

                            if (Model.Header is not null)
                            {
                                div.AddHtmlElement($"{Model.Name}Header");
                            }

                            if (Model.Sider is not null)
                            {
                                div.AddHtmlElement($"{Model.Name}Sider");
                            }

                            div.AddHtmlElement("main", main =>
                            {
                                main.AddAttribute("class", "ux-app-content");
                                main.WithText("@Body");
                            });

                            if (Model.Footer is not null)
                            {
                                div.AddHtmlElement($"{Model.Name}Footer");
                            }
                        });
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
        [IntentManaged(Mode.Ignore)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            var config = RazorFile.GetConfig();
            return new RazorFileConfig(config.ClassName, @namespace: config.Namespace, 
                relativeLocation: config.LocationInProject,
                overwriteBehaviour: Intent.Templates.OverwriteBehaviour.OverwriteDisabled)
                .WithAIContext(GetIntentionContext());
        }

        private string GetIntentionContext()
        {
            var intention = new StringBuilder();
            AddThemeContent(intention);
            AddNavigationContext(intention);
            return intention.ToString();
        }

        private void AddThemeContent(StringBuilder intention)
        {
            if (!ExecutionContext.GetSettings().GetBlazor().EnableThemeToggle())
            {
                intention.AppendLine("This Layout does not have a ThemeToggle component in its Header, so it will not support theme toggling.");
                return;
            }

            intention.AppendLine("This Layout has a ThemeToggle component in its Header, so it will support theme toggling.");
        }

        private void AddNavigationContext(StringBuilder intention)
        {
            intention.AppendLine("This Layout's navigation consists of the following. Each item's region (Header/Sider/Footer/Profile) is stated in its own placement comment below and must be resolved from that text when populating a region file — this list is not pre-sorted by region:");
            foreach (var associationEnd in Model.InternalElement.AssociatedElements
                .Where(a => a.IsNavigationTargetEndModel()))
            {
                var navTarget = associationEnd.AsNavigationTargetEndModel();
                // the target should always be a components
                var targetComponent = navTarget?.Association?.SourceEnd?.InternalElement?.ParentElement?.AsComponentModel();

                if (targetComponent is null || !targetComponent.HasPage())
                {
                    continue;
                }

                var pageRoute = targetComponent.GetPage().Route();
                var isSecured = targetComponent.HasSecured();
                var roles = targetComponent.HasSecured() ? targetComponent.GetSecured().Roles()?.Split(',') : [];
                var policies = targetComponent.HasSecured() ? targetComponent.GetSecured().Policy()?.Split(',') : [];

                intention.AppendLine($"- a navigation link to the {targetComponent.Name} with route '{pageRoute}'");

                if (isSecured)
                {
                    intention.Append(" and requires authorization");
                }

                if (roles is not null && roles?.Length != 0)
                {
                    intention.Append($" with required role(s): {string.Join(',', roles)}");

                    if (policies is not null && policies?.Length != 0)
                    {
                        intention.Append(" and ");
                    }
                }

                if (policies is not null && policies?.Length != 0)
                {
                    intention.Append($" with required policies(s): {string.Join(',', policies)}");
                }

                intention.Append(string.IsNullOrWhiteSpace(navTarget.Comment)
                    ? " (no placement comment - defaults to Sider)"
                    : $" (placement comment: \"{navTarget.Comment}\")");
            }
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public override string TransformText() => RazorFile.ToString();
    }
}
