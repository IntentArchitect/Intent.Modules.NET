using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies;

static class ImplementationStrategyTemplatesExtensions
{
    public static string GetDomainEntityName(this ICSharpTemplate template, ClassModel domainModel)
    {
        var entityName = template
            .GetTypeName("Domain.Entity", domainModel, TemplateDiscoveryOptions.DoNotThrow);
        return entityName;
    }

    public static string GetEntityRepositoryInterfaceName(this ICSharpTemplate template, ClassModel domainModel)
    {
        var repo = template
            .GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, domainModel,
                TemplateDiscoveryOptions.DoNotThrow);
        return repo;
    }

    public static string GetDtoName(this ICSharpTemplate template, DTOModel dtoModel)
    {
        var dtoTemplate = template.GetTemplate<IClassProvider>("Application.Contract.Dto", dtoModel, TemplateDiscoveryOptions.DoNotThrow);
        if (dtoTemplate == null)
        {
            return null;
        }
        template.AddUsing(dtoTemplate.Namespace);

        return dtoTemplate.ClassName;
    }
}