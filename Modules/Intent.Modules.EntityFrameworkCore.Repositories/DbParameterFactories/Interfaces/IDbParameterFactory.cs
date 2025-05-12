using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.DbParameterFactories.Interfaces
{
    internal interface IDbParameterFactory
    {
        CSharpStatement CreateForOutput(string invocationPrefix,
            string valueVariableName,
            Parameter parameter);

        CSharpStatement CreateForInput(
            string invocationPrefix,
            string valueVariableName,
            Parameter parameter);

        CSharpStatement CreateForTableType(
            string invocationPrefix,
            Parameter parameter);
    }
}
