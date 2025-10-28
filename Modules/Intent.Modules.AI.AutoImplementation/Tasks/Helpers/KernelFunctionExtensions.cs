using System;
using System.Collections.Generic;
using Intent.Exceptions;
using Microsoft.SemanticKernel;

namespace Intent.Modules.AI.AutoImplementation.Tasks.Helpers;

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
            if (arguments is null)
            {
                arguments = new KernelArguments();
            }
            arguments["previousError"] = previousError;
            
            FunctionResult result;
            try
            {
                result = kernelFunction.InvokeAsync(kernel, arguments).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var rootException = GetRootException(ex);
                var message = rootException.Message;

                if (message.Contains("reasoning_effort", StringComparison.OrdinalIgnoreCase))
                {
                    throw new FriendlyException(@"The selected model does not have thinking capabilities. Please choose a different model or set the thinking level to 'None'.");
                }

                throw;
            }

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

    private static Exception GetRootException(Exception exception)
    {
        if (exception is AggregateException aggregateException)
        {
            var flattened = aggregateException.Flatten();
            if (flattened.InnerExceptions.Count == 1)
            {
                return GetRootException(flattened.InnerExceptions[0]);
            }

            return flattened;
        }

        return exception.InnerException is null ? exception : GetRootException(exception.InnerException);
    }
}