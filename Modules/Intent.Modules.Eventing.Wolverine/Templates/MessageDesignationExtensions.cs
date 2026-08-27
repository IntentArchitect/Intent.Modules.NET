using System.Collections.Generic;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;

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
}
