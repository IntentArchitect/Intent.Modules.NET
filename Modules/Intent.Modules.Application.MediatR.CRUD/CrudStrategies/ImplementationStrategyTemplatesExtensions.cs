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

    public static ClassModel GetAggregateRootOwner(this ClassModel model)
    {
        var aggregateRootAssociation = model.AssociatedClasses
            .SingleOrDefault(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
                                  p.IsSourceEnd() && !p.IsCollection && !p.IsNullable);
        return aggregateRootAssociation?.Class;
    }

    public static DTOFieldModel GetAggregateRootIdField(this IEnumerable<DTOFieldModel> properties, ClassModel aggregateRootOwner)
    {
        var explicitKeyField = GetExplicitForeignKeyAggregateRootField(properties, aggregateRootOwner);
        if (explicitKeyField != null) return explicitKeyField;
        var implicitKeyField = GetImplicitForeignKeyAggregateRootField(properties, aggregateRootOwner);
        return implicitKeyField;
    }
    
    private static DTOFieldModel GetExplicitForeignKeyAggregateRootField(IEnumerable<DTOFieldModel> properties, ClassModel aggregateRootOwner)
    {
        var idField = properties
            .FirstOrDefault(p => 
                p.Mapping?.Element.AsAttributeModel()?.Name.Contains(aggregateRootOwner.Name, StringComparison.OrdinalIgnoreCase) == true);
        return idField;
    }

    private static DTOFieldModel GetImplicitForeignKeyAggregateRootField(IEnumerable<DTOFieldModel> properties, ClassModel aggregateRootOwner)
    {
        var idField = properties.FirstOrDefault(p => p.Name.Equals($"{aggregateRootOwner.Name}Id", StringComparison.OrdinalIgnoreCase));
        return idField;
    }

    public static AssociationEndModel GetNestedCompositeAssociation(this ClassModel aggregateRootEntity, ClassModel nestedCompositionEntity)
    {
        return aggregateRootEntity.AssociatedClasses.FirstOrDefault(p => p.Class == nestedCompositionEntity);
    }
}