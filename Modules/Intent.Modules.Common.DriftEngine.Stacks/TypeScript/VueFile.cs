using System.Text;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

/// <summary>
/// A Vue single-file component's scripts: its <c>&lt;script&gt;</c> and <c>&lt;script setup&gt;</c> blocks, as one script
/// whose positions are the file's, with everything else blanked (line breaks kept). Its <c>&lt;template&gt;</c> is markup
/// (<see cref="VueTemplate"/>).
/// </summary>
internal static class VueFile
{
    private static readonly Regex Comment = new(@"<!--.*?-->", RegexOptions.Singleline | RegexOptions.CultureInvariant);

    private static readonly Regex Script = new(@"<script\b(?<attributes>[^>]*)>(?<code>.*?)</script\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex Lang = new(@"\blang\s*=\s*[""']?(?<lang>\w+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex Setup = new(@"(?:^|\s)setup(?:\s|=|$)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <returns>The script, how to parse it (<c>ts</c>, <c>tsx</c>, <c>js</c> or <c>jsx</c>), and where <c>&lt;script setup&gt;</c>'s code is, if it has one.</returns>
    public static (string Text, string Kind, (int Start, int End)? Setup) Scripts(string file)
    {
        // A block inside a comment isn't one: blank the comments, keeping positions, before looking.
        var visible = Comment.Replace(file, x => new string(' ', x.Length));
        var text = new StringBuilder(Blank(file));
        string? lang = null;
        (int, int)? setup = null;
        foreach (Match block in Script.Matches(visible))
        {
            var code = block.Groups["code"];
            text.Remove(code.Index, code.Length).Insert(code.Index, file.Substring(code.Index, code.Length));
            lang ??= Lang.Match(block.Groups["attributes"].Value) is { Success: true } x ? x.Groups["lang"].Value.ToLowerInvariant() : null;
            if (Setup.IsMatch(block.Groups["attributes"].Value))
            {
                setup = (code.Index, code.Index + code.Length);
            }
        }

        return (text.ToString(), lang is "ts" or "tsx" or "jsx" ? lang : "js", setup);
    }

    private static string Blank(string text) => new(text.Select(x => x is '\n' or '\r' ? x : ' ').ToArray());
}
