using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.Wolverine.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineMessageBus;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.Templates.WolverineCompositeConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WolverineCompositeConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Wolverine.WolverineCompositeConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineCompositeConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // Point 5: the DI-registration half of CompositeMessageBus conformance. Kept separate
            // from Intent.Wolverine.Common's WolverineConfiguration - that class configures
            // WolverineOptions (the host builder shape); this configures IServiceCollection (the DI
            // shape CompositeMessageBusConfiguration expects every conforming broker's own
            // Add{Broker}Configuration(services, configuration, registry) to expose). Only emitted
            // when this application actually needs a composite bus - a non-composite app keeps
            // registering WolverineMessageBus the existing way, via
            // WolverineEventingRegistrationExtension.RegisterWolverineMessageBus's
            // ContainerRegistrationRequest.
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusConfiguration);

            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            AddTypeSource(IntegrationCommandTemplate.TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass("WolverineCompositeConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "AddWolverineEventingConfiguration", method =>
                    {
                        method.Static();
                        method.AddParameter(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "services", param => param.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddParameter(this.GetMessageBrokerRegistryName(), "registry");

                        // GetTemplate first, ThrowIfNotFound = false / TrackDependency = false, THEN
                        // GetTypeName - never GetTypeName alone on a foreign template. Per this
                        // module's own CONTEXT.md: GetTypeName on a foreign template whose own
                        // instance has not yet finished constructing in this Software Factory pass
                        // hits NormalizeNamespace against that template's not-yet-populated file
                        // metadata and throws NullReferenceException. Forcing the existence check
                        // first forces the target to fully construct before its name is read.
                        GetTemplate<object>(WolverineMessageBusTemplate.TemplateId,
                            new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false });
                        var busTypeName = GetTypeName(WolverineMessageBusTemplate.TemplateId);
                        method.AddStatement($"services.AddScoped<{busTypeName}>();");

                        // Same Wolverine-designated published-message / sent-command sets
                        // WolverineEventingRegistrationExtension's EventingContext gathers for the
                        // WolverineOptions side - recomputed here rather than shared, since that
                        // context is private to a different factory extension.
                        var publishedMessages = this.GetWolverineDesignatedMessages(
                            ExecutionContext.MetadataManager.GetExplicitlyPublishedMessageModels(OutputTarget.Application)).ToList();
                        var sentCommands = this.GetWolverineDesignatedIntegrationCommands(
                            ExecutionContext.MetadataManager.GetExplicitlySentIntegrationCommandModels(OutputTarget.Application)).ToList();

                        var isFirst = true;

                        void AddRegisterStatement(string typeName)
                        {
                            var separate = isFirst;
                            isFirst = false;
                            method.AddStatement($"registry.Register<{typeName}, {busTypeName}>();",
                                s => { if (separate) s.SeparatedFromPrevious(); });
                        }

                        foreach (var message in publishedMessages)
                        {
                            AddRegisterStatement(UseType(GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message)));
                        }

                        foreach (var command in sentCommands)
                        {
                            AddRegisterStatement(UseType(GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, command)));
                        }

                        method.AddReturn("services", s => s.SeparatedFromPrevious());
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

        /// <summary>
        /// Composite-only: a non-composite app never generates this file at all, so
        /// CompositeMessageBusConfiguration's role-based discovery only ever finds it when it has
        /// something to call.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && this.RequiresCompositeMessageBus();
        }
    }
}