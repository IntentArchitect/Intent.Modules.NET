using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.IaC.Terraform.Templates.SubscriptionsTf;

internal class EventGridTerraformExtension
{
    private readonly List<EventGridSubscription> _subscriptions = [];
    private bool _isOrdered;
    
    private record EventGridSubscription(string TopicName, string[] FullyQualifiedEventNames, string HandlerFunctionName);
    
    public bool HasSubscriptions()
    {
        return _subscriptions.Count != 0;
    }

    public void ProcessEvent(InfrastructureRegisteredEvent @event)
    {
        if (@event.InfrastructureComponent is not Infrastructure.AzureEventGrid.Subscription)
        {
            return;
        }

        _subscriptions.Add(new EventGridSubscription(
            TopicName: @event.Properties[Infrastructure.AzureEventGrid.Property.TopicName].ToPascalCase(),
            FullyQualifiedEventNames: @event.Properties[Infrastructure.AzureEventGrid.Property.MessageNames].Split(';', StringSplitOptions.RemoveEmptyEntries),
            HandlerFunctionName: @event.Properties[Infrastructure.AzureEventGrid.Property.HandlerFunctionName]));
    }

    public void ApplyVariables(TerraformFileBuilder builder)
    {
        OrderSubscriptionsOnceOff();
        foreach (var subscription in _subscriptions)
        {
            var varName = $"eventGridTopic{subscription.TopicName}".ToSnakeCase();
            builder.AddVariable($"{varName}_id", v => v
                .AddSetting("description", $"The ID of {subscription.TopicName.ToSentenceCase()}")
                .AddRawSetting("type", "string"));
        }
    }

    public void ApplySubscriptions(TerraformFileBuilder builder)
    {
        OrderSubscriptionsOnceOff();
        builder.AddComment("Event Grid Subscriptions");

        foreach (var subscription in _subscriptions)
        {
            var varName = $"eventGridTopic{subscription.TopicName}".ToSnakeCase();
            builder.AddResource("azurerm_eventgrid_event_subscription", $"{varName}_subscription", resource => resource
                .AddSetting("name", $"{varName.ToKebabCase()}-sub")
                .AddRawSetting("scope", $"var.{varName}_id")
                .AddBlock("azure_function_endpoint", endpoint => endpoint
                    .AddSetting("function_id", $"${{var.function_app_id}}/functions/{subscription.HandlerFunctionName}")
                    .AddSetting("max_events_per_batch", 1)
                    .AddSetting("preferred_batch_size_in_kilobytes", 64))
                .AddSetting("included_event_types", subscription.FullyQualifiedEventNames));
        }
    }

    private void OrderSubscriptionsOnceOff()
    {
        if (_isOrdered)
        {
            return;
        }
        _subscriptions.Sort((x, y) => string.Compare(x.TopicName, y.TopicName, StringComparison.Ordinal));
        _isOrdered = true;
    }
}