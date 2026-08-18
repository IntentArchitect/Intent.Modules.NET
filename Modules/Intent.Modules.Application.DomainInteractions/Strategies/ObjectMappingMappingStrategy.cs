using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.DomainInteractions.Strategies;

internal class ObjectMappingMappingStrategy : IMappingStrategy
{
    public bool IsMatch(ICSharpClassMethodDeclaration method)
    {
        return method.File.Template.ExecutionContext.InstalledModules.Any(x => x.ModuleId == "Intent.Application.Dtos.ObjectMapping");
    }

    public void ImplementMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements,
        EntityDetails entity, ICSharpTemplate template, ITypeReference returnType, string? returnDto)
    {
        // Both registers the using clause for the extension methods and guards against emitting a
        // call site that references a Mapping Extension Class which was never generated.
        if (!template.TryGetTypeName("Intent.Application.Dtos.ObjectMapping.MappingExtensions", returnType.Element, out _))
        {
            return;
        }

        var nullable = returnType.IsNullable ? "?" : "";
        var list = returnType.IsCollection ? "List" : "";
        statements.Add($"return {entity.VariableName}{nullable}.MapTo{returnDto}{list}();");
    }

    public void ImplementPagedMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements, EntityDetails entity,
        ICSharpTemplate template, ITypeReference returnType, string? returnDto, string? mappingMethod)
    {
        statements.Add($"return {entity.VariableName}.{mappingMethod}(x => x.MapTo{returnDto}());");
    }

    public bool HasProjectTo() => false;
}
