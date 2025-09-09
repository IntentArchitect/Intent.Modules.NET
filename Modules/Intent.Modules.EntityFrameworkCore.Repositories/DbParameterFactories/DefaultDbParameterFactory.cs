using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.EntityFrameworkCore.Repositories.DbParameterFactories.Interfaces;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.DbParameterFactories
{
    internal class DefaultDbParameterFactory : IDbParameterFactory 
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public DefaultDbParameterFactory(ICSharpFileBuilderTemplate template)
        {
            _template = template;
        }

        public CSharpStatement CreateForInput(string invocationPrefix, string valueVariableName, string spParameterName, Parameter parameter)
        {
            return new CSharpStatement();
        }

        public CSharpStatement CreateForOutput(string invocationPrefix, string valueVariableName, string spParameterName, Parameter parameter)
        {
            return new CSharpStatement();
        }

        public CSharpStatement CreateForTableType(string invocationPrefix, string valueVariableName, Parameter parameter)
        {
            return new CSharpStatement();
        }

        public string GenerateScalarSqlStatement(string storeProcedureName, List<SqlParameter> parameters)
        {
            return string.Empty;
        }

        public string GenerateTypeElementSqlStatement(string storeProcedureName, List<SqlParameter> parameters)
        {
            return string.Empty;
        }

        public string GenerateTableTypeSqlStatement(string storeProcedureName, List<SqlParameter> parameters)
        {
            return string.Empty;
        }
    }
}
