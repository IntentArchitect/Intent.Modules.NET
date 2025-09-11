using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

public static class CSharpStatementMappingExtensions
{
    public static string GetPredicateExpression(this CSharpClassMappingManager mappingManager, IElementToElementMapping queryMapping)
    {
        var queryFields = queryMapping.MappedEnds.Where(x => x.SourceElement == null || !x.SourceElement.TypeReference.IsNullable)
            .Select(x => x.SourceElement == null || x.IsOneToOne()
                ? GetCalculatedComparisonExpression(x, mappingManager, queryMapping)
                : $"x.{x.TargetElement.Name}.{mappingManager.GenerateSourceStatementForMapping(queryMapping, x)}")
            .ToList();

        var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}" : null;
        return expression;
    }

    private static string GetCalculatedComparisonExpression(IElementToElementMappedEnd mapping, CSharpClassMappingManager mappingManager, IElementToElementMapping queryMapping)
        => (mapping.TargetElement.TypeReference?.Element.Name, mappingManager.GenerateSourceStatementForMapping(queryMapping, mapping).ToString()) switch
        {
            ("bool", "true") => $"x.{mapping.TargetElement.Name}",
            (_, var sourceStatement) => $"x.{mapping.TargetElement.Name} == {sourceStatement}"
        };
}