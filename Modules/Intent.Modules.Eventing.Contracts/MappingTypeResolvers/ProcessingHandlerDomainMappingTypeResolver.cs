using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Application.MediatR.CRUD.Eventing.MappingTypeResolvers;

public class ProcessingHandlerDomainMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ProcessingHandlerDomainMappingTypeResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        var model = mappingModel.Model;

        if (model.IsClassModel() || model.IsConstructorModel())
        {
            return new ObjectInitializationMapping(mappingModel, (ICSharpTemplate)_template);
        }

        if (model.TypeReference?.Element?.SpecializationType == "Value Object")
        {
            return new ConstructorMapping(mappingModel, (ICSharpTemplate)_template);
        }

        return null;
    }
}
