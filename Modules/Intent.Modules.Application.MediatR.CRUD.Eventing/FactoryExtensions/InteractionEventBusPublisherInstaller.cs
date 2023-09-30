using System;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.Contracts.DomainMapping.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Eventing.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InteractionEventBusPublisherInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.CRUD.Eventing.EventBusPublisherInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Application.Command.Handler");
            foreach (var template in templates)
            {
                var model = (template as ITemplateWithModel)?.Model as CommandModel ?? throw new Exception("Unable to resolve CommandModel for Application.Command.Handler template");
                if (!model.PublishedIntegrationEvents().Any())
                {
                    continue;
                }

                template.AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
                template.CSharpFile.AfterBuild(file =>
                    {
                        var @class = file.Classes.First();
                        var constructor = @class.Constructors.First();
                        constructor.AddParameter(template.GetTypeName(EventBusInterfaceTemplate.TemplateId), "eventBus", ctor => ctor.IntroduceReadonlyField());

                        if (!file.TryGetMetadata<CSharpClassMappingManager>("mapping-manager", out var csharpMapping))
                        {
                            csharpMapping = new CSharpClassMappingManager(template);
                        }
                        csharpMapping.AddMappingResolver(new MessageCreationMappingTypeResolver(template));
                        csharpMapping.SetFromReplacement(model.InternalElement, "request");
                        
                        foreach (var publish in model.PublishedIntegrationEvents())
                        {
                            var creation = csharpMapping.GenerateCreationStatement(publish.Mappings.Single());
                            AddPublishStatement(@class, new CSharpInvocationStatement("_eventBus.Publish")
                                .AddArgument(creation));
                        }
                    });
            }
        }
        

        private static void AddPublishStatement(CSharpClass @class, CSharpStatement publishStatement)
        {
            var method = @class.FindMethod("Handle");
            var returnClause = method.Statements.FirstOrDefault(p => p.GetText("").Trim().StartsWith("return"));

            if (returnClause != null)
            {
                returnClause.InsertAbove(publishStatement);
            }
            else
            {
                method.Statements.Add(publishStatement);
            }
        }
        
    }
}