using System;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.IaC.Terraform.Templates;

internal static class AzureHelper
{
    private static readonly Regex SpecialCharsRegex = new (@"[^a-zA-Z0-9\s]", RegexOptions.Compiled);
    private static readonly Regex MultipleSpacesRegex = new (@"\s+", RegexOptions.Compiled);
    private static readonly Regex VowelRegex = new (@"[aeiou]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Creates a deterministic, readable resource name by intelligently combining components to overcome Azure's resource naming constraints.
    /// Azure resources have strict character limits and naming rules - this method handles length restrictions by using progressive truncation 
    /// strategies including vowel removal and proportional sizing when the full concatenation exceeds the maximum allowed length.
    /// </summary>
    /// <param name="components">Array of string components to combine into the Azure resource name. Null, empty, or whitespace-only components are automatically filtered out.</param>
    /// <param name="maxLength">Maximum allowed length for the Azure resource name (varies by resource type: Storage Account=24, Key Vault=24, Resource Group=64, etc.). Must be greater than 0.</param>
    /// <param name="delimiter">String to insert between components. Defaults to empty string if null. When delimiters would exceed Azure's length limits, they are reduced or removed as needed.</param>
    /// <returns>
    /// A cleaned, lowercase Azure-compliant resource name that fits within the specified length limit. 
    /// Returns empty string if no valid components are provided or maxLength is 0 or negative.
    /// The result is deterministic - same inputs always produce the same output, ensuring consistent Azure resource naming.
    /// </returns>
    /// <remarks>
    /// Don't attempt to add Terraform expressions as part of the components, it will get reduced along the other pieces of string.
    /// You will need to add a post-processing step to introduce Terraform expressions.
    /// </remarks>
    public static string CreateResourceName(string[]? components, int maxLength, string? delimiter = null)
    {
        if (components == null || components.Length == 0 || maxLength <= 0)
            return string.Empty;

        // Filter out null or empty components
        var validComponents = components.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
        if (validComponents.Length == 0)
            return string.Empty;

        // Default delimiter to empty string if null
        delimiter ??= string.Empty;

        // Clean all components (basic cleaning only)
        var cleanComponents = validComponents.Select(CleanName).Where(c => !string.IsNullOrEmpty(c)).ToArray();
        if (cleanComponents.Length == 0)
            return string.Empty;

        // Calculate total delimiter length
        var totalDelimiterLength = delimiter.Length * (cleanComponents.Length - 1);

        // If delimiters alone exceed maxLength, accommodate what we can
        if (totalDelimiterLength >= maxLength)
        {
            return AccommodateWithMinimalDelimiters(cleanComponents, maxLength, delimiter).ToKebabCase();
        }

        // Try the full concatenation first
        var fullName = string.Join(delimiter, cleanComponents);

        if (fullName.Length <= maxLength)
            return fullName.ToKebabCase();

        // If too long, apply intelligent truncation
        return TruncateIntelligently(cleanComponents, maxLength, delimiter).ToKebabCase();
    }

    private static string CleanName(string input)
    {
        // Basic cleaning - remove special characters and normalize whitespace
        var cleaned = SpecialCharsRegex.Replace(input.Trim(), string.Empty);
        cleaned = MultipleSpacesRegex.Replace(cleaned, " ");
        return cleaned.Trim().ToLowerInvariant();
    }

    private static string AccommodateWithMinimalDelimiters(string[] components, int maxLength, string delimiter)
    {
        // If even delimiters are too much, just concatenate components without delimiters
        if (delimiter.Length > 0)
        {
            // Try with fewer components and delimiters
            var result = string.Empty;
            var currentLength = 0;

            for (int i = 0; i < components.Length; i++)
            {
                var component = CleanAndCompact(components[i], maxLength - currentLength);
                if (component.Length == 0)
                    break;

                var delimiterToAdd = (i > 0 && result.Length > 0) ? delimiter : string.Empty;
                var potentialAddition = delimiterToAdd + component;

                if (currentLength + potentialAddition.Length > maxLength)
                {
                    // Try to fit as much as possible of this component
                    var remainingSpace = maxLength - currentLength - delimiterToAdd.Length;
                    if (remainingSpace > 0)
                    {
                        component = component.Substring(0, Math.Min(component.Length, remainingSpace));
                        result += delimiterToAdd + component;
                    }

                    break;
                }

                result += potentialAddition;
                currentLength += potentialAddition.Length;
            }

            return result;
        }

        // No delimiter - just concatenate what fits
        return string.Join(string.Empty, components.Select(CleanName))
            .Substring(0, Math.Min(maxLength, string.Join(string.Empty, components.Select(CleanName)).Length));
    }

    private static string TruncateIntelligently(string[] components, int maxLength, string delimiter)
    {
        var totalDelimiterLength = delimiter.Length * (components.Length - 1);
        var availableLength = maxLength - totalDelimiterLength;

        // Ensure we have some space for content
        if (availableLength <= 0)
        {
            return AccommodateWithMinimalDelimiters(components, maxLength, delimiter);
        }

        // Strategy 1: Proportional compacting - remove vowels based on component lengths
        var compactedComponents = CompactComponents(components, availableLength);
        var compactResult = string.Join(delimiter, compactedComponents);

        if (compactResult.Length <= maxLength)
            return compactResult;

        // Strategy 2: Progressive truncation with proportional allocation
        return TruncateProportionally(compactedComponents, availableLength, delimiter);
    }

    private static string[] CompactComponents(string[] components, int availableLength)
    {
        var totalOriginalLength = components.Sum(c => c.Length);

        if (totalOriginalLength <= availableLength)
            return components;

        return components.Select(component =>
        {
            // Calculate proportional target length
            var proportion = (double)component.Length / totalOriginalLength;
            var targetLength = Math.Max(1, (int)(availableLength * proportion));

            return CompactString(component, targetLength);
        }).ToArray();
    }

    private static string TruncateProportionally(string[] components, int availableLength, string delimiter)
    {
        var workingComponents = components.ToArray();
        var totalOriginalLength = workingComponents.Sum(c => c.Length);

        // Ensure minimum length for each component (but accommodate if impossible)
        var minLengthPerComponent = Math.Max(1, Math.Min(availableLength / components.Length, 3));

        for (int i = 0; i < workingComponents.Length; i++)
        {
            var proportion = (double)components[i].Length / totalOriginalLength;
            var targetLength = Math.Max(minLengthPerComponent, (int)(availableLength * proportion));

            workingComponents[i] = TruncatePreservingWords(workingComponents[i], targetLength);
        }

        // Final adjustment if still too long
        var currentResult = string.Join(delimiter, workingComponents);
        var totalDelimiterLength = delimiter.Length * (components.Length - 1);

        if (currentResult.Length <= availableLength + totalDelimiterLength)
            return currentResult;

        // Progressive truncation from longest to shortest until we fit
        return ProgressiveTruncation(workingComponents, availableLength, delimiter);
    }

    private static string ProgressiveTruncation(string[] components, int availableLength, string delimiter)
    {
        var workingComponents = components.ToArray();
        var minComponentLength = 1;

        while (workingComponents.Sum(c => c.Length) > availableLength && workingComponents.Any(c => c.Length > minComponentLength))
        {
            // Find the longest component(s) and reduce them
            var maxLength = workingComponents.Max(c => c.Length);

            for (int i = 0; i < workingComponents.Length; i++)
            {
                if (workingComponents[i].Length == maxLength)
                {
                    workingComponents[i] = TruncatePreservingWords(workingComponents[i], Math.Max(minComponentLength, maxLength - 1));
                }
            }
        }

        return string.Join(delimiter, workingComponents);
    }

    private static string CleanAndCompact(string input, int maxLength)
    {
        var cleaned = CleanName(input);
        return CompactString(cleaned, maxLength);
    }

    private static string CompactString(string input, int maxLength)
    {
        if (string.IsNullOrEmpty(input) || maxLength <= 0)
            return string.Empty;

        if (input.Length <= maxLength)
            return input;

        // Remove vowels from the middle of words, keeping first and last characters
        var words = input.Split(' ');
        var compactedWords = words.Select(word =>
        {
            if (word.Length <= 3)
                return word;

            // Keep first and last character, remove vowels from middle
            var first = word[0];
            var last = word[word.Length - 1];
            var middle = word.Substring(1, word.Length - 2);
            var compactMiddle = VowelRegex.Replace(middle, "");

            return $"{first}{compactMiddle}{last}";
        });

        var result = string.Join(" ", compactedWords);
        return result.Length <= maxLength ? result : TruncatePreservingWords(result, maxLength);
    }

    private static string TruncatePreservingWords(string input, int maxLength)
    {
        if (string.IsNullOrEmpty(input) || maxLength <= 0)
            return string.Empty;

        if (input.Length <= maxLength)
            return input;

        // Try to truncate at word boundaries (spaces)
        var lastSpace = input.LastIndexOf(' ', Math.Min(maxLength - 1, input.Length - 1));
        if (lastSpace > maxLength / 2) // Only use if we don't lose too much
            return input.Substring(0, lastSpace);

        // Otherwise, just truncate
        return input.Substring(0, maxLength);
    }
}