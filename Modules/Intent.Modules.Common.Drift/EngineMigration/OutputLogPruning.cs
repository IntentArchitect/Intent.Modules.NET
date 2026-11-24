using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// Makes the Software Factory forget the contract templates' files, so it doesn't delete them once the templates are gone.
    /// It decides what to delete from three records, in the formats of Intent's <c>OutputLogPersistable</c>,
    /// <c>LegacyOutputLogPersistable</c> and <c>OutputConfigPersistable</c>:
    /// <list type="bullet">
    /// <item><c>.intent/output-log.xml</c>, per machine: entries by correlation id, paths relative to the output folder;</item>
    /// <item><c>&lt;application&gt;.application.output.log</c>, the committed legacy log: the same, paths relative to the application;</item>
    /// <item><c>&lt;application&gt;.application.output.config.xml</c>, committed: ignored files by path, with no correlation id.</item>
    /// </list>
    /// It also removes them from <c>&lt;application&gt;.application.managed-files.xml</c>, Intent's committed inventory of the
    /// files it generates (<c>ManagedFilesPersistable</c>), which Changes Review reads and the Software Factory rebuilds from this
    /// run's outputs. That matters on a teammate's machine, where <see cref="ReleasedContractFileTemplate"/> makes the contract
    /// files outputs of the run, listed under their contract template ids.
    /// Only contract entries go (<see cref="V1Footprint.IsContractCorrelation"/>); an entry with no correlation id is never
    /// one. A record that isn't there is fine, and the records are read and written as they are, so nothing else in them changes.
    /// </summary>
    internal static class OutputLogPruning
    {
        public static string OutputLogPath(string applicationConfigPath) =>
            Path.Combine(Path.GetDirectoryName(applicationConfigPath)!, ".intent", "output-log.xml");

        public static string LegacyLogPath(string applicationConfigPath) =>
            Path.Combine(Path.GetDirectoryName(applicationConfigPath)!, $"{Path.GetFileNameWithoutExtension(applicationConfigPath)}.output.log");

        public static string OutputConfigPath(string applicationConfigPath) =>
            Path.Combine(Path.GetDirectoryName(applicationConfigPath)!, $"{Path.GetFileNameWithoutExtension(applicationConfigPath)}.output.config.xml");

        public static string ManagedFilesPath(string applicationConfigPath) =>
            Path.Combine(Path.GetDirectoryName(applicationConfigPath)!, $"{Path.GetFileNameWithoutExtension(applicationConfigPath)}.managed-files.xml");

        /// <summary>The contract files both logs list, relative to the output folder with <c>/</c> separators.</summary>
        public static IReadOnlyList<string> ContractFiles(string applicationConfigPath, string outputFolder) =>
            ContractEntries(applicationConfigPath, outputFolder).Select(x => x.RelativePath).ToList();

        /// <summary>
        /// The contract entries both logs list, one per file: <c>.intent/output-log.xml</c>'s first, as the Software Factory reads
        /// a file's later entries only when it has no earlier one.
        /// </summary>
        public static IReadOnlyList<ContractFile> ContractEntries(string applicationConfigPath, string outputFolder)
        {
            var applicationFolder = Path.GetDirectoryName(applicationConfigPath)!;
            var files = new List<ContractFile>();
            if (Load(OutputLogPath(applicationConfigPath)) is { } log)
            {
                files.AddRange(OutputLogEntries(log.Document)
                    .Where(x => V1Footprint.IsContractCorrelation(x.CorrelationId))
                    .Select(x => new ContractFile(Normalize(x.Path), x.CorrelationId, x.ProjectId)));
            }

            if (Load(LegacyLogPath(applicationConfigPath)) is { } legacy)
            {
                files.AddRange(LegacyEntries(legacy.Document)
                    .Where(x => V1Footprint.IsContractCorrelation(x.CorrelationId))
                    .Select(x => new ContractFile(
                        Normalize(Path.GetRelativePath(outputFolder, Path.GetFullPath(Path.Combine(applicationFolder, x.Path)))),
                        x.CorrelationId,
                        x.ProjectId)));
            }

            return files.DistinctBy(x => x.RelativePath, StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <summary>Removes the contract entries from all three records; what each record lost, by file name.</summary>
        public static IReadOnlyDictionary<string, int> Prune(string applicationConfigPath, string outputFolder)
        {
            var contractFiles = new HashSet<string>(ContractFiles(applicationConfigPath, outputFolder), StringComparer.OrdinalIgnoreCase);
            return new Dictionary<string, int>
            {
                [".intent/output-log.xml"] = Rewrite(OutputLogPath(applicationConfigPath), PruneOutputLog),
                [Path.GetFileName(LegacyLogPath(applicationConfigPath))] = Rewrite(LegacyLogPath(applicationConfigPath), PruneLegacyLog),
                [Path.GetFileName(OutputConfigPath(applicationConfigPath))] = Rewrite(OutputConfigPath(applicationConfigPath), x => PruneOutputConfig(x, contractFiles)),
                [Path.GetFileName(ManagedFilesPath(applicationConfigPath))] = Rewrite(ManagedFilesPath(applicationConfigPath), PruneManagedFiles),
            };
        }

        /// <summary>Removes <c>managed-files.xml</c>'s entries for contract files, matched by their template id; how many went.</summary>
        public static int PruneManagedFiles(XDocument document) =>
            RemoveAll(document.Root?.Elements("file")
                .Where(x => (string)x.Attribute("templateId") is { } templateId && V1Footprint.ContractTemplateIds.Contains(templateId))
                ?? Enumerable.Empty<XElement>());

        /// <summary>Removes <c>.intent/output-log.xml</c>'s contract entries; how many went.</summary>
        public static int PruneOutputLog(XDocument document) =>
            RemoveAll(OutputLogEntries(document).Where(x => V1Footprint.IsContractCorrelation(x.CorrelationId)).Select(x => x.Element));

        /// <summary>Removes the legacy log's contract entries; how many went.</summary>
        public static int PruneLegacyLog(XDocument document) =>
            RemoveAll(LegacyEntries(document).Where(x => V1Footprint.IsContractCorrelation(x.CorrelationId)).Select(x => x.Element));

        /// <summary>Removes <c>output.config.xml</c>'s entries for contract files, matched by path; how many went.</summary>
        public static int PruneOutputConfig(XDocument document, IReadOnlyCollection<string> contractFiles) =>
            RemoveAll(document.Root?.Element("files")?.Elements("file")
                .Where(x => (string)x.Attribute("relativePath") is { } path && contractFiles.Contains(Normalize(path), StringComparer.OrdinalIgnoreCase))
                ?? Enumerable.Empty<XElement>());

        private static IEnumerable<Entry> OutputLogEntries(XDocument document) =>
            document.Root?.Element("files")?.Elements("file")
                .Select(x => new Entry(x, (string)x.Attribute("relativePath"), (string)x.Element("correlationId"), (string)x.Element("projectId")))
            ?? Enumerable.Empty<Entry>();

        private static IEnumerable<Entry> LegacyEntries(XDocument document) =>
            document.Root?.Element("FileLogs")?.Elements("FileLog")
                .Select(x => new Entry(x, (string)x.Element("ApplicationRelativeFilePath"), (string)x.Element("CorrelationId"), (string)x.Element("ProjectId")))
            ?? Enumerable.Empty<Entry>();

        /// <summary>Removes each element with the indentation before it, so the rest of the file keeps its layout.</summary>
        private static int RemoveAll(IEnumerable<XElement> elements)
        {
            var list = elements.ToList();
            foreach (var element in list)
            {
                if (element.PreviousNode is XText { Value: var text } whitespace && string.IsNullOrWhiteSpace(text))
                {
                    whitespace.Remove();
                }

                element.Remove();
            }

            return list.Count;
        }

        private static int Rewrite(string path, Func<XDocument, int> prune)
        {
            if (Load(path) is not { } record)
            {
                return 0;
            }

            var removed = prune(record.Document);
            if (removed > 0)
            {
                File.WriteAllText(path, Write(record.Document, record.Text), record.Encoding);
            }

            return removed;
        }

        /// <summary>
        /// The document as text laid out as the original was: its declaration and root on their own lines, and whatever followed
        /// the root kept. <see cref="XDocument"/> keeps whitespace inside the root but not around it.
        /// </summary>
        public static string Write(XDocument document, string original)
        {
            // The parser reads every line break as \n; each is written back as the original's.
            var newLine = original.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
            var trailing = original[original.TrimEnd().Length..];
            var root = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = false, NewLineHandling = NewLineHandling.Replace, NewLineChars = newLine };
            using (var writer = XmlWriter.Create(root, settings))
            {
                document.Root!.WriteTo(writer);
            }

            var declaration = document.Declaration == null ? string.Empty : document.Declaration + newLine;
            return declaration + root + trailing;
        }

        private static (XDocument Document, Encoding Encoding, string Text)? Load(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using var reader = new StreamReader(path, new UTF8Encoding(false), detectEncodingFromByteOrderMarks: true);
            var text = reader.ReadToEnd();
            return (XDocument.Parse(text, LoadOptions.PreserveWhitespace), reader.CurrentEncoding, text);
        }

        private static string Normalize(string path) => path?.Replace('\\', '/');

        private sealed record Entry(XElement Element, string Path, string CorrelationId, string ProjectId);

        /// <summary>
        /// A contract file a log lists: its path relative to the output folder with <c>/</c> separators, its correlation id, and
        /// the output target it was generated into (none when the log didn't record one).
        /// </summary>
        public sealed record ContractFile(string RelativePath, string CorrelationId, string ProjectId);
    }
}
