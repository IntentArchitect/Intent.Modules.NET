using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Utils;
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

    public static void AddValueObjectFactoryMethod(this ICSharpFileBuilderTemplate template, string mappingMethodName, IElement domain, DTOFieldModel field)
    {
        var @class = template.CSharpFile.Classes.First(x => x.HasMetadata("handler"));
        var targetDto = field.TypeReference.Element.AsDTOModel();
        if (!template.MethodExists(mappingMethodName, @class, targetDto))
        {
            var domainType = template.GetTypeName(domain);
            @class.AddMethod(domainType, mappingMethodName, method =>
            {
                method.Static()
                    .AddAttribute(CSharpIntentManagedAttribute.Fully())
                    .AddParameter(template.GetTypeName(targetDto.InternalElement), "dto");

                var attributeModels = domain.GetDomainAttibuteModels();

                var attributeMap = attributeModels.Select(a => (Domain: a, Dto: targetDto.Fields.FirstOrDefault(f => f.Mapping?.Element.Id == a.Id)));
                if (attributeMap.Any(x => x.Dto == null))
                {
                    method.AddStatement($@"#warning Not all fields specified for ValueObject.");
                }
                var ctorParameters = string.Join(",", attributeMap.Select(m => $"{m.Domain.Name.ToParameterName()}: {(m.Dto == null ? $"default({template.GetTypeName(m.Domain.TypeReference)})" : $"dto.{m.Dto.Name.ToPascalCase()}")}"));
                method.AddStatement($"return new {domainType}({ctorParameters});");
            });
        }
    }

    public static bool MethodExists(this ICSharpFileBuilderTemplate template, string mappingMethodName, CSharpClass @class, DTOModel targetDto)
    {
        return @class.FindMethod((method) =>
                                    method.Name == mappingMethodName
                                    && method.Parameters.Count == 1
                                    && method.Parameters[0].Type == template.GetTypeName(targetDto.InternalElement)) != null;
    }

    public static IList<AttributeModel> GetDomainAttibuteModels(this IElement element)
    {
        return element.ChildElements.Where(x => x.IsAttributeModel()).Select(x => x.AsAttributeModel()).ToList();
    }


    // This is duplicated in Intent.Modules.Application.Shared
    public static ClassModel GetNestedCompositionalOwner(this ClassModel entity)
    {
        var aggregateRootClass = entity.AssociatedClasses
            .Where(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
                        p.IsSourceEnd() && !p.IsCollection && !p.IsNullable)
            .Select(s => s.Class)
            .Distinct()
            .ToList();
        if (aggregateRootClass.Count > 1)
        {
            throw new ElementException(entity.InternalElement, $"{entity.Name} has multiple owners ({string.Join(",", aggregateRootClass.Select(a => a.Name))}). Owned entities can only have 1 owner.");
        }
        return aggregateRootClass.SingleOrDefault();
    }

    // This is duplicated in Intent.Modules.Application.Shared
    public interface IEntityId
    {
        string IdName { get; }
        string Type { get; }
        AttributeModel Attribute { get; }
    }

    // This is duplicated in Intent.Modules.Application.Shared
    public record EntityIdAttribute(string IdName, string Type, AttributeModel Attribute) : IEntityId;

    // This is duplicated in Intent.Modules.Application.Shared
    public static EntityIdAttribute GetEntityPkAttribute(this ClassModel entity, ISoftwareFactoryExecutionContext executionContext)
    {
        return GetEntityPkAttributes(entity, executionContext).FirstOrDefault();
    }

    // This is duplicated in Intent.Modules.Application.Shared
    public static IList<EntityIdAttribute> GetEntityPkAttributes(this ClassModel entity, ISoftwareFactoryExecutionContext executionContext)
    {
        while (entity != null)
        {
            var primaryKeys = entity.Attributes.Where(x => x.IsPrimaryKey()).ToArray();
            if (!primaryKeys.Any())
            {
                entity = entity.ParentClass;
                continue;
            }

            return primaryKeys
                .Select(attribute => new EntityIdAttribute(attribute.Name, GetKeyTypeName(attribute.Type), attribute))
                .ToList();
        }

        // Implicit Key:
        return new List<EntityIdAttribute>
        {
            new("Id", GetDefaultSurrogateKeyType(executionContext), null)
        };
    }

    // This is duplicated in Intent.Modules.Application.Shared
    public static IList<EntityIdAttribute> GetEntityFkAttributes(
        this ClassModel composite,
        ClassModel owner,
        ISoftwareFactoryExecutionContext executionContext)
    {
        var fkAttributes = new List<AttributeModel>(0);

        while (composite != null)
        {
            var fkAttributesAndAssociations = composite.Attributes
                .Where(x => x.IsForeignKey())
                .Select(x => new
                {
                    Attribute = x,
                    Association = x.GetForeignKeyAssociation()
                })
                .ToArray();

            // Modern method of matching by the association on the FK:
            fkAttributes = fkAttributesAndAssociations
                .Where(x => owner.AssociationEnds().Any(y => x.Association.Id == y.Id))
                .Select(x => x.Attribute)
                .ToList();
            if (fkAttributes.Any())
            {
                break;
            }

            // Fallback to legacy method of matching by name:
            fkAttributes = fkAttributesAndAssociations
                .Where(x =>
                    x.Association == null &&
                    x.Attribute.Name.Contains(owner.Name, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Attribute)
                .ToList();
            if (fkAttributes.Any())
            {
                break;
            }

            composite = composite.ParentClass;
        }

        return fkAttributes
            .Select(x => new EntityIdAttribute(
                IdName: x.Name,
                Type: GetKeyTypeName(x.TypeReference),
                Attribute: x))
            .ToList();
    }

    // This is duplicated in Intent.Modules.Application.Shared
    public record EntityNestedCompositionalIdAttribute(string IdName, string Type, AttributeModel Attribute) : IEntityId;

    // This is duplicated in Intent.Modules.Application.Shared
    public static EntityNestedCompositionalIdAttribute GetNestedCompositionalOwnerIdAttribute(this ClassModel entity, ClassModel owner, ISoftwareFactoryExecutionContext executionContext)
    {
        return GetNestedCompositionalOwnerIdAttributes(entity, owner, executionContext).FirstOrDefault();
    }

    // This is duplicated in Intent.Modules.Application.Shared
    public static IList<EntityNestedCompositionalIdAttribute> GetNestedCompositionalOwnerIdAttributes(this ClassModel entity, ClassModel owner, ISoftwareFactoryExecutionContext executionContext)
    {
        var curEntity = entity;
        while (curEntity is not null)
        {
            var foreignKeys = curEntity.Attributes.Where(x => x.IsForeignKey()).ToArray();
            if (!foreignKeys.Any())
            {
                curEntity = curEntity.ParentClass;
                continue;
            }

            return foreignKeys
                .Where(attr =>
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
                })
                .Select(attribute => new EntityNestedCompositionalIdAttribute(attribute.Name, GetKeyTypeName(attribute.Type), attribute))
                .ToList();
        }

        // Nested Entity without Aggregate FK
        var aggregateOwner = entity.GetNestedCompositionalOwner();
        if (aggregateOwner is not null)
        {
            var pks = aggregateOwner.Attributes.Where(attr => attr.IsPrimaryKey()).ToArray();
            if (pks.Any())
            {
                return pks
                    .Select(attr => new EntityNestedCompositionalIdAttribute($"{owner.Name.ToPascalCase()}{attr.Name.ToPascalCase()}", GetKeyTypeName(attr.Type), attr))
                    .ToList();
            }
        }
        
        // Implicit Key:
        return new List<EntityNestedCompositionalIdAttribute>
        {
            new($"{owner.Name}Id", GetDefaultSurrogateKeyType(executionContext), null)
        };
    }

    // This is duplicated in Intent.Modules.Application.Shared
    public static DTOFieldModel GetEntityIdField(this ICollection<DTOFieldModel> fields, ClassModel entity, ISoftwareFactoryExecutionContext context)
    {
        return GetEntityIdFields(fields, entity, context).FirstOrDefault();
    }

    // This is duplicated in Intent.Modules.Application.Shared
    public static List<DTOFieldModel> GetEntityIdFields(this ICollection<DTOFieldModel> fields, ClassModel entity, ISoftwareFactoryExecutionContext context)
    {
        var primaryKeys = entity.GetEntityPkAttributes(context);

        return primaryKeys
            .Select(GetField)
            .Where(x => x != null)
            .ToList();

        DTOFieldModel GetField(EntityIdAttribute pk)
        {
            return fields
                .Where(field =>
                {
                    if (field.Mapping?.ElementId != null)
                    {
                        return field.Mapping.ElementId == pk.Attribute?.Id;
                    }

                    // Fallback to matching by name
                    return string.Equals(pk.IdName, field.Name, StringComparison.OrdinalIgnoreCase);
                })
                // We give priority to mapped elements and fallback to matches by name
                .MinBy(x => x.Mapping?.ElementId != null);
        }
    }

    public static string GetPropertyToRequestMatchClause(this IList<DTOFieldModel> idFields)
    {
        if (idFields.Count == 1)
            return $"p => p.{idFields.First().Name.ToPascalCase()} == request.{idFields.First().Name.ToPascalCase()}";
        return $"p => {string.Join(" && ", idFields.Select(idField => $"p.{idField.Name.ToPascalCase()} == request.{idField.Name.ToPascalCase()}"))}";
    }

    public static string GetEntityIdFromRequest(this IList<DTOFieldModel> idFields, IElement element)
    {
        if (idFields.Any(x => x is null))
        {
            Logging.Log.Warning(
                $"\"{element.Name}\" [{element.Id}] is missing one or more fields for its owning aggregate's " +
                $"primary keys, add the missing fields either by mapping them for data mappings, or adding " +
                $"them manually in the format of \"<owningEntityName><primaryKeyAttributeName>\" for " +
                $"operation/constructor mappings.");
        }

        var values = idFields
            .Select(idField => $"request.{idField?.Name.ToPascalCase() ?? "MISSING_FIELD"}")
            .ToArray();

        return values.Length == 1
            ? values[0]
            : $"({string.Join(", ", values)})";
    }

    public static string GetEntityIdFromRequestDescription(this IList<DTOFieldModel> idFields)
    {
        var values = idFields
            .Select(idField => $"{{request.{idField?.Name.ToPascalCase() ?? "MISSING_FIELD"}}}")
            .ToArray();

        return values.Length == 1
            ? values[0]
            : $"({string.Join(", ", values)})";
    }

    public static DTOFieldModel GetNestedCompositionalOwnerIdField(this IEnumerable<DTOFieldModel> properties, ClassModel owner)
    {
        return GetNestedCompositionalOwnerIdFields(properties, owner).FirstOrDefault();
    }

    public static IList<DTOFieldModel> GetNestedCompositionalOwnerIdFields(this IEnumerable<DTOFieldModel> properties, ClassModel owner)
    {
        var explicitKeyField = GetExplicitForeignKeyNestedCompOwnerField(properties, owner);
        if (explicitKeyField.Any()) return explicitKeyField;
        var implicitKeyField = GetImplicitForeignKeyNestedCompOwnerField(properties, owner);
        if (implicitKeyField != null)
        {
            return new List<DTOFieldModel> { implicitKeyField };
        }
        return new List<DTOFieldModel>();

        static List<DTOFieldModel> GetExplicitForeignKeyNestedCompOwnerField(IEnumerable<DTOFieldModel> properties, ClassModel owner)
        {
            var idField = properties
                .Where(field =>
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
            return idField.ToList();
        }

        static DTOFieldModel GetImplicitForeignKeyNestedCompOwnerField(IEnumerable<DTOFieldModel> properties, ClassModel owner)
        {
            var idField = properties.FirstOrDefault(p => p.Name.Contains($"{owner.Name}Id", StringComparison.OrdinalIgnoreCase));
            return idField;
        }
    }

    /// <remarks>
    /// Only ever used for constructors and operations which can't possibly have mappings to
    /// associations or attributes of the owner.The designer's "Create CRUD script" in such cases
    /// always automatically creates a field for each owner's pk and prefixes it with the owning
    /// entity's name.
    /// </remarks>
    public static IList<DTOFieldModel> GetCompositesOwnerIdFieldsForOperations(
        this IList<DTOFieldModel> fields, 
        ClassModel owner, 
        ClassModel composite,
        ISoftwareFactoryExecutionContext executionContext)
    {
        // Try match by create CRUD script naming convention:
        {
            var pks = owner.GetEntityPkAttributes(executionContext);
            var fieldsToReturn = pks
                .Select(ownerPkAttribute =>
                {
                    var entityName = owner.Name.ToLowerInvariant();
                    var pkAttributeName = ownerPkAttribute.IdName.ToLowerInvariant();
                    var fieldName = $"{entityName}{pkAttributeName.RemoveSuffix(entityName)}";

                    return fields.FirstOrDefault(field =>
                        string.Equals(field.Name, fieldName, StringComparison.OrdinalIgnoreCase));
                })
                .Where(x => x != null)
                .ToList();
            if (fieldsToReturn.Any())
            {
                return fieldsToReturn;
            }
        }

        // Try match by field name matching attribute with fk to to owner:
        {
            var fkAttributes = composite.GetEntityFkAttributes(owner, executionContext);
            var fieldsToReturn = fields
                .Where(field => fkAttributes.Any(attribute => string.Equals(attribute.IdName, field.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            if (fieldsToReturn.Any())
            {
                return fieldsToReturn;
            }
        }

        return new List<DTOFieldModel>(0);
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

    // This is duplicated in Intent.Modules.Application.Shared
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

    // This is duplicated in Intent.Modules.Application.Shared
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