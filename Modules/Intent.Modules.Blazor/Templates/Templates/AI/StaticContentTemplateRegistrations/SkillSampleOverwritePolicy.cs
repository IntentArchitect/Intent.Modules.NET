using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Intent.Modules.Common.Templates.StaticContent;

namespace Intent.Modules.Blazor.Templates.Templates.StaticContentTemplateRegistrations
{
    /// <summary>
    /// Decides whether a skill's bundled sample files should still be overwritten by the Software
    /// Factory, based on whether the skill's own SKILL.md has diverged from its generated content hash.
    /// </summary>
    public static class SkillSampleOverwritePolicy
    {
        // Mirrors Intent.Modules.Common.Templates.AIStaticContent.MarkdownContentHash's read-path
        // (front-matter split + hash compare), which is internal to that package and so cannot be
        // called directly from here.
        private static readonly Regex FrontMatterRegex = new(
            @"\A---(?:\r\n|\n)(.*?)(?:\r\n|\n)---(?:\r\n|\n|$)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex ContentHashFieldRegex = new(
            @"(?mi)^[ \t]*contentHash[ \t]*:[ \t]*(.+?)[ \t]*$",
            RegexOptions.Compiled);

        private static readonly Regex ContentHashLineRegex = new(
            @"(?mi)^[ \t]*contentHash[ \t]*:.*(?:\r\n|\n|$)",
            RegexOptions.Compiled);

        private static readonly Regex BlankLinesRegex = new(
            @"\n{3,}",
            RegexOptions.Compiled);

        public static bool ShouldDisableOverwrite(StaticContentTemplate template)
        {
            if (!template.TryGetExistingFilePath(out var samplePath))
            {
                return false;
            }

            var skillPath = Path.Combine(Path.GetDirectoryName(samplePath) ?? string.Empty, "SKILL.md");
            return File.Exists(skillPath) && HasSkillDivergedFromHash(skillPath);
        }

        private static bool HasSkillDivergedFromHash(string skillPath)
        {
            var markdown = File.ReadAllText(skillPath);

            if (!TrySplitFrontMatter(markdown, out var frontMatter, out var body))
            {
                return false;
            }

            var hashMatch = ContentHashFieldRegex.Match(frontMatter);
            if (!hashMatch.Success)
            {
                return false;
            }

            var existingHash = hashMatch.Groups[1].Value.Trim();
            var newline = DetectNewline(markdown);
            var frontMatterWithoutHash = RemoveContentHashField(frontMatter, newline);
            var documentWithoutHash = BuildDocument(frontMatterWithoutHash, body, newline);
            var computedHash = ComputeSha256(documentWithoutHash);

            return !string.Equals(existingHash, computedHash, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TrySplitFrontMatter(string markdown, out string frontMatter, out string body)
        {
            frontMatter = string.Empty;
            body = markdown;

            if (string.IsNullOrEmpty(markdown))
            {
                return false;
            }

            var match = FrontMatterRegex.Match(markdown);
            if (!match.Success)
            {
                return false;
            }

            frontMatter = match.Groups[1].Value;
            body = markdown[match.Length..];
            return true;
        }

        private static string RemoveContentHashField(string frontMatter, string newline)
        {
            if (string.IsNullOrEmpty(frontMatter))
            {
                return frontMatter;
            }

            var result = ContentHashLineRegex.Replace(frontMatter, string.Empty);
            result = NormalizeBlankLines(result, newline);
            return result.TrimEnd('\r', '\n', ' ', '\t');
        }

        private static string BuildDocument(string frontMatter, string body, string newline)
        {
            var sb = new StringBuilder();
            sb.Append("---").Append(newline);

            if (!string.IsNullOrEmpty(frontMatter))
            {
                sb.Append(frontMatter.TrimEnd('\r', '\n')).Append(newline);
            }

            sb.Append("---");

            if (!string.IsNullOrEmpty(body))
            {
                sb.Append(newline).Append(body);
            }

            return sb.ToString();
        }

        private static string ComputeSha256(string content)
        {
            var normalized = NormalizeLineEndings(content);
            var bytes = Encoding.UTF8.GetBytes(normalized);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }

        private static string DetectNewline(string content) => content.Contains("\r\n") ? "\r\n" : "\n";

        private static string NormalizeLineEndings(string content) => content.Replace("\r\n", "\n").Replace("\r", "\n");

        private static string NormalizeBlankLines(string text, string newline)
        {
            var normalized = NormalizeLineEndings(text);
            normalized = BlankLinesRegex.Replace(normalized, "\n\n");
            return newline == "\r\n" ? normalized.Replace("\n", "\r\n") : normalized;
        }
    }
}
