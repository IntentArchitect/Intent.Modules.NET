using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Intent.Dapr.AspNetCore.Pubsub.Api;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandler;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandlerImplementation;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Persistence.UnitOfWork.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprEventHandlerController
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprEventHandlerControllerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Pubsub.DaprEventHandlerController";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprEventHandlerControllerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.DaprAspNetCore(outputTarget));
            AddNugetDependency(NugetPackages.MediatR(outputTarget));
            var shouldInstallUnitOfWork = new Lazy<bool>(this.SystemUsesPersistenceUnitOfWork);
            var shouldInstallMessageBus = new Lazy<bool>(() => OutputTarget.FindTemplateInstance<IClassProvider>(TemplateRoles.Application.Eventing.EventBusInterface, accessibleTo: null) != null);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Dapr")
                .AddUsing("MediatR")
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .IntentManagedFully()
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .AddClass("DaprEventHandlerController", @class =>
                {
                    @class
                        .AddAttribute("[Route(\"api/v1/[controller]/[action]\")]")
                        .AddAttribute("[ApiController]")
                        .WithBaseType("ControllerBase")
                        .AddConstructor(ctor =>
                        {
                            ctor.AddParameter("ISender", "mediatr", p => p.IntroduceReadonlyField());
                            ctor.AddParameter("IServiceProvider", "serviceProvider", p => p.IntroduceReadonlyField());

                            if (shouldInstallMessageBus.Value)
                            {
                                ctor.AddParameter(
                                    GetTypeName(TemplateRoles.Application.Eventing.EventBusInterface), "eventBus",
                                    p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });
                            }
                        });

                    //Eventing Designer
                    var eventHandlerTemplates = ExecutionContext.FindTemplateInstances<EventHandlerImplementationTemplate>(TemplateDependency.OfType<EventHandlerImplementationTemplate>())
                        .ToArray();

                    foreach (var eventHandler in eventHandlerTemplates)
                    {
                        var eventType = this.GetIntegrationEventMessageName(eventHandler.Model);

                        @class.AddMethod("Task", $"Handle{eventType}", method =>
                        {
                            method
                                .AddAttribute("[HttpPost]")
                                .AddAttribute($"[Topic({eventType}.PubsubName, {eventType}.TopicName)]")
                                .Async()
                                .AddParameter(eventType, "@event")
                                .AddParameter("CancellationToken", "cancellationToken")
                                .AddStatement("await _mediatr.Send(@event, cancellationToken);");
                        });
                    }

                    //Service Designer
                    var eventSubscriptionTemplates = ExecutionContext.FindTemplateInstances<EventHandlerTemplate>(TemplateDependency.OfType<EventHandlerTemplate>())
                        .ToArray();
                    if (eventSubscriptionTemplates.Length > 0)
                    {
                        AddUsing("System");
                        AddUsing("Microsoft.Extensions.DependencyInjection");
                    }
                    foreach (var eventHandler in eventSubscriptionTemplates)
                    {
                        foreach (var subscription in eventHandler.Model.IntegrationEventSubscriptions())
                        {
                            var eventType = GetMessageName(subscription);
                            @class.AddMethod("Task", $"Handle{eventType}", method =>
                            {
                                method.AddAttribute("[HttpPost]");
                                method.AddAttribute($"[Topic({eventType}.PubsubName, {eventType}.TopicName)]");
                                method.Async();
                                method.AddParameter(eventType, "@event");
                                method.AddParameter("CancellationToken", "cancellationToken");
                                method.AddStatement($"var handler = _serviceProvider.GetRequiredService<{this.GetIntegrationEventHandlerInterfaceName()}<{eventType}>>();");

                                var invocationStatement = "await handler.HandleAsync(@event, cancellationToken);";

                                if (shouldInstallUnitOfWork.Value)
                                {
                                    method.ApplyUnitOfWorkImplementations(
                                        template: this,
                                        constructor: @class.Constructors.First(),
                                        invocationStatement: invocationStatement,
                                        returnType: null,
                                        resultVariableName: "result",
                                        fieldSuffix: "unitOfWork",
                                        includeComments: false);
                                }
                                else
                                {
                                    method.AddStatement(invocationStatement);
                                }

                                if (shouldInstallMessageBus.Value)
                                {
                                    method.AddStatement("await _eventBus.FlushAllAsync(cancellationToken);", stmt => stmt.AddMetadata("eventbus-flush", true));
                                }
                            });
                        }
                    }
                });
        }

        private string GetMessageName(SubscribeIntegrationEventTargetEndModel subscription)
        {
            return this.GetIntegrationEventMessageName(subscription.TypeReference.Element.AsMessageModel());
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            var startupTemplate = ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            startupTemplate?.CSharpFile.AfterBuild(_ =>
            {
                startupTemplate.StartupFile.ConfigureApp((statements, context) =>
                {
                    ExecutionContext.EventDispatcher.Publish(
                        ApplicationBuilderRegistrationRequest.ToRegister(
                                extensionMethodName: "UseCloudEvents")
                            .WithPriority(-200));
                });

                startupTemplate.StartupFile.ConfigureEndpoints((statements, context) =>
                {
                    statements
                        .FindStatement(x => x.ToString()!.Contains(".MapControllers("))
                        .InsertAbove($"{context.Endpoints}.MapSubscribeHandler();");
                });
            });
        }

        public override bool CanRunTemplate()
        {
            if (!ExecutionContext.FindTemplateInstances<EventHandlerImplementationTemplate>(TemplateDependency.OfType<EventHandlerImplementationTemplate>()).Any() &&
                !ExecutionContext.FindTemplateInstances<EventHandlerTemplate>(TemplateDependency.OfType<EventHandlerTemplate>()).Any())
            {
                return false;
            }

            return base.CanRunTemplate();
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