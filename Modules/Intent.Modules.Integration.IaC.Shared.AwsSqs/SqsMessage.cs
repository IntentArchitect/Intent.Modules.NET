using System;
using System.Collections.Generic;
using Intent.Aws.Sqs.Api;
using Intent.Metadata.Models; // Will be created in Phase 2
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

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
        if (message.HasAWSSQS())
        {
            var stereotypeName = message.GetAWSSQS().QueueName();
            // Only use stereotype value if it's not empty
            if (!string.IsNullOrWhiteSpace(stereotypeName))
            {
                return stereotypeName;
            }
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
        
        if (message.HasAWSSQS())
        {
            var name = message.GetAWSSQS().QueueName();
            // Only use stereotype value if it's not empty
            if (!string.IsNullOrWhiteSpace(name))
            {
                return prefix + name.ToPascalCase() + ":QueueUrl";
            }
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

    public override IEnumerable<IStereotype> Stereotypes => MessageModel.Stereotypes;

    public override string Name => MessageModel.Name;

    public override FolderModel Folder => MessageModel.Folder;

    public override IElement InternalElement => MessageModel.InternalElement;
}
