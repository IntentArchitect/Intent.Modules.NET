using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.AI;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Wolverine.FactoryExtentions;
using Intent.Modules.Application.Wolverine.Templates.CommandHandler;
using Intent.Modules.Application.Wolverine.Templates.CommandModels;
using Intent.Modules.Application.Wolverine.Templates.QueryHandler;
using Intent.Modules.Application.Wolverine.Templates.QueryModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AITasksFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Wolverine.AITasksFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.AITaskManager.RegisterTaskProvider(new TemplateAITaskProvider(application, GetTasks));
        }

        public IAITask[] GetTasks(IChange[] changes, IOutputFile[] outputFiles, IApplication application)
        {
            var tasks = new List<IAITask>();

            tasks.AddRange(GetMissingHandlerImplementationAITasks(changes, application));

            return tasks.ToArray();
        }

        private IEnumerable<IAITask> GetMissingHandlerImplementationAITasks(IChange[] changes, IApplication application)
        {
            return GetMissingCommandHandlerImplementationTasks(changes, application)
                .Concat(GetMissingQueryHandlerImplementationTasks(changes, application));
        }

        private IEnumerable<IAITask> GetMissingQueryHandlerImplementationTasks(IChange[] changes, IApplication application)
        {
            var handlerChanges = changes.Where(c =>
                                            IsQueryHandlerTemplate(application, c) &&
                                            HasMissingImplementation(c) &&
                                            c.ChangeType != ChangeType.Delete
                                            );

            foreach (var change in handlerChanges)
            {
                if (!change.Template!.TryCastTemplate<ICSharpFileBuilderTemplate, QueryModel>(out var template, out var model))
                {
                    continue;
                }

                yield return CreateImplementQueryHandlerAITask(template, model);
            }
        }

        private IEnumerable<IAITask> GetMissingCommandHandlerImplementationTasks(IChange[] changes, IApplication application)
        {
            var handlerChanges = changes.Where(c =>
                                            IsCommandHandlerTemplate(application, c) &&
                                            HasMissingImplementation(c) &&
                                            c.ChangeType != ChangeType.Delete
                                            );

            foreach (var change in handlerChanges)
            {
                if (!change.Template!.TryCastTemplate<ICSharpFileBuilderTemplate, CommandModel>(out var template, out var model))
                {
                    continue;
                }

                yield return CreateImplementCommandHandlerAITask(template, model);
            }
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

                ## Tool Guidance
                Do not use the run_software_factory tool in this conversation unless I explicitly ask you to.
                
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

                ## Tool Guidance
                Do not use the run_software_factory tool in this conversation unless I explicitly ask you to.
                
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

        private static bool HasMissingImplementation(IChange change) => change.Content.Contains("throw new NotImplementedException");

        private static bool ChangeSetIncludesTemplate(IEnumerable<IChange> changes, ITemplate template) => changes.Any(x => x.Template?.Equals(template) == true);

        private bool IsCommandHandlerTemplate(IApplication application, IChange change)
        {
            var expectedTemplateId = CommandHandlerTemplate.TemplateId;

            return change.Template?.Id == expectedTemplateId;
        }

        private bool IsQueryHandlerTemplate(IApplication application, IChange change)
        {
            var expectedTemplateId = QueryHandlerTemplate.TemplateId;

            return change.Template?.Id == expectedTemplateId;
        }
    }
}