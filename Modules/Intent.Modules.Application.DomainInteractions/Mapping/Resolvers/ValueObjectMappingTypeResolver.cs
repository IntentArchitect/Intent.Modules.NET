using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

public class ValueObjectMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _sourceTemplate;

    public ValueObjectMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        var model = mappingModel.Model;
        if (model.TypeReference?.Element?.SpecializationType == "Value Object")
        {
            return new ConstructorMapping(mappingModel, _sourceTemplate);
        }

        return null;
    }
}