using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class DomainEventMappingTypeResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _sourceTemplate;

    public DomainEventMappingTypeResolver(ICSharpFileBuilderTemplate sourceTemplate)
    {
        _sourceTemplate = sourceTemplate;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        var model = mappingModel.Model;
        if (model.SpecializationType == "Domain Event")
        {
            return new ImplicitConstructorMapping(mappingModel, _sourceTemplate);
        }
        return new DefaultCSharpMapping(mappingModel, _sourceTemplate);
    }
}