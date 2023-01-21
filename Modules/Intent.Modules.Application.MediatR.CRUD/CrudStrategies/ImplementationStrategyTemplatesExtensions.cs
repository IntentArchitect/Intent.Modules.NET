using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.RDBMS.Api;
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

    public record EntityIdAttribute(string IdName);

    public static EntityIdAttribute GetEntityIdAttribute(this ClassModel entity)
    {
        var explicitKeyField = GetExplicitEntityIdField(entity);
        if (explicitKeyField != null) return explicitKeyField;
        return new EntityIdAttribute("Id");

        EntityIdAttribute GetExplicitEntityIdField(ClassModel entity)
        {
            return entity.Attributes.Where(p => p.HasPrimaryKey()).Select(s => new EntityIdAttribute(s.Name)).FirstOrDefault();
        }
    }

    public static DTOFieldModel GetEntityIdField(this IEnumerable<DTOFieldModel> properties, ClassModel entity)
    {
        var explicitKeyField = GetExplicitEntityIdField(properties, entity);
        if (explicitKeyField != null) return explicitKeyField;
        var implicitKeyField = GetImplicitEntityIdField(properties, entity);
        return implicitKeyField;
        
        DTOFieldModel GetExplicitEntityIdField(IEnumerable<DTOFieldModel> properties, ClassModel entity)
        {
            var idField = properties
                .FirstOrDefault(field =>
                {
                    var attr = field.Mapping?.Element.AsAttributeModel();
                    if (attr == null)
                    {
                        return false;
                    }

                    return attr.HasPrimaryKey() && entity.Attributes.Any(p => p.Id == attr.Id);
                });
            return idField;
        }

        DTOFieldModel GetImplicitEntityIdField(IEnumerable<DTOFieldModel> properties, ClassModel entity)
        {
            var idField = properties.FirstOrDefault(p =>
                string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(p.Name, $"{entity.Name}Id", StringComparison.InvariantCultureIgnoreCase));
            return idField;
        }
    }

    public static DTOFieldModel GetNestedCompositionalOwnerIdField(this IEnumerable<DTOFieldModel> properties, ClassModel owner)
    {
        var explicitKeyField = GetExplicitForeignKeyNestedCompOwnerField(properties, owner);
        if (explicitKeyField != null) return explicitKeyField;
        var implicitKeyField = GetImplicitForeignKeyNestedCompOwnerField(properties, owner);
        return implicitKeyField;
        
        DTOFieldModel GetExplicitForeignKeyNestedCompOwnerField(IEnumerable<DTOFieldModel> properties, ClassModel owner)
        {
            var idField = properties
                .FirstOrDefault(field =>
                {
                    var attr = field.Mapping?.Element.AsAttributeModel();
                    if (attr == null)
                    {
                        return false;
                    }

                    return attr.HasForeignKey() && attr.Name.Contains(owner.Name, StringComparison.OrdinalIgnoreCase);
                });
            return idField;
        }

        DTOFieldModel GetImplicitForeignKeyNestedCompOwnerField(IEnumerable<DTOFieldModel> properties, ClassModel owner)
        {
            var idField = properties.FirstOrDefault(p => p.Name.Contains($"{owner.Name}Id", StringComparison.OrdinalIgnoreCase));
            return idField;
        }
    }

    public static AssociationEndModel GetNestedCompositeAssociation(this ClassModel owner, ClassModel nestedCompositionEntity)
    {
        return owner.AssociatedClasses.FirstOrDefault(p => p.Class == nestedCompositionEntity);
    }
}