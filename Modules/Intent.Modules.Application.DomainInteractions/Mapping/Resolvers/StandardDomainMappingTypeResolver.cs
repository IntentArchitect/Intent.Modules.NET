using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

public class StandardDomainMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public StandardDomainMappingTypeResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        var model = mappingModel.Model;

        if (model.IsGeneralizationTargetEndModel())
        {
            return new InheritedChildrenMapping(mappingModel, _template);
        }

        if (mappingModel.Mapping?.SourceElement?.TypeReference?.IsCollection == true && mappingModel.Model.SpecializationType == "Operation")
        {
            return new ForLoopMethodInvocationMapping(mappingModel, _template);
        }

        if (model.SpecializationType == "Class Constructor")
        {
            return new ConstructorMapping(mappingModel, _template);
        }

        if (model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(mappingModel, _template);
        }

        if (mappingModel.Model.SpecializationType == "Class")
        {
            return new MapChildrenMapping(mappingModel, _template);
        }

        if (mappingModel.Model.SpecializationType == "Create Entity Action Target End")
        {
            return new MapChildrenMapping(mappingModel, _template);
        }

        if (mappingModel.Model.SpecializationType == "Update Entity Action Target End")
        {
            return new MapChildrenMapping(mappingModel, _template);
        }

        if (mappingModel.Model.SpecializationType == "Parameter" &&
            mappingModel.Model.TypeReference.Element.SpecializationType is "DTO" or "Command" or "Query")
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }

        return null;
    }
}