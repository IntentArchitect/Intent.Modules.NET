using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

public class CommandQueryMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CommandQueryMappingResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Model.SpecializationType == "Command" || mappingModel.Model.SpecializationType == "Query")
        {
            return new ConstructorMapping(mappingModel, _template);
        }
        //if (mappingModel.Model.TypeReference?.Element?.SpecializationType == "DTO")
        //{
        //    return new ObjectInitializationMapping(mappingModel, _template);
        //}
        //if (mappingModel.Model.TypeReference?.Element?.IsTypeDefinitionModel() == true
        //    || mappingModel.Model.TypeReference?.Element?.IsEnumModel() == true)
        //{
        //    return new TypeConvertingCSharpMapping(mappingModel, _template);
        //}
        return null;
    }
}