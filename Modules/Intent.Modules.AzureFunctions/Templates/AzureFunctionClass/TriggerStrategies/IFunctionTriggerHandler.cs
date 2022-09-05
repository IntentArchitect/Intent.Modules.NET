using System.Collections.Generic;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

public interface IFunctionTriggerHandler
{
    IEnumerable<string> GetMethodParameterDefinitionList();
    IEnumerable<string> GetRunMethodEntryStatementList();
    IEnumerable<INugetPackageInfo> GetNugetDependencies();
    IEnumerable<ExceptionCatchBlock> GetExceptionCatchBlocks();

}