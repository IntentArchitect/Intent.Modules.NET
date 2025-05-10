using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.IaC.Terraform.Templates;

internal static class WeaverHelper
{
    public static string MergeKeyValuePairs(this IntentTemplateBase template, Dictionary<string, string> keysWithDefaults)
    {
        if (!template.TryGetExistingFileContent(out var content))
        {
            return GenerateContent(keysWithDefaults);
        }
            
        var existingLines = content.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var existingKeyValuePairs = new Dictionary<string, string>();
        var commentLines = new List<string>();
        var lineOrder = new List<string>(); // Keeps track of the original order (keys and comments)

        // First pass: collect existing key-value pairs and comments
        foreach (var line in existingLines)
        {
            var trimmedLine = line.Trim();
            
            // Handle comments or empty lines
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith('#'))
            {
                commentLines.Add(line);
                lineOrder.Add($"COMMENT:{commentLines.Count - 1}");
                continue;
            }
            
            // Handle key-value pairs
            var parts = trimmedLine.Split(['='], 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                existingKeyValuePairs[key] = parts[1].Trim();
                lineOrder.Add($"KEY:{key}");
            }
        }

        // Copy the existing pairs to a new dictionary
        var mergedKeyValuePairs = new Dictionary<string, string>(existingKeyValuePairs);

        // Add missing key-value pairs
        var newKeys = new List<string>();
        foreach (var kvp in keysWithDefaults)
        {
            if (!mergedKeyValuePairs.ContainsKey(kvp.Key))
            {
                mergedKeyValuePairs[kvp.Key] = kvp.Value;
                newKeys.Add(kvp.Key);
            }
        }

        // If nothing new to add, return the original content
        if (newKeys.Count == 0)
        {
            return content;
        }

        // Generate content preserving original order and comments
        var maxKeyLength = mergedKeyValuePairs.Keys.Max(k => k.Length);
        var sb = new StringBuilder();
        
        // First output all the existing lines in original order
        foreach (var item in lineOrder)
        {
            if (item.StartsWith("COMMENT:"))
            {
                int index = int.Parse(item.Substring(8));
                sb.AppendLine(commentLines[index]);
            }
            else if (item.StartsWith("KEY:"))
            {
                var key = item.Substring(4);
                var padding = new string(' ', maxKeyLength - key.Length);
                sb.AppendLine($"{key}{padding} = {mergedKeyValuePairs[key]}");
                
                // Remove the key so we know we've processed it
                mergedKeyValuePairs.Remove(key);
            }
        }

        // Add a blank line before new entries if the file doesn't end with one
        if (lineOrder.Count > 0 && !string.IsNullOrWhiteSpace(existingLines[^1].Trim()))
        {
            sb.AppendLine();
        }

        // Now add any new keys that weren't in the original file
        foreach (var key in newKeys.OrderBy(k => k))
        {
            if (mergedKeyValuePairs.ContainsKey(key))
            {
                var padding = new string(' ', maxKeyLength - key.Length);
                sb.AppendLine($"{key}{padding} = {mergedKeyValuePairs[key]}");
            }
        }
        
        return sb.ToString();
    }
    
    private static string GenerateContent(Dictionary<string, string> keysWithValues)
    {
        var maxKeyLength = keysWithValues.Keys.Max(k => k.Length);
        var sb = new StringBuilder();
        
        foreach (var kvp in keysWithValues.OrderBy(k => k.Key))
        {
            var padding = new string(' ', maxKeyLength - kvp.Key.Length);
            sb.AppendLine($"{kvp.Key}{padding} = {kvp.Value}");
        }
        
        return sb.ToString();
    }
}
