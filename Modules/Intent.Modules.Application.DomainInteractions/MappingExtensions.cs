using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Application.DomainInteractions;

public static class MappingExtensions
{
    public static IElementToElementMapping GetQueryEntityMapping(this IEnumerable<IElementToElementMapping> mappings)
    {
        return mappings.SingleOrDefault(x => x.Type == "Query Entity Mapping");
    }

    public static IElementToElementMapping GetUpdateEntityMapping(this IEnumerable<IElementToElementMapping> mappings)
    {
        return mappings.SingleOrDefault(x => x.Type == "Update Entity Mapping");
    }

    public static bool IsQueryEntityMapping(this IElementToElementMapping mapping)
    {
        return mapping.Type == "Query Entity Mapping";
    }
}