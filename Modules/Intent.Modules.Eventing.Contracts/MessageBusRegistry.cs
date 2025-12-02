using System.Collections.Generic;
using System.Linq;
using Intent.Eventing.Contracts.Api;

namespace Intent.Modules.Eventing.Contracts;

public static class MessageBusRegistry
{
    private static readonly Dictionary<string, string[]> _messageBusImplementations = new();
    private static int? _implementationCountCache;
    private static List<string>? _stereotypeDefinitionIdSet;

    public static void Register(string messageBusName, string[] messageBusStereotypeDefinitionIds)
    {
        if (_messageBusImplementations.TryAdd(messageBusName, messageBusStereotypeDefinitionIds))
        {
            _implementationCountCache = null;
            _stereotypeDefinitionIdSet = null;
        }
    }
    
    public static bool HasMultipleMessageBusImplementations()
    {
        _implementationCountCache ??= _messageBusImplementations.Values.Count;
        return _implementationCountCache > 1;
    }
    
    public static IReadOnlyList<string> GetAllMessageBusStereotypeDefinitionIds()
    {
        if (_stereotypeDefinitionIdSet == null)
        {
            var allStereotypeIds = new List<string>
            {
                FolderModelStereotypeExtensions.MessageBus.DefinitionId
            };
            foreach (var id in _messageBusImplementations.Values.SelectMany(stereotypeIds => stereotypeIds))
            {
                allStereotypeIds.Add(id);
            }

            _stereotypeDefinitionIdSet = allStereotypeIds;
        }
        
        return _stereotypeDefinitionIdSet;
    }
}