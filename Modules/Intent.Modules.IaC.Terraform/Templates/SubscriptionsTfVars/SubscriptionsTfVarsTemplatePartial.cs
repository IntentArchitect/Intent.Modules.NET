using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.SubscriptionsTfVars
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class SubscriptionsTfVarsTemplate : IntentTemplateBase<object>
    {
        private readonly List<InfrastructureRegisteredEvent> _receivedEvents = [];

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Terraform.SubscriptionsTfVarsTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public SubscriptionsTfVarsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(Handle);
        }

        private void Handle(InfrastructureRegisteredEvent @event)
        {
            _receivedEvents.Add(@event);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"terraform",
                fileExtension: "tfvars",
                relativeLocation: "terraform/02-subscriptions",
                OverwriteBehaviour.OverwriteDisabled
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace('.', '-').ToKebabCase();
            var sb = new StringBuilder(128);
            
            sb.AppendLine($@"resource_group_name = ""rg-{sanitizedAppName}""");
            sb.AppendLine($@"function_app_id = """"");
            
            foreach (var receivedEvent in _receivedEvents)
            {
                switch (receivedEvent.InfrastructureComponent)
                {
                    case Infrastructure.AzureEventGrid.Subscription:
                    {
                        var topicName = receivedEvent.Properties[Infrastructure.AzureEventGrid.Property.TopicName].ToPascalCase();
                        var varName = $"eventGridTopic{topicName}".ToSnakeCase();
                        sb.AppendLine($@"{varName}_id = """"");
                    }
                        break;
                }
            }
            
            return sb.ToString();
        }
    }
}