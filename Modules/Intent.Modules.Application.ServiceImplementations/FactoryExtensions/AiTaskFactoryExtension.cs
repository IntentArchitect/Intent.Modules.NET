using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using Intent.AI;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AiTaskFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.ServiceImplementations.AiTaskFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.AITaskManager.RegisterTaskProvider(new TemplateAITaskProvider(application, GetTasks));
        }

        public IAITask[] GetTasks(IChange[] changes, IOutputFile[] outputFiles, IApplication application)
        {
            var tasks = new List<IAITask>();

            tasks.AddRange(GetMissingOperationImplementationAITasks(changes, application));
            tasks.AddRange(GetContractChangedButServiceHasntAITasks(changes, application));

            return tasks.ToArray();
        }

        private IEnumerable<IAITask> GetContractChangedButServiceHasntAITasks(IChange[] changes, IApplication application)
        {
            var handlerChanges = changes.Where(c =>
                                            c.Template?.Id == ServiceImplementationTemplate.TemplateId &&
                                            HasMissingImplementation(c) &&
                                            c.ChangeType != ChangeType.Delete
                                            );

            foreach (var change in handlerChanges)
            {
                if (!change.Template!.TryCastTemplate<ICSharpFileBuilderTemplate, ServiceModel>(out var template, out var model))
                {
                    continue;
                }

                yield return CreateMissingImplementationServiceImplementationAITask(template, model);
            }
        }

        private static bool HasMissingImplementation(IChange change) => change.Content.Contains("throw new NotImplementedException");


        private IEnumerable<IAITask> GetMissingOperationImplementationAITasks(IChange[] changes, IApplication application)
        {
            var handlerChanges = changes.Where(c =>
                                            c.Template?.Id == ServiceImplementationTemplate.TemplateId &&
                                            HasMissingImplementation(c) &&
                                            c.ChangeType != ChangeType.Delete
                                            );

            foreach (var change in handlerChanges)
            {
                if (!change.Template!.TryCastTemplate<ICSharpFileBuilderTemplate, ServiceModel>(out var template, out var model))
                {
                    continue;
                }

                yield return CreateMissingImplementationServiceImplementationAITask(template, model);
            }
        }

        private IAITask CreateMissingImplementationServiceImplementationAITask(ICSharpFileBuilderTemplate template, ServiceModel model)
        {
            var intention = new StringBuilder();
            intention.AppendLine($"## Intentions for the service");
            foreach (var operation in model.Operations)
            {
                intention.AppendLine($"### For Operation {operation.Name}");

                foreach (var associationEnd in operation.InternalElement.AssociatedElements)
                {
                    intention.AppendLine($" - must `{associationEnd.SpecializationType}` against the {associationEnd.TypeReference.Element.Name}.");
                }
            }

            return new TemplateAITask(template)
            {
                Type = "Implement Service",
                Title = $"Implement Service: {template.ClassName}",
                Instructions =
                        $"""
                        Implement the missing functionality for the {model.Name} service in the {template.ClassName} class.
                        """,
                Context =
                        $"""
                        ## User has modeled the following intentions:
                        {intention}

                        ## Implementation Rules:
                        - Only ever inject in dependencies from the Domain or Application layers.
                        - Never introduce dependencies on infrastructural NuGet packages (e.g. Entity Framework, Dapper, etc.) directly in the handler. If data access is required, use the appropriate repository in the Domain layer and inject that into the handler.
                        - Follow the user's modeled intentions as best as possible.
                        - Search code usages to discover a way to implement the required functionality.
                        - Calling `SaveChangesAsync` is only required if this command returns a payload that includes a surrogate key (e.g. `Id`).
                        """,
            };

        }
    }
}