﻿using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates;

public static class MappingMethodHelper
{
    public static void AddDtoToDomainMappingMethods(this ICSharpFileBuilderTemplate template, CSharpClass builderClass, CommandModel commandModel, ClassModel domainModel, bool skipIdFields = true)
    {
        builderClass.AddMethod(template.GetTypeName(domainModel.InternalElement), GetCreateMethodName(domainModel.InternalElement), method =>
        {
            method.Private();
            method.Static();
            method.AddParameter(template.GetTypeName(commandModel.InternalElement), "dto");
            method.AddStatementBlock($"return new {template.GetTypeName(domainModel.InternalElement)}", block =>
            {
                block.AddStatements(GetDtoToDomainPropertyAssignments(template, "", "dto", domainModel, commandModel.Properties, skipIdFields));
                block.WithSemicolon();
            });
        });
    }

    public static void AddDomainToDtoMappingMethods(this ICSharpFileBuilderTemplate template, CSharpClass builderClass, ClassModel domainModel, DTOModel dtoModel)
    {
        builderClass.AddMethod(template.GetTypeName(dtoModel.InternalElement), GetCreateMethodName(dtoModel.InternalElement), method =>
        {
            method.Private();
            method.Static();
            method.AddParameter(template.GetTypeName(domainModel.InternalElement), "entity");
            method.AddStatementBlock($"return new {template.GetTypeName(dtoModel.InternalElement)}", block =>
            {
                block.AddStatements(GetDomainToDtoPropertyAssignments(template, "", "entity", dtoModel, domainModel));
                block.WithSemicolon();
            });
        });
    }

    private static IEnumerable<CSharpStatement> GetDtoToDomainPropertyAssignments(
        ICSharpFileBuilderTemplate template, 
        string entityVarName, 
        string dtoVarName, 
        ClassModel domainModel,
        IList<DTOFieldModel> dtoFields,
        bool skipIdFields)
    {
        var codeLines = new List<CSharpStatement>();
        foreach (var field in dtoFields)
        {
            var entityVarExpr = !string.IsNullOrWhiteSpace(entityVarName) ? $"{entityVarName}." : string.Empty;
            
            // Implicit ID case check
            if (!skipIdFields &&
                field.Mapping?.Element == null &&
                field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                codeLines.Add($"{entityVarExpr}Id = {dtoVarName}.{field.Name.ToPascalCase()},");
                continue;
            }

            // Implicit Owner ID case check
            ClassModel ownerEntity;
            if (field.Mapping?.Element == null &&
                (ownerEntity = domainModel.GetNestedCompositionalOwner()) != null &&
                field.Name.Equals($"{ownerEntity.Name}id", StringComparison.OrdinalIgnoreCase))
            {
                codeLines.Add($"{entityVarExpr}{ownerEntity.Name}Id = {dtoVarName}.{field.Name.ToPascalCase()},");
                continue;
            }

            if (field.Mapping?.Element == null && 
                domainModel.Attributes.All(p => p.Name != field.Name))
            {
                continue;
            }

            switch (field.Mapping?.Element?.SpecializationTypeId)
            {
                default:
                case AttributeModel.SpecializationTypeId:
                    var attribute = field.Mapping?.Element?.AsAttributeModel()
                                    ?? domainModel.Attributes.First(p => p.Name == field.Name);
                    if (!skipIdFields || !attribute.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()},");
                        break;
                    }

                    break;
                case AssociationTargetEndModel.SpecializationTypeId:
                {
                    
                    var association = field.Mapping.Element.AsAssociationTargetEndModel();
                    if (association.Association.AssociationType == AssociationType.Aggregation)
                    {
                        codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                        break;
                    }
                    
                    var targetType = association.Element.AsClassModel();
                    var attributeName = association.Name.ToPascalCase();
                    var fieldDtoModel = field.TypeReference.Element.AsDTOModel();

                    if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                    {
                        codeLines.Add($"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()} != null ? {GetCreateMethodName(targetType.InternalElement, attributeName)}({dtoVarName}.{field.Name.ToPascalCase()}) : null,");
                    }
                    else
                    {
                        codeLines.Add($"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()}?.Select({GetCreateMethodName(targetType.InternalElement, attributeName)}).ToList() ?? new List<{targetType.Name.ToPascalCase()}>(),");
                    }

                    var @class = template.CSharpFile.Classes.First();
                    @class.AddMethod(template.GetTypeName(targetType.InternalElement),
                        GetCreateMethodName(targetType.InternalElement, attributeName),
                        method => method.Private().Static()
                            .AddParameter(template.GetTypeName((IElement)field.TypeReference.Element), "dto")
                            .AddStatement($"return new {targetType.Name.ToPascalCase()}")
                            .AddStatement(new CSharpStatementBlock()
                                .AddStatements(GetDtoToDomainPropertyAssignments(template, "", $"dto", targetType, fieldDtoModel.Fields, skipIdFields))
                                .WithSemicolon()));
                }
                    break;
            }
        }

        return codeLines;
    }
    
   private static IEnumerable<CSharpStatement> GetDomainToDtoPropertyAssignments(
        ICSharpFileBuilderTemplate template, 
        string dtoVarName, 
        string entityVarName, 
        DTOModel dtoModel,
        ClassModel domainModel)
    {
        var codeLines = new List<CSharpStatement>();
        foreach (var field in dtoModel.Fields)
        {
            var dtoVarExpr = !string.IsNullOrWhiteSpace(dtoVarName) ? $"{dtoVarName}." : string.Empty;
            
            // Implicit ID case check
            if (field.Mapping?.Element == null &&
                field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                codeLines.Add($"{dtoVarExpr}{field.Name.ToPascalCase()} = {entityVarName}.Id,");
                continue;
            }
            
            // Implicit Owner ID case check
            ClassModel ownerEntity;
            if (field.Mapping?.Element == null &&
                (ownerEntity = domainModel.GetNestedCompositionalOwner()) != null &&
                field.Name.Equals($"{ownerEntity.Name}id", StringComparison.OrdinalIgnoreCase))
            {
                codeLines.Add($"{dtoVarExpr}{field.Name.ToPascalCase()} = {entityVarName}.{ownerEntity.Name}Id,");
                continue;
            }
            
            if (field.Mapping?.Element == null
                && domainModel.Attributes.All(p => p.Name != field.Name))
            {
                continue;
            }
            
            switch (field.Mapping?.Element?.SpecializationTypeId)
            {
                default:
                case AttributeModel.SpecializationTypeId:
                    var attribute = field.Mapping?.Element?.AsAttributeModel()
                                    ?? domainModel.Attributes.First(p => p.Name == field.Name);
                    codeLines.Add($"{dtoVarExpr}{field.Name.ToPascalCase()} = {entityVarName}.{attribute.Name.ToPascalCase()},");

                    break;
                case AssociationTargetEndModel.SpecializationTypeId:
                {
                    var association = field.Mapping.Element.AsAssociationTargetEndModel();
                    var targetType = association.Element.AsClassModel();
                    var attributeName = association.Name.ToPascalCase();
                    var fieldDtoModel = field.TypeReference.Element.AsDTOModel();

                    if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                    {
                        codeLines.Add($"{dtoVarExpr}{field.Name.ToPascalCase()} = {entityVarName}.{attributeName} != null ? {GetCreateMethodName(targetType.InternalElement, attributeName)}({entityVarName}.{attributeName}) : null,");
                    }
                    else
                    {
                        codeLines.Add($"{dtoVarExpr}{field.Name.ToPascalCase()} = {entityVarName}.{attributeName}?.Select({GetCreateMethodName(targetType.InternalElement, attributeName)}).ToList() ?? new List<{fieldDtoModel.Name.ToPascalCase()}>(),");
                    }

                    var @class = template.CSharpFile.Classes.First();
                    @class.AddMethod(template.GetTypeName(fieldDtoModel.InternalElement),
                        GetCreateMethodName(targetType.InternalElement, attributeName),
                        method => method.Private().Static()
                            .AddParameter(targetType.Name.ToPascalCase(), "entity")
                            .AddStatement($"return new {template.GetTypeName(fieldDtoModel.InternalElement)}")
                            .AddStatement(new CSharpStatementBlock()
                                .AddStatements(GetDomainToDtoPropertyAssignments(
                                    template: template, 
                                    dtoVarName: "", 
                                    entityVarName: $"entity",
                                    dtoModel: fieldDtoModel,
                                    domainModel: targetType))
                                .WithSemicolon()));
                }
                    break;
            }
        }

        return codeLines;
    }

    private static string GetCreateMethodName(IElement targetType, string attributeName = "")
    {
        return $"CreateExpected{targetType.Name.ToPascalCase()}";
    }
}