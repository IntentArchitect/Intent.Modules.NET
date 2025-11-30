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
    /// Filters a sequence of message models to only those applicable to the current message broker when a
    /// composite message bus is in use. If a composite bus is not required, the original sequence is returned
    /// unchanged.
    /// </summary>
    /// <typeparam name="T">The message model type which must implement <see cref="IHasStereotypes"/>.</typeparam>
    /// <param name="sequence">The sequence of message models to filter.</param>
    /// <param name="template">The calling template which provides access to the execution context.</param>
    /// <param name="brokerStereotypeNameOrIds">The set of stereotype names or IDs that indicate the target broker.</param>
    /// <returns>An enumerable of messages applicable to this broker.</returns>
    /// <exception cref="Exception">
    /// Thrown when a composite message bus is required and one or more messages have stereotypes but none match
    /// the provided broker stereotypes; in such cases each offending message must specify a message broker stereotype.
    /// </exception>
    public static IEnumerable<T> FilterMessagesForThisMessageBroker<T>(this IEnumerable<T> sequence,
        IIntentTemplate template, string[] brokerStereotypeNameOrIds)
        where T : IHasStereotypes
    {
        return FilterMessagesForThisMessageBroker(sequence, template.ExecutionContext, brokerStereotypeNameOrIds);
    }

    /// <summary>
    /// Filters a sequence of message models to only those applicable to the current message broker when a
    /// composite message bus is in use. If a composite bus is not required, the original sequence is returned
    /// unchanged.
    /// </summary>
    /// <typeparam name="T">The message model type which must implement <see cref="IHasStereotypes"/>.</typeparam>
    /// <param name="sequence">The sequence of message models to filter.</param>
    /// <param name="factoryExecutionContext">The software factory execution context.</param>
    /// <param name="brokerStereotypeNameOrIds">The set of stereotype names or IDs that indicate the target broker.</param>
    /// <returns>An enumerable of messages applicable to this broker.</returns>
    /// <exception cref="Exception">
    /// Thrown when a composite message bus is required and one or more messages have stereotypes but none match
    /// the provided broker stereotypes; in such cases each offending message must specify a message broker stereotype.
    /// </exception>
    public static IEnumerable<T> FilterMessagesForThisMessageBroker<T>(
        this IEnumerable<T> sequence,
        ISoftwareFactoryExecutionContext factoryExecutionContext,
        string[] brokerStereotypeNameOrIds)
        where T : IHasStereotypes
    {
        if (!factoryExecutionContext.RequiresCompositeMessageBus())
        {
            foreach (var element in sequence)
            {
                yield return element;
            }

            yield break;
        }

        var messagesNotSpecified = new List<T>();
        foreach (var element in sequence)
        {
            if (HasMatchingStereotype(element, brokerStereotypeNameOrIds))
            {
                yield return element;
            }
            else if (!HasKnownStereotype(element))
            {
                messagesNotSpecified.Add(element);
            }
        }

        if (messagesNotSpecified.Count == 0)
        {
            yield break;
        }
        
        var names = messagesNotSpecified
            .Select(x => x is IHasName named ? named.Name : x.ToString())
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
        // Make sure Message Extension Validation Script is updated too!
        string[] stereotypeIds = [
            "dca28d4b-c277-4fb3-afe0-17f35ea8b59b", // Azure Event Grid
            "84e5a563-953d-43f5-b2ca-a24ce3104e0b", // Azure Event Grid Folder Settings
            "b440c77b-3bde-4a96-bcb6-3289a23e5b1d", // Event Domain
            "7b57f640-600d-4b91-98a7-2a304c715f27", // Azure Queue Storage
            "a9fcfa58-3b1d-4126-8bdc-26d441f9880c", // Azure Queue Storage Folder Settings
            "32372bfd-d61a-4782-b542-427b0f6eb7b3", // Azure Queue Storage Package Settings
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