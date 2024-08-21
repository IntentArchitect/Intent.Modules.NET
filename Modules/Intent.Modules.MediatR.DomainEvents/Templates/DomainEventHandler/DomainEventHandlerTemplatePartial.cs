using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modelers.Services.Api;
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
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyFully());
                            method.Async();
                            method.AddParameter($"{GetDomainEventNotificationType()}<{GetDomainEventType(handledDomainEvents)}>", "notification");
                            method.AddParameter("CancellationToken", "cancellationToken");
                        });
                    }
                })
                .AfterBuild(file =>
                {
                    // TODO: MOVE TO FACTORY EXTENSION:
                    foreach (var handledDomainEvents in Model.HandledDomainEvents())
                    {
                        var @class = file.Classes.First();
                        var method = (CSharpClassMethod)@class.GetReferenceForModel(handledDomainEvents);
                        var csharpMapping = new CSharpClassMappingManager(this);
                        csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(this));
                        csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(this));
                        csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(this));
                        csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(this));
                        csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(this));
                        var domainInteractionManager = new DomainInteractionsManager(this, csharpMapping);

                        csharpMapping.SetFromReplacement(handledDomainEvents, "notification.DomainEvent");
                        method.AddMetadata("mapping-manager", csharpMapping);

                        method.AddStatements(domainInteractionManager.CreateInteractionStatements(@class, handledDomainEvents));
                    }
                })
                .AfterBuild(file =>
                {
                    // TODO: MOVE TO FACTORY EXTENSION:
                    foreach (var handledDomainEvents in Model.HandledDomainEvents())
                    {
                        var method = (CSharpClassMethod)file.Classes.First().GetReferenceForModel(handledDomainEvents);
                        if (method.Statements.Count == 0)
                        {
                            method.AddStatement($"// TODO: Implement {method.Name} {file.Classes.First().Name}) functionality");
                            method.AddStatement("throw new NotImplementedException(\"Implement your handler logic here...\");");
                        }
                    }
                }, 10000);
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