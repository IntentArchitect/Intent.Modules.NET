using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.DomainEvents.FactoryExtensions;

public class DomainEventMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _sourceTemplate;

    public DomainEventMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        var model = mappingModel.Model;
        if (model.SpecializationType == "Domain Event")
        {
            return new ConstructorMapping(mappingModel, _sourceTemplate);
        }

        if (model.TypeReference?.Element?.SpecializationType == "Value Object")
        {
            return new ConstructorMapping(mappingModel, _sourceTemplate);
        }

        return null;
    }
}