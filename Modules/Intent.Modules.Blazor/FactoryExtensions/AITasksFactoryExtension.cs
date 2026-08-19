using System.Collections.Generic;
using System.Linq;
using Intent.AI;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Microsoft.Build.Evaluation;
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

            var codeBehindTemplateIds = ComponentTemplateIds.AllClientCodeBehindTemplateIds
                .Concat(ComponentTemplateIds.AllServerCodeBehindTemplateIds)
                .ToArray();

            var handlerChanges = changes.Where(c =>
                c.Template is not null &&
                codeBehindTemplateIds.Contains(c.Template.Id) &&
                relevantChangeTypes.Contains(c.ChangeType) &&
                !c.IsIgnored);

            foreach (var change in handlerChanges)
            {
                if (change.Template is null)
                {
                    continue;
                }

                if (!change.Template.TryCastTemplate<ICSharpFileBuilderTemplate, ComponentModel>(out var template, out var model))
                {
                    continue;
                }

                yield return CreateGenerateComponentAITask(application, template, model, change);
            }
        }

        private IAITask CreateGenerateComponentAITask(IApplication application, ICSharpFileBuilderTemplate template, ComponentModel model, IChange change)
        {
            var templateInstructionExtension = "";

            var (LayoutTemplates, Instructions) = AddLayoutComponentInstructions(template, model, change);
            templateInstructionExtension += Instructions;

            var componenRazorTemplate = template.ExecutionContext.FindTemplateInstance(model.GetRazorTemplateId(), model.Id);

            var relatedTemplates = new[]
            {
                componenRazorTemplate,
            }
                .Where(t => t is not null)
                .Cast<ITemplate>()
                .Concat(LayoutTemplates.Where(t => t is not null));

            return new TemplateAITask(template, [.. relatedTemplates])
            {
                Type = "Implement Blazor Component",
                Title = $"Implement Blazor Component: {model.Name}",
                Context = @$"""
                ## Tool Guidance
                Do not use the run_software_factory tool in this conversation unless I explicitly ask you to.

                ## Implementation permissions
                - If a page’s .razor.cs is a skeleton (only parameters/navigation + empty lifecycle), you may add missing members/methods needed to fulfill the modeled intentions (load/save/model state).
                - You may inject IScopedMediator and use it to call Application commands/queries (if Mediator is installed, you may NOT add it yourself to the application if it’s not there).
                - Do not invent new service abstractions beyond IScopedMediator.
                - Do not change existing navigation methods; you may call them.
                """,
                Instructions =
                $"""Implement the {model.Name} Blazor {templateInstructionExtension}component using the appropriate skill(s)."""
            };
        }

        private static (List<ITemplate> LayoutTemplates, string Instructions) AddLayoutComponentInstructions(ICSharpFileBuilderTemplate template, ComponentModel model, IChange change)
        {
            List<ITemplate> menuTemplates = [];
            string templateInstructionExtension = "";

            foreach (var associationEnd in model.InternalElement.AssociatedElements.Where(a => a.IsNavigationSourceEndModel() && !a.IsNavigable))
            {
                // we only want to add the menu template when the item is being created
                if (change.ChangeType == ChangeType.Create)
                {
                    var layoutTemplate = template.ExecutionContext.FindTemplateInstance(RazorLayoutTemplate.TemplateId, associationEnd.TypeReference.Element.Id);
                    if (layoutTemplate is null)
                    {
                        continue;
                    }

                    menuTemplates.Add(layoutTemplate);
                    templateInstructionExtension = $"as well as the {associationEnd.TypeReference.Element.Name} Layout ";
                }
            }

            return (LayoutTemplates: menuTemplates, Instructions: templateInstructionExtension);
        }
    }
}
