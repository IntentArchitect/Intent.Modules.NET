using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

namespace Intent.Modules.IaC.Bicep.Templates.AzureEventGridSubscriptionsBicep
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AzureEventGridSubscriptionsBicepTemplate : IntentTemplateBase<object>
    {
        private readonly List<EventGridSubscription> _eventGridSubscriptions = [];
        private readonly List<AppSettingRegistrationRequest> _appSettingsRequests = [];
        private bool _beforeTemplateExecutionCalled;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IaC.Bicep.AzureEventGridSubscriptionsBicepTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AzureEventGridSubscriptionsBicepTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<AppSettingRegistrationRequest>(Handle);
            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(Handle);
        }

        private void Handle(InfrastructureRegisteredEvent @event)
        {
            if (@event.InfrastructureComponent is not Infrastructure.AzureEventGrid.Subscription)
            {
                return;
            }

            _eventGridSubscriptions.Add(new EventGridSubscription(@event));
        }

        private void Handle(AppSettingRegistrationRequest request)
        {
            _appSettingsRequests.Add(request);
        }

        public override bool CanRunTemplate()
        {
            return !_beforeTemplateExecutionCalled ||
                   _eventGridSubscriptions.Count > 0;
        }

        public override void BeforeTemplateExecution()
        {
            _beforeTemplateExecutionCalled = true;
            base.BeforeTemplateExecution();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"event_grid_subscriptions",
                fileExtension: "bicep"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sanitizedAppName = ExecutionContext.GetApplicationConfig().Name.Replace('.', '-').ToKebabCase();
            const string functionAppParam = "functionAppName";
            const string eventGridResourceName = "functionAppEventGrid";

            var sb = new StringBuilder();
            sb.AppendLine($"param {functionAppParam} string = '{sanitizedAppName}-${{uniqueString(resourceGroup().id)}}'");
            sb.AppendLine();

            sb.AppendLine(new BicepResource(eventGridResourceName, "'Microsoft.Web/sites@2021-02-01'")
                .WithExisting(true)
                .Set("name", functionAppParam)
                .Build());

            var subscriptions = _eventGridSubscriptions.OrderBy(x => x.TopicName).ToArray();

            foreach (var sub in subscriptions)
            {
                sb.AppendLine(new BicepResource(sub.TopicResourceName, "'Microsoft.EventGrid/topics@2021-12-01'")
                    .WithExisting(true)
                    .Set("name", $"'{sub.TopicName}'")
                    .Build());
            }

            foreach (var sub in subscriptions)
            {
                sb.AppendLine(new BicepResource(sub.SubscriptionResourceName, "'Microsoft.EventGrid/eventSubscriptions@2021-12-01'")
                    .Set("name", $"'{sub.TopicName}-eg-sub'")
                    .Set("scope", sub.TopicResourceName)
                    .Block("properties", props =>
                    {
                        props.Block("destination", dest =>
                        {
                            dest.Set("endpointType", "'AzureFunction'");
                            dest.Block("properties", destProps =>
                            {
                                destProps.Set("resourceId", $"'${{{eventGridResourceName}.id}}/functions/{sub.HandlerFunctionName}'");
                            });
                        });
                        props.Block("filter", filter =>
                        {
                            filter.Array("includedEventTypes", types =>
                            {
                                foreach (var fullyQualifiedEventName in sub.FullyQualifiedEventNames.Split(";").Order())
                                {
                                    types.ScalarValue(fullyQualifiedEventName);
                                }
                            });
                        });
                    })
                    .Build());
            }

            return sb.ToString();
        }
    }
}