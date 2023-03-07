using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates;

public static class MappingMethodHelper
{
    public static void AddDtoToDomainMappingMethods(this ICSharpFileBuilderTemplate template, CSharpClass builderClass, CommandModel commandModel, ClassModel domainModel)
    {
        builderClass.AddMethod(template.GetTypeName(domainModel.InternalElement), GetCreateMethodName(domainModel.InternalElement), method =>
        {
            method.Static();
            method.AddParameter(template.GetTypeName(commandModel.InternalElement), "dto");
            method.AddStatementBlock($"return new {template.GetTypeName(domainModel.InternalElement)}", block =>
            {
                block.AddStatements(GetDTOPropertyAssignments(template, "", "dto", domainModel.InternalElement, commandModel.Properties));
                block.WithSemicolon();
            });
        });
    }

    public static void AddDtoToDomainMappingMethods(this ICSharpFileBuilderTemplate template, CSharpClass builderClass, DTOModel dtoModel, ClassModel domainModel)
    {
        builderClass.AddMethod(template.GetTypeName(domainModel.InternalElement), GetCreateMethodName(domainModel.InternalElement), method =>
        {
            method.Static();
            method.AddParameter(template.GetTypeName(dtoModel.InternalElement), "dto");
            method.AddStatementBlock($"return new {template.GetTypeName(domainModel.InternalElement)}", block =>
            {
                block.AddStatements(GetDTOPropertyAssignments(template, "", "dto", domainModel.InternalElement, dtoModel.Fields));
                block.WithSemicolon();
            });
        });
    }

    private static IEnumerable<CSharpStatement> GetDTOPropertyAssignments(ICSharpFileBuilderTemplate template, string entityVarName, string dtoVarName, IElement domainModel,
        IList<DTOFieldModel> dtoFields)
    {
        var codeLines = new List<CSharpStatement>();
        foreach (var field in dtoFields)
        {
            if (field.Mapping?.Element == null
                && domainModel.ChildElements.All(p => p.Name != field.Name))
            {
                codeLines.Add($"#warning No matching field found for {field.Name}");
                continue;
            }

            var entityVarExpr = !string.IsNullOrWhiteSpace(entityVarName) ? $"{entityVarName}." : string.Empty;
            switch (field.Mapping?.Element?.SpecializationTypeId)
            {
                default:
                    var mappedPropertyName = field.Mapping?.Element?.Name ?? "<null>";
                    codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and DTO: {field.Name}");
                    break;
                case null:
                case AttributeModel.SpecializationTypeId:
                    var attribute = field.Mapping?.Element
                                    ?? domainModel.ChildElements.First(p => p.Name == field.Name);
                    if (!attribute.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()},");
                        break;
                    }

                    break;
                case AssociationTargetEndModel.SpecializationTypeId:
                {
                    var association = field.Mapping.Element.AsAssociationTargetEndModel();
                    var targetType = association.Element as IElement;
                    var attributeName = association.Name.ToPascalCase();

                    if (association.Association.AssociationType == AssociationType.Aggregation)
                    {
                        codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                        break;
                    }

                    if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                    {
                        if (field.TypeReference.IsNullable)
                        {
                            codeLines.Add(
                                $"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()} != null ? {GetCreateMethodName(targetType, attributeName)}({dtoVarName}.{field.Name.ToPascalCase()}) : null,");
                        }
                        else
                        {
                            codeLines.Add($"{entityVarExpr}{attributeName} = {GetCreateMethodName(targetType, attributeName)}({dtoVarName}.{field.Name.ToPascalCase()}),");
                        }
                    }
                    else
                    {
                        codeLines.Add(
                            $"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()}{(field.TypeReference.IsNullable ? "?" : "")}.Select({GetCreateMethodName(targetType, attributeName)}).ToList(){(field.TypeReference.IsNullable ? $" ?? new List<{targetType.Name.ToPascalCase()}>()" : "")},");
                    }

                    var @class = template.CSharpFile.Classes.First();
                    @class.AddMethod(template.GetTypeName(targetType),
                        GetCreateMethodName(targetType, attributeName),
                        method => method.Private().Static()
                            .AddParameter(template.GetTypeName((IElement)field.TypeReference.Element), "dto")
                            .AddStatement($"return new {targetType.Name.ToPascalCase()}")
                            .AddStatement(new CSharpStatementBlock()
                                .AddStatements(GetDTOPropertyAssignments(template, "", $"dto", targetType,
                                    ((IElement)field.TypeReference.Element).ChildElements.Where(x => x.IsDTOFieldModel()).Select(x => x.AsDTOFieldModel()).ToList()))
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