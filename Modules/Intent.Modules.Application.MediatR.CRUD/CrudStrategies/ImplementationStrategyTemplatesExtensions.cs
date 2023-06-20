using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using ParameterModel = Intent.Modelers.Domain.Api.ParameterModel;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies;

public static class ImplementationStrategyTemplatesExtensions
{
    public static string GetNotFoundExceptionName(this ICSharpTemplate template)
    {
        var exceptionName = template
            .GetTypeName("Domain.NotFoundException", TemplateDiscoveryOptions.DoNotThrow);
        return exceptionName;
    }
    
    public static string GetDomainEntityName(this ICSharpTemplate template, ClassModel domainModel)
    {
        var entityName = template
            .GetTypeName("Domain.Entity", domainModel, TemplateDiscoveryOptions.DoNotThrow);
        return entityName;
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

    public record EntityIdAttribute(string IdName, string Type);

    public static EntityIdAttribute GetEntityIdAttribute(this ClassModel entity, ISoftwareFactoryExecutionContext executionContext)
    {
        var explicitKeyField = GetExplicitEntityIdField(entity);
        if (explicitKeyField != null) return explicitKeyField;
        return new EntityIdAttribute("Id", GetDefaultSurrogateKeyType(executionContext));

        EntityIdAttribute GetExplicitEntityIdField(ClassModel entity)
        {
            return entity.Attributes.Where(p => p.IsPrimaryKey()).Select(s => new EntityIdAttribute(s.Name, GetKeyTypeName(s.Type))).FirstOrDefault();
        }
    }

    public record EntityNestedCompositionalIdAttribute(string IdName, string Type);
    
    public static EntityNestedCompositionalIdAttribute GetNestedCompositionalOwnerIdAttribute(this ClassModel entity, ClassModel owner, ISoftwareFactoryExecutionContext executionContext)
    {
        var explicitKeyField = GetExplicitForeignKeyNestedCompOwnerField(entity, owner);
        if (explicitKeyField != null) return explicitKeyField;
        return new EntityNestedCompositionalIdAttribute($"{owner.Name}Id", GetDefaultSurrogateKeyType(executionContext));
        
        EntityNestedCompositionalIdAttribute GetExplicitForeignKeyNestedCompOwnerField(ClassModel entity, ClassModel owner)
        {
            var idField = entity.Attributes
                .FirstOrDefault(attr =>
                {
                    if (!attr.IsForeignKey())
                    {
                        return false;
                    }

                    var fkAssociation = attr.GetForeignKeyAssociation();
                    // Backward compatible lookup method
                    if (fkAssociation == null)
                    {
                        return attr.Name.Contains(owner.Name, StringComparison.OrdinalIgnoreCase);
                    }

                    return owner.AssociationEnds().Any(p => p.Id == fkAssociation.Id);
                });
            if (idField == null)
            {
                return null;
            }
            return new EntityNestedCompositionalIdAttribute(idField.Name, GetKeyTypeName(idField.Type));
        }
    }

    public static DTOFieldModel GetEntityIdField(this IEnumerable<DTOFieldModel> properties, ClassModel entity)
    {
        var explicitKeyField = GetExplicitEntityIdField(properties);
        if (explicitKeyField != null) return explicitKeyField;
        var implicitKeyField = GetImplicitEntityIdField(properties, entity);
        return implicitKeyField;
        
        DTOFieldModel GetExplicitEntityIdField(IEnumerable<DTOFieldModel> properties)
        {
            var idField = properties
                .FirstOrDefault(field =>
                {
                    var attr = field.Mapping?.Element.AsAttributeModel();
                    return attr != null && attr.IsPrimaryKey();
                });
            return idField;
        }

        DTOFieldModel GetImplicitEntityIdField(IEnumerable<DTOFieldModel> properties, ClassModel entity)
        {
            var idField = properties.FirstOrDefault(p =>
                string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
                (entity != null && string.Equals(p.Name, $"{entity.Name}Id", StringComparison.InvariantCultureIgnoreCase)));
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

                    if (!attr.IsForeignKey())
                    {
                        return false;
                    }

                    var fkAssociation = attr.GetForeignKeyAssociation();
                    // Backward compatible lookup method
                    if (fkAssociation == null)
                    {
                        return attr.Name.Contains(owner.Name, StringComparison.OrdinalIgnoreCase);
                    }

                    return owner.AssociationEnds().Any(p => p.Id == fkAssociation.Id);
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
    
    public static IReadOnlyCollection<RequiredService> GetAdditionalServicesFromParameters(this ICSharpTemplate template, IEnumerable<ParameterModel> parameters)
    {
        return parameters
            .Select(parameter => template.FindRequiredService(parameter.Type.Element))
            .Where(service => service != null)
            .ToList();
    }

    public static RequiredService FindRequiredService(this ICSharpTemplate template, IMetadataModel element)
    {
        return element switch
        {
            _ when template.TryGetTypeName("Domain.DomainServices.Interface", element, out var typeName) => 
                new RequiredService(typeName, "domainService"),
            _ => null
        };
    }

    private static string GetKeyTypeName(ITypeReference typeReference)
    {
        return typeReference switch
        {
            _ when typeReference.HasIntType() => "int",
            _ when typeReference.HasLongType() => "long",
            _ when typeReference.HasGuidType() => "System.Guid",
            _ => typeReference.Element.Name
        };
    }
    
    private static string GetDefaultSurrogateKeyType(ISoftwareFactoryExecutionContext executionContext)
    {
        //var settingType = executionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "guid";
        var settingType = "guid"; // GCB - we want to deprecate this asap.
        switch (settingType)
        {
            case "guid":
                return "System.Guid";
            case "int":
                return "int";
            case "long":
                return "long";
            default:
                return settingType;
        }
    }
}