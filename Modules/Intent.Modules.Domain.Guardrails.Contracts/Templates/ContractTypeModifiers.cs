using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Domain.Guardrails.Contracts.Settings;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The modifiers a class, record or enum contract is declared with. Under plain Mode.Merge, RoslynWeaver replaces the existing
    /// declaration's modifiers with the generated ones, so they must match the code. The element's Type Configuration Modifiers
    /// (Enum Configuration Access Modifier for an enum) wins; otherwise the Default Type Modifiers module setting applies;
    /// otherwise <c>public</c>.
    /// Same rules as the Services module's ContractTypeModifiers; keep the two in step.
    /// </summary>
    internal static class ContractTypeModifiers
    {
        private const string FallbackModifiers = "public";
        private const string SupportedModifiersText = "Use public or internal, plus any of sealed, abstract, static or partial.";

        /// <summary>
        /// Applies the modifiers to <paramref name="type"/>. Call it after the type's members are configured, because abstract
        /// also comes from the element's Is Abstract and from abstract operations, and sealed or static can't be combined with it.
        /// The module setting is a convention for every type, so its sealed is left off a type that can't be sealed (abstract,
        /// or inherited by another modelled type). Modifiers set on the element are taken as given, and a conflict is an error.
        /// </summary>
        public static void ApplyContractModifiers(this CSharpClass type, ICSharpTemplate template, IElement element, string configuredModifiers, bool isInherited)
        {
            if (!string.IsNullOrWhiteSpace(configuredModifiers))
            {
                var configured = Parse(configuredModifiers, error => new ElementException(element,
                    $"The Type Configuration Modifiers '{configuredModifiers.Trim()}' on '{element.Name}' aren't valid: {error}"));
                ValidateConfigured(type, element, configured, isInherited);
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
        /// Applies an enum's access modifier: its Enum Configuration Access Modifier, otherwise the access modifier of the
        /// Default Type Modifiers module setting. An enum can't be sealed or partial, so the rest of the setting doesn't apply.
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
                default:
                    throw new ElementException(element,
                        $"The Enum Configuration Access Modifier '{access}' on '{element.Name}' isn't valid. Use public or internal.");
            }
        }

        private static ParsedModifiers GetDefaultModifiers(ICSharpTemplate template)
        {
            var setting = template.ExecutionContext.Settings.GetDomainGuardrailsContractsSettings().DefaultTypeModifiers().Value;
            var defaults = Parse(string.IsNullOrWhiteSpace(setting) ? FallbackModifiers : setting, error => new FriendlyException(
                $"The Default Type Modifiers setting (Domain Guardrails Contracts Settings) is '{setting}', which isn't valid: {error} Choose one of the setting's options."));
            if (defaults.IsAbstract || defaults.IsStatic)
            {
                throw new FriendlyException(
                    $"The Default Type Modifiers setting (Domain Guardrails Contracts Settings) is '{setting}', but abstract and static can't be a default for every type. " +
                    "Choose one of the setting's options, and set abstract or static through Type Configuration Modifiers on the elements that need it.");
            }

            return defaults;
        }

        public static bool IsInheritedByAnyClass(this ICSharpTemplate template, ClassModel model)
        {
            return template.GetDomainDesigner().GetClassModels().Any(x => x.ParentClassTypeReference?.Element?.Id == model.Id);
        }

        public static bool IsInheritedByAnyDataContract(this ICSharpTemplate template, DataContractModel model)
        {
            return template.GetDomainDesigner().GetDataContractModels().Any(x => x.BaseDataContract?.Id == model.Id);
        }

        public static bool IsInheritedByAnyDomainEvent(this ICSharpTemplate template, IElement domainEvent)
        {
            return template.GetDomainDesigner().GetDomainEvents().Any(x => x.GetBaseDomainEvent()?.Id == domainEvent.Id);
        }

        private static IDesigner GetDomainDesigner(this ICSharpTemplate template)
        {
            return template.ExecutionContext.MetadataManager.Domain(template.ExecutionContext.GetApplicationConfig().Id);
        }

        private static void ValidateConfigured(CSharpClass type, IElement element, ParsedModifiers modifiers, bool isInherited)
        {
            if (type.IsAbstract && (modifiers.IsSealed || modifiers.IsStatic))
            {
                throw new ElementException(element,
                    $"'{element.Name}' is abstract (Is Abstract, or an abstract operation), so its Type Configuration Modifiers can't include {(modifiers.IsSealed ? "sealed" : "static")}.");
            }

            if (isInherited && (modifiers.IsSealed || modifiers.IsStatic))
            {
                throw new ElementException(element,
                    $"Another modelled type inherits from '{element.Name}', so its Type Configuration Modifiers can't include {(modifiers.IsSealed ? "sealed" : "static")}.");
            }

            if (modifiers.IsStatic && type is CSharpRecord)
            {
                throw new ElementException(element,
                    $"'{element.Name}' is a record, and C# doesn't allow a static record. Remove static from its Type Configuration Modifiers, or set Type to class.");
            }
        }

        private static void Apply(CSharpClass type, ParsedModifiers modifiers)
        {
            if (modifiers.Access == "internal")
            {
                type.Internal();
            }
            else
            {
                type.Public();
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

        /// <summary>
        /// Keywords in any order, separated by whitespace. Exactly one access modifier is required: the builder always writes
        /// one, so a type declared without one (implicitly internal) must be modelled as internal.
        /// </summary>
        private static ParsedModifiers Parse(string value, Func<string, Exception> invalid)
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
                            throw invalid($"it has two access modifiers ({result.Access} and {token}). {SupportedModifiersText}");
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
                    default:
                        throw invalid($"'{token}' isn't supported. {SupportedModifiersText}");
                }
            }

            if (result.Access == null)
            {
                throw invalid($"it has no access modifier. A type declared without one is internal, so use internal. {SupportedModifiersText}");
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
