using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;

public class EntityPatchMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _sourceTemplate;

    public EntityPatchMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping? ResolveMappings(MappingModel mappingModel, MappingTypeResolverDelegate next)
    {
        if (mappingModel.MappingTypeId != "01721b1a-a85d-4320-a5cd-8bd39247196a") // Update Mapping
        {
            return null;
        }
        
        var model = mappingModel.Model;

        if (model.SpecializationType is "Attribute" or "Association Target End"
            // Will null check on a collection, but won't apply further (deeper) than that (i.e. inside the collection updates). This may be a bit overly simplistic:
            && mappingModel.GetTargetPath().SkipLast(1).All(x => x.Element.TypeReference?.IsCollection != true)
            && mappingModel.GetSourcePath().TakeLast(1).Any(x => x.Element.TypeReference?.IsNullable == true))
        {
            return new IfNotNullMapping(mappingModel, _sourceTemplate, next(mappingModel));
        }

        return next(mappingModel);
    }
}