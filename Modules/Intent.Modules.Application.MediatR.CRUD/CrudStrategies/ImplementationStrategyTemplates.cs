using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies;

static class ImplementationStrategyTemplates
{
    public static string GetDomainEntityName(this IntentTemplateBase template, ClassModel domainModel)
    {
        var entityName = template
            .GetTypeName("Domain.Entity", domainModel, TemplateDiscoveryOptions.DoNotThrow);
        return entityName;
    }

    public static string GetEntityRepositoryInterfaceName(this IntentTemplateBase template, ClassModel domainModel)
    {
        var repo = template
            .GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, domainModel,
                TemplateDiscoveryOptions.DoNotThrow);
        return repo;
    }

    public static string GetDtoName(this IntentTemplateBase template, DTOModel dtoModel)
    {
        return template.GetTypeName("Application.Contract.Dto", dtoModel, TemplateDiscoveryOptions.DoNotThrow);
    }
}