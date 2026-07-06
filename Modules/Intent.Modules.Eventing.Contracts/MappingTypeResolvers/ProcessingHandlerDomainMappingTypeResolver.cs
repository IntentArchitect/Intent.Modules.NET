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

        // Also handle a node whose *type* is a Class — e.g. an Association Target End / DTO-field
        // collection projecting onto an entity collection (CatalogueItems -> ICollection<CatalogueItem>).
        // IsClassModel() only matches when the node's own model is a class (the root entity); it misses
        // a collection node whose element type is a class, which would otherwise fall through to a bare
        // assignment (CS0266) instead of a Select(...).ToList() projection. Mirrors EntityCreationMappingTypeResolver.
        if (model.IsClassModel() || model.IsConstructorModel() ||
            model.TypeReference?.Element?.SpecializationType == "Class")
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
