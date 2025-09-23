using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Application.DomainInteractions.Strategies
{
    public interface IMappingStrategy
    {
        bool IsMatch(ICSharpClassMethodDeclaration method);

        void ImplementMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements,
            EntityDetails entity, ICSharpTemplate template, ITypeReference returnType, string? returnDto);

        void ImplementPagedMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements, EntityDetails entity, ICSharpTemplate template,
            ITypeReference returnType, string? returnDto, string? mappingMethod);
    }

    public class AutoMapperMappingStrategy : IMappingStrategy
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
    }

    public class MapperlyMappingStrategy : IMappingStrategy
    {
        public bool IsMatch(ICSharpClassMethodDeclaration method)
        {
            return method.File.Template.ExecutionContext.InstalledModules.Any(x => x.ModuleId == "Intent.Application.Dtos.Mapperly");
        }

        public void ImplementMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements,
            EntityDetails entity, ICSharpTemplate template, ITypeReference returnType, string? returnDto)
        {
            var nullable = returnType.IsNullable ? "?" : "";
            template.TryGetTypeName("Intent.Application.Dtos.Mapperly.DtoMappingProfile", returnType.Element, out var _);
            statements.Add($"var mapper = new {entity.ElementModel.Name}DtoMapper();");
            statements.Add($"return mapper.{entity.ElementModel.Name}To{returnDto}{(returnType.IsCollection ? "List" : "")}({entity.VariableName}{(returnType.IsCollection ? ".ToList()" : "")});");
        }

        public void ImplementPagedMappingStatement(ICSharpClassMethodDeclaration method, List<CSharpStatement> statements, EntityDetails entity,
            ICSharpTemplate template, ITypeReference returnType, string? returnDto, string? mappingMethod)
        {
            template.TryGetTypeName("Intent.Application.Dtos.Mapperly.DtoMappingProfile", returnType.Element, out var _);
            statements.Add($"var mapper = new {entity.ElementModel.Name}DtoMapper();");
            statements.Add($"return {entity.VariableName}.{mappingMethod}(x => mapper.{entity.ElementModel.Name}To{returnDto}(x));");
            return;
            throw new NotImplementedException();
        }
    }
}
