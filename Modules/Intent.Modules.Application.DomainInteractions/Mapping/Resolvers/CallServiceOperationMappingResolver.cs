﻿using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.Eventing.Contracts.InteractionStrategies;

[IntentManaged(Mode.Ignore)]
public class CallServiceOperationMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CallServiceOperationMappingResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(mappingModel, _template);
        }

        if (mappingModel.Model.SpecializationType == "DTO-Field")
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }
        return null;
    }
}