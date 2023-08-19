namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public interface IMappingTypeResolver
{
    ICSharpMapping ResolveMappings(MappingModel mappingModel);
}