using System;
using Microsoft.SemanticKernel;

namespace Intent.Modules.AI.UnitTests.Tasks.Helpers;

#nullable enable

internal static class KernelFunctionExtensions
{
    private const int MaxAttempts = 2;
    
    public static FileChangesResult InvokeFileChangesPrompt(this KernelFunction kernelFunction, Kernel kernel, KernelArguments? arguments = null, int maxAttempts = MaxAttempts)
    {
        FileChangesResult? fileChangesResult = null;
        var previousError = string.Empty;
        
        for (var i = 0; i < maxAttempts; i++)
        {
            arguments ??= new KernelArguments();
            arguments.Add("previousError", previousError);
            
            var result = kernelFunction.InvokeAsync(kernel, arguments).Result;

            if (FileChangesHelper.TryGetFileChangesResult(result, out fileChangesResult))
            {
                break;
            }

            previousError = "The previous prompt execution failed. You need to return ONLY the JSON response in the defined schema format!";
        }

        if (fileChangesResult is null)
        {
            throw new Exception("AI Prompt failed to return a valid response.");
        }

        return fileChangesResult;
    }
}