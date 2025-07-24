using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.DbParameterFactories.Interfaces
{
    internal interface IDbParameterFactory
    {
        CSharpStatement CreateForOutput(
            string invocationPrefix,
            string valueVariableName,
            string spParameterName,
            Parameter parameter);

        CSharpStatement CreateForInput(
            string invocationPrefix,
            string valueVariableName,
            string spParameterName,
            Parameter parameter);

        CSharpStatement CreateForTableType(
            string invocationPrefix,
            string valueVariableName,
            Parameter parameter);

        string GenerateScalarSqlStatement(string storeProcedureName, List<SqlParameter> parameters);
        string GenerateTypeElementSqlStatement(string storeProcedureName, List<SqlParameter> parameters);
        string GenerateTableTypeSqlStatement(string storeProcedureName, List<SqlParameter> parameters);
    }

    internal record SqlParameter(string SpParameterName, string VariableName, string OutputKeyword);
}
