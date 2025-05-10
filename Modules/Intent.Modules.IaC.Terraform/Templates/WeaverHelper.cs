using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.IaC.Terraform.Templates;

public static class WeaverHelper
{
    public static string MergeKeyValuePairs(this IntentTemplateBase template, Dictionary<string, string> keysWithDefaults)
    {
        if (!template.TryGetExistingFileContent(out var content))
        {
            return GenerateContent(keysWithDefaults);
        }
            
        var existingLines = content.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var existingKeyValuePairs = new Dictionary<string, string>();

        foreach (var line in existingLines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith('#'))
            {
                continue;
            }
            var parts = trimmedLine.Split(['='], 2);
            if (parts.Length == 2)
            {
                existingKeyValuePairs.Add(parts[0].Trim(), parts[1].Trim());
            }
        }

        foreach (var kvp in keysWithDefaults)
        {
            if (!existingKeyValuePairs.ContainsKey(kvp.Key))
            {
                existingKeyValuePairs.Add(kvp.Key, kvp.Value);
            }
        }

        return GenerateContent(existingKeyValuePairs);
    }
    
    private static string GenerateContent(Dictionary<string, string> keysWithValues)
    {
        var maxKeyLength = keysWithValues.Keys.Max(k => k.Length);
        var sb = new StringBuilder();
        
        foreach (var kvp in keysWithValues)
        {
            var padding = new string(' ', maxKeyLength - kvp.Key.Length);
            sb.AppendLine($"{kvp.Key}{padding} = {kvp.Value}");
        }
        
        return sb.ToString();
    }
}