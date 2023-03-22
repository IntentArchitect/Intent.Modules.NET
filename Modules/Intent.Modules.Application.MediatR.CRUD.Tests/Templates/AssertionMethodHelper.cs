using System;
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

public static class AssertionMethodHelper
{
    public static void AddAssertionMappingMethods(this ICSharpFileBuilderTemplate template, CSharpClass builderClass, CommandModel commandModel, ClassModel domainModel, bool skipIdFields = true)
    {
        builderClass.AddMethod("void", GetAssertionMethodName(domainModel.InternalElement), method =>
        {
            method.Static();
            method.AddParameter(template.GetTypeName(commandModel.InternalElement), "expectedDto");
            method.AddParameter(template.GetTypeName(domainModel.InternalElement), "actualEntity");
            method.AddStatements(GetDtoToDomainPropertyAssignments(template, "actualEntity", "expectedDto", domainModel, commandModel.Properties, skipIdFields));
        });
    }

    public static void AddDomainToDtoMappingMethods(this ICSharpFileBuilderTemplate template, CSharpClass builderClass, ClassModel domainModel, DTOModel dtoModel)
    {
        builderClass.AddMethod(template.GetTypeName(dtoModel.InternalElement), GetAssertionMethodName(dtoModel.InternalElement), method =>
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
        string actualEntityVarName, 
        string expectedDtoVarName, 
        ClassModel domainModel,
        IList<DTOFieldModel> dtoFields,
        bool skipIdFields)
    {
        var codeLines = new List<CSharpStatement>();

        codeLines.Add(new CSharpStatementBlock("if (expectedDto == null)")
            .AddStatements(@"
            actualEntity.Should().BeNull();
            return;"));
        
        codeLines.Add(string.Empty);
        
        codeLines.Add("actualEntity.Should().NotBeNull();");
        
        foreach (var field in dtoFields)
        {
            // Implicit ID case check
            if (!skipIdFields &&
                field.Mapping?.Element == null &&
                field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                codeLines.Add($"{actualEntityVarName}.Id.Should().Be({expectedDtoVarName}.{field.Name.ToPascalCase()});");
                continue;
            }

            // Implicit Owner ID case check
            ClassModel ownerEntity;
            if (field.Mapping?.Element == null &&
                (ownerEntity = domainModel.GetNestedCompositionalOwner()) != null &&
                field.Name.Equals($"{ownerEntity.Name}id", StringComparison.OrdinalIgnoreCase))
            {
                codeLines.Add($"{actualEntityVarName}.Id.Should().Be({expectedDtoVarName}.{field.Name.ToPascalCase()});");
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
                        codeLines.Add($"{actualEntityVarName}.{attribute.Name.ToPascalCase()}.Should().Be({expectedDtoVarName}.{field.Name.ToPascalCase()});");
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

                    codeLines.Add($"AssertEquivalent({expectedDtoVarName}.{field.Name.ToPascalCase()}, {actualEntityVarName}.{attributeName});");

                    var @class = template.CSharpFile.Classes.First();
                    @class.AddMethod("void", GetAssertionMethodName(targetType.InternalElement, attributeName),
                        method => method.Static()
                            .AddParameter(template.GetTypeName((IElement)field.TypeReference.Element), "expectedDto")
                            .AddParameter(template.GetTypeName(targetType.InternalElement), "actualEntity")
                            .AddStatements(GetDtoToDomainPropertyAssignments(template, "actualEntity", "expectedDto", targetType, fieldDtoModel.Fields, skipIdFields)));
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
                        codeLines.Add($"{dtoVarExpr}{field.Name.ToPascalCase()} = {entityVarName}.{attributeName} != null ? {GetAssertionMethodName(targetType.InternalElement, attributeName)}({entityVarName}.{attributeName}) : null,");
                    }
                    else
                    {
                        codeLines.Add($"{dtoVarExpr}{field.Name.ToPascalCase()} = {entityVarName}.{attributeName}?.Select({GetAssertionMethodName(targetType.InternalElement, attributeName)}).ToList() ?? new List<{fieldDtoModel.Name.ToPascalCase()}>(),");
                    }

                    var @class = template.CSharpFile.Classes.First();
                    @class.AddMethod(template.GetTypeName(fieldDtoModel.InternalElement),
                        GetAssertionMethodName(targetType.InternalElement, attributeName),
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

    private static string GetAssertionMethodName(IElement targetType, string attributeName = "")
    {
        return $"AssertEquivalent";
    }
}