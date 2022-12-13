using System.Linq;
using Intent.Engine;
using Intent.Eventing.Contracts.DomainMapping.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
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
                .Select(x =>
                {
                    var @class = x.CSharpFile.Classes.First();
                    if (!@class.TryGetMetadata("model", out var model))
                    {
                        return null;
                    }

                    var command = (CommandModel)model;
                    return new
                    {
                        Template = x,
                        Command = command,
                        Entity = command.Mapping.Element.AsClassModel()
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
                    Message = messageLookup[s.Entity.Id].SingleOrDefault(p => GetConventionName(s.Command.Name) == GetConventionName(p.Name)),
                    Convention = GetConventionName(s.Command.Name)
                })
                .Where(p => p.Message != null)
                .ToArray();
            foreach (var commandHandler in commandHandlers)
            {
                var messageTemplate = commandHandler.Template.GetTemplate<IClassProvider>(IntegrationEventMessageTemplate.TemplateId, commandHandler.Message);
                var @class = commandHandler.Template.CSharpFile.Classes.First();
                if (commandHandler.Convention == "create")
                {
                    @class.FindMethod("Handle")
                        .Statements
                        .FirstOrDefault(p => p.GetText("").Contains("return"))
                        ?.InsertAbove($"_eventBus.Publish(new{commandHandler.Entity.Name}.MapTo{messageTemplate.ClassName}());");
                } 
                else if (commandHandler.Convention == "update")
                {
                    @class.FindMethod("Handle")
                        .Statements
                        .FirstOrDefault(p => p.GetText("").Contains("return"))
                        ?.InsertAbove($"_eventBus.Publish(existing{commandHandler.Entity.Name}.MapTo{messageTemplate.ClassName}());");
                }
                else if (commandHandler.Convention == "delete")
                {
                    @class.FindMethod("Handle")
                        .Statements
                        .FirstOrDefault(p => p.GetText("").Contains("return"))
                        ?.InsertAbove($"_eventBus.Publish(existing{commandHandler.Entity.Name}.MapTo{messageTemplate.ClassName}());");
                }
                
            }
        }

        private string GetConventionName(string name)
        {
            return name.ToLower() switch
            {
                var x when x.Contains("create") || x.Contains("new") || x.Contains("add") => "create",
                var x when x.Contains("update") || x.Contains("edit") => "update",
                var x when x.Contains("delete") || x.Contains("remove") => "delete",
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