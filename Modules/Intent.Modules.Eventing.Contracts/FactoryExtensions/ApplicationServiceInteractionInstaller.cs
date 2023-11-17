using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Linq;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Constants;
using System.Reflection;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ApplicationServiceInteractionInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.Contracts.ApplicationServiceInteractionInstaller";

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
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Application.Eventing.EventHandler);
            foreach (var template in templates)
            {
                var model = (template as ITemplateWithModel)?.Model as IntegrationEventHandlerModel ?? throw new Exception($"Unable to resolve {nameof(IntegrationEventHandlerModel)} for {TemplateFulfillingRoles.Application.Eventing.EventHandler} template");
                var @class = template.CSharpFile.Classes.First();
                foreach (var subscription in model.IntegrationEventSubscriptions())
                {
                    var method = (CSharpClassMethod)@class.GetReferenceForModel(subscription);

                    // PUBLISH EVENTS FROM COMMAND:
                    if (subscription.SentCommandDestinations().Any())
                    {
                        var ctor = @class.Constructors.First();
                        if (ctor.Parameters.All(x => x.Type != template.UseType("MediatR.ISender")))
                        {
                            ctor.AddParameter(template.UseType("MediatR.ISender"), "mediator", param =>
                            {
                                param.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException());
                            });
                        }
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyFully());
                        var mappingManager = new CSharpClassMappingManager(template);
                        mappingManager.AddMappingResolver(new CreateCommandMappingResolver(template));
                        mappingManager.SetFromReplacement(subscription, "message");
                        mappingManager.SetFromReplacement(subscription.Element, "message");
                        foreach (var sendCommand in subscription.SentCommandDestinations().Where(x => x.Mappings.Any()))
                        {
                            method.AddStatement(new CSharpAssignmentStatement("var command", mappingManager.GenerateCreationStatement(sendCommand.Mappings.Single())).WithSemicolon());
                            method.AddStatement(string.Empty);
                            method.AddStatement(new CSharpInvocationStatement("await _mediator.Send").AddArgument("command").AddArgument("cancellationToken"));
                        }
                    }
                }
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