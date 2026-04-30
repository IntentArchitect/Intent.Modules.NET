using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Intent.AI;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Entities.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AITaskFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Entities.AITaskFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.AITaskManager.RegisterTaskProvider(new TemplateAITaskProvider(application, GetTasks));
        }

        public IAITask[] GetTasks(IChange[] changes, IOutputFile[] outputFiles, IApplication application)
        {
            var tasks = new List<IAITask>();

            tasks.AddRange(GetMissingOperationImplementationAITasks(changes, application));

            return tasks.ToArray();
        }

        private IEnumerable<IAITask> GetMissingOperationImplementationAITasks(IChange[] changes, IApplication application)
        {
            var entities = changes.Where(c =>
                                            ContainsDomainBehaviour(application, c) &&
                                            HasMissingImplementation(c) &&
                                            c.ChangeType != ChangeType.Delete
                                            );

            foreach (var change in entities)
            {
                if (!change.Template!.TryCastTemplate<ICSharpFileBuilderTemplate, ClassModel>(out var template, out var model))
                {
                    continue;
                }

                yield return CreateMissingImplementationDomainEntityAITask(template, model);
            }

        }

        private IAITask CreateMissingImplementationDomainEntityAITask(ICSharpFileBuilderTemplate template, ClassModel model)
        {
            var intention = new StringBuilder();
            intention.AppendLine($"## Intentions for the entity");
            foreach (var behaviour in model.Operations)
            {
                intention.AppendLine($"### For behaviour {behaviour.Name}");

                foreach (var associationEnd in behaviour.InternalElement.AssociatedElements)
                {
                    intention.AppendLine($" - must `{associationEnd.SpecializationType}` against the {associationEnd.TypeReference.Element.Name}.");
                }
            }


            return new TemplateAITask(template)
            {
                Type = "Implement Domain Entity",
                Title = $"Implement Domain Entity: {template.ClassName}",
                Instructions =
                        $"""
                        Implement the missing functionality for the {model.Name} entity in the {template.ClassName} class.
                        """,
                Context =
                        $"""
                        ## User has modeled the following intentions:
                        {intention}

                        ## Implementation Rules:
                        - Never introduce dependencies on any NuGet packages, unless explicitly told to. 
                        - Follow the user's modeled intentions as best as possible.
                        - Search code usages to discover a way to implement the required functionality.
                        """,
            };
        }

        private bool ContainsDomainBehaviour(IApplication application, IChange c)
        {
            if (application.GetSettings().GetDomainSettings().SeparateStateFromBehaviour())
            {
                return c.Template?.Id == DomainEntityStateTemplate.TemplateId;
            }
            return c.Template?.Id == DomainEntityTemplate.TemplateId;
        }

        private static bool HasMissingImplementation(IChange change) => change.Content.Contains("throw new NotImplementedException");

    }
}