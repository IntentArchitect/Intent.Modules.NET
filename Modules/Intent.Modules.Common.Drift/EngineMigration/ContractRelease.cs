using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// Hands a contract file back to its developer: takes out what told Intent's weaver how to merge into it, the
    /// <c>[assembly: DefaultIntentManaged]</c> and <c>[assembly: IntentTemplate]</c> attributes, every
    /// <c>Intent.RoslynWeaver.Attributes</c> attribute on its types and members, and the <c>using</c> for them. Everything else
    /// stays as written: a line is removed whole only when it held nothing else, and a blank line only when a removal left two
    /// together or one at the start of the file.
    /// </summary>
    internal static class ContractRelease
    {
        private const string AttributesNamespace = "Intent.RoslynWeaver.Attributes";

        private static readonly HashSet<string> AttributeNames = new(StringComparer.Ordinal)
        {
            "DefaultIntentManaged",
            "IntentTemplate",
            "IntentTagMode",
            "IntentTagModeExplicit",
            "IntentTagModeImplicit",
            "IntentManaged",
            "IntentInitialGen",
            "IntentFully",
            "IntentFullyAttributes",
            "IntentFullyBody",
            "IntentFullyComments",
            "IntentFullySignature",
            "IntentIgnore",
            "IntentIgnoreAttributes",
            "IntentIgnoreBody",
            "IntentIgnoreComments",
            "IntentIgnoreSignature",
            "IntentMerge",
            "IntentMergeAttributes",
            "IntentMergeBody",
            "IntentMergeComments",
            "IntentMergeSignature",
        };

        private static readonly Regex TemplateAttribute = new(@"\[\s*assembly\s*:\s*(?:Intent\.RoslynWeaver\.Attributes\.)?IntentTemplate(?:Attribute)?\(\s*""([^""]+)""", RegexOptions.CultureInvariant);

        private static readonly HashSet<string> SkippedFolders = new(StringComparer.OrdinalIgnoreCase) { "bin", "obj", "node_modules", ".git", ".vs", ".intent", ".cache", "Intent.Metadata" };

        /// <summary>
        /// The C# files under the output folder whose template attribute names a contract template, relative to it with
        /// <c>/</c> separators. A machine's log doesn't always list every contract file (one that was reset, or a fresh clone),
        /// and the file's own header says which template wrote it.
        /// </summary>
        /// <param name="otherOutputFolders">Other applications' output folders, which aren't searched: one may still be on v1.</param>
        public static IReadOnlyList<string> FindContractFiles(string outputFolder, IReadOnlyCollection<string> otherOutputFolders)
        {
            var others = new HashSet<string>(otherOutputFolders.Select(x => Path.GetFullPath(x).TrimEnd(Path.DirectorySeparatorChar)), StringComparer.OrdinalIgnoreCase);
            var found = new List<string>();
            var folders = new Stack<DirectoryInfo>(new[] { new DirectoryInfo(outputFolder) });
            while (folders.Count > 0)
            {
                var folder = folders.Pop();
                foreach (var child in folder.EnumerateDirectories().Where(x => !SkippedFolders.Contains(x.Name) && !others.Contains(x.FullName.TrimEnd(Path.DirectorySeparatorChar))))
                {
                    folders.Push(child);
                }

                foreach (var file in folder.EnumerateFiles("*.cs"))
                {
                    var match = TemplateAttribute.Match(File.ReadAllText(file.FullName));
                    if (match.Success && V1Footprint.ContractTemplateIds.Contains(match.Groups[1].Value))
                    {
                        found.Add(Path.GetRelativePath(outputFolder, file.FullName).Replace('\\', '/'));
                    }
                }
            }

            return found.OrderBy(x => x, StringComparer.Ordinal).ToList();
        }

        /// <summary>Releases a file on disk; false when it isn't there or has nothing of Intent's, in which case it isn't written.</summary>
        public static bool ReleaseFile(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            string text;
            Encoding encoding;
            using (var reader = new StreamReader(path, new UTF8Encoding(false), detectEncodingFromByteOrderMarks: true))
            {
                text = reader.ReadToEnd();
                encoding = reader.CurrentEncoding;
            }

            var released = Strip(text);
            if (ReferenceEquals(released, text))
            {
                return false;
            }

            File.WriteAllText(path, released, encoding);
            return true;
        }

        /// <summary>The text without Intent's attributes and using; the same instance when it has none.</summary>
        public static string Strip(string text)
        {
            var source = SourceText.From(text);
            var (removedLines, changes) = Find(source, imported: null);
            if (changes.Count > 0)
            {
                // Edits inside a line first, then the lines left holding nothing but Intent's, found again in the edited text.
                source = source.WithChanges(changes);
                (removedLines, _) = Find(source, imported: true);
            }

            return removedLines.Count == 0 && changes.Count == 0 ? text : Rebuild(source, removedLines);
        }

        /// <param name="imported">Whether the file imports the attributes' namespace; null to find out from its usings.</param>
        private static (HashSet<int> RemovedLines, List<TextChange> Changes) Find(SourceText source, bool? imported)
        {
            var root = CSharpSyntaxTree.ParseText(source).GetCompilationUnitRoot();
            var usings = root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(IsAttributesUsing).ToList();
            var isImported = imported ?? usings.Count > 0;

            var removedLines = new HashSet<int>();
            var changes = new List<TextChange>();
            foreach (var directive in usings)
            {
                Remove(source, directive.Span, removedLines, changes);
            }

            foreach (var list in root.DescendantNodes().OfType<AttributeListSyntax>())
            {
                var intent = list.Attributes.Where(x => IsIntentAttribute(x.Name, isImported)).ToList();
                if (intent.Count == 0)
                {
                    continue;
                }

                if (intent.Count == list.Attributes.Count)
                {
                    Remove(source, list.Span, removedLines, changes);
                }
                else
                {
                    var target = list.Target == null ? string.Empty : list.Target.ToString() + " ";
                    var kept = string.Join(", ", list.Attributes.Except(intent).Select(x => x.ToString()));
                    changes.Add(new TextChange(list.Span, $"[{target}{kept}]"));
                }
            }

            return (removedLines, changes);
        }

        private static bool IsAttributesUsing(UsingDirectiveSyntax directive) =>
            directive.Alias == null && directive.StaticKeyword.IsKind(SyntaxKind.None) && directive.Name?.ToString() == AttributesNamespace;

        private static bool IsIntentAttribute(NameSyntax name, bool imported)
        {
            var text = name.ToString();
            var qualified = text.StartsWith(AttributesNamespace + ".", StringComparison.Ordinal) ||
                            text.StartsWith("global::" + AttributesNamespace + ".", StringComparison.Ordinal);
            if (!qualified && !imported)
            {
                return false;
            }

            var simple = text[(text.LastIndexOf('.') + 1)..];
            if (simple.EndsWith("Attribute", StringComparison.Ordinal) && AttributeNames.Contains(simple[..^"Attribute".Length]))
            {
                return true;
            }

            return AttributeNames.Contains(simple);
        }

        /// <summary>Removes a node's lines when it is alone on them, or else the node and the spaces after it.</summary>
        private static void Remove(SourceText source, TextSpan span, HashSet<int> removedLines, List<TextChange> changes)
        {
            var first = source.Lines.GetLineFromPosition(span.Start);
            var last = source.Lines.GetLineFromPosition(span.End);
            var before = source.ToString(TextSpan.FromBounds(first.Start, span.Start));
            var after = source.ToString(TextSpan.FromBounds(span.End, last.End));
            if (string.IsNullOrWhiteSpace(before) && string.IsNullOrWhiteSpace(after))
            {
                for (var line = first.LineNumber; line <= last.LineNumber; line++)
                {
                    removedLines.Add(line);
                }

                return;
            }

            var end = span.End;
            while (end < last.End && (source[end] == ' ' || source[end] == '\t'))
            {
                end++;
            }

            changes.Add(new TextChange(TextSpan.FromBounds(span.Start, end), string.Empty));
        }

        private static string Rebuild(SourceText source, HashSet<int> removedLines)
        {
            var result = new StringBuilder(source.Length);
            var removedSinceLastLine = false;
            var lastWasBlank = true;
            foreach (var line in source.Lines)
            {
                if (removedLines.Contains(line.LineNumber))
                {
                    removedSinceLastLine = true;
                    continue;
                }

                var content = source.ToString(line.Span);
                var blank = string.IsNullOrWhiteSpace(content);
                if (blank && removedSinceLastLine && lastWasBlank)
                {
                    continue;
                }

                result.Append(content).Append(source.ToString(TextSpan.FromBounds(line.End, line.EndIncludingLineBreak)));
                removedSinceLastLine = false;
                lastWasBlank = blank;
            }

            return result.ToString();
        }
    }
}
