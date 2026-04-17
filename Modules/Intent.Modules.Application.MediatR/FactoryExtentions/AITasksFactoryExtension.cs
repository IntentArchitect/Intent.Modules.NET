using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using Intent.AI;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AITasksFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.AITasksFactoryExtension";

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

        internal class TemplateAITaskProvider(IApplication application, Func<IChange[], IOutputFile[], IApplication, IAITask[]> createTask) : IAITaskProvider
        {

            public IAITask[] GetTasks(IChange[] changes, IOutputFile[] outputFiles)
            {
                return createTask(changes, outputFiles, application);
            }
        }

        public IAITask[] GetTasks(IChange[] changes, IOutputFile[] outputFiles, IApplication application)
        {
            var tasks = new List<IAITask>();

            tasks.AddRange(GetMissingHandlerImplementationAITasks(changes, application));

            if (!application.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile())
            {
                tasks.AddRange(GetOnlyContractChangedAITasks(changes, application));
            }

            return tasks.ToArray();
        }

        private IEnumerable<IAITask> GetMissingQueryHandlerImplementationTasks(IChange[] changes, IApplication application)
        {
            var handlerChanges = changes.Where(c =>
                IsQueryHandlerTemplate(application, c) &&
                HasMissingImplementation(c));

            foreach (var change in handlerChanges)
            {
                if (!TryGetTemplate<ICSharpFileBuilderTemplate, QueryModel>(change.Template, out var template, out var model))
                {
                    continue;
                }

                yield return CreateImplementQueryHandlerAITask(template, model);
            }
        }

        private IEnumerable<IAITask> GetOnlyContractChangedCommandTasks(IChange[] changes, IApplication application)
        {
            var commandChanges = changes.Where(c => c.Template?.Id == CommandModelsTemplate.TemplateId);

            foreach (var change in commandChanges)
            {
                if (!TryGetTemplate<ICSharpFileBuilderTemplate, CommandModel>(change.Template, out var template, out var model))
                {
                    continue;
                }

                var handlerTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                    CommandHandlerTemplate.TemplateId,
                    model);

                if (handlerTemplate == null || ChangeSetIncludesTemplate(changes, handlerTemplate))
                {
                    continue;
                }

                yield return CreateCommandChangedUpdateCommandHandlerAITask(template, handlerTemplate, model);
            }
        }

        private IEnumerable<IAITask> GetOnlyContractChangedQueryTasks(IChange[] changes, IApplication application)
        {
            var queryChanges = changes.Where(c => c.Template?.Id == QueryModelsTemplate.TemplateId);

            foreach (var change in queryChanges)
            {
                if (!TryGetTemplate<ICSharpFileBuilderTemplate, QueryModel>(change.Template, out var template, out var model))
                {
                    continue;
                }

                var handlerTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                    QueryHandlerTemplate.TemplateId,
                    model);

                if (handlerTemplate == null || ChangeSetIncludesTemplate(changes, handlerTemplate))
                {
                    continue;
                }

                yield return CreateQueryChangedUpdateQueryHandlerAITask(template, handlerTemplate, model);
            }
        }

        private IEnumerable<IAITask> GetMissingCommandHandlerImplementationTasks(IChange[] changes, IApplication application)
        {
            var handlerChanges = changes.Where(c =>
                IsCommandHandlerTemplate(application, c) &&
                HasMissingImplementation(c));

            foreach (var change in handlerChanges)
            {
                if (!TryGetTemplate<ICSharpFileBuilderTemplate, CommandModel>(change.Template, out var template, out var model))
                {
                    continue;
                }
                
                yield return CreateImplementCommandHandlerAITask(template, model);
            }
        }


        private IAITask CreateQueryChangedUpdateQueryHandlerAITask(ICSharpFileBuilderTemplate queryTemplate, ICSharpFileBuilderTemplate handlerTemplate, QueryModel model)
        {
            var intention = new StringBuilder();
            foreach (var associationEnd in model.InternalElement.AssociatedElements)
            {
                intention.AppendLine($"- This query must `{associationEnd.SpecializationType}` against the {associationEnd.TypeReference.Element.Name}.");
            }

            return new TemplateAITask(queryTemplate, [handlerTemplate.GetMetadata().GetFilePath()])
            {
                Type = "Update Query Handler",
                Title = $"Update Handler: {handlerTemplate.ClassName}",
                Instructions =
                    $"""
                    Update the {handlerTemplate.ClassName} handler based on the changes to {queryTemplate.ClassName}.
                    """,
                Context = GetQueryHandlerContext(handlerTemplate, model)
            };
        }

        private IAITask CreateCommandChangedUpdateCommandHandlerAITask(ICSharpFileBuilderTemplate commandTemplate, ICSharpFileBuilderTemplate commandHandlertemplate, CommandModel model)
        {
            var intention = new StringBuilder();
            foreach (var associationEnd in model.InternalElement.AssociatedElements)
            {
                intention.AppendLine($"- This command must `{associationEnd.SpecializationType}` against the {associationEnd.TypeReference.Element.Name}.");
            }

            return new TemplateAITask(commandTemplate, [commandHandlertemplate.GetMetadata().GetFilePath()])
            {
                Type = "Update Command Handler",
                Title = $"Update Handler: {commandHandlertemplate.ClassName}",
                Instructions =
                    $"""
                    Update the {commandHandlertemplate.ClassName} handler based on the changes to {commandTemplate.ClassName}.
                    """,
                Context = GetCommandHandlerContext(commandHandlertemplate, model)
            };
        }

        private IAITask CreateImplementQueryHandlerAITask(ICSharpFileBuilderTemplate template, QueryModel model)
        {
            return new TemplateAITask(template)
            {
                Type = "Implement Query Handler",
                Title = $"Implement Handler: {template.ClassName}",
                Instructions =
                    $"""
                    Implement the functionality for handling the {model.Name} query in the {template.ClassName} class.
                    """,
                Context = GetQueryHandlerContext(template, model)
            };
        }


        private IAITask CreateImplementCommandHandlerAITask(ICSharpFileBuilderTemplate template, CommandModel model)
        {
            return new TemplateAITask(template)
            {
                Type = "Implement Command Handler",
                Title = $"Implement Handler: {template.ClassName}",
                Instructions =
                        $"""
                        Implement the functionality for handling the {model.Name} command in the {template.ClassName} class.
                        """,
                Context = GetCommandHandlerContext(template, model)
            };
        }

        private string GetQueryHandlerContext(ICSharpFileBuilderTemplate template, QueryModel model)
        {
            var intention = new StringBuilder();
            foreach (var associationEnd in model.InternalElement.AssociatedElements)
            {
                intention.AppendLine($"- This query must `{associationEnd.SpecializationType}` against the {associationEnd.TypeReference.Element.Name}.");
            }

            return
                $"""
                ## User has modeled the following intentions:
                {intention}

                ## Implementation Rules:
                - Only ever inject in dependencies from the Domain or Application layers.
                - Never introduce dependencies on infrastructural NuGet packages (e.g. Entity Framework, Dapper, etc.) directly in the handler. If data access is required, use the appropriate repository in the Domain layer and inject that into the handler.
                - Follow the user's modeled intentions as best as possible.
                - Search code usages to discover a way to implement the required functionality.
                """;
        }


        private string GetCommandHandlerContext(ICSharpFileBuilderTemplate template, CommandModel model)
        {
            var intention = new StringBuilder();
            foreach (var associationEnd in model.InternalElement.AssociatedElements)
            {
                intention.AppendLine($"- This command must `{associationEnd.SpecializationType}` against the {associationEnd.TypeReference.Element.Name}.");
            }

            return
                $"""
                ## User has modeled the following intentions:
                {intention}

                ## Implementation Rules:
                - Only ever inject in dependencies from the Domain or Application layers.
                - Never introduce dependencies on infrastructural NuGet packages (e.g. Entity Framework, Dapper, etc.) directly in the handler. If data access is required, use the appropriate repository in the Domain layer and inject that into the handler.
                - Follow the user's modeled intentions as best as possible.
                - Search code usages to discover a way to implement the required functionality.
                - Calling `SaveChangesAsync` is only required if this command returns a payload that includes a surrogate key (e.g. `Id`).
                """;
        }

        private static bool TryGetTemplate<TTemplate, TModel>(
            ITemplate? candidateTemplate,
            [NotNullWhen(true)] out TTemplate? template,
            [NotNullWhen(true)] out TModel? model)
            where TTemplate : class, ITemplate
            where TModel : class
        {
            model = null;
            template = candidateTemplate as TTemplate;
            return template is not null && template.TryGetModel<TModel>(out model);
        }

        private IEnumerable<IAITask> GetMissingHandlerImplementationAITasks(IChange[] changes, IApplication application)
        {
            return GetMissingCommandHandlerImplementationTasks(changes, application)
                .Concat(GetMissingQueryHandlerImplementationTasks(changes, application));
        }

        /// <summary>
        /// The Query or Command has changed but there are no changes to the Handler. (AI should get involved)
        /// </summary>
        /// <param name="changes"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        private IEnumerable<IAITask> GetOnlyContractChangedAITasks(IChange[] changes, IApplication application)
        {
            return GetOnlyContractChangedCommandTasks(changes, application)
                .Concat(GetOnlyContractChangedQueryTasks(changes, application));
        }

        private static bool HasMissingImplementation(IChange change) =>  change.Content.Contains("throw new NotImplementedException");

        private static bool ChangeSetIncludesTemplate(IEnumerable<IChange> changes, ITemplate template) => changes.Any(x => x.Template?.Equals(template) == true);

        private bool IsCommandHandlerTemplate(IApplication application, IChange change)
        {
            var expectedTemplateId = application.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
                ? CommandModelsTemplate.TemplateId
                : CommandHandlerTemplate.TemplateId;

            return change.Template?.Id == expectedTemplateId;
        }

        private bool IsQueryHandlerTemplate(IApplication application, IChange change)
        {
            var expectedTemplateId = application.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
                ? QueryModelsTemplate.TemplateId
                : QueryHandlerTemplate.TemplateId;

            return change.Template?.Id == expectedTemplateId;
        }
    }
}