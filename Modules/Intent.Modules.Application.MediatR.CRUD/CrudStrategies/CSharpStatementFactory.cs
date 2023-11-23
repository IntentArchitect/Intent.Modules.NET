using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    internal static class CSharpStatementFactory
    {
        public static CSharpStatement CreateThrowNotFoundIfNullStatement(
            this ICSharpTemplate template,
            string variable,
            string message)
        {
            var ifStatement = new CSharpIfStatement($"{variable} is null");
            ifStatement.SeparatedFromPrevious(false);
            ifStatement.AddStatement($@"throw new {template.GetNotFoundExceptionName()}($""{message}"");");

            return ifStatement;
        }

        public static CSharpStatement CreateReturnNullIfNullStatement(
            this ICSharpTemplate template,
            string variable)
        {
            var ifStatement = new CSharpIfStatement($"{variable} is null");
            ifStatement.SeparatedFromPrevious(false);
            ifStatement.AddStatement($@"return null;");

            return ifStatement;
        }
    }
}
