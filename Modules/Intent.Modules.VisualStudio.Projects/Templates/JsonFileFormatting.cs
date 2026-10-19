#nullable enable
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates;

/// <summary>
/// Reads and writes JSON files that may already exist on disk (<c>launchSettings.json</c>,
/// <c>appsettings.json</c>) so that values and formatting survive a rewrite.
/// </summary>
internal static class JsonFileFormatting
{
    public static JObject Parse(string content)
    {
        using var reader = new JsonTextReader(new StringReader(content))
        {
            // Keep date- and number-like values exactly as they were written
            DateParseHandling = DateParseHandling.None,
            FloatParseHandling = FloatParseHandling.Decimal
        };

        return JObject.Load(reader);
    }

    /// <summary>
    /// Serializes <paramref name="json"/> using the indentation, newline style and leading/trailing
    /// whitespace of <paramref name="originalContent"/>.
    /// </summary>
    public static string Serialize(JObject json, string originalContent)
    {
        var leadingWhitespace = originalContent[..(originalContent.Length - originalContent.TrimStart().Length)];
        var trailingWhitespace = originalContent[originalContent.TrimEnd().Length..];
        var (indentChar, indentation) = DetectIndentation(originalContent);

        return leadingWhitespace + Serialize(json, indentChar, indentation, DetectNewLine(originalContent)) + trailingWhitespace;
    }

    public static string Serialize(JObject json, char indentChar, int indentation, string newLine)
    {
        using var stringWriter = new StringWriter(CultureInfo.InvariantCulture) { NewLine = newLine };
        using (var jsonWriter = new JsonTextWriter(stringWriter))
        {
            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.IndentChar = indentChar;
            jsonWriter.Indentation = indentation;

            json.WriteTo(jsonWriter);
        }

        return stringWriter.ToString();
    }

    public static string DetectNewLine(string content)
    {
        var index = content.IndexOf('\n');
        if (index < 0)
        {
            return Environment.NewLine;
        }

        return index > 0 && content[index - 1] == '\r' ? "\r\n" : "\n";
    }

    public static (char IndentChar, int Indentation) DetectIndentation(string content)
    {
        // The first indented line is one level deep, so its leading whitespace is one indent
        foreach (var line in content.Split('\n').Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var width = line.Length - line.TrimStart(' ', '\t').Length;
            if (width > 0)
            {
                return (line[0], width);
            }
        }

        return (' ', 2);
    }
}
