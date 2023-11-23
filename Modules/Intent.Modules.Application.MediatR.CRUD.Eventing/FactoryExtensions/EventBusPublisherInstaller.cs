using System;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.Contracts.DomainMapping.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Eventing.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EventBusPublisherInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.CRUD.Eventing.EventBusPublisherInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var app = application.MetadataManager.Eventing(application).GetApplicationModels().SingleOrDefault();
            var messageLookup = application.MetadataManager.Eventing(application).GetMessageModels()
                .Where(model => HasMappedDomainEntityPresent(app, model, application))
                .ToLookup(k => k.GetMapFromDomainMapping().ElementId);
            var commands = application.FindTemplateInstances("Application.Command.Handler", p => p is ICSharpFileBuilderTemplate)
                .Cast<ICSharpFileBuilderTemplate>()
                .Select(template =>
                {
                    if (template == null)
                    {
                        return null;
                    }

                    var @class = template.CSharpFile.Classes.FirstOrDefault(x => x.HasMetadata("handler"));
                    if (@class == null || !@class.TryGetMetadata("model", out var model) || model == null)
                    {
                        return null;
                    }

                    var command = (CommandModel)model;
                    if (command.Mapping?.Element == null)
                    {
                        return null;
                    }

                    var classModel = command.Mapping.Element.SpecializationType switch
                    {
                        ClassModel.SpecializationType => command.Mapping.Element.AsClassModel(),
                        OperationModel.SpecializationType => command.Mapping.Element.AsOperationModel().InternalElement.ParentElement.AsClassModel(),
                        ClassConstructorModel.SpecializationType => command.Mapping.Element.AsClassConstructorModel().InternalElement.ParentElement.AsClassModel(),
                        _ => throw new Exception($"Unknown specialization: {command.Mapping.Element.SpecializationType}")
                    };

                    return new
                    {
                        Template = template,
                        Command = command,
                        Entity = classModel
                    };
                })
                .Where(p => p != null)
                .ToArray();
            var commandHandlers = commands.Where(p => messageLookup.Contains(p.Entity.Id))
                .Select(s => new
                {
                    Template = s.Template,
                    Command = s.Command,
                    Entity = s.Entity,
                    Message = messageLookup[s.Entity.Id].FirstOrDefault(p =>
                    {
                        var commandNameConvention = GetConventionName(s.Command.Name);
                        return commandNameConvention == GetConventionName(p.Name) && commandNameConvention is not null;
                    }),
                    Convention = GetConventionName(s.Command.Name)
                })
                .Where(p => p.Message is not null)
                .ToArray();
            foreach (var commandHandler in commandHandlers)
            {
                commandHandler.Template.CSharpFile.AfterBuild(file =>
                    PerformConventionCodeInjection(commandHandler.Template, commandHandler.Convention,
                        commandHandler.Message, commandHandler.Entity, application));
            }
        }

        private void PerformConventionCodeInjection(
            ICSharpFileBuilderTemplate template,
            string convention,
            MessageModel message,
            ClassModel entity,
            IApplication application)
        {
            var messageTemplate = template.GetTemplate<IClassProvider>(IntegrationEventMessageTemplate.TemplateId, message);
            var @class = template.CSharpFile.Classes.First(x => x.HasMetadata("handler"));

            switch (convention)
            {
                case "create":
                    {
                        InjectEventBusCode(@class, template);
                        AddMessageExtensionsUsings(application, template.CSharpFile, message);
                        AddPublishStatement(
                            @class: @class,
                            publishStatement: $"_eventBus.Publish({entity.GetNewVariableName()}.MapTo{messageTemplate.ClassName}());");
                    }
                    break;
                case "update":
                    {
                        InjectEventBusCode(@class, template);
                        AddMessageExtensionsUsings(application, template.CSharpFile, message);
                        AddPublishStatement(
                            @class: @class,
                            publishStatement: $"_eventBus.Publish({entity.GetExistingVariableName()}.MapTo{messageTemplate.ClassName}());");
                    }
                    break;
                case "delete":
                    {
                        InjectEventBusCode(@class, template);
                        AddMessageExtensionsUsings(application, template.CSharpFile, message);
                        AddPublishStatement(
                            @class: @class,
                            publishStatement: $"_eventBus.Publish({entity.GetExistingVariableName()}.MapTo{messageTemplate.ClassName}());");
                    }
                    break;
            }
        }

        private static void AddPublishStatement(CSharpClass @class, string publishStatement)
        {
            var method = @class.FindMethod("Handle");
            var returnClause = method.Statements.FirstOrDefault(p => p.GetText("").Trim().StartsWith("return"));

            if (returnClause != null)
            {
                returnClause.InsertAbove(publishStatement);
            }
            else
            {
                method.Statements.Add(publishStatement);
            }
        }

        private static void AddMessageExtensionsUsings(IApplication application, CSharpFile cSharpFile, MessageModel message)
        {
            var mapToMethodTemplate = application.FindTemplateInstance<IClassProvider>("Application.Eventing.MessageExtensions", message);
            cSharpFile.AddUsing(mapToMethodTemplate.Namespace);
        }

        private void InjectEventBusCode(CSharpClass @class, ICSharpFileBuilderTemplate template)
        {
            var constructor = @class.Constructors.First();
            constructor.AddParameter(template.GetTypeName(EventBusInterfaceTemplate.TemplateId), "eventBus", ctor => ctor.IntroduceReadonlyField());
        }

        private string GetConventionName(string name)
        {
            return name.ToLower() switch
            {
                var x when x.Contains("create", StringComparison.OrdinalIgnoreCase) || x.StartsWith("new", StringComparison.OrdinalIgnoreCase) || x.Contains("add", StringComparison.OrdinalIgnoreCase) => "create",
                var x when x.Contains("update", StringComparison.OrdinalIgnoreCase) || x.Contains("edit", StringComparison.OrdinalIgnoreCase) => "update",
                var x when x.Contains("delete", StringComparison.OrdinalIgnoreCase) || x.Contains("remove", StringComparison.OrdinalIgnoreCase) => "delete",
                _ => null
            };
        }

        private bool HasMappedDomainEntityPresent(ApplicationModel applicationModel, MessageModel messageModel, IApplication application)
        {
            if (applicationModel.PublishedMessages().All(p => p.Element.AsMessageModel().Id != messageModel.Id))
            {
                return false;
            }

            var domainMapping = messageModel.GetMapFromDomainMapping();
            if (domainMapping == null)
            {
                return false;
            }

            var domainClasses = application.MetadataManager.Domain(application).GetClassModels();
            return domainClasses.Any(p => p.Id == domainMapping.ElementId);
        }
    }
}