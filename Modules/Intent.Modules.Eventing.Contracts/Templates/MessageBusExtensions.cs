using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Eventing.Contracts.Settings;
using Intent.Modules.Eventing.Contracts.Templates.CompositeMessageBusConfiguration;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.MessageBusInterface;
using Intent.Utils;

namespace Intent.Modules.Eventing.Contracts.Templates;

public static class MessageBusExtensions
{
    /// <summary>
    /// Returns the appropriate bus interface name based on the UseLegacyInterfaceName setting.
    /// If true, returns IEventBus; if false, returns IMessageBus.
    /// </summary>
    public static string GetBusInterfaceName(this IIntentTemplate template)
    {
        var useLegacy = template.ExecutionContext.Settings.GetEventingSettings().UseLegacyInterfaceName();
        return useLegacy
            ? template.GetTypeName(EventBusInterfaceTemplate.TemplateId)
            : template.GetTypeName(MessageBusInterfaceTemplate.TemplateId);
    }

    /// <summary>
    /// Returns the appropriate bus interface template ID based on the UseLegacyInterfaceName setting.
    /// If true, returns EventBusInterface template ID; if false, returns MessageBusInterface template ID.
    /// </summary>
    public static string GetBusInterfaceTemplateId(this ISoftwareFactoryExecutionContext factoryExecutionContext)
    {
        var useLegacy = factoryExecutionContext.Settings.GetEventingSettings().UseLegacyInterfaceName();
        return useLegacy
            ? EventBusInterfaceTemplate.TemplateId
            : MessageBusInterfaceTemplate.TemplateId;
    }

    public static string GetBusInterfaceTemplateId(this IIntentTemplate template)
    {
        return GetBusInterfaceTemplateId(template.ExecutionContext);
    }

    /// <summary>
    /// Returns a suggested variable name for the bus instance based on the UseLegacyInterfaceName setting.
    /// </summary>
    /// <param name="factoryExecutionContext">The software factory execution context used to read settings.</param>
    /// <returns>
    /// <c>"eventBus"</c> when legacy interface naming is enabled; otherwise <c>"messageBus"</c>.
    /// </returns>
    public static string GetBusVariableName(this ISoftwareFactoryExecutionContext factoryExecutionContext)
    {
        var useLegacy = factoryExecutionContext.Settings.GetEventingSettings().UseLegacyInterfaceName();
        return useLegacy ? "eventBus" : "messageBus";
    }

    /// <summary>
    /// Returns a suggested variable name for the bus instance based on the UseLegacyInterfaceName setting.
    /// </summary>
    /// <param name="template">The calling template which provides access to the execution context.</param>
    /// <returns>
    /// <c>"eventBus"</c> when legacy interface naming is enabled; otherwise <c>"messageBus"</c>.
    /// </returns>
    public static string GetBusVariableName(this IIntentTemplate template)
    {
        return GetBusVariableName(template.ExecutionContext);
    }

    /// <summary>
    /// Determines whether the current execution context requires generating a composite message bus.
    /// </summary>
    /// <param name="factoryExecutionContext">The software factory execution context.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="CompositeMessageBusConfigurationTemplate"/> can run (i.e. multiple
    /// message bus implementations are present); otherwise <c>false</c>.
    /// </returns>
    public static bool RequiresCompositeMessageBus(this ISoftwareFactoryExecutionContext factoryExecutionContext)
    {
        var compositeTemplate = factoryExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(
            TemplateDependency.OnTemplate(CompositeMessageBusConfigurationTemplate.TemplateId));
        return compositeTemplate?.CanRunTemplate() == true;
    }

    /// <summary>
    /// Determines whether the current template's execution context requires generating a composite message bus.
    /// </summary>
    /// <param name="template">The calling template which provides access to the execution context.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="CompositeMessageBusConfigurationTemplate"/> can run; otherwise <c>false</c>.
    /// </returns>
    public static bool RequiresCompositeMessageBus(this IIntentTemplate template)
    {
        return RequiresCompositeMessageBus(template.ExecutionContext);
    }
    
    /// <summary>
    /// Filters a sequence of messages to include only those associated with the specified message broker(s).
    /// When a composite message bus is not required, all messages are returned. When a composite message bus
    /// is required, only messages with matching broker stereotypes (on the message, its folder hierarchy, or package)
    /// are included. Messages without any known broker stereotype will cause a failure.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to filter, which must implement <see cref="IHasStereotypes"/>.</typeparam>
    /// <param name="sequence">The sequence of messages to filter.</param>
    /// <param name="template">The calling template which provides access to the execution context.</param>
    /// <param name="brokerStereotypeNameOrIds">An array of broker stereotype names or IDs to match against.</param>
    /// <returns>
    /// A filtered sequence containing only messages associated with the specified broker(s).
    /// If no composite message bus is required, returns all messages unchanged.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="template"/> or <paramref name="brokerStereotypeNameOrIds"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when a composite message bus is required and one or more messages lack any known broker stereotype.
    /// </exception>
    public static IEnumerable<TMessage> FilterMessagesForThisMessageBroker<TMessage>(
        this IEnumerable<TMessage> sequence,
        IIntentTemplate template,
        string[] brokerStereotypeNameOrIds)
        where TMessage : IHasStereotypes
    {
        return FilterMessagesForThisMessageBroker(
            sequence,
            template.ExecutionContext,
            brokerStereotypeNameOrIds,
            element => element);
    }

    /// <summary>
    /// Filters a sequence of messages to include only those associated with the specified message broker(s).
    /// When a composite message bus is not required, all messages are returned. When a composite message bus
    /// is required, only messages with matching broker stereotypes (on the message, its folder hierarchy, or package)
    /// are included. Messages without any known broker stereotype will cause a failure.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to filter, which must implement <see cref="IHasStereotypes"/>.</typeparam>
    /// <param name="sequence">The sequence of messages to filter.</param>
    /// <param name="factoryExecutionContext">The software factory execution context.</param>
    /// <param name="brokerStereotypeNameOrIds">An array of broker stereotype names or IDs to match against.</param>
    /// <returns>
    /// A filtered sequence containing only messages associated with the specified broker(s).
    /// If no composite message bus is required, returns all messages unchanged.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="factoryExecutionContext"/> or <paramref name="brokerStereotypeNameOrIds"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when a composite message bus is required and one or more messages lack any known broker stereotype.
    /// </exception>
    public static IEnumerable<TMessage> FilterMessagesForThisMessageBroker<TMessage>(
        this IEnumerable<TMessage> sequence,
        ISoftwareFactoryExecutionContext factoryExecutionContext,
        string[] brokerStereotypeNameOrIds)
        where TMessage : IHasStereotypes
    {
        return FilterMessagesForThisMessageBroker(
            sequence,
            factoryExecutionContext,
            brokerStereotypeNameOrIds,
            element => element);
    }
    
    /// <summary>
    /// Filters a sequence of messages to include only those associated with the specified message broker(s),
    /// using a selector function to access the element containing the broker stereotypes.
    /// When a composite message bus is not required, all messages are returned. When a composite message bus
    /// is required, only messages whose selected element has matching broker stereotypes (on the element itself,
    /// its folder hierarchy, or package) are included. Messages without any known broker stereotype will cause a failure.
    /// </summary>
    /// <typeparam name="TMessage">The type of message in the sequence.</typeparam>
    /// <typeparam name="TElementWithStereotype">
    /// The type of element containing stereotypes, which must implement <see cref="IHasStereotypes"/>.
    /// </typeparam>
    /// <param name="sequence">The sequence of messages to filter.</param>
    /// <param name="factoryExecutionContext">The software factory execution context.</param>
    /// <param name="brokerStereotypeNameOrIds">An array of broker stereotype names or IDs to match against.</param>
    /// <param name="selector">
    /// A function that extracts the element containing stereotypes from each message. This element is used
    /// for both stereotype checking and name extraction in error messages.
    /// </param>
    /// <returns>
    /// A filtered sequence containing only messages associated with the specified broker(s).
    /// If no composite message bus is required, returns all messages unchanged.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="factoryExecutionContext"/>, <paramref name="brokerStereotypeNameOrIds"/>,
    /// or <paramref name="selector"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when a composite message bus is required and one or more messages lack any known broker stereotype.
    /// </exception>
    public static IEnumerable<TMessage> FilterMessagesForThisMessageBroker<TMessage, TElementWithStereotype>(
        this IEnumerable<TMessage> sequence,
        ISoftwareFactoryExecutionContext factoryExecutionContext,
        string[] brokerStereotypeNameOrIds,
        Func<TMessage, TElementWithStereotype> selector)
        where TElementWithStereotype : IHasStereotypes
    {
        ArgumentNullException.ThrowIfNull(factoryExecutionContext);
        ArgumentNullException.ThrowIfNull(brokerStereotypeNameOrIds);
        ArgumentNullException.ThrowIfNull(selector);

        if (!factoryExecutionContext.RequiresCompositeMessageBus())
        {
            foreach (var element in sequence)
            {
                yield return element;
            }
            yield break;
        }

        var messagesNotSpecified = new List<TMessage>();
        foreach (var element in sequence)
        {
            var selectedElement = selector(element);
            if (HasMatchingStereotype(selectedElement, brokerStereotypeNameOrIds))
            {
                yield return element;
            }
            else if (!HasKnownStereotype(selectedElement))
            {
                messagesNotSpecified.Add(element);
            }
        }

        if (messagesNotSpecified.Count == 0)
        {
            yield break;
        }

        var names = messagesNotSpecified
            .Select(x => selector(x) is IHasName named ? named.Name : x.ToString())
            .ToList();

        var message =
            $"The following message models must specify a message broker stereotype when multiple message bus implementations are present: {string.Join(", ", names)}";

        Logging.Log.Failure(message);
        throw new Exception(message);
    }

    // Until we have a better way to do this, we're hardcoding the message broker stereotypes we know about.
    private static bool HasKnownStereotype<T>(T element)
        where T : IHasStereotypes
    {
        // Make sure Message and Command Extension Validation Scripts are updated too!
        string[] stereotypeIds = [
            "dca28d4b-c277-4fb3-afe0-17f35ea8b59b", // Azure Event Grid
            "84e5a563-953d-43f5-b2ca-a24ce3104e0b", // Azure Event Grid Folder Settings
            "b440c77b-3bde-4a96-bcb6-3289a23e5b1d", // Event Domain
            "7b57f640-600d-4b91-98a7-2a304c715f27", // Azure Queue Storage
            "a9fcfa58-3b1d-4126-8bdc-26d441f9880c", // Azure Queue Storage Folder Settings
            "32372bfd-d61a-4782-b542-427b0f6eb7b3", // Azure Queue Storage Package Settings
            "1f60bd15-005b-4184-8c12-c44c20158001", // Azure Service Bus
            "706576a2-ddb3-4ccd-9b13-ee58147b0e6b", // Azure Service Bus Folder Settings
            "7c7514b5-f17f-4c91-aade-c5b5b0634281", // Azure Service Bus Package Settings
            "74fbdee0-4098-4544-8ecf-f7c5787c78c3", // Aws Sqs
            "721c45f1-add5-4143-8ef7-c6f54db6e06d", // Aws Sqs Folder Settings
            "6ffbabf9-435f-442b-866a-37c4a83bb770", // Aws Sqs Package Settings
            "762ba7ac-4039-4a56-8f8d-e0ecd59038c5", // Kafka Folder Settings 
            "f18ed242-c439-4b46-834c-bc2947731486", // Kafka Message Settings
            "a08a5b95-24a9-46dd-b52a-5faed1db9955", // Kafka Package Settings
            "32eb7ec0-1c6f-42ae-ab3f-4a71d8882ad5", // MassTransit Folder Settings
            "fbe53252-9913-453c-b734-73b4e2dfdb46", // MassTransit Message Settings
            "f997aa34-0a9a-4733-b5a9-fdcf9e37170c", // MassTransit Package Settings
            "56e898f3-74db-486d-86f9-3e885e7509e6", // Solace Publish
            "d0dc5fff-184e-421d-951f-036c0e250bec", // Solace Folder Settings
            "1edcb13a-0108-42a3-b9d0-dea95dc655b8", // Solace Package Settings
        ];
        return HasMatchingStereotype(element, stereotypeIds);
    }

    /// <summary>
    /// Determines whether the given element or its container hierarchy has any of the specified broker stereotypes.
    /// </summary>
    /// <typeparam name="T">The element type which must implement <see cref="IHasStereotypes"/>.</typeparam>
    /// <param name="element">The element to inspect.</param>
    /// <param name="brokerStereotypeNameOrIds">The set of stereotype names or IDs to match.</param>
    /// <returns>
    /// <c>true</c> if the element, any of its ancestor folders, or its package has a matching stereotype; otherwise <c>false</c>.
    /// </returns>
    private static bool HasMatchingStereotype<T>(T element, string[] brokerStereotypeNameOrIds)
        where T : IHasStereotypes
    {
        // Check element itself
        if (brokerStereotypeNameOrIds.Any(x => element.HasStereotype(x)))
        {
            return true;
        }

        // Check folder hierarchy
        if (element is IHasFolder hasFolder)
        {
            var currentFolder = hasFolder.Folder;
            while (currentFolder != null)
            {
                if (brokerStereotypeNameOrIds.Any(x => currentFolder.HasStereotype(x)))
                {
                    return true;
                }

                currentFolder = currentFolder.Folder;
            }
        }

        // Check package level
        if (element is not IElementWrapper wrapper)
        {
            return false;
        }
        
        var package = wrapper.InternalElement.Package;
        return brokerStereotypeNameOrIds.Any(x => package.HasStereotype(x));
    }
}