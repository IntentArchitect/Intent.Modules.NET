using Intent.Eventing.AzureQueueStorage.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Eventing.AzureQueueStorage;

public static class HelperExtensions
{
    // public static List<string> GetSubscribeQueues(this IIntentTemplate template)
    // {
    //     var messageQueues = template.ExecutionContext.MetadataManager
    //             .GetExplicitlySubscribedToMessageModels(template.OutputTarget.Application)
    //             .Select(GetMessageQueue);
    //
    //     var eventingMessageQueues = template.ExecutionContext.MetadataManager
    //         .Eventing(template.ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
    //         .SelectMany(x => x.SubscribedMessages())
    //         .Select(x => x.TypeReference.Element.AsMessageModel())
    //         .Select(GetMessageQueue);
    //
    //     var commandQueues = template.ExecutionContext.MetadataManager
    //             .GetExplicitlySubscribedToIntegrationCommandModels(template.OutputTarget.Application)
    //             .Select(GetIntegrationCommandQueue);
    //
    //     return Enumerable.Empty<string>()
    //         .Concat(messageQueues)
    //         .Concat(eventingMessageQueues)
    //         .Concat(commandQueues)
    //         .OrderBy(x => x)
    //         .Distinct()
    //         .ToList();
    // }
    
    public static List<MessageModel> GetSubscribedMessages(this IIntentTemplate template)
    {
        var serviceDesignerSubscribeMessages = template.ExecutionContext.MetadataManager
                .GetExplicitlySubscribedToMessageModels(template.OutputTarget.Application);

        var eventingDesignerSubscribedMessages = template.ExecutionContext.MetadataManager
            .Eventing(template.ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
            .SelectMany(x => x.SubscribedMessages())
            .Select(x => x.TypeReference.Element.AsMessageModel());

        var messageModels = Enumerable.Empty<MessageModel>()
            .Concat(serviceDesignerSubscribeMessages)
            .Concat(eventingDesignerSubscribedMessages)
            .OrderBy(x => x.Name)
            .Distinct()
            .ToList();

        return messageModels;
    }

    public static List<IntegrationCommandModel> GetSubscribedIntegrationCommands(this IIntentTemplate template)
    {
        var serviceDesignerSubscribeCommands = template.ExecutionContext.MetadataManager
                .GetExplicitlySubscribedToIntegrationCommandModels(template.OutputTarget.Application);

        var commandModels = Enumerable.Empty<IntegrationCommandModel>()
            .Concat(serviceDesignerSubscribeCommands)
            .OrderBy(x => x.Name)
            .Distinct()
            .ToList();

        return commandModels;
    }

    public static List<MessageModel> GetPublishedMessages(this IIntentTemplate template)
    {
        var serviceDesignerPublishedMessages = template.ExecutionContext.MetadataManager
                .GetExplicitlyPublishedMessageModels(template.OutputTarget.Application);

        var eventingDesignerPublishedMessages = template.ExecutionContext.MetadataManager
            .Eventing(template.ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
            .SelectMany(x => x.PublishedMessages())
            .Select(x => x.TypeReference.Element.AsMessageModel());

        var messageModels = Enumerable.Empty<MessageModel>()
            .Concat(serviceDesignerPublishedMessages)
            .Concat(eventingDesignerPublishedMessages)
            .OrderBy(x => x.Name)
            .Distinct()
            .ToList();

        return messageModels;
    }

    public static List<IntegrationCommandModel> GetSentIntegrationCommands(this IIntentTemplate template)
    {
        var serviceDesignerPublishedCommands = template.ExecutionContext.MetadataManager
                .GetExplicitlySentIntegrationCommandModels(template.OutputTarget.Application);

        var commandModels = Enumerable.Empty<IntegrationCommandModel>()
            .Concat(serviceDesignerPublishedCommands)
            .OrderBy(x => x.Name)
            .Distinct()
            .ToList();

        return commandModels;
    }

    public static int GetSubscribedMessageCount(this IIntentTemplate template)
    {
        return
            template.ExecutionContext.MetadataManager
                .GetExplicitlySubscribedToMessageModels(template.OutputTarget.Application)
                .Count +
            template.ExecutionContext.MetadataManager
                .Eventing(template.ExecutionContext.GetApplicationConfig().Id)
                .GetApplicationModels().SelectMany(x => x.SubscribedMessages())
                .Select(x => x.TypeReference.Element.AsMessageModel()).Count() +
            template.ExecutionContext.MetadataManager
                    .GetExplicitlySubscribedToIntegrationCommandModels(template.OutputTarget.Application).Count;
    }

    public static string GetMessageQueue(MessageModel messageModel)
    {
        var stack = new Stack<string>();
        var element = messageModel.InternalElement;

        if (messageModel.HasAzureQueueStorage() && !string.IsNullOrWhiteSpace(messageModel.GetAzureQueueStorage().QueueName()))
        {
            return messageModel.GetAzureQueueStorage().QueueName();
        }

        while (true)
        {
            stack.Push(element.Name.ToLower());

            if (element.ParentElement == null)
            {
                stack.Push(element.Package.Name.ToLower());
                break;
            }

            element = element.ParentElement;
        }

        return string.Join('-', stack).ToLower().Replace('.', '-');
    }

    public static string GetIntegrationCommandQueue(IntegrationCommandModel commandMode)
    {
        var stack = new Stack<string>();
        var element = commandMode.InternalElement;

        if (commandMode.HasAzureQueueStorage() && !string.IsNullOrWhiteSpace(commandMode.GetAzureQueueStorage().QueueName()))
        {
            return commandMode.GetAzureQueueStorage().QueueName();
        }

        while (true)
        {
            stack.Push(element.Name.ToLower());

            if (element.ParentElement == null)
            {
                stack.Push(element.Package.Name.ToLower());
                break;
            }

            element = element.ParentElement;
        }

        return string.Join('-', stack).ToLower().Replace('.', '-');
    }
}
