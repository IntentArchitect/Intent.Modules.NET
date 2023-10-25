using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandlerInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using MessageModelExtensions = Intent.Modelers.Eventing.Api.MessageModelExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IntegrationEventHandlerTemplate : CSharpTemplateBase<IntegrationEventHandlerModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.IntegrationEventHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationEventHandlerTemplate(IOutputTarget outputTarget, IntegrationEventHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateFulfillingRoles.Application.Command);
            AddTypeSource(TemplateFulfillingRoles.Application.Query);
            AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Enum);
            AddTypeSource(TemplateFulfillingRoles.Domain.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name.ToPascalCase()}", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });

                    foreach (var subscription in Model.IntegrationEventsSubscriptions())
                    {
                        @class.ImplementsInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName(subscription)}>");
                        @class.AddMethod("Task", "HandleAsync", method =>
                        {
                            method.Async();
                            method.AddParameter(this.GetIntegrationEventMessageName(subscription.TypeReference.Element.AsMessageModel()), "message");
                            method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));


                            if (subscription.SentCommandDestinations().Any())
                            {
                                var ctor = @class.Constructors.First();
                                if (ctor.Parameters.All(x => x.Type != UseType("MediatR.ISender")))
                                {
                                    ctor.AddParameter(UseType("MediatR.ISender"), "mediator", param =>
                                    {
                                        param.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException());
                                    });
                                }
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyFully());
                                var mappingManager = new CSharpClassMappingManager(this);
                                mappingManager.AddMappingResolver(new CreateCommandMappingResolver(this));
                                mappingManager.SetFromReplacement(subscription, "message");
                                mappingManager.SetFromReplacement(subscription.Element, "message");
                                foreach (var sendCommand in subscription.SentCommandDestinations().Where(x => x.Mappings.Any()))
                                {
                                    method.AddStatement(new CSharpAssignmentStatement("var command", mappingManager.GenerateCreationStatement(sendCommand.Mappings.Single())).WithSemicolon());
                                    method.AddStatement(string.Empty);
                                    method.AddStatement(new CSharpInvocationStatement("await _mediator.Send").AddArgument("command").AddArgument("cancellationToken"));
                                }
                            }
                            else
                            {
                                method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                                method.AddStatement("throw new NotImplementedException();");
                            }
                        });
                    }

                });
        }

        private string GetMessageName(SubscribeIntegrationEventTargetEndModel subscription)
        {
            return this.GetIntegrationEventMessageName(subscription.TypeReference.Element.AsMessageModel());
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

        public override void BeforeTemplateExecution()
        {
            foreach (var subscription in Model.IntegrationEventsSubscriptions())
            {
                ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                    .ForInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName(subscription)}>")
                    .ForConcern("Application")
                    .WithPriority(100)
                    .HasDependency(GetTemplate<IClassProvider>(IntegrationEventHandlerInterfaceTemplate.TemplateId))
                    .HasDependency(GetTemplate<IClassProvider>(IntegrationEventMessageTemplate.TemplateId, subscription.TypeReference.Element)));
            }
        }

    }

    public class CreateCommandMappingResolver : IMappingTypeResolver
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public CreateCommandMappingResolver(ICSharpFileBuilderTemplate template)
        {
            _template = template;
        }

        public ICSharpMapping ResolveMappings(MappingModel mappingModel)
        {
            if (mappingModel.Model.SpecializationType == "Command")
            {
                return new ConstructorMapping(mappingModel, _template);
            }
            if (mappingModel.Model.TypeReference?.Element?.SpecializationType == "DTO")
            {
                return new ObjectInitializationMapping(mappingModel, _template);
            }
            return null;
        }
    }
}