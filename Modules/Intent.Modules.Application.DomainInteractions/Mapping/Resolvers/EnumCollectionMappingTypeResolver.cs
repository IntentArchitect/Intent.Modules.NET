using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
public class EnumCollectionMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _sourceTemplate;
    public EnumCollectionMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if ((mappingModel.Mapping?.SourceElement?.TypeReference?.IsCollection ?? false) && (mappingModel.Mapping?.TargetElement?.TypeReference?.IsCollection ?? false) &&
            (mappingModel.Mapping?.SourceElement?.TypeReference?.Element?.SpecializationType == "Enum") && (mappingModel.Mapping?.TargetElement?.TypeReference?.Element?.SpecializationType == "Enum"))
        {
            return new ToListMapping(mappingModel, _sourceTemplate);
        }

        return null;
    }
}
