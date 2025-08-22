using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.MongoDb.Templates.MongoDbDocumentInterface;
using Intent.Modules.MongoDb.Templates.MongoDbValueObjectDocumentInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using AttributeModel = Intent.Modelers.Domain.Api.AttributeModel;

namespace Intent.Modules.MongoDb.Templates
{
    internal static class DocumentTemplateHelpers
    {
        public static string GetDocumentInterfaceName<T>(this CSharpTemplateBase<T> template, ITypeReference typeReference)
        {
            if (!template.TryGetTemplate<IClassProvider>(MongoDbDocumentInterfaceTemplate.TemplateId, typeReference.Element, out var classProvider))
            {
                if (!template.TryGetTemplate(MongoDbValueObjectDocumentInterfaceTemplate.TemplateId, typeReference.Element, out classProvider))
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

        public static void AddMongoDbMappingMethods<TModel>(
            this CSharpTemplateBase<TModel> template,
            CSharpClass @class,
            IReadOnlyList<AttributeModel> attributes,
            IReadOnlyList<AssociationEndModel> associationEnds,
            string entityInterfaceTypeName,
            string entityImplementationTypeName,
            bool entityRequiresReflectionConstruction,
            bool entityRequiresReflectionPropertySetting,
            bool isAggregate,
            bool hasBaseType,
            string primaryKeyType,
            string primaryKeyName)
        {
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

                    // If this is the PK which is not a string, we need to convert it:
                    //if (isAggregate && attribute.HasPrimaryKey() && !string.Equals(attributeTypeName, "string"))
                    //{
                    //    assignmentValueExpression = attributeTypeName switch
                    //    {
                    //        "int" or "long" => $"{attributeTypeName}.Parse({assignmentValueExpression}, {template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                    //        "guid" => $"{template.UseType("System.Guid")}.Parse({assignmentValueExpression})",
                    //        _ => throw new Exception(
                    //            $"Unsupported primary key type \"{attributeTypeName}\" [{attribute.TypeReference.Element?.Id}] for attribute " +
                    //            $"\"{attribute.Name}\" [{attribute.Id}] " +
                    //            $"on \"{attribute.InternalElement.ParentElement.Name}\" [{attribute.InternalElement.ParentElement.Id}]")
                    //    };
                    //}
                    //else 
                    if (attribute.TypeReference?.Element?.SpecializationType == "Value Object")
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
                        assignmentValueExpression = $"{associationEnd.Name}{nullable}.Select(x => (x as {associationEnd.Name.Singularize()}Document).ToEntity()).ToList()";
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

                foreach (var attribute in attributes)
                {
                    var suffix = string.Empty;
                    var accessEntityAttribute = true;

                    // If this is the PK which is not a string, we need to convert it:
                    //var attributeTypeName = attribute.TypeReference.Element?.Name.ToLowerInvariant();
                    //if (isAggregate && attribute.HasPrimaryKey() && !string.Equals(attributeTypeName, "string"))
                    //{
                    //    suffix = attributeTypeName switch
                    //    {
                    //        "int" or "long" => $".ToString({template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                    //        "guid" => ".ToString()",
                    //        _ => throw new Exception(
                    //            $"Unsupported primary key type \"{attributeTypeName}\" [{attribute.TypeReference.Element?.Id}] for attribute " +
                    //            $"\"{attribute.Name}\" [{attribute.Id}] " +
                    //            $"on \"{attribute.InternalElement.ParentElement.Name}\" [{attribute.InternalElement.ParentElement.Id}]")
                    //    };
                    //}
                    //else 
                    if (attribute.TypeReference?.Element?.SpecializationType == "Value Object")
                    {
                        var documentTypeName = template.GetTypeName((IElement)attribute.TypeReference.Element);
                        if (attribute.TypeReference.IsCollection)
                        {
                            suffix = $".Select(x => {documentTypeName}Document.FromEntity(x)!)";
                        }
                        else
                        {
                            var nullableSuppression = attribute.TypeReference.IsNullable ? string.Empty : "!";
                            suffix = $"{documentTypeName}Document.FromEntity(entity.{attribute.Name.ToPascalCase()}){nullableSuppression}";

                            accessEntityAttribute = false;
                        }
                    }

                    if (attribute.TypeReference.IsCollection)
                    {
                        template.AddUsing("System.Linq");
                        suffix = $"{suffix}{(attribute.TypeReference.IsNullable ? "?" : "")}.ToList()";
                    }

                    if (attribute.HasStereotype("64f6a994-4909-4a9d-a0a9-afc5adf2ef74")
                        && (primaryKeyType == Helpers.PrimaryKeyType || primaryKeyType == Helpers.PrimaryKeyTypeGuid)
                        && attribute.Stereotypes.First(s => s.DefinitionId == "64f6a994-4909-4a9d-a0a9-afc5adf2ef74").TryGetProperty("ce12cf69-e97f-401b-9b08-7e2c62171d4e", out var property))
                    {
                        if(property.Value == "Auto-generated")
                        {
                            method.AddStatement($"{attribute.Name.ToPascalCase()} = ObjectId.GenerateNewId().ToString();");
                        }
                        else
                        {
                            method.AddStatement($"{attribute.Name.ToPascalCase()} = {(accessEntityAttribute ? $"entity.{attribute.Name.ToPascalCase()}" : "")}{suffix};");
                        }
                    }
                    else
                    {
                        method.AddStatement($"{attribute.Name.ToPascalCase()} = {(accessEntityAttribute ? $"entity.{attribute.Name.ToPascalCase()}" : "")}{suffix};");
                    }
                }

                foreach (var associationEnd in associationEnds)
                {
                    var documentTypeName = template.GetTypeName((IElement)associationEnd.TypeReference.Element);

                    if (associationEnd.IsCollection)
                    {
                        template.AddUsing("System.Linq");

                        var nullable = associationEnd.IsNullable ? "?" : string.Empty;
                        method.AddStatement($"{associationEnd.Name} = entity.{associationEnd.Name}{nullable}.Select(x => {documentTypeName}Document.FromEntity(x)!).ToList();");
                        continue;
                    }

                    var nullableSuppression = associationEnd.IsNullable ? string.Empty : "!";
                    method.AddStatement($"{associationEnd.Name} = {documentTypeName}.FromEntity(entity.{associationEnd.Name}){nullableSuppression};");
                }

                if (hasBaseType)
                {
                    method.AddStatement($"base.PopulateFromEntity(entity);");
                }

                method.AddStatement("return this;", s => s.SeparatedFromPrevious());
            });

            if (!@class.IsAbstract && (!string.IsNullOrEmpty(primaryKeyType) || !string.IsNullOrEmpty(primaryKeyName)))
            {
                @class.AddMethod($"{@class.Name}{genericTypeArguments}?", "FromEntity", method =>
                {
                    method.AddParameter($"{entityInterfaceTypeName}{genericTypeArguments}?", "entity");

                    method.Static();

                    method.AddIfStatement("entity is null", @if => @if.AddStatement("return null;"));

                    method.AddStatement($"return new {@class.Name}{genericTypeArguments}().PopulateFromEntity(entity);", s => s.SeparatedFromPrevious());
                });

                @class.AddMethod($"FilterDefinition<{@class.Name}{genericTypeArguments}>", "GetIdFilter", method =>
                {
                    method.AddParameter(primaryKeyType, primaryKeyName.ToCamelCase());

                    method.Static();

                    method.AddStatement($"return Builders<{@class.Name}{genericTypeArguments}>.Filter.Eq(d => d.{primaryKeyName.ToPascalCase()}, {primaryKeyName.ToCamelCase()});", s => s.SeparatedFromPrevious());
                });

                @class.AddMethod($"FilterDefinition<{@class.Name}{genericTypeArguments}>", "GetIdsFilter", method =>
                {
                    method.AddParameter($"{primaryKeyType}[]", primaryKeyName.Pluralize().ToCamelCase());

                    method.Static();

                    method.AddStatement($"return Builders<{@class.Name}{genericTypeArguments}>.Filter.In(d => d.{primaryKeyName.ToPascalCase()}, {primaryKeyName.Pluralize().ToCamelCase()});", s => s.SeparatedFromPrevious());
                });

                @class.AddMethod($"FilterDefinition<{@class.Name}{genericTypeArguments}>", "GetIdFilter", method =>
                {
                    method.WithExpressionBody($"GetIdFilter({primaryKeyName.ToPascalCase()})");
                });
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

        public static void AddMongoDbDocumentProperties<TModel>(
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

                    if (attribute.HasFieldSettings())
                    {
                        property.AddAttribute($"{template.UseType("Newtonsoft.Json.JsonProperty")}(\"{attribute.GetFieldSettings().Name()}\")");
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

                    if (associationEnd is AssociationTargetEndModel targetEnd && targetEnd.HasFieldSettings())
                    {
                        property.AddAttribute($"{template.UseType("Newtonsoft.Json.JsonProperty")}(\"{targetEnd.GetFieldSettings().Name()}\")");
                    }
                });

                if (documentInterfaceTemplateId == null)
                {
                    continue;
                }

                template.AddDocumentInterfaceAccessor(@class, documentInterfaceTemplateId, associationEnd.TypeReference, associationEnd.Name.ToPascalCase());
            }
        }
    }
}
