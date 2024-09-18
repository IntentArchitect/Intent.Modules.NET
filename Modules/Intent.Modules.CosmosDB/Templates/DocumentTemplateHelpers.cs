using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.CosmosDB.Api;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.CosmosDB.Settings;
using Intent.Modules.CosmosDB.Templates.CosmosDBDocumentInterface;
using Intent.Modules.CosmosDB.Templates.CosmosDBValueObjectDocument;
using Intent.Modules.CosmosDB.Templates.CosmosDBValueObjectDocumentInterface;

namespace Intent.Modules.CosmosDB.Templates
{
    internal static class DocumentTemplateHelpers
    {
        public static void AddCosmosDBDocumentProperties<TModel>(
            this CSharpTemplateBase<TModel> template,
            CSharpClass @class,
            IEnumerable<AttributeModel> attributes,
            IEnumerable<AssociationEndModel> associationEnds,
            string documentInterfaceTemplateId = null)
            where TModel : IMetadataModel
        {
            foreach (var attribute in attributes)
            {
                @class.AddProperty(template.GetTypeName(attribute.TypeReference), attribute.Name.ToPascalCase(), property =>
                {
                    if (template.IsNonNullableReferenceType(attribute.TypeReference))
                    {
                        property.WithInitialValue("default!");
                    }

                    if (attribute.HasFieldSetting())
                    {
                        property.AddAttribute($"{template.UseType("Newtonsoft.Json.JsonProperty")}(\"{attribute.GetFieldSetting().Name()}\")");
                    }

                    if (attribute.TypeReference?.Element?.IsEnumModel() == true && template.ExecutionContext.Settings.GetCosmosDb().StoreEnumsAsStrings())
                    {
                        property.AddAttribute($"{template.UseType("Newtonsoft.Json.JsonConverter")}(typeof({template.GetEnumJsonConverterName()}))");
                    }

                    if (attribute.TypeReference?.Element?.SpecializationType == "Value Object")
                    {
                        template.AddDocumentInterfaceAccessor(@class, documentInterfaceTemplateId, attribute.TypeReference, attribute.Name.ToPascalCase());
                    }
                });

                if (attribute.TypeReference.IsCollection)
                {
                    @class.AddProperty(
                        type: $"{template.UseType("System.Collections.Generic.IReadOnlyList")}<{template.GetTypeName((IElement)attribute.TypeReference.Element)}>",
                        name: attribute.Name.ToPascalCase(),
                        configure: property =>
                        {
                            property.ExplicitlyImplements(template.GetTypeName(documentInterfaceTemplateId, template.Model));
                            property.Getter.WithExpressionImplementation(attribute.Name.ToPascalCase());
                            property.WithoutSetter();
                        });
                }
            }

            foreach (var associationEnd in associationEnds)
            {
                @class.AddProperty(template.GetTypeName(associationEnd), associationEnd.Name.ToPascalCase(), property =>
                {
                    if (!associationEnd.TypeReference.IsNullable)
                    {
                        property.WithInitialValue("default!");
                    }

                    if (associationEnd is AssociationTargetEndModel targetEnd && targetEnd.HasFieldSetting())
                    {
                        property.AddAttribute($"{template.UseType("Newtonsoft.Json.JsonProperty")}(\"{targetEnd.GetFieldSetting().Name()}\")");
                    }
                });

                if (documentInterfaceTemplateId == null)
                {
                    continue;
                }
                
                template.AddDocumentInterfaceAccessor(@class, documentInterfaceTemplateId, associationEnd.TypeReference, associationEnd.Name.ToPascalCase());
            }
        }

        private static void AddDocumentInterfaceAccessor<TModel>(
            this CSharpTemplateBase<TModel> template,
            CSharpClass @class,
            string documentInterfaceTemplateId,
            ITypeReference elementReference,
            string entityPropertyName)
            where TModel : IMetadataModel
        {
            @class.AddProperty(template.GetDocumentInterfaceName(elementReference), entityPropertyName,
                property =>
                {
                    property.ExplicitlyImplements(template.GetTypeName(documentInterfaceTemplateId, template.Model));
                    property.Getter.WithExpressionImplementation(entityPropertyName);
                    property.WithoutSetter();
                });
        }

        public static string GetDocumentInterfaceName<T>(this CSharpTemplateBase<T> template, ITypeReference typeReference)
        {
            if (!template.TryGetTemplate<IClassProvider>(CosmosDBDocumentInterfaceTemplate.TemplateId, typeReference.Element, out var classProvider))
            {
                if (!template.TryGetTemplate(CosmosDBValueObjectDocumentInterfaceTemplate.TemplateId, typeReference.Element, out classProvider))
                {
                    throw new Exception($"No Interface template found for {typeReference.Element.Name}.");
                }
            }

            var typeName = template.UseType(classProvider.FullTypeName());

            if (typeReference.IsCollection)
            {
                typeName = template.UseType($"System.Collections.Generic.IReadOnlyList<{typeName}>");
            }

            return typeName;
        }

        internal static bool IsSeparateDatabaseMultiTenancy(IApplicationSettingsProvider settings)
        {
            const string multiTenancySettings = "41ae5a02-3eb2-42a6-ade2-322b3c1f1115";
            const string dataIsolationSetting = "be7c671e-bbef-4d75-b42d-a6547de3ae82";

            return settings.GetGroup(multiTenancySettings)?.GetSetting(dataIsolationSetting)?.Value == "separate-database";
        }

        public static void AddCosmosDBMappingMethods<TModel>(
            this CSharpTemplateBase<TModel> template,
            CSharpClass @class,
            IReadOnlyList<AttributeModel> attributes,
            IReadOnlyList<AssociationEndModel> associationEnds,
            AttributeModel partitionKeyAttribute,
            string entityInterfaceTypeName,
            string entityImplementationTypeName,
            bool entityRequiresReflectionConstruction,
            bool entityRequiresReflectionPropertySetting,
            bool isAggregate,
            bool hasBaseType)
        {
            var useOptimisticConcurrency = template.ExecutionContext.Settings.GetCosmosDb().UseOptimisticConcurrency();
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

                    var attributeTypeName = attribute.TypeReference.Element?.Name.ToLowerInvariant();
                    // Partition keys must be strings
                    if (isAggregate && partitionKeyAttribute != null && partitionKeyAttribute.Id == attribute.Id && !string.Equals(attributeTypeName, "string"))
                    {
                        assignmentValueExpression = attributeTypeName switch
                        {
                            "int" or "long" => $"{attributeTypeName}.Parse({assignmentValueExpression}, {template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                            "guid" => $"{template.UseType("System.Guid")}.Parse({assignmentValueExpression})",
                            _ => throw new Exception(
                                $"Unsupported partition key type \"{attributeTypeName}\" [{attribute.TypeReference.Element?.Id}] for attribute " +
                                $"\"{attribute.Name}\" [{attribute.Id}] " +
                                $"on \"{attribute.InternalElement.ParentElement.Name}\" [{attribute.InternalElement.ParentElement.Id}]")
                        };
                    }
                    // If this is the PK which is not a string, we need to convert it:
                    else if (isAggregate && attribute.HasPrimaryKey() && !string.Equals(attributeTypeName, "string"))
                    {
                        assignmentValueExpression = attributeTypeName switch
                        {
                            "int" or "long" => $"{attributeTypeName}.Parse({assignmentValueExpression}, {template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                            "guid" => $"{template.UseType("System.Guid")}.Parse({assignmentValueExpression})",
                            _ => throw new Exception(
                                $"Unsupported primary key type \"{attributeTypeName}\" [{attribute.TypeReference.Element?.Id}] for attribute " +
                                $"\"{attribute.Name}\" [{attribute.Id}] " +
                                $"on \"{attribute.InternalElement.ParentElement.Name}\" [{attribute.InternalElement.ParentElement.Id}]")
                        };
                    }
                    else if (attribute.TypeReference?.Element?.SpecializationType == "Value Object")
                    {
                        var nullable = attribute.TypeReference.IsNullable ? "?" : string.Empty;
                        assignmentValueExpression = $"{attribute.Name.ToPascalCase()}{nullable}.ToEntity()";

                        if (attribute.TypeReference.IsCollection)
                        {
                            assignmentValueExpression = $"{attribute.Name.ToPascalCase()}{nullable}.Select(x => x.ToEntity()).ToList()";
                        }
                    }

                    if (template.IsNonNullableReferenceType(attribute.TypeReference))
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

                if (useOptimisticConcurrency && template.Id != CosmosDBValueObjectDocumentTemplate.TemplateId && isAggregate)
                {
                    method.AddParameter("Func<string, string?>", "getEtag");
                }

                foreach (var attribute in attributes)
                {
                    var suffix = string.Empty;
                    var accessEntityAttribute = true;

                    // If this is the PK which is not a string, we need to convert it:
                    var attributeTypeName = attribute.TypeReference.Element?.Name.ToLowerInvariant();
                    // Partition keys must be strings
                    if (isAggregate && partitionKeyAttribute != null && partitionKeyAttribute.Id == attribute.Id && !string.Equals(attributeTypeName, "string"))
                    {
                        suffix = attributeTypeName switch
                        {
                            "int" or "long" => $".ToString({template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                            "guid" => ".ToString()",
                            _ => throw new Exception(
                                $"Unsupported partition key type \"{attributeTypeName}\" [{attribute.TypeReference.Element?.Id}] for attribute " +
                                $"\"{attribute.Name}\" [{attribute.Id}] " +
                                $"on \"{attribute.InternalElement.ParentElement.Name}\" [{attribute.InternalElement.ParentElement.Id}]")
                        };
                    }
                    else if (isAggregate && attribute.HasPrimaryKey() && !string.Equals(attributeTypeName, "string"))
                    {
                        suffix = attributeTypeName switch
                        {
                            "int" or "long" => $".ToString({template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                            "guid" => ".ToString()",
                            _ => throw new Exception(
                                $"Unsupported primary key type \"{attributeTypeName}\" [{attribute.TypeReference.Element?.Id}] for attribute " +
                                $"\"{attribute.Name}\" [{attribute.Id}] " +
                                $"on \"{attribute.InternalElement.ParentElement.Name}\" [{attribute.InternalElement.ParentElement.Id}]")
                        };
                    }
                    else if (attribute.TypeReference?.Element?.SpecializationType == "Value Object")
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
                        }

                        accessEntityAttribute = false;
                    }

                    if (attribute.TypeReference.IsCollection)
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

                if (useOptimisticConcurrency && template.Id != CosmosDBValueObjectDocumentTemplate.TemplateId && isAggregate)
                {
                    method.AddStatement($"_etag = _etag == null ? getEtag((({template.UseType("Microsoft.Azure.CosmosRepository.IItem")})this).Id) : _etag;",
                        s => s.SeparatedFromPrevious());
                }

                if (hasBaseType)
                {
                    var getEtagArgument = useOptimisticConcurrency
                        ? ", getEtag"
                        : string.Empty;
                    method.AddStatement($"base.PopulateFromEntity(entity{getEtagArgument});");
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

                    if (useOptimisticConcurrency && template.Id != CosmosDBValueObjectDocumentTemplate.TemplateId && isAggregate)
                    {
                        method.AddParameter("Func<string, string?>", "getEtag");
                        method.AddStatement($"return new {@class.Name}{genericTypeArguments}().PopulateFromEntity(entity, getEtag);", s => s.SeparatedFromPrevious());
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