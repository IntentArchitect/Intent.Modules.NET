using System;
using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Migrations
{
    public class Migration_03_01_00_Pre_01 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_03_01_00_Pre_01(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Dapr.AspNetCore.Pubsub";
        [IntentFully]
        public string ModuleVersion => "3.1.0-pre.1";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            const string VSDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
            var vsDesigner = app.TryGetDesigner(VSDesignerId);
            if (vsDesigner is null)
            {
                return;
            }

            foreach (var package in vsDesigner.GetPackages())
            {
                const string TemplateOutputId = "d421c322-7a51-4094-89fa-e5d8a0a97b27";
                var templateOutputs = package.GetElementsOfType(TemplateOutputId);

                const string IntentEventingContracts_IntegrationEventHandler = "Intent.Eventing.Contracts.IntegrationEventHandler";
                const string IntentDaprAspNetCorePubsub_EventHandler = "Intent.Dapr.AspNetCore.Pubsub.EventHandler";
                
                // If we see this, its probably a glitch in the migration process. Raise it.
                if (templateOutputs.Any(x => x.Name == IntentEventingContracts_IntegrationEventHandler))
                {
                    Logging.Log.Failure(@"The ""Intent.Eventing.Contracts.IntegrationEventHandler"" already exists. This may indicate a failure in the module update process. Please contact Intent support.");
                    continue;
                }
                
                // Ok lets install the new role where the old one is right now.
                var pubsubEventHandler = templateOutputs.FirstOrDefault(x => x.Name == IntentDaprAspNetCorePubsub_EventHandler);
                // If this can't be found, something weird is happening.
                if (pubsubEventHandler is null)
                {
                    Logging.Log.Failure(@"Could not find the ""Intent.Dapr.AspNetCore.Pubsub.EventHandler"" template output in the VS Designer. This may have code loss consequences. Try and reinstall the Intent.Dapr.AspNetCore.Pubsub module and if this error persists, please contact Intent support.");
                    continue;
                }
                
                // Get the pubsubEventHandler's parent element so we can create the new element in the same place.
                var parentElement = package.GetElementById(pubsubEventHandler.ParentFolderId);
                var newTemplateOutput = ElementPersistable.Create(
                    specializationType: "Template Output", 
                    specializationTypeId: TemplateOutputId, 
                    name: IntentEventingContracts_IntegrationEventHandler,
                    parentId: parentElement.Id, 
                    externalReference: "Intent.Eventing.Contracts");
                parentElement.AddElement(newTemplateOutput);
                
                package.Save();
            }
        }

        public void Down()
        {
        }
    }
}