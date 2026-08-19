using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Module.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MassTransitConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModularMonolith.Module.MassTransitConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            base.OnBeforeTemplateRegistrations(application);

            //Doing this in OnBeforeTemplateRegistrations to Ensure this subscription Runs first
            var outputTarget = application.OutputTargets.SingleOrDefault(x => x.OutputsTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection"));
            var template = new Template(outputTarget);

            template.OnEmitOrPublished<ServiceConfigurationRequest>(request =>
            {
                if (request.Concern == "Infrastructure" && request.ExtensionMethodName == "AddMassTransitConfiguration")
                {
                    request.MarkAsHandled();
                }
            });
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

        private class Template : IntentTemplateBase
        {
            public Template(IOutputTarget outputTarget) : base(null, outputTarget)
            {
            }

            public override ITemplateFileConfig GetTemplateFileConfig()
            {
                throw new NotImplementedException();
            }

            public override string TransformText()
            {
                throw new NotImplementedException();
            }
        }
    }
}