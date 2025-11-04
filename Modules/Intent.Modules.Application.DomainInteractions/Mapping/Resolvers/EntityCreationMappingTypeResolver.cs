using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

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

        // Static "factory" methods:
        const string invocationMappingTypeId = "a4c4c5cc-76df-48ed-9d4e-c35caf44b567";
        if (mappingModel.MappingTypeId is invocationMappingTypeId &&
            model.AsOperationModel()?.IsStatic == true &&
            model.TypeReference.ElementId != null)
        {
            return new StaticMethodInvocationMapping(mappingModel, _sourceTemplate);
        }

        const string creationMappingTypeId = "5f172141-fdba-426b-980e-163e782ff53e";
        if (mappingModel.MappingTypeId is not creationMappingTypeId)
        {
            return next?.Invoke(mappingModel);
        }

        if (model.IsGeneralizationTargetEndModel())
        {
            var mapping = new InheritedChildrenMapping(mappingModel, (ICSharpFileBuilderTemplate)_sourceTemplate);
            return mapping;
        }

        if (IsStaticConstructor(model))
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

        return next?.Invoke(mappingModel);
    }

    private static bool IsStaticConstructor(ICanBeReferencedType model)
    {
        if (model.AsOperationModel()?.IsStatic != true)
        {
            return false;
        }

        var entityId = ((IElement)model).ParentId;
        var operationReturnTypeRef = model.TypeReference;

        // Check for generics
        var genericParams = operationReturnTypeRef.GenericTypeParameters.ToList();
        if (genericParams.Count == 1 && genericParams[0].ElementId == entityId)
        {
            // return true if we want to support this result-pattern scenario in the future
            throw new NotSupportedException("Wrapping the entity in a generic type is not supported for static constructor creation mappings.");
        }

        return operationReturnTypeRef?.ElementId == entityId;
    }
}