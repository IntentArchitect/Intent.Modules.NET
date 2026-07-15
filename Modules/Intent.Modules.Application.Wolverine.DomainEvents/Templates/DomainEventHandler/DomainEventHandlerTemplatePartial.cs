using Intent.Engine;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping.Resolvers;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.DomainEvents.Templates.DomainEventHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainEventHandlerTemplate : CSharpTemplateBase<DomainEventHandlerModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.DomainEvents.DomainEventHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEventHandlerTemplate(IOutputTarget outputTarget, DomainEventHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            AddTypeSource(TemplateRoles.Domain.Events);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass(Model.Name, @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });

                    foreach (var handledDomainEvent in Model.HandledDomainEvents())
                    {
                        @class.AddMethod("Task", "Handle", method =>
                        {
                            method.RepresentsModel(handledDomainEvent);
                            method.RegisterAsProcessingHandlerForModel(handledDomainEvent);
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                            method.Async();
                            method.AddParameter(GetTypeName(TemplateRoles.Domain.Events, handledDomainEvent.TypeReference.Element), "domainEvent");
                            method.AddParameter("CancellationToken", "cancellationToken");
                        });
                    }
                })
                .AfterBuild(file =>
                {
                    foreach (var handler in CSharpFile.GetProcessingHandlers())
                    {
                        var interactions = handler.Model.GetInteractions().ToList();
                        if (interactions.Any())
                        {
                            var method = handler.Method;
                            var csharpMapping = method.GetMappingManager();
                            csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(this));
                            csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(this));
                            csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(this));
                            csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(this));
                            csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(this));
                            csharpMapping.AddMappingResolver(new ServiceOperationMappingTypeResolver(this));
                            csharpMapping.AddMappingResolver(new TypeConvertingMappingResolver(this));

                            // TODO: These can go to the handler template:
                            csharpMapping.SetFromReplacement(handler.Model, "domainEvent");
                            csharpMapping.SetFromReplacement(handler.Model.InternalElement, "domainEvent");

                            // Inheritance handling
                            var domainEventModel = handler.Model.InternalElement.TypeReference?.Element?.AsDomainEventModel();
                            var generalization = domainEventModel?.Generalizations().SingleOrDefault();
                            if (generalization != null)
                            {
                                csharpMapping.SetFromReplacement(generalization, "domainEvent");
                                csharpMapping.SetToReplacement(generalization, null);
                            }

                            method.ImplementInteractions(interactions);
                        }
                    }
                })
                .AfterBuild(file =>
                {
                    foreach (var handledDomainEvent in Model.HandledDomainEvents())
                    {
                        var method = (CSharpClassMethod)file.Classes.First().GetReferenceForModel(handledDomainEvent);
                        if (method.Statements.Count == 0)
                        {
                            method.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyMerge();
                            method.AddStatement("// IntentInitialGen");
                            method.AddStatement($"// TODO: Implement {method.Name} ({file.Classes.First().Name}) functionality");
                            method.AddStatement("throw new NotImplementedException(\"Implement your handler logic here...\");");
                        }
                    }
                }, 10000);
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
