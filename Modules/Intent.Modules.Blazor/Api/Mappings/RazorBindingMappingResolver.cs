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
        if (mappingModel.Model?.SpecializationType == "Text")
        {
            return new RazorTextDirectiveMapping(mappingModel, _template);
        }

        if (mappingModel.Mapping?.MappingTypeId == "95ce3c3e-ddf4-440b-a9b2-6b152a5fc1b8") // Event Mapping
        {
            //return new MethodInvocationMapping(mappingModel, _template);
            return new RazorEventBindingMapping(mappingModel, _template);
        }

        if (mappingModel.Mapping != null)
        {
            return new RazorPropertyBindingMapping(mappingModel, _template);
        }

        return null;
    }
}