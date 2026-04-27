using Intent.AI;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.MediatR.DomainEvents.Templates.DefaultDomainEventHandler;
using Intent.Modules.MediatR.DomainEvents.Templates.DomainEventHandler;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AITasksFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MediatR.DomainEvents.AITasksFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.AITaskManager.RegisterTaskProvider(new TemplateAITaskProvider(application, GetTasks));
        }

        public IAITask[] GetTasks(IChange[] changes, IOutputFile[] outputFiles, IApplication application)
        {
            var tasks = new List<IAITask>();

            tasks.AddRange(GetImplementDomainEventingAITasks(changes, application));

            return tasks.ToArray();
        }

        private IEnumerable<IAITask> GetImplementDomainEventingAITasks(IChange[] changes, IApplication application)
        {
            var entities = changes.Where(c =>
                                            IsDomainEventHandler(application, c) &&
                                            HasMissingImplementation(c) &&
                                            c.ChangeType != ChangeType.Delete
                                            );

            foreach (var change in entities)
            {
                if (!change.Template!.TryCastTemplate<ICSharpFileBuilderTemplate, IMetadataModel>(out var template, out var model))
                {
                    continue;
                }

                yield return CreateMissingImplementationDomainEventHandlerAITask(template, model);
            }
        }

        private IAITask CreateMissingImplementationDomainEventHandlerAITask(ICSharpFileBuilderTemplate template, IMetadataModel model)
        {
            var intention = new StringBuilder();

            if (model is DomainEventHandlerModel handler)
            {
                intention.AppendLine($"## Intentions for the handler");
                foreach (var subscription in handler.HandledDomainEvents())
                {
                    if (string.IsNullOrEmpty(subscription.Comment) && !subscription.InternalElement.AssociatedElements.Any())
                    {
                        continue;
                    }

                    intention.AppendLine($"### For event {subscription.InternalAssociation.TargetEnd.TypeReference.Element.Name}");

                    if (!string.IsNullOrEmpty(subscription.Comment))
                    {
                        intention.AppendLine($" - {subscription.Comment}.");
                    }
                    foreach (var associationEnd in subscription.InternalElement.AssociatedElements)
                    {
                        if (associationEnd.TypeReference.Element.SpecializationType == "Class")
                        {
                            intention.AppendLine($"- This handle method must `{associationEnd.SpecializationType.Replace("Action Target End", "")}` against the {associationEnd.TypeReference.Element.Name}.");
                        }
                        else
                        {
                            intention.AppendLine($"- This handle method must `{associationEnd.SpecializationType.Replace("Action Target End", "")}` against the {((IElement)associationEnd.TypeReference.Element)?.ParentElement?.Name}.{associationEnd.TypeReference.Element.Name}.");
                        }
                    }
                }
            }


            return new TemplateAITask(template)
            {
                Type = "Implement Domain Event Handler",
                Title = $"Implement Domain Event Handler: {template.ClassName}",
                Instructions =
                        $"""
                        Implement the missing functionality for the {((IHasName)model).Name} event in the {template.ClassName} class.
                        Find where the domain event is raised and update that method if needed so it performs the intended state change (not just DomainEvents.Add(...)).
                        """,
                Context =
                        $"""

                        ## User Intentions
                        {intention}
                        
                        ## Implementation Rules:
                        - Try to infer intent from template/model names. If unsure, favour asking. Specifically for event handlers: if you cannot infer at least one concrete side effect from existing code, call ask_user_question. Task.CompletedTask may be used as the default implementation **only once the user selects/approves the “no-op” option.
                        - If a domain method’s body is (effectively) only DomainEvents.Add(new <Event>(...)) with no state change / invariant enforcement, treat it as a generated stub. You MUST implement the intended domain behavior in that method (perform the state transition, add idempotency guards, validate invariants) and only then raise the domain event. Do NOT ask the user whether to do this.
                        - Never introduce dependencies on any NuGet packages, unless explicitly told to. 
                        - Follow the user's modeled intentions as best as possible.
                        - Search code usages to discover a way to implement the required functionality.
                        - Calling `SaveChangesAsync` is generally not needed as there should be an ambient unit of work.
                        """,
            };
        }

        private bool IsDomainEventHandler(IApplication application, IChange c)
        {
            return c.Template!.Id == DefaultDomainEventHandlerTemplate.TemplateId || c.Template!.Id == DomainEventHandlerTemplate.TemplateId;
        }

        private static bool HasMissingImplementation(IChange change) => change.Content.Contains("throw new NotImplementedException");

    }
}