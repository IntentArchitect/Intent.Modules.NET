using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Aws.DynamoDB.Api;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Aws.DynamoDB.Settings;
using Intent.Modules.Aws.DynamoDB.Templates.DynamoDBValueObjectDocument;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.Modelers.Domain.Settings;
using AttributeModel = Intent.Modelers.Domain.Api.AttributeModel;

namespace Intent.Modules.Aws.DynamoDB.Templates
{
    internal static class DocumentTemplateHelpers
    {
        public static void AddDynamoDBDocumentProperties<TModel>(
            this CSharpTemplateBase<TModel> template,
            CSharpClass @class,
            IEnumerable<AttributeModel> attributes,
            IEnumerable<AssociationEndModel> associationEnds,
            string entityTypeName)
            where TModel : IMetadataModel
        {
            var useOptimisticConcurrency = template.ExecutionContext.Settings.GetDynamoDBSettings().UseOptimisticConcurrency();

            var isAggregateRoot = false;
            var isDerived = false;
            AttributeModelExtensionMethods.PrimaryKeyData? pks = null;
            AttributeModel? versionAttribute = null;

            if (template.Model is ClassModel classModel)
            {
                if (classModel.IsAggregateRoot())
                {
                    isAggregateRoot = true;
                    pks = classModel.GetPrimaryKeyData();
                    versionAttribute = classModel.GetVersionAttribute();

                    var entityGenericTypeArguments = @class.GenericParameters.Any()
                        ? $"<{string.Join(", ", @class.GenericParameters.Select(x => x.TypeName))}>"
                        : string.Empty;

                    var interfaceTypeName = template.GetDynamoDBDocumentOfTInterfaceName();

                    var entityStateTypeName = template.ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces()
                        ? $", {template.GetTypeName(TemplateRoles.Domain.Entity.Primary, classModel)}{entityGenericTypeArguments}"
                        : string.Empty;

                    @class.ImplementsInterface($"{interfaceTypeName}<{entityTypeName}{entityGenericTypeArguments}{entityStateTypeName}, {@class.Name}{entityGenericTypeArguments}>");

                    var tableName = classModel.TryGetTable(out var tableStereotype) && !string.IsNullOrWhiteSpace(tableStereotype.Name())
                        ? tableStereotype.Name()
                        : classModel.Name.ToPascalCase();
                    @class.AddAttribute($"{template.UseType("Amazon.DynamoDBv2.DataModel.DynamoDBTable")}(\"{tableName.Pluralize().ToKebabCase()}\")");
                }

                isDerived = classModel.ParentClass != null;
            }

            foreach (var attribute in attributes)
            {
                @class.AddProperty(template.GetTypeName(attribute.TypeReference), attribute.Name.ToPascalCase(), property =>
                {
                    if (template.IsNonNullableReferenceType(attribute.TypeReference))
                    {
                        property.WithInitialValue("default!");
                    }

                    if (attribute.Id == pks?.PartitionKeyAttribute.Id)
                    {
                        property.AddAttribute(template.UseType("Amazon.DynamoDBv2.DataModel.DynamoDBHashKey"));
                    }

                    if (attribute.Id == pks?.SortKeyAttribute?.Id)
                    {
                        property.AddAttribute(template.UseType("Amazon.DynamoDBv2.DataModel.DynamoDBRangeKey"));
                    }

                    if (attribute.Id == versionAttribute?.Id)
                    {
                        property.AddAttribute(template.UseType("Amazon.DynamoDBv2.DataModel.DynamoDBVersion"));
                    }

                    // DynamoDBProperty attribute:
                    {
                        var dbPropertyAttributeArguments = new List<string>();

                        if (attribute.TryGetDynamoDBAttributeName(out var name))
                        {
                            dbPropertyAttributeArguments.Add($"\"{name}\"");
                        }

                        if (attribute.TypeReference?.Element?.IsEnumModel() == true && template.ExecutionContext.Settings.GetDynamoDBSettings().StoreEnumsAsStrings())
                        {
                            dbPropertyAttributeArguments.Add($"typeof({template.GetEnumAsStringConverterName()}<{template.GetTypeName(attribute.TypeReference)}>)");
                        }

                        if (dbPropertyAttributeArguments.Count > 0)
                        {
                            property.AddAttribute($"{template.UseType("Amazon.DynamoDBv2.DataModel.DynamoDBProperty")}({string.Join(", ", dbPropertyAttributeArguments)})");
                        }
                    }
                });
            }

            if (pks != null)
            {
                var expression = pks.SortKeyAttribute != null
                    ? $"({pks.PartitionKeyAttribute.Name.ToPascalCase()}, {pks.SortKeyAttribute.Name.ToPascalCase()})"
                    : pks.PartitionKeyAttribute.Name.ToPascalCase();

                @class.AddMethod("object", "GetKey", m => m.WithExpressionBody(expression));
            }

            if (isAggregateRoot && useOptimisticConcurrency)
            {
                if (versionAttribute == null && !isDerived)
                {
                    @class.AddProperty("int?", GetVersionPropertyName(versionAttribute), property =>
                    {
                        property.AddAttribute(template.UseType("Amazon.DynamoDBv2.DataModel.DynamoDBVersion"));
                    });
                }

                @class.AddMethod("int?", "GetVersion", m => m.WithExpressionBody($"{GetVersionPropertyName(versionAttribute)}"));
            }

            foreach (var associationEnd in associationEnds)
            {
                @class.AddProperty(template.GetTypeName(associationEnd), associationEnd.Name.ToPascalCase(), property =>
                {
                    if (!associationEnd.TypeReference.IsNullable)
                    {
                        property.WithInitialValue("default!");
                    }

                    if (associationEnd is AssociationTargetEndModel targetEnd && targetEnd.HasFieldSettings())
                    {
                        property.AddAttribute($"{template.UseType("Amazon.DynamoDBv2.DataModel.DynamoDBProperty")}(\"{targetEnd.GetFieldSettings().Name()}\")");
                    }
                });
            }
        }

        private static string GetVersionPropertyName(AttributeModel? versionAttribute)
        {
            return versionAttribute?.Name.ToPascalCase() ?? "Version";
        }

        public static bool TryGetDynamoDBAttributeName(this AttributeModel model, [NotNullWhen(true)]out string? name)
        {
            name = model.GetFieldSettings()?.Name();
            if (!string.IsNullOrWhiteSpace(name))
            {
                return true;
            }

            name = null;
            return false;

        }

        public static void AddDynamoDBMappingMethods<TModel>(
            this CSharpTemplateBase<TModel> template,
            CSharpClass @class,
            IReadOnlyList<AttributeModel> attributes,
            IReadOnlyList<AssociationEndModel> associationEnds,
            AttributeModel? partitionKeyAttribute,
            string entityInterfaceTypeName,
            string entityImplementationTypeName,
            bool entityRequiresReflectionConstruction,
            bool entityRequiresReflectionPropertySetting,
            bool isAggregate,
            bool hasBaseType)
        {
            var useOptimisticConcurrency = template.ExecutionContext.Settings.GetDynamoDBSettings().UseOptimisticConcurrency();
            var genericTypeArguments = @class.GenericParameters.Any()
                ? $"<{string.Join(", ", @class.GenericParameters.Select(x => x.TypeName))}>"
                : string.Empty;

            @class.AddMethod($"{entityImplementationTypeName}{genericTypeArguments}", "ToEntity", method =>
            {
                method.AddParameter($"{entityImplementationTypeName}{genericTypeArguments}?", "entity", p =>
                {
                    if (!@class.IsAbstract)
                    {
                        p.WithDefaultValue("default");
                    }
                });

                if (!@class.IsAbstract)
                {
                    var instantiation = entityRequiresReflectionConstruction
                        ? $"{template.GetReflectionHelperName()}.CreateNewInstanceOf<{entityImplementationTypeName}{genericTypeArguments}>()"
                        : $"new {entityImplementationTypeName}{genericTypeArguments}()";

                    method.AddStatement($"entity ??= {instantiation};");
                }
                else
                {
                    method.AddIfStatement("entity is null", @if => { @if.AddStatement($"throw new {template.UseType("System.ArgumentNullException")}(nameof(entity));"); });
                }

                for (var index = 0; index < attributes.Count; index++)
                {
                    var attribute = attributes[index];
                    var assignmentValueExpression = attribute.Name.ToPascalCase();

                    if (attribute.TypeReference?.Element?.SpecializationType == "Value Object")
                    {
                        var nullable = attribute.TypeReference.IsNullable ? "?" : string.Empty;
                        assignmentValueExpression = $"{attribute.Name.ToPascalCase()}{nullable}.ToEntity()";

                        if (attribute.TypeReference.IsCollection)
                        {
                            assignmentValueExpression = $"{attribute.Name.ToPascalCase()}{nullable}.Select(x => x.ToEntity()).ToList()";
                        }
                    }

                    if (attribute.TypeReference != null && template.IsNonNullableReferenceType(attribute.TypeReference))
                    {
                        assignmentValueExpression =
                            $"{assignmentValueExpression} ?? throw new {template.UseType("System.Exception")}($\"{{nameof(entity.{attribute.Name.ToPascalCase()})}} is null\")";
                    }

                    method.AddStatement(
                        entityRequiresReflectionPropertySetting
                            ? $"{template.GetReflectionHelperName()}.ForceSetProperty(entity, nameof({attribute.Name.ToPascalCase()}), {assignmentValueExpression});"
                            : $"entity.{attribute.Name.ToPascalCase()} = {assignmentValueExpression};",
                        index == 0 ? s => s.SeparatedFromPrevious() : null);
                }

                foreach (var associationEnd in associationEnds)
                {
                    var nullable = associationEnd.IsNullable ? "?" : string.Empty;

                    var assignmentValueExpression = $"{associationEnd.Name}{nullable}.ToEntity()";

                    if (associationEnd.IsCollection)
                    {
                        assignmentValueExpression = $"{associationEnd.Name}{nullable}.Select(x => x.ToEntity()).ToList()";
                    }
                    else if (!associationEnd.IsNullable)
                    {
                        assignmentValueExpression =
                            $"{assignmentValueExpression} ?? throw new {template.UseType("System.Exception")}($\"{{nameof(entity.{associationEnd.Name.ToPascalCase()})}} is null\")";
                    }

                    method.AddStatement(entityRequiresReflectionPropertySetting
                        ? $"{template.GetReflectionHelperName()}.ForceSetProperty(entity, nameof({associationEnd.Name.ToPascalCase()}), {assignmentValueExpression});"
                        : $"entity.{associationEnd.Name.ToPascalCase()} = {assignmentValueExpression};");
                }

                if (hasBaseType)
                {
                    method.AddStatement("base.ToEntity(entity);");
                }

                method.AddStatement("return entity;", s => s.SeparatedFromPrevious());
            });

            @class.AddMethod($"{@class.Name}{genericTypeArguments}", "PopulateFromEntity", method =>
            {
                method.AddParameter($"{entityInterfaceTypeName}{genericTypeArguments}", "entity");

                if (useOptimisticConcurrency && template.Id != DynamoDBValueObjectDocumentTemplate.TemplateId && isAggregate)
                {
                    method.AddParameter("Func<object, int?>", "getVersion");
                }

                foreach (var attribute in attributes)
                {
                    var suffix = string.Empty;
                    var accessEntityAttribute = true;

                    if (attribute.TypeReference?.Element?.SpecializationType == "Value Object")
                    {
                        var documentTypeName = template.GetTypeName((IElement)attribute.TypeReference.Element);
                        if (attribute.TypeReference.IsCollection)
                        {
                            suffix = $".Select(x => {documentTypeName}.FromEntity(x)!)";
                        }
                        else
                        {
                            var nullableSuppression = attribute.TypeReference.IsNullable ? string.Empty : "!";
                            suffix = $"{documentTypeName}.FromEntity(entity.{attribute.Name.ToPascalCase()}){nullableSuppression}";

                            accessEntityAttribute = false;
                        }
                    }

                    if (attribute.TypeReference?.IsCollection == true)
                    {
                        template.AddUsing("System.Linq");
                        suffix = $"{suffix}{(attribute.TypeReference.IsNullable ? "?" : "")}.ToList()";
                    }

                    method.AddStatement($"{attribute.Name.ToPascalCase()} = {(accessEntityAttribute ? $"entity.{attribute.Name.ToPascalCase()}" : "")}{suffix};");
                }

                foreach (var associationEnd in associationEnds)
                {
                    var documentTypeName = template.GetTypeName((IElement)associationEnd.TypeReference.Element);

                    if (associationEnd.IsCollection)
                    {
                        template.AddUsing("System.Linq");

                        var nullable = associationEnd.IsNullable ? "?" : string.Empty;
                        method.AddStatement($"{associationEnd.Name} = entity.{associationEnd.Name}{nullable}.Select(x => {documentTypeName}.FromEntity(x)!).ToList();");
                        continue;
                    }

                    var nullableSuppression = associationEnd.IsNullable ? string.Empty : "!";
                    method.AddStatement($"{associationEnd.Name} = {documentTypeName}.FromEntity(entity.{associationEnd.Name}){nullableSuppression};");
                }

                if (useOptimisticConcurrency && template.Id != DynamoDBValueObjectDocumentTemplate.TemplateId && isAggregate && !hasBaseType)
                {
                    var classModel = template.Model as ClassModel ?? throw new InvalidOperationException("Could not convert to ClassModel");
                    var versionPropertyName = GetVersionPropertyName(classModel.GetVersionAttribute());

                    method.AddStatement($"{versionPropertyName} ??= getVersion(GetKey());");
                }

                if (hasBaseType)
                {
                    var getVersionArgument = useOptimisticConcurrency
                        ? ", getVersion"
                        : string.Empty;
                    method.AddStatement($"base.PopulateFromEntity(entity{getVersionArgument});");
                }

                method.AddStatement("return this;", s => s.SeparatedFromPrevious());
            });

            if (!@class.IsAbstract)
            {
                @class.AddMethod($"{@class.Name}{genericTypeArguments}?", "FromEntity", method =>
                {
                    method.AddParameter($"{entityInterfaceTypeName}{genericTypeArguments}?", "entity");

                    method.Static();

                    method.AddIfStatement("entity is null", @if => @if.AddStatement("return null;"));

                    if (useOptimisticConcurrency && template.Id != DynamoDBValueObjectDocumentTemplate.TemplateId && isAggregate)
                    {
                        method.AddParameter("Func<object, int?>", "getVersion");
                        method.AddStatement($"return new {@class.Name}{genericTypeArguments}().PopulateFromEntity(entity, getVersion);", s => s.SeparatedFromPrevious());
                    }
                    else
                    {
                        method.AddStatement($"return new {@class.Name}{genericTypeArguments}().PopulateFromEntity(entity);", s => s.SeparatedFromPrevious());
                    }
                });
            }
        }
    }
}