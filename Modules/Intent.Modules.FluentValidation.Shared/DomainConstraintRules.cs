using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.Shared;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Utils;

#nullable enable

namespace Intent.Modules.FluentValidation.Shared;

internal static class DomainConstraintStereotypes
{
    public const string Required = "14680476-e24a-490f-ba44-75eb8dc6fb46";
    public const string TextLimits = "13649b19-4dfe-43ec-967f-0b85a5801dd6";
    public const string NumericLimits = "cb14e47d-672c-4244-8950-7c4ebf8cf8ed";
    public const string CollectionLimits = "06daef0d-5be0-43e0-9cc6-2bb8ea35dc86";
    public const string RegularExpression = "3dd144bc-374b-4acd-841a-7323210df66d";
    public const string Email = "9fb8d1b1-39b3-4f16-88e0-34d24a4e9bf6";
    public const string Base64 = "02308621-429c-4af4-9428-2ebb272e53fa";
    public const string Url = "1b2dc31a-599f-449b-9646-1a5313d23f91";
}

internal static class DomainConstraintRules
{
    internal const string RuleSpaceMetadataKey = "rule-space";

    internal static bool HasAnyRules(AttributeModel attribute)
    {
        return attribute.HasStereotype(DomainConstraintStereotypes.Required) ||
               attribute.HasStereotype(DomainConstraintStereotypes.TextLimits) ||
               attribute.HasStereotype(DomainConstraintStereotypes.NumericLimits) ||
               attribute.HasStereotype(DomainConstraintStereotypes.CollectionLimits) ||
               attribute.HasStereotype(DomainConstraintStereotypes.RegularExpression) ||
               attribute.HasStereotype(DomainConstraintStereotypes.Email) ||
               attribute.HasStereotype(DomainConstraintStereotypes.Base64) ||
               attribute.HasStereotype(DomainConstraintStereotypes.Url);
    }

    /// <summary>
    /// Implements the "Collect and Join" pattern for grouped collection validation.
    /// </summary>
    internal static void ApplyFallbackRules(
        ICSharpTemplate? template,
        CSharpMethodChainStatement validationRuleChain,
        AttributeModel mappedAttribute,
        HashSet<string> appliedRuleSpaces)
    {
        // 1. RDBMS Text Constraints (Physical overrides Logical)
        if (mappedAttribute.HasStereotype("Text Constraints") &&
            !IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.LengthMax))
        {
            try
            {
                var maxLength = mappedAttribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength");
                if (maxLength > 0)
                {
                    validationRuleChain.AddChainStatement($"MaximumLength({maxLength})", x => x.AddMetadata(RuleSpaceMetadataKey, RuleSpaces.LengthMax));
                    appliedRuleSpaces.Add(RuleSpaces.LengthMax);
                }
            }
            catch (Exception e)
            {
                Logging.Log.Debug("Could not resolve [Text Constraints] stereotype: " + e.Message);
            }
        }

        // 2. Container-Level Rules (Required, Collection Limits)
        ApplyContainerRules(mappedAttribute, validationRuleChain, appliedRuleSpaces);

        // 3. Instance-Level Rules (Collect & Join)
        var itemRules = GetItemLevelRules(mappedAttribute, template, appliedRuleSpaces).ToList();
        var validItemRules = itemRules.Where(r => !IsRuleSpaceApplied(appliedRuleSpaces, r.RuleSpace)).ToList();

        if (!validItemRules.Any()) return;

        if (mappedAttribute.TypeReference.IsCollection)
        {
            // Group instance rules for collections
            var joinedRules = string.Join(".", validItemRules.SelectMany(r => r.Statements));
            validationRuleChain.AddChainStatement($"ForEach(x => x.{joinedRules})");
            foreach (var rule in validItemRules) appliedRuleSpaces.Add(rule.RuleSpace);
        }
        else
        {
            // Apply individually for scalars
            foreach (var rule in validItemRules)
            {
                var isFirst = true;
                foreach (var stmt in rule.Statements)
                {
                    if (isFirst)
                    {
                        validationRuleChain.AddChainStatement(stmt, x => x.AddMetadata(RuleSpaceMetadataKey, rule.RuleSpace));
                        isFirst = false;
                    }
                    else
                    {
                        validationRuleChain.AddChainStatement(stmt);
                    }
                }
                appliedRuleSpaces.Add(rule.RuleSpace);
            }
        }
    }

    private static void ApplyContainerRules(AttributeModel attribute, CSharpMethodChainStatement chain, HashSet<string> appliedRuleSpaces)
    {
        // Required Container Check
        if (!IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.Required) &&
            attribute.HasStereotype(DomainConstraintStereotypes.Required))
        {
            chain.AddChainStatement("NotEmpty()", x => x.AddMetadata(RuleSpaceMetadataKey, RuleSpaces.Required));
            appliedRuleSpaces.Add(RuleSpaces.Required);
        }

        // Collection Limits
        if (attribute.HasStereotype(DomainConstraintStereotypes.CollectionLimits))
        {
            var limits = attribute.GetStereotype(DomainConstraintStereotypes.CollectionLimits);
            var minStr = limits?.GetProperty("Min Length")?.Value;
            var maxStr = limits?.GetProperty("Max Length")?.Value;
            int min = 0;
            int max = 0;
            var hasMin = IsPopulated(minStr) && int.TryParse(minStr, out min);
            var hasMax = IsPopulated(maxStr) && int.TryParse(maxStr, out max);

            if (hasMin && min == 1 && !hasMax && !IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.Required))
            {
                chain.AddChainStatement("NotEmpty()", x => x.AddMetadata(RuleSpaceMetadataKey, RuleSpaces.Required));
                appliedRuleSpaces.Add(RuleSpaces.Required);
            }
            else if (hasMin && hasMax && !IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.CollectionMin) && !IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.CollectionMax))
            {
                chain.AddChainStatement($"Must(c => c?.Count >= {min} && c?.Count <= {max})", x => x.AddMetadata(RuleSpaceMetadataKey, RuleSpaces.Collection));
                chain.AddChainStatement($@"WithMessage(""{ToPascalCaseName(attribute.Name)} must contain between {min} and {max} items."")");
                appliedRuleSpaces.Add(RuleSpaces.Collection);
            }
            else if (hasMin && !IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.CollectionMin))
            {
                chain.AddChainStatement($"Must(c => c?.Count >= {min})", x => x.AddMetadata(RuleSpaceMetadataKey, RuleSpaces.CollectionMin));
                chain.AddChainStatement($@"WithMessage(""{ToPascalCaseName(attribute.Name)} must contain at least {min} items."")");
                appliedRuleSpaces.Add(RuleSpaces.CollectionMin);
            }
            else if (hasMax && !IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.CollectionMax))
            {
                chain.AddChainStatement($"Must(c => c?.Count <= {max})", x => x.AddMetadata(RuleSpaceMetadataKey, RuleSpaces.CollectionMax));
                chain.AddChainStatement($@"WithMessage(""{ToPascalCaseName(attribute.Name)} must contain at most {max} items."")");
                appliedRuleSpaces.Add(RuleSpaces.CollectionMax);
            }
        }
    }

    private static IEnumerable<RuleData> GetItemLevelRules(AttributeModel attribute, ICSharpTemplate? template, HashSet<string> appliedRuleSpaces)
    {
        // Text Limits
        if (attribute.HasStereotype(DomainConstraintStereotypes.TextLimits))
        {
            var textLimits = attribute.GetStereotype(DomainConstraintStereotypes.TextLimits);
            var minStr = textLimits?.GetProperty("Min Length")?.Value;
            var maxStr = textLimits?.GetProperty("Max Length")?.Value;
            int min = 0;
            int max = 0;
            var hasMin = IsPopulated(minStr) && int.TryParse(minStr, out min);
            var hasMax = IsPopulated(maxStr) && int.TryParse(maxStr, out max);

            if (hasMin && hasMax)
            {
                var minClaimed = IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.LengthMin);
                var maxClaimed = IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.LengthMax);

                if (!minClaimed && !maxClaimed)
                    yield return new RuleData(RuleSpaces.Length, $"Length({min}, {max})");
                else if (minClaimed && !maxClaimed)
                    yield return new RuleData(RuleSpaces.LengthMax, $"MaximumLength({max})");
                else if (!minClaimed && maxClaimed)
                    yield return new RuleData(RuleSpaces.LengthMin, $"MinimumLength({min})");
                // both claimed → emit nothing
            }
            else if (hasMin) yield return new RuleData(RuleSpaces.LengthMin, $"MinimumLength({min})");
            else if (hasMax) yield return new RuleData(RuleSpaces.LengthMax, $"MaximumLength({max})");
        }

        // Numeric Limits
        if (attribute.HasStereotype(DomainConstraintStereotypes.NumericLimits))
        {
            var numLimits = attribute.GetStereotype(DomainConstraintStereotypes.NumericLimits);
            var minStr = numLimits?.GetProperty("Min Value")?.Value;
            var maxStr = numLimits?.GetProperty("Max Value")?.Value;
            var hasMin = IsPopulated(minStr);
            var hasMax = IsPopulated(maxStr);

            if (hasMin && hasMax)
            {
                var minClaimed = IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.NumericMin);
                var maxClaimed = IsRuleSpaceApplied(appliedRuleSpaces, RuleSpaces.NumericMax);

                if (!minClaimed && !maxClaimed)
                    yield return new RuleData(RuleSpaces.Numeric, $"InclusiveBetween({FormatNumericLiteral(minStr!, attribute)}, {FormatNumericLiteral(maxStr!, attribute)})");
                else if (minClaimed && !maxClaimed)
                    yield return new RuleData(RuleSpaces.NumericMax, $"LessThanOrEqualTo({FormatNumericLiteral(maxStr!, attribute)})");
                else if (!minClaimed && maxClaimed)
                    yield return new RuleData(RuleSpaces.NumericMin, $"GreaterThanOrEqualTo({FormatNumericLiteral(minStr!, attribute)})");
                // both claimed → emit nothing
            }
            else if (hasMin) yield return new RuleData(RuleSpaces.NumericMin, $"GreaterThanOrEqualTo({FormatNumericLiteral(minStr!, attribute)})");
            else if (hasMax) yield return new RuleData(RuleSpaces.NumericMax, $"LessThanOrEqualTo({FormatNumericLiteral(maxStr!, attribute)})");
        }

        // Regex
        if (attribute.HasStereotype(DomainConstraintStereotypes.RegularExpression))
        {
            var pattern = attribute.GetStereotype(DomainConstraintStereotypes.RegularExpression)?.GetProperty("Pattern")?.Value;
            var message = attribute.GetStereotype(DomainConstraintStereotypes.RegularExpression)?.GetProperty("Message")?.Value;
            if (IsPopulated(pattern))
            {
                var rule = $@"Matches(@""{pattern!.Replace("\"", "\"\"")}"")";
                if (IsPopulated(message)) 
                    yield return new RuleData(RuleSpaces.Regex, rule, $@"WithMessage(@""{message}"")");
                else 
                    yield return new RuleData(RuleSpaces.Regex, rule);
            }
        }

        // Email
        if (attribute.HasStereotype(DomainConstraintStereotypes.Email))
        {
            yield return new RuleData(RuleSpaces.Email, "EmailAddress()");
        }

        // Base64
        if (attribute.HasStereotype(DomainConstraintStereotypes.Base64))
        {
            var base64Type = template is not null ? template.UseType("System.Buffers.Text.Base64") : "System.Buffers.Text.Base64";
            yield return new RuleData(RuleSpaces.Base64, $"Must(value => {base64Type}.IsValid(value))", $"WithMessage(\"{ToPascalCaseName(attribute.Name)} must be a valid Base64 string.\")");
        }

        // URL
        if (attribute.HasStereotype(DomainConstraintStereotypes.Url))
        {
            var uriType = template is not null ? template.UseType("System.Uri") : "System.Uri";
            var uriKindType = template is not null ? template.UseType("System.UriKind") : "System.UriKind";
            yield return new RuleData(RuleSpaces.Url, $"Must(value => {uriType}.TryCreate(value, {uriKindType}.Absolute, out _))", $"WithMessage(\"{ToPascalCaseName(attribute.Name)} must be a valid URL.\")");
        }
    }

    internal static bool IsRuleSpaceApplied(HashSet<string> appliedRuleSpaces, string space)
    {
        if (appliedRuleSpaces.Contains(space)) return true;
        if (appliedRuleSpaces.Contains(RuleSpaces.Length) && space is RuleSpaces.LengthMin or RuleSpaces.LengthMax) return true;
        if (appliedRuleSpaces.Contains(RuleSpaces.Numeric) && space is RuleSpaces.NumericMin or RuleSpaces.NumericMax) return true;
        if (appliedRuleSpaces.Contains(RuleSpaces.Collection) && space is RuleSpaces.CollectionMin or RuleSpaces.CollectionMax) return true;
        return false;
    }

    internal static string FormatNumericLiteral(string value, AttributeModel attribute)
        => FormatNumericLiteral(value, attribute.TypeReference);

    internal static string FormatNumericLiteral(string value, ITypeReference typeReference)
    {
        if (typeReference.HasDecimalType()) return value + "m";
        if (typeReference.HasFloatType()) return value + "f";
        if (typeReference.HasDoubleType()) return value + "d";
        return value;
    }

    internal static bool IsPopulated(string? value) => !string.IsNullOrWhiteSpace(value);
    internal static string ToPascalCaseName(string value) => value.ToPascalCase();
}