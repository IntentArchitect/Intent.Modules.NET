using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.SubscriptionsTf
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class SubscriptionsTfTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.IaC.Terraform.SubscriptionsTfTemplate";

        private readonly EventGridTerraformExtension _eventGridTerraformExtension = new();
        private bool _beforeTemplateExecutionCalled;
        
        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public SubscriptionsTfTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(Handle);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"main",
                fileExtension: "tf",
                relativeLocation: "terraform/02-subscriptions"
            );
        }

        private void Handle(InfrastructureRegisteredEvent @event)
        {
            _eventGridTerraformExtension.ProcessEvent(@event);
        }

        public override bool CanRunTemplate()
        {
            return !_beforeTemplateExecutionCalled || _eventGridTerraformExtension.HasSubscriptions();
        }

        public override void BeforeTemplateExecution()
        {
            _beforeTemplateExecutionCalled = true;
            base.BeforeTemplateExecution();
        }
        
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var builder = new TerraformFileBuilder();

            builder.AddTerraformConfig(terraform => terraform
                    .AddBlock("required_providers", block => block
                        .AddObject("azurerm", b => b
                            .AddSetting("source", "hashicorp/azurerm")
                            .AddSetting("version", "~> 3.0")))
                // .AddBackend("azurerm", backend =>
                // {
                //     backend
                //         .AddSetting("resource_group_name", "terraform-state-rg")
                //         .AddSetting("storage_account_name", "tfstateXXXXXXXX")
                //         .AddSetting("container_name", "tfstate")
                //         .AddSetting("key", "subscriptions.tfstate");
                // })
            );

            builder.AddProvider("azurerm", provider => { provider.AddBlock("features"); });

            // Variables for the resources created in the first deployment
            builder.AddVariable("function_app_id", v => v
                .AddSetting("description", "The ID of the Function App")
                .AddRawSetting("type", "string"));
            
            builder.AddVariable("resource_group_name", v => v
                .AddSetting("description", "The name of the resource group")
                .AddRawSetting("type", "string"));

            _eventGridTerraformExtension.ApplyVariables(builder);

            _eventGridTerraformExtension.ApplySubscriptions(builder);

            return builder.Build();
        }
    }
}