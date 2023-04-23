using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;

internal interface IFunctionTriggerHandler
{
    IEnumerable<INugetPackageInfo> GetNugetDependencies();
    void ApplyMethodParameters(CSharpClassMethod method);
    void ApplyMethodStatements(CSharpClassMethod method);
}