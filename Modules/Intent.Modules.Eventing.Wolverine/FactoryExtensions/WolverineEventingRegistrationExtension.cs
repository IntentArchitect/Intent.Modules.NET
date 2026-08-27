using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Wolverine.Settings;
using Intent.Modules.Eventing.Wolverine.Templates;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineEventingConfiguration;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineMessageBus;
using Intent.Modules.Wolverine.Common.Api;
using Intent.Modules.Wolverine.Common.FactoryExtensions;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineEventingRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.Wolverine.WolverineEventingRegistrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // Wolverine host registration is ASP.NET-host-only ("App.Program"). A host-scoped
            // template can have more than one instance in a multi-host application, so this must use
            // the plural lookup and loop rather than FindTemplateInstance, which throws once a second
            // host exists (same reasoning as Intent.Application.Wolverine's own registration
            // extension, which this mirrors).
            foreach (var programTemplate in application.FindTemplateInstances<IProgramTemplate>("App.Program"))
            {
                RegisterWolverineEventingOnHost(programTemplate);
            }

            RegisterWolverineMessageBus(application);
        }

        /// <summary>
        /// R3.9: the bus is only registered against the Contracts <c>IMessageBus</c> interface when
        /// this application has at least one Wolverine-designated published Integration Event or sent
        /// Integration Command - a subscribe-only application gets no registration. The Composite
        /// Message Bus branch is out of scope here: <see cref="WolverineMessageBusInteropExtension"/>
        /// already registers Wolverine into the shared MessageBusRegistry for that case.
        /// </summary>
        private static void RegisterWolverineMessageBus(IApplication application)
        {
            var busTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(WolverineMessageBusTemplate.TemplateId);
            if (busTemplate == null)
            {
                return;
            }

            var hasPublishedMessages = busTemplate.ExecutionContext.MetadataManager
                .GetExplicitlyPublishedMessageModels(application)
                .FilterMessagesForThisMessageBroker(application, Intent.Modules.Eventing.Wolverine.Templates.Constants.BrokerStereotypeIds)
                .Any();

            var hasSentCommands = busTemplate.ExecutionContext.MetadataManager
                .GetExplicitlySentIntegrationCommandModels(application)
                .FilterMessagesForThisMessageBroker(application, Intent.Modules.Eventing.Wolverine.Templates.Constants.BrokerStereotypeIds)
                .Any();

            if (!hasPublishedMessages && !hasSentCommands)
            {
                return;
            }

            var busInterfaceTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(busTemplate.GetBusInterfaceTemplateId());
            if (busInterfaceTemplate == null)
            {
                return;
            }

            application.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(busTemplate)
                .ForInterface(busInterfaceTemplate)
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime());
        }

        private static void RegisterWolverineEventingOnHost(IProgramTemplate programTemplate)
        {
            if (programTemplate == null)
            {
                return;
            }

            programTemplate.CSharpFile.OnBuild(file => file.AddUsing("Wolverine"));

            // Contribute() must be called NOW, synchronously during OnAfterTemplateRegistrations -
            // not deferred inside a CSharpFile.OnBuild callback. WolverineHostRegistrationExtension
            // (Intent.Wolverine.Common) reads the contributions table from its OWN OnBuild callback
            // on this same CSharpFile; OnBuild callbacks fire in registration order, so a contribution
            // registered inside OnBuild here could lose the race against Common's consuming callback
            // depending on factory-extension execution order, silently vanishing from the generated
            // lambda. Calling Contribute() eagerly here guarantees the entry exists in the table
            // before ANY OnBuild callback runs. The type name / configure-method-name resolution
            // still happens lazily inside the ConfigureAction closure, which Common invokes during
            // its own OnBuild callback (after every template has been registered).
            WolverineHostRegistrationExtension.Contribute(programTemplate,
                WolverineHostConfigurationRequest.Configure((lambdaBlock, parameters) =>
                {
                    var opts = parameters[0];
                    var eventingConfigType = programTemplate.GetTypeName(WolverineEventingConfigurationTemplate.TemplateId);
                    var transport = programTemplate.ExecutionContext.Settings.GetWolverineMessageBusSettings().Transport().AsEnum();
                    var configureMethodName = WolverineEventingConfigurationTemplate.GetConfigureMethodName(transport);
                    lambdaBlock.AddStatement($"{eventingConfigType}.{configureMethodName}({opts}, builder.Configuration);");
                }));
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            // Your custom logic here.
        }
    }
}
