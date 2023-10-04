using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modelers.Services.DomainInteractions.Api;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

public class EntityCreationMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _sourceTemplate;

    public EntityCreationMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        var model = mappingModel.Model;
        if (mappingModel.MappingTypeId != "5f172141-fdba-426b-980e-163e782ff53e") // Command to Class Creation Mapping
        {
            return null;
        }

        if (model.SpecializationType == "Class" || model.TypeReference?.Element?.SpecializationType == "Class")
        {
            return new ObjectInitializationMapping(mappingModel, _sourceTemplate);
        }

        return null;
    }
}