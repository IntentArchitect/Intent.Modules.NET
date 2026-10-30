using System;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Designer Values as C# default values: an optional parameter's <c>= value</c> and a property's initializer.
    /// The Value is formatted by Intent.Modules.Common.CSharp's <c>AsFormattedValidTypeValue</c>, the convention the Dtos and
    /// CQRS templates use:
    /// <list type="bullet">
    /// <item>a string is quoted, unless it already is, is <c>null</c> or <c>default</c>, or looks like an expression
    /// (contains <c>(</c> or <c>.</c>, or starts with <c>_</c>);</item>
    /// <item>an enum literal name is qualified with its enum type;</item>
    /// <item>a bare DateTime, Date or DateTimeOffset member (e.g. <c>Now</c>) is qualified with its type;</item>
    /// <item>a value starting with <c>@</c> is used verbatim, without the <c>@</c>;</item>
    /// <item>anything else is used as written.</item>
    /// </list>
    /// Same as the Services module's ContractDefaultValues; keep the two in step.
    /// </summary>
    internal static class ContractDefaultValues
    {
        /// <summary>
        /// Sets a method parameter's default value from its designer Value, if one is set.
        /// </summary>
        public static void WithContractDefaultValue(this CSharpParameter parameter, ICSharpTemplate template, IElement element, string value, ITypeReference typeReference, string modifier)
        {
            var defaultValue = template.GetValidatedParameterDefault(element, value, typeReference, modifier);
            if (defaultValue != null)
            {
                parameter.WithDefaultValue(defaultValue);
            }
        }

        /// <summary>
        /// Sets a constructor parameter's default value from its designer Value, if one is set.
        /// </summary>
        public static void WithContractDefaultValue(this CSharpConstructorParameter parameter, ICSharpTemplate template, IElement element, string value, ITypeReference typeReference, string modifier)
        {
            var defaultValue = template.GetValidatedParameterDefault(element, value, typeReference, modifier);
            if (defaultValue != null)
            {
                parameter.WithDefaultValue(defaultValue);
            }
        }

        /// <summary>
        /// Sets a property's initializer from its designer Value, if one is set.
        /// </summary>
        public static void WithContractInitialValue(this CSharpProperty property, ICSharpTemplate template, string value, ITypeReference typeReference)
        {
            var initialValue = template.GetContractDefaultValue(value, typeReference);
            if (initialValue != null)
            {
                property.WithInitialValue(initialValue);
            }
        }

        /// <summary>
        /// C# doesn't allow a default value on a <c>ref</c>, <c>out</c> or <c>params</c> parameter, so that combination is an
        /// error on the element.
        /// </summary>
        private static string GetValidatedParameterDefault(this ICSharpTemplate template, IElement element, string value, ITypeReference typeReference, string modifier)
        {
            var defaultValue = template.GetContractDefaultValue(value, typeReference);
            if (defaultValue != null && modifier is "ref" or "out" or "params")
            {
                throw new ElementException(element,
                    $"Parameter '{element.Name}' has a Value ({defaultValue}) and the Parameter Modifier {modifier}, but C# doesn't allow a default value on a {modifier} parameter. " +
                    "Clear the Value, or set the Parameter Modifier to match the code.");
            }

            return defaultValue;
        }

        /// <summary>
        /// The C# default value for a designer Value, or null when no Value is set.
        /// </summary>
        private static string GetContractDefaultValue(this ICSharpTemplate template, string value, ITypeReference typeReference)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();
            if (typeReference?.Element == null)
            {
                return trimmed;
            }

            if (template is not ICSharpFileBuilderTemplate fileBuilderTemplate)
            {
                throw new InvalidOperationException($"Template '{template.Id}' must be an {nameof(ICSharpFileBuilderTemplate)} to format default values.");
            }

            return trimmed.AsFormattedValidTypeValue(fileBuilderTemplate, typeReference);
        }
    }
}
