using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.DomainEvents.Templates.DomainEventService
{
    [IntentManaged(Mode.Ignore)]
    public partial class DomainEventServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Wolverine.DomainEvents.DomainEventService";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DomainEventServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Wolverine")
                .AddClass("DomainEventService", @class =>
                {
                    @class.ImplementsInterface(GetTypeName(TemplateRoles.Application.Common.DomainEventServiceInterface));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"ILogger<{@class.Name}>", "logger", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IMessageBus", "messageBus", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("Task", "Publish", method =>
                    {
                        method
                            .Async()
                            .AddParameter(GetTypeName(TemplateRoles.Domain.Common.EventBase), "domainEvent")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                        method.AddStatement("_logger.LogInformation(\"Publishing domain event. Event - {event}\", domainEvent.GetType().Name);");
                        method.AddStatement("await _messageBus.PublishAsync(domainEvent);");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
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
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface(GetTemplate<IClassProvider>(TemplateRoles.Application.Common.DomainEventServiceInterface))
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime());
        }
    }
}
