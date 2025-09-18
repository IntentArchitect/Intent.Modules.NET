using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Constants.TemplateRoles.Domain;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Strategies
{
    public interface IMappingStrategy
    {
        bool IsMatch(ICSharpFileBuilderTemplate template);
        void ImplementCreateMappingStatement(CSharpStatementAggregator codeLines, string variableName, ICSharpTemplate template, DTOModel dtoToReturn, string entityName);
        void ImplementUpdateMappingStatement(List<CSharpStatement> codeLines, string variableName, ICSharpTemplate template, OperationModel operationModel, string entityName);
        void ImplementDeleteWithReturnMappingStatement(CSharpStatementAggregator codeLines, string variableName, string dtoType, string entityName);
        void ImplementGetAllMappingStatement(CSharpStatementAggregator codeLines, string variableName, string unqualifiedDtoTypeName, string entityName);
        void ImplementGetAllPagedMappingStatement(CSharpStatementAggregator codeLines, string variableName, string unqualifiedDtoTypeName, string entityName);
        void ImplementGetByIdMappingStatement(CSharpStatementAggregator codeLines, string variableName, string unqualifiedDtoTypeName, string entityName);
    }

    public class AutoMapperMappingStrategy : IMappingStrategy
    {
        public bool IsMatch(ICSharpFileBuilderTemplate template)
        {
            return template.ExecutionContext.InstalledModules.Any(x => x.ModuleId == "Intent.Application.Dtos.AutoMapper");
        }

        public void ImplementCreateMappingStatement(CSharpStatementAggregator codeLines, string variableName, ICSharpTemplate template, DTOModel dtoToReturn, string entityName)
        {
            codeLines.Add($"return {variableName}.MapTo{template.GetTypeName(dtoToReturn.InternalElement)}(_mapper);");
        }

        public void ImplementDeleteWithReturnMappingStatement(CSharpStatementAggregator codeLines, string variableName, string dtoType, string entityName)
        {
            codeLines.Add($@"return {variableName}.MapTo{dtoType}(_mapper);");
        }

        public void ImplementGetAllMappingStatement(CSharpStatementAggregator codeLines, string variableName, string unqualifiedDtoTypeName, string entityName)
        {
            codeLines.Add($@"return elements.MapTo{unqualifiedDtoTypeName}List(_mapper);");
        }

        public void ImplementGetAllPagedMappingStatement(CSharpStatementAggregator codeLines,string variableName, string unqualifiedDtoTypeName, string entityName)
        {
            codeLines.Add($"return results.MapToPagedResult(x => x.MapTo{unqualifiedDtoTypeName}(_mapper));");
        }

        public void ImplementGetByIdMappingStatement(CSharpStatementAggregator codeLines, string variableName, string unqualifiedDtoTypeName, string entityName)
        {
            codeLines.Add($@"return element.MapTo{unqualifiedDtoTypeName}(_mapper);");
        }

        public void ImplementUpdateMappingStatement(List<CSharpStatement> codeLines, string variableName, ICSharpTemplate template, OperationModel operationModel, string entityName)
        {
            codeLines.Add($"return {variableName}.MapTo{template.GetTypeName((IElement)operationModel.TypeReference.Element)}(_mapper);");
        }
    }

    public class MapperlyMappingStrategy : IMappingStrategy
    {
        public void ImplementCreateMappingStatement(CSharpStatementAggregator codeLines, string variableName, ICSharpTemplate template, DTOModel dtoToReturn, string entityName)
        {
            codeLines.Add($"var mapper = new {entityName}DtoMapper();");
            codeLines.Add($"return mapper.{entityName}To{dtoToReturn.Name}({variableName});");
        }

        public void ImplementDeleteWithReturnMappingStatement(CSharpStatementAggregator codeLines, string variableName, string dtoType, string entityName)
        {
            codeLines.Add($"var mapper = new {entityName}DtoMapper();");
            codeLines.Add($"return mapper.{entityName}To{dtoType}({variableName});");
        }

        public void ImplementGetAllMappingStatement(CSharpStatementAggregator codeLines,string variableName, string unqualifiedDtoTypeName, string entityName)
        {
            codeLines.Add($"var mapper = new {entityName}DtoMapper();");
            codeLines.Add($"return mapper.{entityName}To{unqualifiedDtoTypeName}List({variableName});");
        }

        public void ImplementGetAllPagedMappingStatement(CSharpStatementAggregator codeLines, string variableName, string unqualifiedDtoTypeName, string entityName)
        {
            codeLines.Add($"var mapper = new {entityName}DtoMapper();");
            codeLines.Add($"return results.MapToPagedResult(x => mapper.{entityName}To{unqualifiedDtoTypeName}(x));");
        }

        public void ImplementGetByIdMappingStatement(CSharpStatementAggregator codeLines, string variableName, string unqualifiedDtoTypeName , string entityName)
        {
            codeLines.Add($"var mapper = new {entityName}DtoMapper();");
            codeLines.Add($"return mapper.{entityName}To{unqualifiedDtoTypeName}List({variableName});");
        }

        public void ImplementUpdateMappingStatement(List<CSharpStatement> codeLines, string variableName, ICSharpTemplate template, OperationModel operationModel, string entityName)
        {
            codeLines.Add($"var mapper = new {entityName}DtoMapper();");
            codeLines.Add($"return mapper.{entityName}To{template.GetTypeName((IElement)operationModel.TypeReference.Element)}List({variableName});");
        }

        public bool IsMatch(ICSharpFileBuilderTemplate template)
        {
            return template.ExecutionContext.InstalledModules.Any(x => x.ModuleId == "Intent.Application.Dtos.Mapperly");
        }
    }
}
