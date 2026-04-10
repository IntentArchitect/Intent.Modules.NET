using Intent.AI;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.DependencyInjection.MediatR;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.CommandHandler
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class CommandHandlerTemplate : CSharpTemplateBase<CommandModel, CommandHandlerDecorator>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.Application.MediatR.CommandHandler";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommandHandlerTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            CSharpFile = new CSharpFile($"{this.GetCommandNamespace()}", $"{this.GetCommandFolderPath()}");
            Configure(this, model);
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile();
        }

        internal static void Configure(ICSharpFileBuilderTemplate template, CommandModel model)
        {
            template.AddNugetDependency(NugetPackages.MediatR(template.OutputTarget));
            template.AddTypeSource(TemplateRoles.Application.Command);
            template.AddTypeSource(TemplateRoles.Domain.Enum);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);
            template.CSharpFile
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MediatR")
                .AddClass($"{model.Name}Handler", @class =>
                {
                    @class.AddMetadata("handler", true);
                    @class.AddMetadata("model", model);
                    @class.ImplementsInterface(GetRequestHandlerInterface(template, model));
                    @class.AddAttribute("IntentManaged(Mode.Merge, Signature = Mode.Fully)");
                    @class.AddConstructor(ctor => { ctor.AddAttribute(CSharpIntentManagedAttribute.Merge()); });
                    @class.AddMethod(GetReturnType(template, model), "Handle", method =>
                    {
                        method.RegisterAsProcessingHandlerForModel(model);
                        method.TryAddXmlDocComments(model.InternalElement);
                        method.Async();
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                        method.AddParameter(GetCommandModelName(template, model), "request", p => p.RepresentsModel(model));
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddStatement("// IntentInitialGen");
                        method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                        method.AddStatement("""throw new NotImplementedException("Your implementation here...");""");
                    });
                });

            //var isNotImplemented = file.Classes.First().Methods.Any(x => x.FindStatement(x => x.ToString().Contains("NotImplementedException")) != null);
            template.ExecutionContext.AITaskManager.RegisterTaskProvider(new TemplateAITaskProvider((changes, outputFiles) =>
            {
                var outputFile = outputFiles.FirstOrDefault(x => x.Template?.Equals(template) == true);
                if (changes.All(x => x.Template?.Equals(template) != true)
                    && outputFile != null && !outputFile.Content.Contains("throw new NotImplementedException"))
                {
                    return null;
                }

                var intention = new StringBuilder();
                foreach (var associationEnd in model.InternalElement.AssociatedElements)
                {
                    intention.AppendLine($"- This command must `{associationEnd.SpecializationType}` against the {associationEnd.TypeReference.Element.Name}.");
                }

                return new TemplateAITask(template)
                {
                    Type = "Implement Command Handler",
                    Title = $"Implement Handler: {template.ClassName}",
                    Instructions =
                        $"""
                                 Implement the functionality for handling the {model.Name} command in the {template.ClassName} class.
                                 """,
                    Context =
                        $"""
                                 ## User has modeled the following intentions:
                                 {intention}

                                 ## Implementation Rules:
                                 - ALWAYS follow the architectural guidelines as and when they become apparent.
                                 - NEVER modify the method signature of the Handle method.
                                 - ALWAYS ensure that the `IntentManaged` attribute indicates that the body of the method must be in `Mode.Ignore` (e.g. `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`).
                                 - Only ever inject in dependencies from the Domain or Application layers.
                                 - Never introduce dependencies on infrastructural nuget packages (e.g. Entity Framework, Dapper, etc.) directly in the handler. If data access is required, use the appropriate repository in the Domain layer and inject that into the handler.
                                 - Follow the user's modeled intentions as best as possible.

                                 ## Architectural Guidelines:
                                 - Follow the Single Responsibility Principle. The handler should only be responsible for handling the command and delegating work to other services or components as necessary.
                                 - Use Dependency Injection to inject any required services or repositories into the handler's constructor.
                                 - Ensure that the handler is focused on orchestrating the retrieval of data and does not contain complex data manipulation. Place complex data manipulation logic in the infrastructure layer (e.g. in a repository) if possible.
                                 """,
                };
            }));
        }


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Handler",
                @namespace: $"{this.GetNamespace(additionalFolders: Model.GetConceptName())}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.GetConceptName())}");
        }

        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private static string GetRequestHandlerInterface(ICSharpTemplate template, CommandModel model)
        {
            return model.TypeReference.Element != null
                ? $"IRequestHandler<{GetCommandModelName(template, model)}, {template.GetTypeName(model.TypeReference)}>"
                : $"IRequestHandler<{GetCommandModelName(template, model)}>";
        }

        private static string GetCommandModelName(ICSharpTemplate template, CommandModel model)
        {
            return template.GetTypeName(CommandModelsTemplate.TemplateId, model);
        }

        private static string GetReturnType(ICSharpTemplate template, CommandModel model)
        {
            return model.TypeReference.Element != null
                ? $"Task<{template.GetTypeName(model.TypeReference)}>"
                : "Task";
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "2.0"), new Mediator12Migration());
        }

        private class Mediator12Migration : ITemplateMigration
        {
            public string Execute(string currentText)
            {
                return currentText.Replace(@"return Unit.Value;\r\n", "")
                    .Replace(@"return Unit.Value;\n", "")
                    .Replace(@"return Unit.Value;", "");
            }

            public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.Upgrade(1, 2);
        }

    }

    public class TemplateAITask : IAITask
    {
        private readonly IIntentTemplate _template;
        public TemplateAITask(IIntentTemplate template)
        {
            Id = ((IntentTemplateBase)template).GetCorrelationId() ?? throw new ArgumentException("CorrelationId could not be found for template", nameof(template));
            _template = template;

            RelatedTemplates = _template.GetAllTemplateDependencies()
                .Select(x => _template.ExecutionContext.FindTemplateInstance(x))
                .Distinct()
                .ToList();
        }

        public string Id { get; }

        public ITemplate Template => _template;

        public string Type { get; init; }

        public string Title { get; init; }

        public string Instructions { get; init; }

        public string Context { get; init; }

        public IList<ITemplate> RelatedTemplates { get; }

        public IList<string> FilesToInclude { get; }

        public virtual bool IsApplicableToChanges(IChange[] changes)
        {
            if (changes.Any(change => change.Template == _template)
                || changes.Any(change => RelatedTemplates.Contains(change.Template)))
            {
                return true;
            }

            return false;
        }
    }
}