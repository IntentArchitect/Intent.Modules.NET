using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Eventing.MappingTypeResolvers;

public class ProcessingHandlerDomainUpdateMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ProcessingHandlerDomainUpdateMappingTypeResolver(ICSharpFileBuilderTemplate template)
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

        if (model.SpecializationType == "Class" || (model.SpecializationType == "Association Target End" && model.TypeReference?.Element?.SpecializationType == "Class"))
        {
            return new ObjectUpdateMapping(mappingModel, _template);
        }

        if (model.SpecializationType == "Association Target End" && model.TypeReference?.Element?.SpecializationType == "Value Object" && model.TypeReference.IsCollection)
        {
            return new ValueObjectCollectionUpdateMapping(mappingModel, _template);
        }

        if ((model as IElement)?.ParentElement.SpecializationType != "Value Object"
            && model.TypeReference?.Element?.SpecializationType == "Value Object"
            && model.TypeReference.IsCollection
            && model.SpecializationType != "Operation"
            && model.SpecializationType != "Parameter")
        {
            return new ValueObjectCollectionUpdateMapping(mappingModel, _template);
        }

        if ((model.TypeReference?.Element?.SpecializationType == "Value Object" || model.TypeReference?.Element?.SpecializationType == "Data Contract") && model.TypeReference.IsCollection)
        {
            return new SelectToListMapping(mappingModel, _template);
        }

        return null;
    }
}
