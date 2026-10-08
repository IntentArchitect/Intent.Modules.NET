using System.Text.RegularExpressions;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Works around an arity-blind conflict check in Intent.Modules.Common.CSharp's <c>NormalizeNamespace</c>
    /// (3.10.5 to 3.10.11, and the development source at the time of writing). Its known-types registry holds the
    /// non-generic <c>System.Collections</c> types, such as <c>IEnumerable</c>. So when a file's usings include
    /// <c>System.Collections</c> as well as <c>System.Collections.Generic</c>, it treats <c>IEnumerable&lt;T&gt;</c> as
    /// ambiguous and writes <c>System.Collections.Generic.IEnumerable&lt;T&gt;</c>, although C# tells the two apart by
    /// arity. The usings it checks include those of the existing file on disk, and legacy files often import
    /// <c>System.Collections</c>. Same as the Domain module's ContractTypeNames; keep the two in step.
    /// </summary>
    public static class ContractTypeNames
    {
        private const string GenericCollectionsNamespace = "System.Collections.Generic";

        // The System.Collections.Generic types whose simple name is also a non-generic System.Collections type. Only a
        // generic use (the name followed by '<') is shortened, and never a global::-qualified one.
        private static readonly Regex ArityOnlyQualifier = new Regex(
            @"(?<![\w.:])System\.Collections\.Generic\.(?=(?:Comparer|ICollection|IComparer|IDictionary|IEnumerable|IEnumerator|IEqualityComparer|IList|Queue|SortedList|Stack)\s*<)",
            RegexOptions.CultureInvariant);

        /// <summary>
        /// Drops the <c>System.Collections.Generic.</c> qualifier that the arity-blind check added, and makes sure the
        /// file has the <c>using</c> the short name needs.
        /// </summary>
        public static string WithoutArityOnlyQualifiers<TModel>(this CSharpTemplateBase<TModel> template, string typeName)
        {
            if (string.IsNullOrEmpty(typeName) || !ArityOnlyQualifier.IsMatch(typeName))
            {
                return typeName;
            }

            template.AddUsing(GenericCollectionsNamespace);
            return ArityOnlyQualifier.Replace(typeName, string.Empty);
        }
    }
}
