using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.AI;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
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

            var handlerChanges = changes.Where(c =>
                c.Template?.Id == RazorComponentCodeBehindTemplate.TemplateId &&
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
            var intention = new StringBuilder();
            var templateInstructionExtension = "";

            var (LayoutTemplates, Instructions) = AddLayoutComponentInstructions(template, model, change, intention);
            templateInstructionExtension += Instructions;

            AddNavigatesToContext(model, intention);
            AddShowDialogContext(model, intention);
            AddCallServiceOperationContext(model, intention);
            AddCompositionContext(model, intention);

            var componenRazorTemplate = template.ExecutionContext.FindTemplateInstance(RazorComponentTemplate.TemplateId, model.Id);

            var relatedTemplates = new[]
            {
                componenRazorTemplate,
            }
            .Where(t => t is not null)
            .Concat(LayoutTemplates.Where(t => t is not null));

            return new TemplateAITask(template, [.. relatedTemplates])
            {
                Type = "Implement Blazor Component",
                Title = $"Implement Blazor Component: {model.Name}",
                Context = @$"""
                            ## Tool Guidance
                            Do not use the run_software_factory tool in this conversation unless I explicitly ask you to.

                            ## User has modeled the following intentions:
                            {intention}

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

            foreach (var association in model.InternalElement.AssociatedElements.Where(e => e.IsShowDialogTargetEndModel()))
            {
                var dialogTargetEnd = association.AsShowDialogTargetEndModel();
                intention.AppendLine($"- {model.Name} opens a dialog to show the {dialogTargetEnd.TypeReference.Element.Name} component");
            }
        }

        // Add context about which components the current component is composed of (composition relationships)
        private static void AddCompositionContext(ComponentModel model, StringBuilder intention)
        {
            foreach (var association in model.InternalElement.AssociatedElements.Where(e => e.IsCompositionTargetEndModel()))
            {
                var compositionTargetEnd = association.AsCompositionTargetEndModel();
                intention.AppendLine($"- {model.Name} is composed of the {compositionTargetEnd.TypeReference.Element.Name} component.");
            }
        }

        // Add context about which service calls
        private static void AddCallServiceOperationContext(ComponentModel model, StringBuilder intention)
        {
            // Show Dialog associations
            foreach (var serviceCall in model.InternalElement.AssociatedElements.Where(o => o.IsCallServiceOperationActionEndModel()))
            {
                var serviceCallEnd = serviceCall.AsCallServiceOperationActionEndModel();
                intention.AppendLine($"- The {model.Name} page calls the {serviceCallEnd.TypeReference.Element.Name} service");
            }
        }

        private static (List<ITemplate> LayoutTemplates, string Instructions) AddLayoutComponentInstructions(ICSharpFileBuilderTemplate template, ComponentModel model, IChange change, StringBuilder intention)
        {
            List<ITemplate> menuTemplates = [];
            string templateInstructionExtension = "";

            foreach (var associationEnd in model.InternalElement.AssociatedElements.Where(a => a.IsNavigationSourceEndModel() && !a.IsNavigable))
            {
                intention.AppendLine($"- This pages is navigated to from a {associationEnd.TypeReference.Element.Name} menu item");

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