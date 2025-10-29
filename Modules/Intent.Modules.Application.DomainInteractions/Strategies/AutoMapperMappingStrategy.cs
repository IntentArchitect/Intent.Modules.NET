using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.DomainInteractions.Strategies;

internal class AutoMapperMappingStrategy : IMappingStrategy
{
    public bool IsMatch(ICSharpClassMethodDeclaration method)
    {
        return method.File.Template.ExecutionContext.InstalledModules.Any(x => x.ModuleId == "Intent.Application.Dtos.AutoMapper");
    }

    public void ImplementMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements,
        EntityDetails entity, ICSharpTemplate template, ITypeReference returnType, string? returnDto)
    {
        //Adding Using Clause for Extension Methods
        template.TryGetTypeName("Intent.Application.Dtos.EntityDtoMappingExtensions", returnType.Element, out _);
        var autoMapperFieldName = method.Class.InjectService(template.UseType("AutoMapper.IMapper"));
        var nullable = returnType.IsNullable ? "?" : "";
        statements.Add($"return {entity.VariableName}{nullable}.MapTo{returnDto}{(returnType.IsCollection ? "List" : "")}({autoMapperFieldName});");
    }

    public void ImplementPagedMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements, EntityDetails entity,
        ICSharpTemplate template, ITypeReference returnType, string? returnDto, string? mappingMethod)
    {
        var autoMapperFieldName = method.Class.InjectService(template.UseType("AutoMapper.IMapper"));
        statements.Add($"return {entity.VariableName}.{mappingMethod}(x => x.MapTo{returnDto}({autoMapperFieldName}));");
    }

    public bool HasProjectTo() => true;
}