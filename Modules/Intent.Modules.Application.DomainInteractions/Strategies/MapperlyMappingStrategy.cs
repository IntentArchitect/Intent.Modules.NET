using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.DomainInteractions.Strategies;

internal class MapperlyMappingStrategy : IMappingStrategy
{
    public bool IsMatch(ICSharpClassMethodDeclaration method)
    {
        return method.File.Template.ExecutionContext.InstalledModules.Any(x => x.ModuleId == "Intent.Application.Dtos.Mapperly");
    }

    public void ImplementMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements,
        EntityDetails entity, ICSharpTemplate template, ITypeReference returnType, string? returnDto)
    {
        template.TryGetTypeName("Intent.Application.Dtos.Mapperly.DtoMappingProfile", returnType.Element, out var _);
        var ctor = method.Class.Constructors.FirstOrDefault();
        if (ctor is null)
        {
            method.Class.AddConstructor();
            ctor = method.Class.Constructors.First();
        }
        if (ctor.Parameters.All(x => x.Type != $"{entity.ElementModel.Name}DtoMapper"))
        {
            ctor.AddParameter($"{entity.ElementModel.Name}DtoMapper", "mapper", param => param.IntroduceReadonlyField());
        }
        statements.Add($"return _mapper.{entity.ElementModel.Name}To{returnDto}{(returnType.IsCollection ? "List" : "")}({entity.VariableName}{(returnType.IsCollection ? ".ToList()" : "")});");
    }

    public void ImplementPagedMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements, EntityDetails entity,
        ICSharpTemplate template, ITypeReference returnType, string? returnDto, string? mappingMethod)
    {
        template.TryGetTypeName("Intent.Application.Dtos.Mapperly.DtoMappingProfile", returnType.Element, out var _);
        var ctor = method.Class.Constructors.FirstOrDefault();
        if (ctor is null)
        {
            method.Class.AddConstructor();
            ctor = method.Class.Constructors.First();
        }
        if (ctor.Parameters.All(x => x.Type != $"{entity.ElementModel.Name}DtoMapper"))
        {
            ctor.AddParameter($"{entity.ElementModel.Name}DtoMapper", "mapper", param => param.IntroduceReadonlyField());
        }
        statements.Add($"return {entity.VariableName}.{mappingMethod}(x => _mapper.{entity.ElementModel.Name}To{returnDto}(x));");
    }

    public bool HasProjectTo() => false;
}