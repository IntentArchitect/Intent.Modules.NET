using System;
using System.Linq;
using System.Text;
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

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutFooter
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RazorLayoutFooterTemplate : RazorTemplateBase<LayoutFooterModel>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorLayoutFooterTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RazorLayoutFooterTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RazorLayoutFooterTemplate(IOutputTarget outputTarget, LayoutFooterModel model) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"{Model.InternalElement.ParentElement.Name}{Model.Name}")
                .Configure(file => { });
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
                overwriteBehaviour: Intent.Templates.OverwriteBehaviour.OverwriteDisabled)
                .WithAIContext(GetIntentionContext());
        }

        private string GetIntentionContext()
        {
            var intention = new StringBuilder();
            AddFooterNavigationContext(intention);
            return intention.ToString();
        }

        private void AddFooterNavigationContext(StringBuilder intention)
        {
            intention.AppendLine("The footer consists of the following:");
            foreach (var associationEnd in Model.InternalElement.ParentElement.AssociatedElements
                .Where(a => a.IsNavigationTargetEndModel()
                && a.AsNavigationTargetEndModel().HasLayoutPlacement()
                && a.AsNavigationTargetEndModel().GetLayoutPlacement().Regions().Any(e => e.Name == "Footer")))
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
            }
        }
        private string GetRelativeLocation()
        {
            var path = string.Join("/", Model.InternalElement.ParentElement.AsLayoutModel().GetParentFolderNames());
            return path;
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        public override string TransformText() => "// Content to be generated";
    }
}