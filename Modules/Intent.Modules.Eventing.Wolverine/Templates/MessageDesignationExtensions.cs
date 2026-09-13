using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineMessageBus;

namespace Intent.Modules.Eventing.Wolverine.Templates;

public static class MessageDesignationExtensions
{
    public static IEnumerable<MessageModel> GetWolverineDesignatedMessages(this IIntentTemplate template, IEnumerable<MessageModel> messages)
    {
        return messages.FilterMessagesForThisMessageBroker(template, Constants.BrokerStereotypeIds);
    }

    public static IEnumerable<IntegrationCommandModel> GetWolverineDesignatedIntegrationCommands(this IIntentTemplate template, IEnumerable<IntegrationCommandModel> commands)
    {
        return commands.FilterMessagesForThisMessageBroker(template, Constants.BrokerStereotypeIds);
    }

    /// <summary>
    /// Mirrors WolverineEventingRegistrationExtension.RegisterWolverineMessageBus exactly: the
    /// concrete WolverineMessageBus is only in the container when this application has at least one
    /// Wolverine-designated published Integration Event or sent Integration Command (R3.9) - a
    /// subscribe-only application gets no registration at all, standalone OR composite - EXCEPT a
    /// Composite Message Bus app, which registers it unconditionally
    /// (WolverineCompositeConfigurationTemplatePartial.cs's "services.AddScoped&lt;WolverineMessageBus&gt;()").
    /// A consumer that injects WolverineMessageBus without checking this throws
    /// "No service for type ... has been registered" the first time a subscribe-only app receives a
    /// message - not caught at compile time, only by actually running the app.
    /// </summary>
    public static bool WolverineMessageBusIsRegistered(this ICSharpFileBuilderTemplate template)
    {
        var busTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>(WolverineMessageBusTemplate.TemplateId,
            new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false });
        if (busTemplate == null)
        {
            return false;
        }

        if (busTemplate.RequiresCompositeMessageBus())
        {
            return true;
        }

        var application = template.OutputTarget.Application;
        var hasPublishedMessages = template.GetWolverineDesignatedMessages(
            template.ExecutionContext.MetadataManager.GetExplicitlyPublishedMessageModels(application)).Any();
        var hasSentCommands = template.GetWolverineDesignatedIntegrationCommands(
            template.ExecutionContext.MetadataManager.GetExplicitlySentIntegrationCommandModels(application)).Any();

        return hasPublishedMessages || hasSentCommands;
    }
}
