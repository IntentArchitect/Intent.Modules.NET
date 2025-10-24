using System;
using Intent.Aws.Sqs.Api;  // Will be created in Phase 2
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Integration.IaC.Shared.AwsSqs;

internal record SqsMessage : SqsItemBase
{
    public SqsMessage(
        string applicationId, 
        string applicationName, 
        MessageModel messageModel, 
        SqsMethodType methodType)
    {
        ApplicationId = applicationId;
        ApplicationName = applicationName;
        MessageModel = messageModel;
        MethodType = methodType;
        QueueName = GetQueueName(messageModel);
        QueueConfigurationName = GetQueueConfigurationName(messageModel);
    }

    public MessageModel MessageModel { get; init; }

    private static string GetQueueName(MessageModel message)
    {
        // Check if AWS SQS stereotype is applied
        if (message.HasAwsSqs())
        {
            return message.GetAwsSqs().QueueName();
        }
        
        // Default naming convention: kebab-case, remove common suffixes
        var resolvedName = message.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationEvent", "Event", "Message");
        resolvedName = resolvedName.ToKebabCase();
        return resolvedName;
    }

    private static string GetQueueConfigurationName(MessageModel message)
    {
        const string prefix = "AwsSqs:";
        
        if (message.HasAwsSqs())
        {
            var name = message.GetAwsSqs().QueueName();
            return prefix + name.ToPascalCase();
        }

        // Default: PascalCase name
        var resolvedName = message.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationEvent", "Event", "Message");
        resolvedName = resolvedName.ToPascalCase();
        resolvedName = resolvedName + ":QueueUrl";
        return prefix + resolvedName;
    }

    public override string GetModelTypeName(IntentTemplateBase template)
    {
        return template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage", MessageModel);
    }

    public override string GetSubscriberTypeName<T>(IntentTemplateBase<T> template)
    {
        return $"{template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventHandlerInterface")}<{GetModelTypeName(template)}>";
    }
}
