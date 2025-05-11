using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.InfrastructureTfVars
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class InfrastructureTfVarsTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.IaC.Terraform.InfrastructureTfVarsTemplate";

        private bool _hasAzureServiceBus;
        
        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public InfrastructureTfVarsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(Handle);
        }

        private void Handle(InfrastructureRegisteredEvent @event)
        {
            if (@event.InfrastructureComponent is Infrastructure.AzureServiceBus.QueueType or
                Infrastructure.AzureServiceBus.TopicType or
                Infrastructure.AzureServiceBus.SubscriptionType)
            {
                _hasAzureServiceBus = true;
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"terraform",
                fileExtension: "tfvars",
                relativeLocation: "terraform/02-infrastructure"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace('.', '-').ToKebabCase();
            var keysWithDefaults = new Dictionary<string, string>
            {
                { "app_insights_name", @"""" },
                { "resource_group_location", @"""East US""" },
                { "resource_group_name", $@"""rg-{sanitizedAppName}""" }
            };

            if (_hasAzureServiceBus)
            {
                keysWithDefaults["service_bus_namespace_name"] = "";
            }

            return this.MergeKeyValuePairs(keysWithDefaults);
        }
    }
}