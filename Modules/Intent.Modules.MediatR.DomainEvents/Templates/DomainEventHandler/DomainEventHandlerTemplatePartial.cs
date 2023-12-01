using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.DomainEvents.Templates.DomainEvent;
using Intent.Modules.MediatR.DomainEvents.Templates.DomainEventNotification;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Templates.DomainEventHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainEventHandlerTemplate : CSharpTemplateBase<DomainEventHandlerModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MediatR.DomainEvents.DomainEventHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEventHandlerTemplate(IOutputTarget outputTarget, DomainEventHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("MediatR")
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });

                    foreach (var handledDomainEvents in Model.HandledDomainEvents())
                    {
                        @class.ImplementsInterface($"INotificationHandler<{GetDomainEventNotificationType()}<{GetDomainEventType(handledDomainEvents)}>>");

                        @class.AddMethod("Task", "Handle", method =>
                        {
                            method.RepresentsModel(handledDomainEvents);
                            method.RegisterAsProcessingHandlerForModel(handledDomainEvents);
                            method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                            method.Async();
                            method.AddParameter($"{GetDomainEventNotificationType()}<{GetDomainEventType(handledDomainEvents)}>", "notification");
                            method.AddParameter("CancellationToken", "cancellationToken");

                            var csharpMapping = new CSharpClassMappingManager(this);
                            csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(this));
                            csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(this));
                            csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(this));
                            csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(this));
                            var domainInteractionManager = new DomainInteractionsManager(this, csharpMapping);

                            csharpMapping.SetFromReplacement(handledDomainEvents, "notification.DomainEvent");
                            method.AddMetadata("mapping-manager", csharpMapping);

                            foreach (var createAction in handledDomainEvents.CreateEntityActions())
                            {
                                method.AddStatements(domainInteractionManager.CreateEntity(createAction));
                            }

                            foreach (var updateAction in handledDomainEvents.UpdateEntityActions())
                            {
                                var entity = updateAction.Element.AsClassModel() ?? updateAction.Element.AsOperationModel().ParentClass;

                                method.AddStatements(domainInteractionManager.QueryEntity(entity, updateAction.InternalAssociationEnd));

                                method.AddStatement(string.Empty);
                                method.AddStatements(domainInteractionManager.UpdateEntity(updateAction));
                            }

                            foreach (var deleteAction in handledDomainEvents.DeleteEntityActions())
                            {
                                var foundEntity = deleteAction.Element.AsClassModel();
                                method.AddStatements(domainInteractionManager.QueryEntity(foundEntity, deleteAction.InternalAssociationEnd));
                                method.AddStatements(domainInteractionManager.DeleteEntity(deleteAction));
                            }

                            foreach (var entity in domainInteractionManager.TrackedEntities.Values.Where(x => x.IsNew))
                            {
                                method.AddStatement(entity.DataAccessProvider.AddEntity(entity.VariableName));
                            }
                        });
                    }
                }).AfterBuild(file =>
                {
                    foreach (var handleMethod in file.Classes.First().Methods.Where(x => x.Name == "Handle"))
                    {
                        if (handleMethod?.Statements.Count == 0)
                        {
                            handleMethod.AddStatement("throw new NotImplementedException(\"Implement your handler logic here...\");");
                        }
                    }

                }, 1000);
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private string GetDomainEventNotificationType()
        {
            return GetTypeName(DomainEventNotificationTemplate.TemplateId);
        }

        private string GetDomainEventType(DomainEventHandlerAssociationTargetEndModel handledDomainEvent)
        {
            return GetTypeName(DomainEventTemplate.TemplateId, handledDomainEvent.TypeReference.Element);
        }
    }
}