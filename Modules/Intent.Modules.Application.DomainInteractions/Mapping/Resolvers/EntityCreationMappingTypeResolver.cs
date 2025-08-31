using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Templates;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

public class EntityCreationMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpTemplate _sourceTemplate;

    public EntityCreationMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping? ResolveMappings(MappingModel mappingModel, MappingTypeResolverDelegate next)
    {
        var model = mappingModel.Model;
        if (mappingModel.MappingTypeId != "5f172141-fdba-426b-980e-163e782ff53e") // Command to Class Creation Mapping
        {
            return next != null ? next(mappingModel) : null;
        }

        if (model.IsGeneralizationTargetEndModel())
        {
            var mapping = new InheritedChildrenMapping(mappingModel, (ICSharpFileBuilderTemplate)_sourceTemplate);
            return mapping;
        }

        if (model.AsOperationModel()?.IsStatic == true && model.TypeReference.ElementId == ((IElement)model).ParentId)
        {
            return new StaticMethodInvocationMapping(mappingModel, _sourceTemplate);
        }

        if (model.IsClassConstructorModel())
        {
            return new ConstructorMapping(mappingModel, _sourceTemplate);
        }

        if (model.SpecializationType == "Class" || model.TypeReference?.Element?.SpecializationType == "Class")
        {
            return new ObjectInitializationMapping(mappingModel, _sourceTemplate);
        }

        if ((model.TypeReference?.Element?.SpecializationType is "Value Object" or "Data Contract") && model.TypeReference.IsCollection)
        {
            return new SelectToListMapping(mappingModel, _sourceTemplate);
        }

        return next != null ? next(mappingModel) : null;
    }
}