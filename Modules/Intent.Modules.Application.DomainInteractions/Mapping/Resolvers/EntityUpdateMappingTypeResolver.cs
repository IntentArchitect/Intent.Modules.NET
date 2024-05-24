using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

public class EntityUpdateMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _sourceTemplate;

    public EntityUpdateMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.MappingTypeId != "01721b1a-a85d-4320-a5cd-8bd39247196a")
        {
            return null;
        }

        var model = mappingModel.Model;

        if (model.IsGeneralizationTargetEndModel())
        {
            return new InheritedChildrenMapping(mappingModel, _sourceTemplate);
        }

        if (model.SpecializationType == "Class" || (model.SpecializationType == "Association Target End" && model.TypeReference?.Element?.SpecializationType == "Class"))
        {
            return new ObjectUpdateMapping(mappingModel, _sourceTemplate);
        }

        if (model.SpecializationType == "Association Target End" && model.TypeReference?.Element?.SpecializationType == "Value Object" && model.TypeReference.IsCollection)
        {
            return new ValueObjectCollectionUpdateMapping(mappingModel, _sourceTemplate);
        }

		if ((model.TypeReference?.Element?.SpecializationType == "Value Object" || model.TypeReference?.Element?.SpecializationType == "Data Contract") && model.TypeReference.IsCollection)
		{
			return new SelectToListMapping(mappingModel, _sourceTemplate);
		}


		return null;
    }
}