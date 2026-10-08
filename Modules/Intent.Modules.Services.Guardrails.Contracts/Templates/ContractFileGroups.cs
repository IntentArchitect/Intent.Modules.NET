using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using CSharpKeywords = Intent.Modules.Common.Templates.CSharp;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Implemented by the Services contract templates so that templates writing to the same
    /// output file can find each other. Declared on a hand-written partial of each template, because
    /// the base list of the designer-generated partial is Signature-Fully managed.
    /// </summary>
    public interface IServicesContractTemplate : ICSharpFileBuilderTemplate
    {
        /// <summary>The designer element this template generates a type for.</summary>
        IElement ContractElement { get; }

        /// <summary>True when the element's File Config has No Namespace ticked: the type is emitted in the global namespace.</summary>
        bool HasNoNamespace { get; }
    }

    /// <summary>
    /// File Config support: an element's file name can differ from its type name, and several
    /// elements can share one file. Templates whose output paths are equal (OrdinalIgnoreCase, the
    /// same comparison the Software Factory uses to detect duplicate output paths) form a group.
    /// One owner writes the file containing every member's type; the others do not run but stay
    /// discoverable for type resolution.
    /// </summary>
    public static class ContractFileGroups
    {
        private static readonly char[] PathSeparators = ['/', '\\'];

        // Invalid on Windows; listed explicitly so validation does not depend on the host OS.
        private static readonly char[] InvalidFileNameChars = [':', '*', '?', '"', '<', '>', '|'];

        /// <summary>
        /// Applies a File Config Filename, if any. A trailing ".cs" is optional. The file always stays
        /// in the element's own folder. Empty or absent means the default (the element name).
        /// Must be called in the template constructor, before the file config is read.
        /// </summary>
        public static CSharpFile WithContractFileName(this CSharpFile file, IElement element, string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return file;
            }

            var name = filename.Trim();
            if (name.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - ".cs".Length);
            }

            if (name.IndexOfAny(PathSeparators) >= 0)
            {
                throw new ElementException(element,
                    $"{element.SpecializationType} '{element.Name}' has a File Config Filename of '{filename}', which contains a folder path. " +
                    "Filename must be a file name only, because the file is always placed in the element's own folder. " +
                    "Move the element into the designer folder that matches the file's location and set Filename to just the file name.");
            }

            if (name.Length == 0 ||
                name == "." ||
                name == ".." ||
                name.EndsWith(".", StringComparison.Ordinal) ||
                name.IndexOfAny(InvalidFileNameChars) >= 0 ||
                name.Any(char.IsControl))
            {
                throw new ElementException(element,
                    $"{element.SpecializationType} '{element.Name}' has a File Config Filename of '{filename}', which is not a valid file name. " +
                    "Set Filename to the name of the existing file, for example OrderDto or OrderDto.cs, without characters such as : * ? \" < > |, " +
                    "or clear it to use the element name.");
            }

            return file.WithFileName(name);
        }

        /// <summary>
        /// The namespace for a contract's file: empty when File Config No Namespace is ticked (the
        /// global namespace), a File Config Namespace when one is set, otherwise the namespace derived
        /// from the element's folders. The override is trimmed but its casing is never changed. Type
        /// resolution in other templates reads the template's config Namespace, so references and usings
        /// elsewhere pick the override up automatically. For No Namespace the template must also override
        /// <c>Namespace</c> (the config omits an empty namespace, which would otherwise fall back to the
        /// project name) and unwrap its output (see <see cref="GlobalNamespaceOutput"/>).
        /// </summary>
        public static string GetContractNamespace<TModel>(this CSharpTemplateBase<TModel> template, IElement element, string namespaceOverride, bool noNamespace)
            where TModel : IHasFolder
        {
            return ResolveContractNamespace(() => template.GetNamespace(), element, namespaceOverride, noNamespace);
        }

        /// <summary>
        /// The same rules for a template whose model is a raw <see cref="IElement"/> (Domain Event Handlers).
        /// The built-in <c>GetNamespace()</c> only follows folders for <see cref="IHasFolder"/> models and would
        /// return just the project namespace for an <see cref="IElement"/>, so the folder-derived default is
        /// computed here with the same rule (folders that are namespace providers).
        /// </summary>
        public static string GetContractNamespace(this CSharpTemplateBase<IElement> template, IElement element, string namespaceOverride, bool noNamespace)
        {
            return ResolveContractNamespace(
                () => string.Join(".", new[] { template.OutputTarget.GetNamespace() }
                    .Concat(GetParentFolders(element)
                        .Where(x => !string.IsNullOrWhiteSpace(x.Name) && (!x.HasFolderOptions() || x.GetFolderOptions().NamespaceProvider()))
                        .Select(x => x.Name))),
                element, namespaceOverride, noNamespace);
        }

        /// <summary>
        /// Folder path for a raw <see cref="IElement"/> model, matching <c>GetFolderPath()</c> for
        /// <see cref="IHasFolder"/> models (which returns an empty path for an <see cref="IElement"/>).
        /// </summary>
        public static string GetElementFolderPath(this IElement element)
        {
            return string.Join("/", GetParentFolders(element).Select(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static IList<FolderModel> GetParentFolders(IElement element)
        {
            var parent = element.ParentElement;
            return new ElementFolder(parent != null && parent.IsFolderModel() ? parent.AsFolderModel() : null).GetParentFolders();
        }

        private sealed class ElementFolder : IHasFolder
        {
            public ElementFolder(FolderModel folder)
            {
                Folder = folder;
            }

            public FolderModel Folder { get; }
        }

        private static string ResolveContractNamespace(Func<string> derivedNamespace, IElement element, string namespaceOverride, bool noNamespace)
        {
            if (noNamespace)
            {
                if (!string.IsNullOrWhiteSpace(namespaceOverride))
                {
                    throw new ElementException(element,
                        $"{element.SpecializationType} '{element.Name}' has File Config No Namespace ticked and also a Namespace of '{namespaceOverride}'. " +
                        "Choose one: clear the Namespace to place the type in the global namespace, or untick No Namespace to use the Namespace.");
                }

                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(namespaceOverride))
            {
                return derivedNamespace();
            }

            var value = namespaceOverride.Trim();
            if (!IsValidNamespace(value))
            {
                throw new ElementException(element,
                    $"{element.SpecializationType} '{element.Name}' has a File Config Namespace of '{namespaceOverride}', which is not a valid C# namespace. " +
                    "Use dot-separated names that each start with a letter or underscore and contain only letters, digits and underscores, for example Acme.Orders.Dtos. " +
                    "Prefix a part that is a C# keyword with @, for example Acme.@event, or clear the Namespace to use the one derived from the element's folders.");
            }

            return value;
        }

        /// <summary>
        /// Each dot-separated segment must be an identifier that <c>ToCSharpNamespace()</c> (applied by
        /// <see cref="CSharpFile"/>) leaves unchanged, so the generated namespace is exactly what was
        /// modelled. A segment may be '@'-prefixed; a keyword segment must be.
        /// </summary>
        private static bool IsValidNamespace(string value)
        {
            foreach (var rawSegment in value.Split('.'))
            {
                var isVerbatim = rawSegment.StartsWith("@", StringComparison.Ordinal);
                var segment = isVerbatim ? rawSegment.Substring(1) : rawSegment;

                if (segment.Length == 0 || !IsIdentifierStart(segment[0]) || !segment.Skip(1).All(IsIdentifierPart))
                {
                    return false;
                }

                if (!isVerbatim && CSharpKeywords.ReservedWords.Contains(segment))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsIdentifierStart(char c)
        {
            return c == '_' || char.IsLetter(c) || char.GetUnicodeCategory(c) == UnicodeCategory.LetterNumber;
        }

        private static bool IsIdentifierPart(char c)
        {
            return IsIdentifierStart(c) ||
                   char.GetUnicodeCategory(c) is UnicodeCategory.DecimalDigitNumber or UnicodeCategory.Format;
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
        public static void RegisterContractFileGroup(this IServicesContractTemplate template)
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
                    if (member.HasNoNamespace != template.HasNoNamespace)
                    {
                        var (noNamespaceType, namespacedType) = member.HasNoNamespace ? (member, template) : (template, member);
                        throw new ElementException(member.ContractElement,
                            $"{member.ContractElement.SpecializationType} '{member.ContractElement.Name}' shares the file {template.FileMetadata.FileName}.cs " +
                            $"with {template.ContractElement.SpecializationType} '{template.ContractElement.Name}', but '{noNamespaceType.ContractElement.Name}' has File Config No Namespace ticked " +
                            $"and '{namespacedType.ContractElement.Name}' does not. All types in one file must be in the same namespace. " +
                            "Tick No Namespace on every type in that file, or on none.");
                    }

                    if (!string.Equals(member.CSharpFile.Namespace, file.Namespace, StringComparison.Ordinal))
                    {
                        throw new ElementException(member.ContractElement,
                            $"{member.ContractElement.SpecializationType} '{member.ContractElement.Name}' shares the file {template.FileMetadata.FileName}.cs " +
                            $"with {template.ContractElement.SpecializationType} '{template.ContractElement.Name}', but its namespace is {member.CSharpFile.Namespace} " +
                            $"while '{template.ContractElement.Name}' is in {file.Namespace}. All types in one file must be in the same namespace. " +
                            "Give every type in that file the same File Config Namespace, or none.");
                    }

                    ContractTypeBuilders.AddContract(file, template, member.ContractElement);
                }
            });
        }

        /// <summary>
        /// The CanRunTemplate rule: a template runs unless another template owns its output file.
        /// </summary>
        public static bool IsFileOwner(this IServicesContractTemplate template)
        {
            var group = GetFileGroup(template);
            return group == null || ReferenceEquals(group.Owner, template);
        }

        private sealed record FileGroup(IServicesContractTemplate Owner, IReadOnlyList<IServicesContractTemplate> Others);

        /// <summary>
        /// Recomputed on every call from a single pass over the current template instances; never
        /// cached, so it can't outlive a Software Factory run or freeze a partially registered set.
        /// Returns null when the template does not share its file.
        /// </summary>
        private static FileGroup GetFileGroup(IServicesContractTemplate template)
        {
            var metadata = template.FileMetadata;
            if (metadata == null)
            {
                return null;
            }

            var fileName = metadata.FileName;
            var filePath = metadata.GetFilePath();

            var members = new List<IServicesContractTemplate>();
            foreach (var instance in template.ExecutionContext.OutputTargets.SelectMany(x => x.TemplateInstances))
            {
                if (instance is not IServicesContractTemplate candidate ||
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
    }
}
