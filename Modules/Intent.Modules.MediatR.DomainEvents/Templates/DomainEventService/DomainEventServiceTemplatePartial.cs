using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.MediatR.DomainEvents.Templates.DomainEventNotification;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Templates.DomainEventService
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class DomainEventServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.MediatR.DomainEvents.DomainEventService";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DomainEventServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MediatR")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddClass($"DomainEventService", @class =>
                {
                    @class.ImplementsInterface(GetInterfaceType());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"ILogger<{@class.Name}>", "logger", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IPublisher", "mediator", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("Task", "Publish", method =>
                    {
                        method
                            .Async()
                            .AddParameter(GetDomainEventBaseType(), "domainEvent")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                        method.AddStatement("_logger.LogInformation(\"Publishing domain event. Event - {event}\", domainEvent.GetType().Name);");
                        method.AddStatement("await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent), cancellationToken);");
                    });

                    @class.AddMethod("INotification", "GetNotificationCorrespondingToDomainEvent", method =>
                    {
                        method
                            .Private()
                            .AddParameter(GetDomainEventBaseType(), "domainEvent");
                        method.AddStatement(@$"var result = Activator.CreateInstance(
                typeof({GetDomainEventNotificationType()}<>).MakeGenericType(domainEvent.GetType()), domainEvent);");
                        method.AddStatement(@"if (result == null)
                throw new Exception($""Unable to create DomainEventNotification<{domainEvent.GetType().Name}>"");");
                        method.AddStatement("return (INotification)result;");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DomainEventService",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface(GetTemplate<IClassProvider>(DomainEventServiceInterfaceTemplate.TemplateId))
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime());
        }

        private string GetDomainEventBaseType()
        {
            return GetTypeName(DomainEventBaseTemplate.TemplateId);
        }

        private string GetInterfaceType()
        {
            return GetTypeName(DomainEventServiceInterfaceTemplate.TemplateId);
        }

        private string GetDomainEventNotificationType()
        {
            return GetTypeName(DomainEventNotificationTemplate.TemplateId);
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "2.0"), new Migration());
        }

        /// <summary>
        /// Fixes that for a very long time this template's default mode was Ignore.
        /// </summary>
        private class Migration : ITemplateMigration
        {
            public string Execute(string currentText)
            {
                return currentText.Replace(
                    "[assembly: DefaultIntentManaged(Mode.Ignore)]",
                    "[assembly: DefaultIntentManaged(Mode.Fully)]");
            }

            public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.Upgrade(1, 2);
        }
    }
}