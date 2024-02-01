using System;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

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
            var templates = application.FindTemplateInstances<ITemplate>(TemplateRoles.Application.Eventing.EventHandler)
                .OfType<ICSharpFileBuilderTemplate>();
            foreach (var template in templates)
            {
                var @class = template.CSharpFile.Classes.First();
                foreach (var handler in template.CSharpFile.GetProcessingHandlers())
                {
                    var method = handler.Method;
                    var integrationData = handler.Model switch
                    {
                        SubscribeIntegrationEventTargetEndModel eventTargetEndModel => new
                        {
                            Model = (IMetadataModel)eventTargetEndModel,
                            ElementModel = (IMetadataModel)eventTargetEndModel.Element,
                            SentCommandDestinations = eventTargetEndModel.SentCommandDestinations(),
                            CalledServiceOperations = eventTargetEndModel.CalledServiceOperations()
                        },
                        SubscribeIntegrationCommandTargetEndModel eventTargetEndModel => new
                        {
                            Model = (IMetadataModel)eventTargetEndModel,
                            ElementModel = (IMetadataModel)eventTargetEndModel.Element,
                            SentCommandDestinations = eventTargetEndModel.SentCommandDestinations(),
                            CalledServiceOperations = eventTargetEndModel.CalledServiceOperations()
                        },
                        _ => throw new NotSupportedException($"{handler.Model.GetType().FullName} is not supported")
                    };

                    // SEND COMMANDS FROM INTEGRATION EVENT HANDLER:
                    var ctor = @class.Constructors.First();
                    method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyFully());
                    var mappingManager = new CSharpClassMappingManager(template);
                    mappingManager.AddMappingResolver(new CallServiceOperationMappingResolver(template));
                    mappingManager.AddMappingResolver(new CreateCommandMappingResolver(template));
                    mappingManager.SetFromReplacement(integrationData.Model, "message");
                    mappingManager.SetFromReplacement(integrationData.ElementModel, "message");

                    foreach (var sendCommand in integrationData.SentCommandDestinations.Where(x => x.Mappings.Any()))
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

                    foreach (var calledOperation in integrationData.CalledServiceOperations.Where(x => x.Mappings.Any()))
                    {
                        var serviceInterfaceType = template.GetTypeName(TemplateRoles.Application.Services.Interface, calledOperation.Element.AsOperationModel().ParentService.InternalElement);
                        var serviceFieldName = @class.Fields.FirstOrDefault(x => x.Type == serviceInterfaceType)?.Name;
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

                        var operationInvocation = mappingManager.GenerateUpdateStatements(calledOperation.Mappings.Single()).First();
                        (operationInvocation as CSharpInvocationStatement)?.AddArgument("cancellationToken");
                        method.AddStatement(new CSharpAccessMemberStatement($"await {serviceFieldName}", operationInvocation));
                    }
                }
            }
        }
    }

    [IntentManaged(Mode.Ignore)]
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
            if (mappingModel.Model.TypeReference?.Element?.IsTypeDefinitionModel() == true
                || mappingModel.Model.TypeReference?.Element?.IsEnumModel() == true)
            {
                return new TypeConvertingCSharpMapping(mappingModel, _template);
            }
            return null;
        }
    }

    [IntentManaged(Mode.Ignore)]
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