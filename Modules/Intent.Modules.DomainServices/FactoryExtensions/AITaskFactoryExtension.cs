using Intent.AI;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.DomainServices.Templates.DomainServiceImplementation;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.DomainServices.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AITaskFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.DomainServices.AITaskFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.AITaskManager.RegisterTaskProvider(new TemplateAITaskProvider(application, GetTasks));
        }

        public IAITask[] GetTasks(IChange[] changes, IOutputFile[] outputFiles, IApplication application)
        {
            var tasks = new List<IAITask>();

            tasks.AddRange(GetImplementDomainServiceAITasks(changes, application));

            return tasks.ToArray();
        }

        private IEnumerable<IAITask> GetImplementDomainServiceAITasks(IChange[] changes, IApplication application)
        {
            var entities = changes.Where(c =>
                                    c.Template!.Id == DomainServiceImplementationTemplate.TemplateId &&
                                    HasMissingImplementation(c) &&
                                    c.ChangeType != ChangeType.Delete
                                    );

            foreach (var change in entities)
            {
                if (!change.Template!.TryCastTemplate<ICSharpFileBuilderTemplate, DomainServiceModel>(out var template, out var model))
                {
                    continue;
                }

                yield return CreateMissingImplementationDomainServiceAITask(template, model);
            }
        }

        private IAITask CreateMissingImplementationDomainServiceAITask(ICSharpFileBuilderTemplate template, DomainServiceModel model)
        {
            return new TemplateAITask(template)
            {
                Type = "Implement Domain Service",
                Title = $"Implement Domain Service: {template.ClassName}",
                Instructions =
                        $"""
                        Implement the missing functionality for the {model.Name} event in the {template.ClassName} class.
                        """,
                Context =
                        $"""
                        
                        ## Implementation Rules:
                        - Try to infer intent from template/model names. If unsure, favour asking. Specifically for event handlers: if you cannot infer at least one concrete side effect from existing code, call ask_user_question. Task.CompletedTask may be used as the default implementation **only once the user selects/approves the “no-op” option.
                        - Never introduce dependencies on any NuGet packages, unless explicitly told to. 
                        - Follow the user's modeled intentions as best as possible.
                        - Search code usages to discover a way to implement the required functionality.
                        - Calling `SaveChangesAsync` is generally not needed as there should be an ambient unit of work.
                        """,
            };
        }

        private static bool HasMissingImplementation(IChange change) => change.Content.Contains("throw new NotImplementedException");
    }
}