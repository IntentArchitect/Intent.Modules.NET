using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates;

internal static class AzureFunctionsHelper
{
    public enum AzureFunctionsProcessType
    {
        InProcess,
        Isolated
    }

    public static AzureFunctionsProcessType GetAzureFunctionsProcessType(IOutputTarget output)
    {
        return output.GetProject().GetMaxNetAppVersion().Major switch
        {
            >= 8 => AzureFunctionsProcessType.Isolated,
            >= 6 => AzureFunctionsProcessType.InProcess,
            _ => throw new NotSupportedException($"Unsupported Azure functions process: {output.GetProject().GetMaxNetAppVersion()}")
        };
    }

    public static AzureFunctionsProcessType SwapState(this AzureFunctionsProcessType type)
    {
        return type switch
        {
            AzureFunctionsProcessType.InProcess => AzureFunctionsProcessType.Isolated,
            AzureFunctionsProcessType.Isolated => AzureFunctionsProcessType.InProcess,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}