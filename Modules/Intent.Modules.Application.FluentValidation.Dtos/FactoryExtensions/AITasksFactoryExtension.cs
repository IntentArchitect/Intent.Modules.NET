using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.AI;
using Intent.Application.FluentValidation.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.FluentValidation.Dtos.Templates.DTOValidator;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Dtos.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AITasksFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.FluentValidation.Dtos.AITasksFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.AITaskManager.RegisterTaskProvider(new TemplateAITaskProvider(application, GetTasks));
        }

        public IAITask[] GetTasks(IChange[] changes, IOutputFile[] outputFiles, IApplication application)
        {
            var tasks = new List<IAITask>();

            tasks.AddRange(GetImplementCustomValidationAITasks(changes, application));

            return tasks.ToArray();
        }

        private static IEnumerable<IAITask> GetImplementCustomValidationAITasks(IChange[] changes, IApplication application)
        {
            var entities = changes.Where(c =>
                IsValidator(application, c) &&
                HasMissingImplementation(c) &&
                c.ChangeType != ChangeType.Delete
            );

            foreach (var change in entities)
            {
                if (change.Template?.TryCastTemplate<ICSharpFileBuilderTemplate, IMetadataModel>(out var template, out var model) != true)
                {
                    continue;
                }

                yield return CreateMissingValidatorImplementationAITask(template, model);
            }
        }

        private static IAITask CreateMissingValidatorImplementationAITask(ICSharpFileBuilderTemplate template, IMetadataModel model)
        {
            return new TemplateAITask(template)
            {
                Type = "Implement Custom Validator",
                Title = $"Implement Custom Validator: {template.ClassName}",
                Instructions =
                        $"""
                        Implement the missing functionality for any ValidateAsync methods in the {template.ClassName} class which are not yet implemented.
                        """,
            };
        }

        private static bool IsValidator(IApplication application, IChange c)
        {
            return c.Template?.Id == DTOValidatorTemplate.TemplateId;
        }

        private static bool HasMissingImplementation(IChange change) => change.Content.Contains("throw new NotImplementedException");
    }
}
