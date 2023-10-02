﻿using System;
using System.Collections.Generic;
using System.Linq;
using Intent.CosmosDB.Api;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.CosmosDB.Templates
{
    internal static class DocumentTemplateHelpers
    {
        public static void AddCosmosDBDocumentProperties<TModel>(
            this CSharpTemplateBase<TModel> template,
            CSharpClass @class,
            IEnumerable<AttributeModel> attributes,
            IEnumerable<AssociationEndModel> associationEnds)
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
                });
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
            }
        }

        public static void AddCosmosDBMappingMethods(
            this ICSharpTemplate template,
            CSharpClass @class,
            IReadOnlyList<AttributeModel> attributes,
            IReadOnlyList<AssociationEndModel> associationEnds,
            string entityInterfaceTypeName,
            string entityStateTypeName,
            bool entityRequiresReflectionConstruction,
            bool entityRequiresReflectionPropertySetting,
            bool isAggregate,
            bool hasBaseType)
        {
            var genericTypeArguments = @class.GenericParameters.Any()
                ? $"<{string.Join(", ", @class.GenericParameters.Select(x => x.TypeName))}>"
                : string.Empty;

            @class.AddMethod($"{entityInterfaceTypeName}{genericTypeArguments}", "ToEntity", method =>
            {
                method.AddParameter($"{entityStateTypeName}{genericTypeArguments}?", "entity", p =>
                {
                    if (!@class.IsAbstract)
                    {
                        p.WithDefaultValue("default");
                    }
                });

                if (!@class.IsAbstract)
                {
                    var instantiation = entityRequiresReflectionConstruction
                        ? $"{template.GetReflectionHelperName()}.CreateNewInstanceOf<{entityStateTypeName}{genericTypeArguments}>()"
                        : $"new {entityStateTypeName}{genericTypeArguments}()";

                    method.AddStatement($"entity ??= {instantiation};");
                }
                else
                {
                    method.AddIfStatement("entity is null", @if =>
                    {
                        @if.AddStatement($"throw new {template.UseType("System.ArgumentNullException")}(nameof(entity));");
                    });
                }

                for (var index = 0; index < attributes.Count; index++)
                {
                    var attribute = attributes[index];
                    var assignmentValueExpression = attribute.Name;

                    // If this is the PK which is not a string, we need to convert it:
                    var attributeTypeName = attribute.TypeReference.Element?.Name.ToLowerInvariant();
                    if (isAggregate && attribute.HasPrimaryKey() && !string.Equals(attributeTypeName, "string"))
                    {
                        assignmentValueExpression = attributeTypeName switch
                        {
                            "int" or "long" => $"{attributeTypeName}.Parse({assignmentValueExpression}, {template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                            "guid" => $"{template.UseType("System.Guid")}.Parse({assignmentValueExpression})",
                            _ => throw new Exception(
                                $"Unsupported primary key type \"{attributeTypeName}\" [{attribute.TypeReference.Element?.Id}] for attribute " + $"\"{attribute.Name}\" [{attribute.Id}] " +
                                $"on \"{attribute.InternalElement.ParentElement.Name}\" [{attribute.InternalElement.ParentElement.Id}]")
                        };
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
                    var nullableSuppression = associationEnd.IsNullable || entityRequiresReflectionPropertySetting ? string.Empty : "!";

                    var assignmentValueExpression = $"{associationEnd.Name}{nullable}.ToEntity(){nullableSuppression}";

                    if (associationEnd.IsCollection)
                    {
                        assignmentValueExpression = $"{associationEnd.Name}{nullable}.Select(x => x.ToEntity()).ToList()";
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

                    // If this is the PK which is not a string, we need to convert it:
                    var attributeTypeName = attribute.TypeReference.Element?.Name.ToLowerInvariant();
                    if (isAggregate && attribute.HasPrimaryKey() && !string.Equals(attributeTypeName, "string"))
                    {
                        suffix = attributeTypeName switch
                        {
                            "int" or "long" => $".ToString({template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                            "guid" => ".ToString()",
                            _ => throw new Exception(
                                $"Unsupported primary key type \"{attributeTypeName}\" [{attribute.TypeReference.Element?.Id}] for attribute " + $"\"{attribute.Name}\" [{attribute.Id}] " +
                                $"on \"{attribute.InternalElement.ParentElement.Name}\" [{attribute.InternalElement.ParentElement.Id}]")
                        };
                    }

                    method.AddStatement($"{attribute.Name} = entity.{attribute.Name}{suffix};");
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

                if (hasBaseType)
                {
                    method.AddStatement("base.PopulateFromEntity(entity);");
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
                    method.AddStatement($"return new {@class.Name}{genericTypeArguments}().PopulateFromEntity(entity);", s => s.SeparatedFromPrevious());
                });
            }
        }
    }
}