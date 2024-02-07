using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Eventing.MappingTypeResolvers;

public class MessageCreationMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public MessageCreationMappingTypeResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        //if (mappingModel.MappingTypeId != "5f172141-fdba-426b-980e-163e782ff53e") // Command to Class Creation Mapping
        //{
        //    return null;
        //}

        var model = mappingModel.Model;
        if (model.IsMessageModel() || model.IsIntegrationCommandModel() || model.TypeReference?.Element?.IsEventingDTOModel() == true)
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }
        if (mappingModel.Model.TypeReference?.Element?.IsTypeDefinitionModel() == true
            || mappingModel.Model.TypeReference?.Element?.IsEnumModel() == true)
        {
            return new TypeConvertingCSharpMapping(mappingModel, _template);
        }
        return null;
    }
}