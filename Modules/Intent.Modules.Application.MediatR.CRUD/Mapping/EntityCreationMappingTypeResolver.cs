using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class EntityCreationMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _sourceTemplate;

    public EntityCreationMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        var model = mappingModel.Model;
        if (model.TypeReference?.Element?.SpecializationType == "Value Object")
        {
            return new ImplicitConstructorMapping(mappingModel, _sourceTemplate);
        }
        if (model.SpecializationType == "Class Constructor")
        {
            return new ImplicitConstructorMapping(mappingModel, _sourceTemplate);
        }
        if (model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(mappingModel, _sourceTemplate);
        }
        return new ObjectInitializationMapping(mappingModel, _sourceTemplate);
    }
}