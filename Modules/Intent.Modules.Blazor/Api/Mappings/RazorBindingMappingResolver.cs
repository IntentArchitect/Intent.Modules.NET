using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

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
        if (mappingModel.Model.SpecializationType == "Text")
        {
            return new RazorTextDirectiveMapping(mappingModel, _template);
        }

        if (mappingModel.Model.SpecializationType == "Static Mappable Settings"
            && mappingModel.Mapping.MappingTypeId == "e4f0c63b-0f00-42bd-a703-00adf44f3364") // Invokable Mapping
        {
            return new RazorEventBindingMapping(mappingModel, _template);
        }

        return new RazorPropertyBindingMapping(mappingModel, _template);
    }
}