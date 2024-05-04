using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Api.Mappings;

public class CallServiceOperationMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpTemplate _template;

    public CallServiceOperationMappingResolver(ICSharpTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Model?.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(mappingModel, _template);
        }

        if (mappingModel.Model?.TypeReference?.Element?.SpecializationType is "Command" or "DTO" or "Model Definition")
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }
        return null;
    }
}