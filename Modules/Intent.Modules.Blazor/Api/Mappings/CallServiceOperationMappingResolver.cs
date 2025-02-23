using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

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
        if (mappingModel.Model.SpecializationType is "Operation" or "Component Operation")
        {
            return new MethodInvocationMapping(mappingModel, _template);
        }

        if (mappingModel.Mapping?.MappingTypeId == "720f119b-39b3-4f11-8d96-27fa82d1f4e2" // Invocation Mapping
            && mappingModel.Model.SpecializationType is "Event Emitter")
        {
            //return new MethodInvocationMapping(mappingModel, _template);
            return new RazorEventEmitterInvocationMapping(mappingModel, _template);
        }

        if (mappingModel.Model.TypeReference?.Element?.SpecializationType is "Command" or "DTO" or "Model Definition"
            && mappingModel.Model.SpecializationType is not "Event Emitter")
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }

        return null;
    }
}