using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventNotificationFactory;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainEventServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.DomainEvents.DomainEventService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEventServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsLogging(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("DomainEventService", @class =>
                {
                    @class.ImplementsInterface(GetTypeName(TemplateRoles.Application.Common.DomainEventServiceInterface));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"{UseType("Microsoft.Extensions.Logging.ILogger")}<{@class.Name}>", "logger", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(UseType("Mediator.IPublisher"), "mediator", param => param.IntroduceReadonlyField());
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "Publish", method =>
                    {
                        method.Async();
                        method.AddParameter(GetTypeName(TemplateRoles.Domain.Common.EventBase), "domainEvent");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", parameter => parameter.WithDefaultValue("default"));
                        method.AddStatement("_logger.LogInformation(\"Publishing domain event. Event - {event}\", domainEvent.GetType().Name);");
                        method.AddStatement($"await _mediator.Publish({GetTypeName(DomainEventNotificationFactoryTemplate.TemplateId)}.Create(domainEvent), cancellationToken);");
                    });
                });
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

        [IntentManaged(Mode.Ignore)]
        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface(GetTemplate<IClassProvider>(TemplateRoles.Application.Common.DomainEventServiceInterface))
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime());
        }
    }
}