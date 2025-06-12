using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Intent.Modules.Integration.IaC.Shared.AzureServiceBus;

internal static class AzureHelper
{
    // Pre-compiled regex for better performance
    private static readonly Regex InvalidCharactersRegex = new (@"[^\.a-z0-9]", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    /// <summary>
    /// Truncates and ensures a string fits within Azure resource naming constraints
    /// </summary>
    /// <param name="input">The input string to process</param>
    /// <param name="maxLength">Maximum allowed length (default 24 for storage accounts)</param>
    /// <param name="hashLength">Length of hash suffix when truncation is needed (default 8)</param>
    /// <returns>A valid Azure resource name within the specified length</returns>
    public static string EnsureValidLength(string input, int maxLength = 24, int hashLength = 8)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException("Input cannot be null or empty", nameof(input));
        
        if (maxLength <= hashLength)
            throw new ArgumentException($"maxLength must be greater than hashLength ({hashLength})", nameof(maxLength));

        // Clean the input (remove invalid characters, convert to lowercase)
        var cleaned = CleanForAzure(input);
        
        // If it's already within the limit, return as-is
        if (cleaned.Length <= maxLength)
            return cleaned;
        
        // Calculate how much space we have for the original string
        var availableLength = maxLength - hashLength;
        
        // Truncate and append deterministic hash
        var truncated = cleaned.Substring(0, availableLength);
        var hash = GenerateShortHash(cleaned, hashLength);
        
        return truncated + hash;
    }
    
    private static string CleanForAzure(string input)
    {
        // Convert to lowercase and remove invalid characters using pre-compiled regex
        // This example is for storage accounts (alphanumeric only)
        // Adjust regex based on your specific Azure resource requirements
        var cleaned = InvalidCharactersRegex.Replace(input.ToLowerInvariant(), "").Replace(".", "-");
        
        // Ensure it doesn't start with a number (if required by the resource type)
        if (cleaned.Length > 0 && char.IsDigit(cleaned[0]))
            cleaned = "a" + cleaned.Substring(1);
            
        return cleaned;
    }
    
    private static string GenerateShortHash(string input, int length)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

        // Convert to alphanumeric string
        var result = new StringBuilder(length);
        for (var i = 0; i < hashBytes.Length && result.Length < length; i++)
        {
            // Convert byte to base36 (0-9, a-z)
            var base36 = Convert.ToString(hashBytes[i] % 36, 36);
            result.Append(base36);
        }

        return result.ToString().PadRight(length, '0').Substring(0, length);
    }
}
