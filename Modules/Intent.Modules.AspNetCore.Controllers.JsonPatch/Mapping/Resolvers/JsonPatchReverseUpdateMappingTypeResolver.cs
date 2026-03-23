using System.Linq;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.Mapping.Resolvers;

internal class JsonPatchReverseUpdateMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpTemplate _template;

    public JsonPatchReverseUpdateMappingTypeResolver(ICSharpTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.MappingTypeId != "01721b1a-a85d-4320-a5cd-8bd39247196a" && mappingModel.MappingTypeId != "01bc7593-a6a2-45aa-8497-b4b6a269ab68")
        {
            return null;
        }

        var model = mappingModel.Model;
        if (model.SpecializationType == "DTO-Field" && model.TypeReference.IsCollection && mappingModel.Children.Any())
        {
            return new SelectToListMapping(mappingModel, _template);
        }

        if (model.SpecializationType == "DTO-Field" && mappingModel.Children.Any())
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }

        return null;
    }
}