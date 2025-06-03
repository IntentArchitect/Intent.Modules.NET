using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Contracts.InteractionStrategies;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
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
            InteractionStrategyProvider.Instance.Register(new PublishIntegrationMessageInteractionStrategy());
            var templates = application.FindTemplateInstances<ITemplate>(TemplateRoles.Application.Eventing.EventHandler)
                .OfType<ICSharpFileBuilderTemplate>();
            foreach (var template in templates)
            {
                var @class = template.CSharpFile.Classes.First();

                foreach (var handler in template.CSharpFile.GetProcessingHandlers())
                {
                    var method = handler.Method;

                    var mappingManager = method.GetMappingManager();
                    //mappingManager.AddMappingResolver(new CallServiceOperationMappingResolver(template));
                    //mappingManager.AddMappingResolver(new CreateCommandMappingResolver(template));
                    mappingManager.AddMappingResolver(new TypeConvertingMappingResolver(template));
                    mappingManager.SetFromReplacement(handler.Model, "message");
                    mappingManager.SetFromReplacement(handler.Model.InternalElement, "message");

                    //// SEND COMMANDS FROM INTEGRATION EVENT HANDLER:
                    method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyFully());
                    method.AddStatements(method.CreateInteractionStatements(handler.Model));
                }
            }
        }

    }

    public class TypeConvertingMappingResolver : IMappingTypeResolver
    {
        private readonly ICSharpTemplate _template;

        public TypeConvertingMappingResolver(ICSharpTemplate template)
        {
            _template = template;
        }

        public ICSharpMapping ResolveMappings(MappingModel mappingModel)
        {
            if (mappingModel.Model.TypeReference?.Element?.IsTypeDefinitionModel() == true
                || mappingModel.Model.TypeReference?.Element?.IsEnumModel() == true)
            {
                return new TypeConvertingCSharpMapping(mappingModel, _template);
            }

            return null;
        }
    }
}