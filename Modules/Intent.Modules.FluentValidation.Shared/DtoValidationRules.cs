using System.Collections.Generic;
using System.Linq;
using Intent.Application.FluentValidation.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

#nullable enable

namespace Intent.Modules.FluentValidation.Shared;

/// <summary>
/// Applies FluentValidation rules derived from the explicit FluentValidation stereotype
/// placed on a DTO field (e.g. NotEmpty, Length, RegularExpression, EmailAddress).
/// Claims rule-spaces in <paramref name="appliedRuleSpaces"/> so that downstream
/// domain-constraint fallback rules are skipped for the same space.
/// </summary>
internal static class DtoValidationRules
{
    /// <summary>
    /// Evaluates all explicit FluentValidation stereotype rules on <paramref name="field"/> and
    /// appends them to <paramref name="validationRuleChain"/>, following the
    /// <b>Collect and Join</b> pattern:
    /// <list type="bullet">
    /// <item><description>
    ///   <b>Container rules</b> (NotEmpty, Equal, NotEqual, Predicate, Custom/Must) are applied
    ///   directly to the chain.
    /// </description></item>
    /// <item><description>
    ///   <b>Item-level rules</b> (Length, Numeric, Regex, Email) are collected as
    ///   <see cref="RuleData"/> instances and, for collection fields, joined into a single
    ///   <c>.ForEach(x => x.Rule1().Rule2()...)</c> chain statement.
    /// </description></item>
    /// </list>
    /// </summary>
    internal static void ApplyRules(
        DTOFieldModel field,
        CSharpMethodChainStatement validationRuleChain,
        bool customValidationEnabled,
        ICSharpTemplate? template,
        HashSet<string> appliedRuleSpaces)
    {
        var validations = field.GetValidations();
        var itemRules = new List<RuleData>();

        if (validations.NotEmpty())
        {
            validationRuleChain.AddChainStatement("NotEmpty()",
                stmt => stmt.AddMetadata(DomainConstraintRules.RuleSpaceMetadataKey, RuleSpaces.Required));
            appliedRuleSpaces.Add(RuleSpaces.Required);
        }

        if (!string.IsNullOrWhiteSpace(validations.Equal()))
        {
            validationRuleChain.AddChainStatement($"Equal({validations.Equal()})");
        }

        if (!string.IsNullOrWhiteSpace(validations.NotEqual()))
        {
            validationRuleChain.AddChainStatement($"NotEqual({validations.NotEqual()})");
        }

        if (validations.MinLength() != null && validations.MaxLength() != null)
        {
            itemRules.Add(new RuleData(RuleSpaces.Length, $"Length({validations.MinLength()}, {validations.MaxLength()})"));
            appliedRuleSpaces.Add(RuleSpaces.Length);
        }
        else if (validations.MinLength() != null)
        {
            itemRules.Add(new RuleData(RuleSpaces.LengthMin, $"MinimumLength({validations.MinLength()})"));
            appliedRuleSpaces.Add(RuleSpaces.LengthMin);
        }
        else if (validations.MaxLength() != null)
        {
            itemRules.Add(new RuleData(RuleSpaces.LengthMax, $"MaximumLength({validations.MaxLength()})"));
            appliedRuleSpaces.Add(RuleSpaces.LengthMax);
        }

        if (validations.Min() != null && validations.Max() != null &&
            int.TryParse(validations.Min(), out var min) && int.TryParse(validations.Max(), out var max))
        {
            itemRules.Add(new RuleData(RuleSpaces.Numeric, $"InclusiveBetween({min}, {max})"));
            appliedRuleSpaces.Add(RuleSpaces.Numeric);
        }
        else if (!string.IsNullOrWhiteSpace(validations.Min()))
        {
            itemRules.Add(new RuleData(RuleSpaces.NumericMin, $"GreaterThanOrEqualTo({validations.Min()})"));
            appliedRuleSpaces.Add(RuleSpaces.NumericMin);
        }
        else if (!string.IsNullOrWhiteSpace(validations.Max()))
        {
            itemRules.Add(new RuleData(RuleSpaces.NumericMax, $"LessThanOrEqualTo({validations.Max()})"));
            appliedRuleSpaces.Add(RuleSpaces.NumericMax);
        }

        if (!string.IsNullOrWhiteSpace(validations.RegularExpression()))
        {
            string? regexRule = null;
            var invocation = new CSharpInvocationStatement($"new {template?.UseType("System.Text.RegularExpressions.Regex") ?? "System.Text.RegularExpressions.Regex"}")
                            .AddArgument($"@\"{validations.RegularExpression()}\"")
                            .AddArgument("RegexOptions.Compiled")
                            .AddArgument($"{template?.UseType("System.TimeSpan") ?? "System.TimeSpan"}.FromSeconds({validations.RegularExpressionTimeout() ?? 1})")
                            .WithoutSemicolon();

            // If template is null or not a file-builder template, use the less efficient inline Regex expression.
            if (template is not ICSharpFileBuilderTemplate builderTemplate)
            {
                regexRule = $@"Matches({invocation})";
            }
            else
            {
                if (builderTemplate.CSharpFile.Classes.Any())
                {
                    var regexName = $"{field.Name}Regex";
                    var @class = builderTemplate.CSharpFile.Classes.First();

                    if (@class.Fields.All(f => f.Name != regexName))
                    {
                        @class.AddField(builderTemplate.UseType("System.Text.RegularExpressions.Regex"), regexName, @field =>
                        {
                            @field.Static().PrivateReadOnly();
                            @field.WithAssignment(invocation);
                        });
                    }

                    regexRule = $@"Matches({regexName})";
                }
                else
                {
                    regexRule = $@"Matches({invocation})";
                }
            }

            if (!string.IsNullOrWhiteSpace(regexRule))
            {
                if (!string.IsNullOrWhiteSpace(validations.RegularExpressionMessage()))
                {
                    itemRules.Add(new RuleData(RuleSpaces.Regex, regexRule, $@"WithMessage(""{validations.RegularExpressionMessage()}"")"));
                }
                else
                {
                    itemRules.Add(new RuleData(RuleSpaces.Regex, regexRule));
                }

                appliedRuleSpaces.Add(RuleSpaces.Regex);
            }

        }

        if (validations.EmailAddress())
        {
            itemRules.Add(new RuleData(RuleSpaces.Email, "EmailAddress()"));
            appliedRuleSpaces.Add(RuleSpaces.Email);
        }

        if (field.TypeReference.IsCollection && itemRules.Any())
        {
            var joinedRules = string.Join(".", itemRules.SelectMany(r => r.Statements));
            validationRuleChain.AddChainStatement($"ForEach(x => x.{joinedRules})");
        }
        else
        {
            foreach (var itemRule in itemRules)
            {
                foreach (var stmt in itemRule.Statements)
                {
                    validationRuleChain.AddChainStatement(stmt);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(validations.Predicate()))
        {
            validationRuleChain.AddChainStatement($"Must({validations.Predicate()})");
            if (!string.IsNullOrWhiteSpace(validations.PredicateMessage()))
            {
                validationRuleChain.AddChainStatement($"WithMessage(\"{validations.PredicateMessage()}\")");
            }
        }

        if (!customValidationEnabled)
        {
            return;
        }
        if (validations.Custom())
        {
            validationRuleChain.AddChainStatement($"CustomAsync(Validate{field.Name.ToPascalCase()}Async)");
        }

        if (!validations.HasCustomValidation() && !validations.Must())
        {
            return;
        }
        validationRuleChain.AddChainStatement($"MustAsync(Validate{field.Name.ToPascalCase()}Async)");

        if (!string.IsNullOrWhiteSpace(validations.MustMessage()))
        {
            validationRuleChain.AddChainStatement($@"WithMessage(""{validations.MustMessage()}"")");
        }
    }
}
