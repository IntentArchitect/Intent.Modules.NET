using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Blazor.Api.Mappings;

public class RazorBindingMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpTemplate _template;

    public RazorBindingMappingResolver(ICSharpTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Model?.SpecializationType == "Text")
        {
            return new RazorTextDirectiveMapping(mappingModel, _template);
        }

        if (mappingModel.Mapping?.MappingTypeId == "95ce3c3e-ddf4-440b-a9b2-6b152a5fc1b8") // Event Mapping
        {   
            //return new MethodInvocationMapping(mappingModel, _template);
            return new RazorEventBindingMapping(mappingModel, _template);
        }

        if(mappingModel.Mapping?.MappingTypeId == "e4f0c63b-0f00-42bd-a703-00adf44f3364") // Invokable Mapping
        {
            return new RazorEventBindingMapping(mappingModel, _template);
        }

        if (mappingModel.Model?.TypeReference?.Element?.IsTypeDefinitionModel() == true
            || mappingModel.Model?.TypeReference?.Element?.IsEnumModel() == true)
        {
            return new RazorPropertyBindingMapping(mappingModel, _template);
        }

        return null;
    }
}

