using System.Collections.Generic;
using Intent.AzureFunctions.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal class TimerTriggerHandler : IFunctionTriggerHandler
{
    private readonly AzureFunctionClassTemplate _template;
    private readonly IAzureFunctionModel _azureFunctionModel;

    public TimerTriggerHandler(AzureFunctionClassTemplate template, IAzureFunctionModel azureFunctionModel)
    {
        _template = template;
        _azureFunctionModel = azureFunctionModel;
    }

    public void ApplyMethodParameters(CSharpClassMethod method)
    {
        if (!_template.OutputTarget.GetProject().IsNetApp(6))
        {
            _template.CSharpFile.AddUsing("TimerInfo = Microsoft.Azure.Functions.Worker.TimerInfo");
            _template.CSharpFile.AddUsing("TimerTriggerAttribute = Microsoft.Azure.Functions.Worker.TimerTriggerAttribute");
        }
        
        method.AddParameter(
            type: "TimerInfo",
            name: "timerInfo",
            configure: param =>
            {
                param.AddAttribute("TimerTrigger", attr =>
                {
                    attr.AddArgument($@"""{_azureFunctionModel.ScheduleExpression}""");
                });
            });
        method.AddParameter(_template.UseType("System.Threading.CancellationToken"), "cancellationToken");
    }

    public void ApplyMethodStatements(CSharpClassMethod method)
    {
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        if (!_template.OutputTarget.GetProject().IsNetApp(6))
        {
            yield return NugetPackages.MicrosoftAzureFunctionsWorkerExtensionsTimer(_template.OutputTarget);
        }
    }
}