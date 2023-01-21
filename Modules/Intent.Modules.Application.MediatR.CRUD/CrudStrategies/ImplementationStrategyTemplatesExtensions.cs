using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
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

    public static ClassModel GetNestedCompositionalOwner(this ClassModel entity)
    {
        var aggregateRootAssociation = entity.AssociatedClasses
            .SingleOrDefault(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
                                  p.IsSourceEnd() && !p.IsCollection && !p.IsNullable);
        return aggregateRootAssociation?.Class;
    }

    public static DTOFieldModel GetNestedCompositionalOwnerId(this IEnumerable<DTOFieldModel> properties, ClassModel owner)
    {
        var explicitKeyField = GetExplicitForeignKeyNestedCompOwnerField(properties, owner);
        if (explicitKeyField != null) return explicitKeyField;
        var implicitKeyField = GetImplicitForeignKeyNestedCompOwnerField(properties, owner);
        return implicitKeyField;
    }

    private static DTOFieldModel GetExplicitForeignKeyNestedCompOwnerField(IEnumerable<DTOFieldModel> properties, ClassModel owner)
    {
        var idField = properties
            .FirstOrDefault(p => 
                p.Mapping?.Element.AsAttributeModel()?.Name.Contains(owner.Name, StringComparison.OrdinalIgnoreCase) == true);
        return idField;
    }

    private static DTOFieldModel GetImplicitForeignKeyNestedCompOwnerField(IEnumerable<DTOFieldModel> properties, ClassModel owner)
    {
        var idField = properties.FirstOrDefault(p => p.Name.Contains($"{owner.Name}Id", StringComparison.OrdinalIgnoreCase));
        return idField;
    }
    
    public static AssociationEndModel GetNestedCompositeAssociation(this ClassModel owner, ClassModel nestedCompositionEntity)
    {
        return owner.AssociatedClasses.FirstOrDefault(p => p.Class == nestedCompositionEntity);
    }
}