using System.Collections.Generic;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The modifiers of a method, constructor or property contract, from Operation, Constructor or Property Configuration. Constructor
    /// and method signatures are fully managed under Merge, so these replace the existing modifiers and must match the code.
    /// The builder silently drops a modifier that conflicts with a later one, so every combination C# rejects is an error on the
    /// element here instead. An empty Access Modifier means public, the stereotype's default; none writes no access modifier.
    /// Same rules as the Domain module's ContractMemberModifiers; keep the two in step.
    /// </summary>
    internal static class ContractMemberModifiers
    {
        /// <summary>
        /// Applies a method's access modifier and static, new, virtual, override and sealed. Abstract is validated here but applied
        /// by the caller, which also makes the class abstract and leaves the async keyword off. Call it before Abstract(); the
        /// validation has already rejected everything Abstract() would clear.
        /// </summary>
        public static void ApplyContractModifiers(this CSharpClassMethod method, IElement element, string accessModifier,
            bool isStatic, bool isAbstract, bool isVirtual, bool isOverride, bool isSealed, bool isNew)
        {
            Validate(element, accessModifier, isStatic, isAbstract, isVirtual, isOverride, isSealed, isNew);

            switch (accessModifier)
            {
                case null or "" or "public":
                    method.Public();
                    break;
                case "protected":
                    method.Protected();
                    break;
                case "internal":
                    method.Internal();
                    break;
                case "private":
                    method.Private();
                    break;
                case "protected internal":
                    method.ProtectedInternal();
                    break;
                case "none":
                    method.WithoutAccessModifier();
                    break;
                default:
                    throw new ElementException(element,
                        $"Operation '{element.Name}' has an Operation Configuration Access Modifier of '{accessModifier}', which isn't valid. " +
                        "Choose public, protected, internal, private, protected internal or none.");
            }

            // Each builder call clears the modifiers C# forbids alongside it; Validate has already rejected those combinations.
            if (isStatic)
            {
                method.Static();
            }

            if (isNew)
            {
                method.New();
            }

            if (isVirtual)
            {
                method.Virtual();
            }

            if (isOverride)
            {
                method.Override();
            }

            if (isSealed)
            {
                method.Sealed();
            }
        }

        /// <summary>
        /// Applies a constructor's access modifier. The builder can't omit a constructor's access modifier, so a constructor the
        /// code declares without one (implicitly private) is modelled as private.
        /// </summary>
        public static void ApplyContractAccessModifier(this CSharpConstructor constructor, IElement element, string accessModifier)
        {
            switch (accessModifier)
            {
                case null or "" or "public":
                    constructor.Public();
                    break;
                case "protected":
                    constructor.Protected();
                    break;
                case "internal":
                    constructor.Internal();
                    break;
                case "private":
                    constructor.Private();
                    break;
                case "protected internal":
                    constructor.ProtectedInternal();
                    break;
                default:
                    throw new ElementException(element,
                        $"Constructor '{element.Name}' has a Constructor Configuration Access Modifier of '{accessModifier}', which isn't valid. " +
                        "Choose public, protected, internal, private or protected internal.");
            }
        }

        /// <summary>
        /// Applies a property's access modifier, from Property Configuration. Under Merge a property's own modifiers follow the
        /// generated ones, so an unmodelled access modifier would be rewritten to public on the first merge.
        /// </summary>
        public static void ApplyContractAccessModifier(this CSharpProperty property, IElement element, string accessModifier)
        {
            switch (accessModifier)
            {
                case null or "" or "public":
                    property.Public();
                    break;
                case "protected":
                    property.Protected();
                    break;
                case "internal":
                    property.Internal();
                    break;
                case "private":
                    property.Private();
                    break;
                case "protected internal":
                    property.ProtectedInternal();
                    break;
                case "none":
                    property.WithoutAccessModifier();
                    break;
                default:
                    throw new ElementException(element,
                        $"Property '{element.Name}' has a Property Configuration Access Modifier of '{accessModifier}', which isn't valid. " +
                        "Choose public, protected, internal, private, protected internal or none.");
            }
        }

        private static void Validate(IElement element, string accessModifier,
            bool isStatic, bool isAbstract, bool isVirtual, bool isOverride, bool isSealed, bool isNew)
        {
            var problems = new List<string>();
            if (isStatic && (isVirtual || isOverride || isSealed || isAbstract))
            {
                problems.Add("a static method can't also be virtual, override, sealed or abstract");
            }

            if (isVirtual && isOverride)
            {
                problems.Add("an override is already virtual, so untick Virtual");
            }

            if (isVirtual && isAbstract)
            {
                problems.Add("an abstract method is already virtual, so untick Virtual");
            }

            if (isSealed && !isOverride)
            {
                problems.Add("sealed is only valid as sealed override, so tick Override as well");
            }

            if (isSealed && isAbstract)
            {
                problems.Add("an abstract method can't be sealed");
            }

            if (isNew && isOverride)
            {
                problems.Add("a method can't be both new and override");
            }

            if ((isVirtual || isAbstract || isOverride) && (accessModifier is "private" or "none"))
            {
                problems.Add("a virtual, abstract or override method can't be private, and none means private");
            }

            if (problems.Count > 0)
            {
                throw new ElementException(element,
                    $"Operation '{element.Name}' has modifiers C# doesn't allow together: {string.Join("; ", problems)}. " +
                    "Set Operation Configuration (and Is Static or Is Abstract, where the operation has them) to the modifiers the code declares.");
            }
        }
    }
}
