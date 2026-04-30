using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.AI;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Build.Framework;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AITasksFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.AITasksFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.AITaskManager.RegisterTaskProvider(new TemplateAITaskProvider(application, GetTasks));
        }

        public IAITask[] GetTasks(IChange[] changes, IOutputFile[] outputFiles, IApplication application)
        {
            var tasks = new List<IAITask>();

            tasks.AddRange(GetBlazorComponentImplementationTasks(changes, application));

            return [.. tasks];
        }

        private IEnumerable<IAITask> GetBlazorComponentImplementationTasks(IChange[] changes, IApplication application)
        {
            var relevantChangeTypes = new ChangeType[] { ChangeType.Create, ChangeType.Overwrite };

            var handlerChanges = changes.Where(c =>
                c.Template?.Id == RazorComponentCodeBehindTemplate.TemplateId &&
                relevantChangeTypes.Contains(c.ChangeType) &&
                !c.IsIgnored);

            foreach (var change in handlerChanges)
            {
                if (!change.Template.TryCastTemplate<ICSharpFileBuilderTemplate, ComponentModel>(out var template, out var model))
                {
                    continue;
                }

                yield return CreateGenerateComponentAITask(application, template, model, change);
            }
        }

        private IAITask CreateGenerateComponentAITask(IApplication application, ICSharpFileBuilderTemplate template, ComponentModel model, IChange change)
        {
            var intention = new StringBuilder();

            AddNavigatesToContext(model, intention);
            AddShowDialogContext(model, intention);

            var componenRazorTemplate = template.ExecutionContext.FindTemplateInstance(RazorComponentTemplate.TemplateId, model.Id);

            var relatedTemplates = new[]
            {
                componenRazorTemplate,
            }
            .Where(t => t is not null);

            return new TemplateAITask(template, [.. relatedTemplates])
            {
                Type = "Implement Blazor Component",
                Title = $"Implement Blazor Component: {model.Name}",
                Context = @$"""
                            ## User has modeled the following intentions:
                            {intention}
                        """,
                Instructions =
                        $"""Implement the {model.Name} Blazor component using the appropriate skill(s)."""
            };
        }

        // Add context about which other pages this component navigates to
        private static void AddNavigatesToContext(ComponentModel model, StringBuilder intention)
        {
            foreach (var navigation in model.InternalElement.AssociatedElements.Where(e => e.IsNavigationEndModel() && e.IsNavigable))
            {
                var navEndModel = navigation.AsNavigationEndModel();
                intention.AppendLine($"- This pages navigates to the {navEndModel.TypeReference.Element.Name} component");
            }
        }

        // Add context about which dialogs this component shows
        private static void AddShowDialogContext(ComponentModel model, StringBuilder intention)
        {
            // Show Dialog associations
            foreach (var operation in model.Operations.Where(o => o.InternalElement.AssociatedElements.Any(e => e.IsShowDialogTargetEndModel())))
            {
                foreach (var association in operation.InternalElement.AssociatedElements.Where(e => e.IsShowDialogTargetEndModel()))
                {
                    var dialogTargetEnd = association.AsShowDialogTargetEndModel();
                    intention.AppendLine($"- The {operation.Name} operation opens a dialog to show the {dialogTargetEnd.TypeReference.Element.Name} component");
                }
            }
        }
    }
}