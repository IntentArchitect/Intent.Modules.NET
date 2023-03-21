using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Intent.Dapr.AspNetCore.Pubsub.Api;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandler;
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
        private readonly bool _canRunTemplate;
        public const string TemplateId = "Intent.Dapr.AspNetCore.Pubsub.DaprEventHandlerController";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprEventHandlerControllerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.DaprAspNetCore);
            AddNugetDependency(NuGetPackages.MediatR);

            var eventHandlerTemplates = ExecutionContext.FindTemplateInstances<EventHandlerTemplate>(TemplateDependency.OfType<EventHandlerTemplate>())
                .ToArray();
            _canRunTemplate = eventHandlerTemplates.Any();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
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
                        );

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
                });
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            var startupTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>("App.Startup");
            if (startupTemplate == null)
            {
                return;
            }

            startupTemplate.CSharpFile.AfterBuild(file =>
            {
                var configureMethod = file.Classes.First().FindMethod("Configure");
                if (configureMethod == null)
                {
                    return;
                }

                configureMethod.Statements[1].BeforeSeparator = CSharpCodeSeparatorType.NewLine;
                configureMethod.Statements[1].InsertAbove("app.UseCloudEvents();", s => s.SeparatedFromPrevious());

                var block = (IHasCSharpStatements)configureMethod
                    .FindStatement(x => x.ToString().Contains("app.UseEndpoints"));

                block?
                    .FindStatement(x => x.ToString().Contains("endpoints.MapControllers()"))
                    .InsertAbove("endpoints.MapSubscribeHandler();");
            });
        }

        public override bool CanRunTemplate()
        {
            return _canRunTemplate && base.CanRunTemplate();
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