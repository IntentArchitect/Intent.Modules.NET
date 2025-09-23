using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.DomainInteractions.InteractionStrategies;

public class CallServiceOperationMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CallServiceOperationMappingResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping? ResolveMappings(MappingModel mappingModel)
    {
        return mappingModel.Model.SpecializationType switch
        {
            "Operation" => new MethodInvocationMapping(mappingModel, _template),
            "Parameter" => new ObjectInitializationMapping(mappingModel, _template),
            "DTO-Field" => new ObjectInitializationMapping(mappingModel, _template),
            _ => null
        };
    }
}