using Intent.Engine;
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
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.Templates;

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
            var templates = application.FindTemplateInstances<ITemplate>(TemplateFulfillingRoles.Application.Eventing.EventHandler)
                .OfType<ICSharpFileBuilderTemplate>();
            foreach (var template in templates)
            {
                var @class = template.CSharpFile.Classes.First();
                foreach (var handler in template.CSharpFile.GetProcessingHandlers())
                {
                    var method = handler.Method;
                    var model = (SubscribeIntegrationEventTargetEndModel)handler.Model; // need to be careful of assumptions like this?

                    // SEND COMMANDS FROM INTEGRATION EVENT HANDLER:
                    var ctor = @class.Constructors.First();
                    method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyFully());
                    var mappingManager = new CSharpClassMappingManager(template);
                    mappingManager.AddMappingResolver(new CreateCommandMappingResolver(template));
                    mappingManager.AddMappingResolver(new CallServiceOperationMappingResolver(template));
                    mappingManager.SetFromReplacement(model, "message");
                    mappingManager.SetFromReplacement(model.Element, "message");
                    foreach (var sendCommand in model.SentCommandDestinations().Where(x => x.Mappings.Any()))
                    {
                        if (ctor.Parameters.All(x => x.Type != template.UseType("MediatR.ISender")))
                        {
                            ctor.AddParameter(template.UseType("MediatR.ISender"), "mediator", param =>
                            {
                                param.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException());
                            });
                        }

                        method.AddStatement(new CSharpAssignmentStatement("var command", mappingManager.GenerateCreationStatement(sendCommand.Mappings.Single())).WithSemicolon());
                        method.AddStatement(string.Empty);
                        method.AddStatement(new CSharpInvocationStatement("await _mediator.Send").AddArgument("command").AddArgument("cancellationToken"));
                    }

                    foreach (var calledOperation in model.CalledServiceOperations().Where(x => x.Mappings.Any()))
                    {
                        var serviceInterfaceType = template.GetTypeName(TemplateFulfillingRoles.Application.Services.Interface, calledOperation.Element.AsOperationModel().ParentService.InternalElement);
                        var serviceFieldName = @class.Fields.FirstOrDefault(x => x.Type != serviceInterfaceType)?.Name;
                        if (serviceFieldName == null)
                        {
                            ctor.AddParameter(serviceInterfaceType, serviceInterfaceType.AsClassName().ToPrivateMemberName(), param =>
                            {
                                param.IntroduceReadonlyField((field, s) =>
                                {
                                    s.ThrowArgumentNullException();
                                    serviceFieldName = field.Name;
                                });
                            });
                        }

                        method.AddStatement(new CSharpAccessMemberStatement($"await {serviceFieldName}", mappingManager.GenerateUpdateStatements(calledOperation.Mappings.Single()).First()));
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

    public class CallServiceOperationMappingResolver : IMappingTypeResolver
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public CallServiceOperationMappingResolver(ICSharpFileBuilderTemplate template)
        {
            _template = template;
        }

        public ICSharpMapping ResolveMappings(MappingModel mappingModel)
        {
            if (mappingModel.Model.SpecializationType == "Operation")
            {
                return new MethodInvocationMapping(mappingModel, _template);
            }
            return null;
        }
    }
}