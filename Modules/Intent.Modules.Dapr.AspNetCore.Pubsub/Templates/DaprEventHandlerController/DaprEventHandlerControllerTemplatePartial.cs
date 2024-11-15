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
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandler;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandlerImplementation;
using Intent.Modules.Eventing.Contracts.Templates;
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
                        .AddConstructor(constructor => constructor
                            .AddParameter("ISender", "mediatr", p => p.IntroduceReadonlyField())
                            .AddParameter("IServiceProvider", "serviceProvider", p => p.IntroduceReadonlyField())
                        );

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
                    if (eventHandlerTemplates.Any())
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
                                method
                                    .AddAttribute("[HttpPost]")
                                    .AddAttribute($"[Topic({eventType}.PubsubName, {eventType}.TopicName)]")
                                    .Async()
                                    .AddParameter(eventType, "@event")
                                    .AddParameter("CancellationToken", "cancellationToken")
                                    .AddStatement($"var handler = _serviceProvider.GetRequiredService<{this.GetIntegrationEventHandlerInterfaceName()}<{eventType}>>();")
                                    .AddStatement("await handler.HandleAsync(@event, cancellationToken);");
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
            var eventHandlerTemplates = ExecutionContext
                .FindTemplateInstances<EventHandlerImplementationTemplate>(TemplateDependency.OfType<EventHandlerImplementationTemplate>())
                .ToArray();

            return eventHandlerTemplates.Any() && base.CanRunTemplate();
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