using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Implemented by the six Domain contract templates so that templates writing to the same output
    /// file can find each other. Declared on a hand-written partial of each template, because the base
    /// list of the designer-generated partial is Signature-Fully managed.
    /// </summary>
    public interface IDomainContractTemplate : ICSharpFileBuilderTemplate
    {
        /// <summary>The designer element this template generates a type for.</summary>
        IElement ContractElement { get; }
    }

    /// <summary>
    /// File Config support: an element's file name and namespace can differ from the defaults, and
    /// several elements can share one file. Templates whose output paths are equal (OrdinalIgnoreCase,
    /// the same comparison the Software Factory uses to detect duplicate output paths) form a group.
    /// One owner writes the file containing every member's type; the others do not run but stay
    /// discoverable for type resolution.
    /// </summary>
    internal static class ContractFileGroups
    {
        private static readonly char[] PathSeparators = ['/', '\\'];

        // Invalid on Windows; listed explicitly so validation does not depend on the host OS.
        private static readonly char[] InvalidFileNameChars = [':', '*', '?', '"', '<', '>', '|'];

        /// <summary>
        /// The namespace for a contract file: empty (the global namespace) when File Config No Namespace is
        /// ticked, a File Config Namespace when one is set, otherwise <paramref name="defaultNamespace"/> (the
        /// one derived from the element's folders). The configured value is used exactly as written, so it is
        /// validated rather than corrected.
        /// </summary>
        public static string GetContractNamespace(IElement element, string configuredNamespace, bool noNamespace, string defaultNamespace)
        {
            if (noNamespace)
            {
                if (!string.IsNullOrWhiteSpace(configuredNamespace))
                {
                    throw new ElementException(element,
                        $"{element.SpecializationType} '{element.Name}' has File Config No Namespace ticked and also a Namespace of '{configuredNamespace}'. " +
                        "Choose one: clear the Namespace to declare the type in the global namespace, or untick No Namespace to use the Namespace.");
                }

                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(configuredNamespace))
            {
                return defaultNamespace;
            }

            var value = configuredNamespace.Trim();
            if (!IsVerbatimNamespace(value))
            {
                throw new ElementException(element,
                    $"{element.SpecializationType} '{element.Name}' has a File Config Namespace of '{configuredNamespace}', which is not a valid C# namespace. " +
                    "Use dot-separated identifiers that each start with a letter or underscore and contain only letters, digits and underscores, " +
                    "for example MyCompany.Ordering.Domain, or clear it to use the namespace derived from the element's folders.");
            }

            return value;
        }

        /// <summary>
        /// Applies a File Config Filename, if any. A trailing ".cs" is optional. With it, the name is taken exactly as
        /// written (only whitespace after ".cs" is ignored), so a file whose name starts or ends with a space, such as
        /// "abc .cs", can be targeted; without it, surrounding whitespace is trimmed. The file always stays in the
        /// element's own folder. Empty or absent means the default (the element name).
        /// Must be called in the template constructor, before the file config is read.
        /// </summary>
        public static CSharpFile WithContractFileName(this CSharpFile file, IElement element, string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return file;
            }

            var name = filename.TrimEnd();
            if (name.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - ".cs".Length);
            }
            else
            {
                name = name.Trim();
            }

            if (name.IndexOfAny(PathSeparators) >= 0)
            {
                throw new ElementException(element,
                    $"{element.SpecializationType} '{element.Name}' has a File Config Filename of '{filename}', which contains a folder path. " +
                    "Filename must be a file name only, because the file is always placed in the element's own folder. " +
                    "Move the element into the designer folder that matches the file's location and set Filename to just the file name.");
            }

            if (string.IsNullOrWhiteSpace(name) ||
                name == "." ||
                name == ".." ||
                name.EndsWith(".", StringComparison.Ordinal) ||
                name.IndexOfAny(InvalidFileNameChars) >= 0 ||
                name.Any(char.IsControl))
            {
                throw new ElementException(element,
                    $"{element.SpecializationType} '{element.Name}' has a File Config Filename of '{filename}', which is not a valid file name. " +
                    "Set Filename to the name of the existing file, for example Order or Order.cs, without characters such as : * ? \" < > |, " +
                    "or clear it to use the element name.");
            }

            return file.WithFileName(name);
        }

        /// <summary>
        /// Registers the multi-type file behaviour. When this template owns a file shared with other
        /// contract templates, their types are added to this template's file.
        /// <para>
        /// Priority 0 (the Core band, the same band as every template's own type configuration):
        /// the added types' own configuration delegates are queued at priority 0 as well, so the
        /// whole file is complete before any Enrichment (100) or later callbacks from other modules
        /// run. Membership is decided by output path, which is fixed at template registration, so
        /// nothing earlier in the build can change the group.
        /// </para>
        /// </summary>
        public static void RegisterContractFileGroup(this IDomainContractTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var group = GetFileGroup(template);
                if (group == null || !ReferenceEquals(group.Owner, template))
                {
                    return;
                }

                foreach (var member in group.Others)
                {
                    // An empty namespace is File Config No Namespace, so this also requires the members to agree on it.
                    if (!string.Equals(member.CSharpFile.Namespace, file.Namespace, StringComparison.Ordinal))
                    {
                        throw new ElementException(member.ContractElement,
                            $"{member.ContractElement.SpecializationType} '{member.ContractElement.Name}' shares the file {template.FileMetadata.FileName}.cs " +
                            $"with {template.ContractElement.SpecializationType} '{template.ContractElement.Name}', but the two would be generated into different namespaces " +
                            $"({Describe(member.CSharpFile.Namespace)} and {Describe(file.Namespace)}). Every type in one file must have the same namespace. " +
                            "Give every type in the file the same File Config Namespace, or none, and tick No Namespace on all of them or on none.");
                    }

                    ContractTypeBuilders.AddContract(file, template, member.ContractElement);
                }
            });
        }

        private static string Describe(string @namespace)
        {
            return string.IsNullOrEmpty(@namespace) ? "the global namespace, from No Namespace" : @namespace;
        }

        /// <summary>
        /// The CanRunTemplate rule: a template runs unless another template owns its output file.
        /// </summary>
        public static bool IsFileOwner(this IDomainContractTemplate template)
        {
            var group = GetFileGroup(template);
            return group == null || ReferenceEquals(group.Owner, template);
        }

        private sealed record FileGroup(IDomainContractTemplate Owner, IReadOnlyList<IDomainContractTemplate> Others);

        /// <summary>
        /// Recomputed on every call from a single pass over the current template instances; never
        /// cached, so it can't outlive a Software Factory run or freeze a partially registered set.
        /// Returns null when the template does not share its file.
        /// </summary>
        private static FileGroup GetFileGroup(IDomainContractTemplate template)
        {
            var metadata = template.FileMetadata;
            if (metadata == null)
            {
                return null;
            }

            var fileName = metadata.FileName;
            var filePath = metadata.GetFilePath();

            var members = new List<IDomainContractTemplate>();
            foreach (var instance in template.ExecutionContext.OutputTargets.SelectMany(x => x.TemplateInstances))
            {
                if (instance is not IDomainContractTemplate candidate ||
                    !candidate.CanRun ||  // the base switch, not CanRunTemplate(), which would recurse into this method
                    candidate.FileMetadata == null ||
                    !string.Equals(candidate.FileMetadata.FileName, fileName, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(candidate.FileMetadata.GetFilePath(), filePath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                members.Add(candidate);
            }

            if (!members.Contains(template))
            {
                members.Add(template);
            }

            if (members.Count == 1)
            {
                return null;
            }

            var ordered = members
                .OrderBy(x => x.ContractElement.Name, StringComparer.Ordinal)
                .ThenBy(x => x.ContractElement.Id, StringComparer.Ordinal)
                .ToList();

            var owner = ordered.FirstOrDefault(x => string.Equals(x.ContractElement.Name, fileName, StringComparison.Ordinal))
                ?? ordered.FirstOrDefault(x => string.Equals(x.ContractElement.Name, fileName, StringComparison.OrdinalIgnoreCase))
                ?? ordered[0];

            return new FileGroup(owner, ordered.Where(x => !ReferenceEquals(x, owner)).ToList());
        }

        /// <summary>
        /// True when every dot-separated segment is a C# identifier (an optional leading @, then a letter
        /// or underscore, then letters, digits or underscores) and the builder's own namespace
        /// normalisation (ToCSharpNamespace, applied by CSharpFile and the template's Namespace) leaves
        /// it unchanged apart from @-escaping keywords. Anything it would rewrite is rejected, so the
        /// emitted namespace is always exactly the configured one.
        /// </summary>
        private static bool IsVerbatimNamespace(string value)
        {
            foreach (var segment in value.Split('.'))
            {
                var identifier = segment.StartsWith("@", StringComparison.Ordinal) ? segment.Substring(1) : segment;
                if (identifier.Length == 0 ||
                    !(char.IsLetter(identifier[0]) || identifier[0] == '_') ||
                    !identifier.All(c => char.IsLetterOrDigit(c) || c == '_'))
                {
                    return false;
                }
            }

            return string.Equals(WithoutAtPrefixes(value.ToCSharpNamespace()), WithoutAtPrefixes(value), StringComparison.Ordinal);
        }

        private static string WithoutAtPrefixes(string value)
        {
            return string.Join('.', value.Split('.').Select(x => x.TrimStart('@')));
        }
    }
}
