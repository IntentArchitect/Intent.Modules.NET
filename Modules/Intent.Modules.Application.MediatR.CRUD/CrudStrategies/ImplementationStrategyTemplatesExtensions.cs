using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
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

    public static void AddValueObjectFactoryMethod(this CommandHandlerTemplate template, string mappingMethodName, IElement domain, DTOFieldModel field)
    {
        var @class = template.CSharpFile.Classes.First();
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

    public static bool MethodExists(this CommandHandlerTemplate template, string mappingMethodName, CSharpClass @class, DTOModel targetDto)
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


    public static ClassModel GetNestedCompositionalOwner(this ClassModel entity)
    {
        var aggregateRootAssociation = entity.AssociatedClasses
            .SingleOrDefault(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
                                  p.IsSourceEnd() && !p.IsCollection && !p.IsNullable);
        return aggregateRootAssociation?.Class;
    }

    public interface IEntityId
    {
        string IdName { get; }
        string Type { get; }
        AttributeModel Attribute { get; }
    }

    public record EntityIdAttribute(string IdName, string Type, AttributeModel Attribute) : IEntityId;

    public static EntityIdAttribute GetEntityIdAttribute(this ClassModel entity, ISoftwareFactoryExecutionContext executionContext)
    {
        return GetEntityIdAttributes(entity, executionContext).FirstOrDefault();
    }

    public static IList<EntityIdAttribute> GetEntityIdAttributes(this ClassModel entity, ISoftwareFactoryExecutionContext executionContext)
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

    public record EntityNestedCompositionalIdAttribute(string IdName, string Type, AttributeModel Attribute) : IEntityId;

    public static EntityNestedCompositionalIdAttribute GetNestedCompositionalOwnerIdAttribute(this ClassModel entity, ClassModel owner, ISoftwareFactoryExecutionContext executionContext)
    {
        return GetNestedCompositionalOwnerIdAttributes(entity, owner, executionContext).FirstOrDefault();
    }

    public static IList<EntityNestedCompositionalIdAttribute> GetNestedCompositionalOwnerIdAttributes(this ClassModel entity, ClassModel owner, ISoftwareFactoryExecutionContext executionContext)
    {
        while (entity != null)
        {
            var foreignKeys = entity.Attributes.Where(x => x.IsForeignKey()).ToArray();
            if (!foreignKeys.Any())
            {
                entity = entity.ParentClass;
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

        // Implicit Key:
        return new List<EntityNestedCompositionalIdAttribute>
        {
            new($"{owner.Name}Id", GetDefaultSurrogateKeyType(executionContext), null)
        };
    }

    public static DTOFieldModel GetEntityIdField(this ICollection<DTOFieldModel> fields, ClassModel entity, ISoftwareFactoryExecutionContext context)
    {
        return GetEntityIdFields(fields, entity, context).FirstOrDefault();
    }

    public static List<DTOFieldModel> GetEntityIdFields(this ICollection<DTOFieldModel> fields, ClassModel entity, ISoftwareFactoryExecutionContext context)
    {
        var primaryKeys = entity.GetEntityIdAttributes(context);

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
        ClassModel entity, 
        ISoftwareFactoryExecutionContext executionContext)
    {
        var pks = entity.GetEntityIdAttributes(executionContext);

        return pks
            .Select(pk =>
            {
                var entityName = entity.Name.ToLowerInvariant();
                var pkAttributeName = pk.IdName.ToLowerInvariant();
                var fieldName = $"{entityName}{pkAttributeName.RemoveSuffix(entityName)}";

                return fields.SingleOrDefault(field => string.Equals(field.Name, fieldName, StringComparison.OrdinalIgnoreCase));
            })
            .ToList();
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