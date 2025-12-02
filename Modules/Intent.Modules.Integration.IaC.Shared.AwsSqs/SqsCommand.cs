using System;
using System.Collections.Generic;
using Intent.Aws.Sqs.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Integration.IaC.Shared.AwsSqs;

internal record SqsCommand : SqsItemBase
{
    public SqsCommand(
        string applicationId, 
        string applicationName, 
        IntegrationCommandModel commandModel, 
        SqsMethodType methodType)
    {
        ApplicationId = applicationId;
        ApplicationName = applicationName;
        CommandModel = commandModel;
        MethodType = methodType;
        QueueName = GetQueueName(commandModel);
        QueueConfigurationName = GetQueueConfigurationName(commandModel);
    }

    public IntegrationCommandModel CommandModel { get; init; }

    private static string GetQueueName(IntegrationCommandModel command)
    {
        // Check if AWS SQS stereotype is applied
        if (command.HasAWSSQS())
        {
            var stereotypeName = command.GetAWSSQS().QueueName();
            // Only use stereotype value if it's not empty
            if (!string.IsNullOrWhiteSpace(stereotypeName))
            {
                return stereotypeName;
            }
        }
        
        // Default naming convention: kebab-case, remove common suffixes
        var resolvedName = command.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationCommand", "Command");
        resolvedName = resolvedName.ToKebabCase();
        return resolvedName;
    }

    private static string GetQueueConfigurationName(IntegrationCommandModel command)
    {
        const string prefix = "AwsSqs:";
        
        if (command.HasAWSSQS())
        {
            var name = command.GetAWSSQS().QueueName();
            // Only use stereotype value if it's not empty
            if (!string.IsNullOrWhiteSpace(name))
            {
                return prefix + name.ToPascalCase() + ":QueueUrl";
            }
        }

        // Default: PascalCase name
        var resolvedName = command.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationCommand", "Command");
        resolvedName = resolvedName.ToPascalCase();
        resolvedName = resolvedName + ":QueueUrl";
        return prefix + resolvedName;
    }

    public override string GetModelTypeName(IntentTemplateBase template)
    {
        return template.GetTypeName("Intent.Eventing.Contracts.IntegrationCommand", CommandModel);
    }

    public override string GetSubscriberTypeName<T>(IntentTemplateBase<T> template)
    {
        return $"{template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventHandlerInterface")}<{GetModelTypeName(template)}>";
    }

    public override IEnumerable<IStereotype> Stereotypes => CommandModel.Stereotypes;

    public override string Name => CommandModel.Name;

    public override FolderModel Folder => CommandModel.Folder;

    public override IElement InternalElement => CommandModel.InternalElement;
}
