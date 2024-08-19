using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandlerInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EventHandlerTemplate : CSharpTemplateBase<IntegrationEventHandlerModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Pubsub.EventHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EventHandlerTemplate(IOutputTarget outputTarget, IntegrationEventHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            AddTypeSource(TemplateRoles.Domain.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name.ToPascalCase()}", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                    @class.AddConstructor(ctor => { ctor.AddAttribute(CSharpIntentManagedAttribute.Merge()); });

                    foreach (var subscription in Model.IntegrationEventSubscriptions())
                    {
                        @class.ImplementsInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName(subscription)}>");
                        @class.AddMethod("Task", "HandleAsync", method =>
                        {
                            method.Async();
                            method.AddParameter(this.GetIntegrationEventMessageName(subscription.TypeReference.Element.AsMessageModel()), "message");
                            method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                            method.RepresentsModel(subscription);
                            method.RegisterAsProcessingHandlerForModel(subscription);
                        });
                    }
                })
                .AfterBuild(file =>
                {
                    foreach (var handleMethod in file.Classes.First().Methods.Where(x => x.Name == "Handle"))
                    {
                        if (handleMethod.Statements.Count == 0)
                        {
                            handleMethod.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                            handleMethod.AddStatement("throw new NotImplementedException(\"Implement your handler logic here...\");");
                        }
                    }
                }, 1000);
        }

        private string GetMessageName(SubscribeIntegrationEventTargetEndModel subscription)
        {
            return this.GetIntegrationEventMessageName(subscription.TypeReference.Element.AsMessageModel());
        }

        public override void BeforeTemplateExecution()
        {
            foreach (var subscription in Model.IntegrationEventSubscriptions())
            {
                ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                    .ForInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName(subscription)}>")
                    .ForConcern("Application")
                    .WithPriority(100)
                    .HasDependency(GetTemplate<IClassProvider>(IntegrationEventHandlerInterfaceTemplate.TemplateId))
                    .HasDependency(GetTemplate<IClassProvider>(IntegrationEventMessageTemplate.TemplateId, subscription.TypeReference.Element)));
            }
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
    }
}