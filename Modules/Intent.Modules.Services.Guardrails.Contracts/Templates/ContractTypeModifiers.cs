using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Services.Guardrails.Contracts.Settings;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The modifiers a class, record or enum contract is declared with. Under plain Mode.Merge, RoslynWeaver replaces the existing
    /// declaration's modifiers with the generated ones, so they must match the code. The element's Type Configuration Modifiers
    /// (Enum Configuration Access Modifier for an enum) wins; otherwise the Default Type Modifiers module setting applies;
    /// otherwise <c>public</c>. The access modifier is optional, so a type the code declares without one is modelled without
    /// one, and <c>none</c> means no modifiers at all.
    /// Same rules as the Domain module's ContractTypeModifiers; keep the two in step.
    /// </summary>
    public static class ContractTypeModifiers
    {
        private const string FallbackModifiers = "public";
        private const string NoModifiers = "none";
        private const string TypeConfigurationName = "Type Configuration";
        private const string SupportedModifiersText = "Use at most one of public or internal, plus any of sealed, abstract, static or partial, or none for no modifiers.";
        private const string SupportedInterfaceModifiersText = "For an interface, use at most one of public or internal, plus partial, or none for no modifiers.";

        /// <summary>
        /// Applies the modifiers to <paramref name="type"/>. Call it after the type's members are configured, because abstract
        /// also comes from the element's Is Abstract, and sealed or static can't be combined with it. The module setting is a
        /// convention for every type, so its sealed is left off a type that can't be sealed (abstract, or inherited by another
        /// modelled type). Modifiers set on the element are taken as given, and a conflict is an error.
        /// <paramref name="configurationName"/> is the stereotype that holds Modifiers, for error messages.
        /// </summary>
        public static void ApplyContractModifiers(this CSharpClass type, ICSharpTemplate template, IElement element, string configuredModifiers, bool isInherited, string configurationName = TypeConfigurationName)
        {
            if (!string.IsNullOrWhiteSpace(configuredModifiers))
            {
                var configured = Parse(configuredModifiers, SupportedModifiersText, error => new ElementException(element,
                    $"The {configurationName} Modifiers '{configuredModifiers.Trim()}' on '{element.Name}' aren't valid: {error}"));
                ValidateConfigured(type, element, configured, isInherited, configurationName);
                Apply(type, configured);
                return;
            }

            var defaults = GetDefaultModifiers(template);
            if (type.IsAbstract || isInherited)
            {
                defaults.IsSealed = false;
            }

            Apply(type, defaults);
        }

        /// <summary>
        /// Applies an interface's modifiers: <paramref name="configuredModifiers"/> when set, otherwise the access modifier of the
        /// Default Type Modifiers module setting, as for an enum. An interface can be partial but not sealed, abstract or
        /// static, so only an access modifier and partial are accepted, and the rest of the setting doesn't apply.
        /// <paramref name="configurationName"/> is the stereotype that holds Modifiers, for error messages.
        /// </summary>
        public static void ApplyContractModifiers(this CSharpInterface type, ICSharpTemplate template, IElement element, string configuredModifiers, string configurationName)
        {
            if (string.IsNullOrWhiteSpace(configuredModifiers))
            {
                ApplyAccessModifier(type, GetDefaultModifiers(template).Access);
                return;
            }

            var configured = Parse(configuredModifiers, SupportedInterfaceModifiersText, error => new ElementException(element,
                $"The {configurationName} Modifiers '{configuredModifiers.Trim()}' on '{element.Name}' aren't valid: {error}"));
            if (configured.IsSealed || configured.IsAbstract || configured.IsStatic)
            {
                throw new ElementException(element,
                    $"'{element.Name}' is an interface, so its {configurationName} Modifiers can't include {(configured.IsSealed ? "sealed" : configured.IsAbstract ? "abstract" : "static")}. {SupportedInterfaceModifiersText}");
            }

            ApplyAccessModifier(type, configured.Access);
            if (configured.IsPartial)
            {
                type.Partial();
            }
        }

        /// <summary>
        /// Applies an enum's access modifier: its Enum Configuration Access Modifier, otherwise the access modifier of the
        /// Default Type Modifiers module setting. An enum can't be sealed or partial, so the rest of the setting doesn't apply.
        /// An Access Modifier of none, or a setting without an access modifier, declares the enum without one.
        /// </summary>
        public static void ApplyContractAccessModifier(this CSharpEnum @enum, ICSharpTemplate template, IElement element, string configuredAccessModifier)
        {
            var access = string.IsNullOrWhiteSpace(configuredAccessModifier)
                ? GetDefaultModifiers(template).Access
                : configuredAccessModifier.Trim();
            switch (access)
            {
                case "public":
                    @enum.Public();
                    break;
                case "internal":
                    @enum.Internal();
                    break;
                case null:
                case NoModifiers:
                    @enum.WithoutAccessModifier();
                    break;
                default:
                    throw new ElementException(element,
                        $"The Enum Configuration Access Modifier '{access}' on '{element.Name}' isn't valid. Use public, internal or none.");
            }
        }

        private static ParsedModifiers GetDefaultModifiers(ICSharpTemplate template)
        {
            var setting = template.ExecutionContext.Settings.GetServicesGuardrailsContractsSettings().DefaultTypeModifiers().Value;
            var defaults = Parse(string.IsNullOrWhiteSpace(setting) ? FallbackModifiers : setting, SupportedModifiersText, error => new FriendlyException(
                $"The Default Type Modifiers setting (Services Guardrails Contracts Settings) is '{setting}', which isn't valid: {error} Choose one of the setting's options."));
            if (defaults.IsAbstract || defaults.IsStatic)
            {
                throw new FriendlyException(
                    $"The Default Type Modifiers setting (Services Guardrails Contracts Settings) is '{setting}', but abstract and static can't be a default for every type. " +
                    "Choose one of the setting's options, and set abstract or static through Type Configuration Modifiers on the elements that need it.");
            }

            return defaults;
        }

        public static bool IsInheritedByAnyDto(this ICSharpTemplate template, DTOModel model)
        {
            return template.ExecutionContext.MetadataManager.Services(template.ExecutionContext.GetApplicationConfig().Id)
                .GetDTOModels()
                .Any(x => x.ParentDtoTypeReference?.Element?.Id == model.Id);
        }

        private static void ValidateConfigured(CSharpClass type, IElement element, ParsedModifiers modifiers, bool isInherited, string configurationName)
        {
            if (type.IsAbstract && (modifiers.IsSealed || modifiers.IsStatic))
            {
                throw new ElementException(element,
                    $"'{element.Name}' is abstract (Is Abstract), so its {configurationName} Modifiers can't include {(modifiers.IsSealed ? "sealed" : "static")}.");
            }

            if (isInherited && (modifiers.IsSealed || modifiers.IsStatic))
            {
                throw new ElementException(element,
                    $"Another modelled type inherits from '{element.Name}', so its {configurationName} Modifiers can't include {(modifiers.IsSealed ? "sealed" : "static")}.");
            }

            if (modifiers.IsStatic && type is CSharpRecord)
            {
                throw new ElementException(element,
                    $"'{element.Name}' is a record, and C# doesn't allow a static record. Remove static from its {configurationName} Modifiers, or set Type to class.");
            }
        }

        private static void Apply(CSharpClass type, ParsedModifiers modifiers)
        {
            switch (modifiers.Access)
            {
                case "public":
                    type.Public();
                    break;
                case "internal":
                    type.Internal();
                    break;
                case null:
                    type.WithoutAccessModifier();
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected access modifier '{modifiers.Access}'.");
            }

            if (modifiers.IsSealed)
            {
                type.Sealed();
            }

            if (modifiers.IsAbstract && !type.IsAbstract)
            {
                type.Abstract();
            }

            if (modifiers.IsStatic)
            {
                type.Static();
            }

            if (modifiers.IsPartial)
            {
                type.Partial();
            }
        }

        private static void ApplyAccessModifier(CSharpInterface type, string access)
        {
            switch (access)
            {
                case "public":
                    type.Public();
                    break;
                case "internal":
                    type.Internal();
                    break;
                case null:
                    type.WithoutAccessModifier();
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected access modifier '{access}'.");
            }
        }

        /// <summary>
        /// Keywords in any order, separated by whitespace. At most one access modifier; without one the type is declared without
        /// one, as the code does. none stands alone and means no modifiers at all: a keyword is needed because empty Modifiers
        /// means the module setting applies. <paramref name="supportedText"/> ends the error messages.
        /// </summary>
        private static ParsedModifiers Parse(string value, string supportedText, Func<string, Exception> invalid)
        {
            var result = new ParsedModifiers();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var token in value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!seen.Add(token))
                {
                    throw invalid($"'{token}' appears more than once.");
                }

                switch (token)
                {
                    case "public":
                    case "internal":
                        if (result.Access != null)
                        {
                            throw invalid($"it has two access modifiers ({result.Access} and {token}). {supportedText}");
                        }

                        result.Access = token;
                        break;
                    case "sealed":
                        result.IsSealed = true;
                        break;
                    case "abstract":
                        result.IsAbstract = true;
                        break;
                    case "static":
                        result.IsStatic = true;
                        break;
                    case "partial":
                        result.IsPartial = true;
                        break;
                    case NoModifiers:
                        break;
                    default:
                        throw invalid($"'{token}' isn't supported. {supportedText}");
                }
            }

            if (seen.Contains(NoModifiers) && seen.Count > 1)
            {
                throw invalid($"none means no modifiers at all, so it can't be combined with other keywords. {supportedText}");
            }

            if (new[] { result.IsSealed, result.IsAbstract, result.IsStatic }.Count(x => x) > 1)
            {
                throw invalid("sealed, abstract and static can't be combined; use at most one of them.");
            }

            return result;
        }

        private sealed class ParsedModifiers
        {
            public string Access { get; set; }
            public bool IsSealed { get; set; }
            public bool IsAbstract { get; set; }
            public bool IsStatic { get; set; }
            public bool IsPartial { get; set; }
        }
    }
}
