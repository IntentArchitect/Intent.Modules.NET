using System;
using Intent.Exceptions;
using Intent.Utils;
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
            var attemptNumber = i + 1;
            Logging.Log.Info($"AI invocation attempt {attemptNumber}/{maxAttempts}");
            
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

            if (FileChangesHelper.TryGetFileChangesResult(result, out fileChangesResult, out var errorDetails))
            {
                Logging.Log.Info($"Attempt {attemptNumber} succeeded");
                break;
            }

            // Provide specific error feedback for the retry
            previousError = BuildRetryErrorMessage(result.ToString(), errorDetails);
            Logging.Log.Warning($"Attempt {attemptNumber} failed: {errorDetails}");
        }

        if (fileChangesResult is null)
        {
            throw new Exception("AI Prompt failed to return a valid response after all retry attempts.");
        }

        return fileChangesResult;
    }

    private static string BuildRetryErrorMessage(string aiResponse, string? errorDetails)
    {
        if (string.IsNullOrWhiteSpace(errorDetails))
        {
            return "The previous prompt execution failed. You need to return ONLY the JSON response in the defined schema format!";
        }

        var message = $"""
                       The previous prompt execution failed with the following error:

                       {errorDetails}

                       """;

        // Add specific guidance based on the error type
        if (errorDetails.Contains("JSON parsing failed", StringComparison.OrdinalIgnoreCase))
        {
            message += """
                       Common JSON errors to avoid:
                       - Trailing commas in arrays or objects
                       - Unescaped quotes or backslashes in strings
                       - Comments (JSON does not support comments)
                       - Single quotes instead of double quotes
                       
                       """;
        }

        message += """
                   Expected schema format:
                   {
                     "fileChanges": [
                       {
                         "filePath": "path/to/file",
                         "fileExplanation": "description",
                         "content": "file content"
                       }
                     ]
                   }
                   """;

        return message;
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