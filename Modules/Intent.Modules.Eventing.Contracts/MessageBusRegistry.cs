using System.Collections.Generic;
using System.Linq;
using Intent.Eventing.Contracts.Api;

namespace Intent.Modules.Eventing.Contracts;

public static class MessageBusRegistry
{
    private static readonly Dictionary<string, string[]> _messageBusImplementations = new();
    private static int? _implementationCountCache;
    private static List<string>? _stereotypeDefinitionIdSet;

    public static void Register(string messageBusId, string[] messageBusStereotypeDefinitionIds)
    {
        if (_messageBusImplementations.TryAdd(messageBusId, messageBusStereotypeDefinitionIds))
        {
            _implementationCountCache = null;
            _stereotypeDefinitionIdSet = null;
        }
    }
    
    internal static bool HasMultipleMessageBusImplementations()
    {
        _implementationCountCache ??= _messageBusImplementations.Values.Count;
        return _implementationCountCache > 1;
    }
    
    internal static IReadOnlyList<string> GetAllMessageBusStereotypeDefinitionIds()
    {
        if (_stereotypeDefinitionIdSet == null)
        {
            var allStereotypeIds = new List<string>();
            foreach (var id in _messageBusImplementations.Values.SelectMany(stereotypeIds => stereotypeIds))
            {
                allStereotypeIds.Add(id);
            }

            _stereotypeDefinitionIdSet = allStereotypeIds;
        }
        
        return _stereotypeDefinitionIdSet;
    }

    internal static bool IsBrokerStereotypesMatch(string messageBusId, IReadOnlyList<string> brokerStereotypeNameOrIds)
    {
        if (!_messageBusImplementations.TryGetValue(messageBusId, out var registeredStereotypeIds))
        {
            return false;
        }

        return brokerStereotypeNameOrIds.Any(x => registeredStereotypeIds.Contains(x));
    }
}