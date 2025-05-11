using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using NuGet.Versioning;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.ModuleInfrastructure.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MassTransitConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModularMonolith.ModuleInfrastructure.MassTransitConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            base.OnBeforeTemplateRegistrations(application);
            //Doing this in OnBeforeTemplateRegistrations to Ensure this subscription Runs first
            application.EventDispatcher.Subscribe<ServiceConfigurationRequest>(HandleEvent);
        }

        private void HandleEvent(ServiceConfigurationRequest request)
        {
            //This stops the Module from adding using Clauses for these Requests
            if (request.Concern == "Infrastructure" && request.ExtensionMethodName == "AddMassTransitConfiguration")
            {
                request.MarkAsHandled();
            }
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            base.OnBeforeTemplateExecution(application);
            RemoveStandardDIConfigForModule(application);
        }

        private void RemoveStandardDIConfigForModule(IApplication application)
        {
            var massTransitModuleConfig = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Eventing.MassTransit.MassTransitConfiguration");

            massTransitModuleConfig?.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var method = @class.FindMethod("AddMassTransitConfiguration");
                if (method is not null)
                {
                    @class.Methods.Remove(method);
                }
                method = @class.FindMethod("AddConsumers");
                method?.Public();

            }, 1000);
        }
    }
}