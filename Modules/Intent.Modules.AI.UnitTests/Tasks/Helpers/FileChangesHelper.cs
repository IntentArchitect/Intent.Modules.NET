using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace Intent.Modules.AI.UnitTests.Tasks.Helpers;

#nullable enable

internal static class FileChangesHelper 
{
    /// <summary>
    /// AI Models / APIs that doesn't support JSON Schema Formatting (like OpenAI) will not completely give you a "clean result".
    /// This will attempt to cut through the thoughts it writes out to get to the payload.
    /// </summary>
    public static bool TryGetFileChangesResult(FunctionResult aiInvocationResponse, [NotNullWhen(true)] out FileChangesResult? fileChanges)
    {
        try
        {
            var textResponse = aiInvocationResponse.ToString();
            string payload;
            var jsonMarkdownStart = textResponse.IndexOf("```", StringComparison.Ordinal);
            if (jsonMarkdownStart < 0)
            {
                // Assume AI didn't respond with ``` wrappers.
                // JSON Deserializer will pick up if it is not valid.
                payload = textResponse;
            }
            else
            {
                var sanitized = textResponse.Substring(jsonMarkdownStart).Replace("```json", "").Replace("```", "");
                payload = sanitized;
            }

            var fileChangesResult = JsonConvert.DeserializeObject<FileChangesResult>(payload);
            if (fileChangesResult is null)
            {
                fileChanges = null;
                return false;
            }

            fileChanges = fileChangesResult;
            return true;
        }
        catch
        {
            fileChanges = null;
            return false;
        }
    }
}

internal class FileChangesResult
{
    public FileChange[] FileChanges { get; set; }
}
    
internal class FileChange
{
    public FileChange()
    {
    }

    public FileChange(string filePath, string fileExplanation, string content)
    {
        FilePath = filePath;
        FileExplanation = fileExplanation;
        Content = content;
    }

    public string FileExplanation { get; set; }
    public string FilePath { get; set; }
    public string Content { get; set; }

    public override string ToString()
    {
        return $"{FilePath} ({FileExplanation})";
    }
}