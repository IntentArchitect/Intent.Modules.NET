using System.Text;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>
/// A path glob as File Classification reads one: <c>**</c> crosses folders, <c>*</c> and <c>?</c> stay within one,
/// and matching ignores case.
/// </summary>
internal sealed class Glob
{
    private readonly Regex _regex;

    public Glob(string pattern)
    {
        Pattern = pattern;
        var regex = new StringBuilder("^");
        for (var i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];
            if (c == '*' && i + 1 < pattern.Length && pattern[i + 1] == '*')
            {
                var slashFollows = i + 2 < pattern.Length && pattern[i + 2] == '/';
                regex.Append(slashFollows ? "(?:.*/)?" : ".*");
                i += slashFollows ? 2 : 1;
            }
            else if (c == '*')
            {
                regex.Append("[^/]*");
            }
            else if (c == '?')
            {
                regex.Append("[^/]");
            }
            else
            {
                regex.Append(Regex.Escape(c.ToString()));
            }
        }

        _regex = new Regex(regex.Append('$').ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    public string Pattern { get; }

    public bool IsMatch(string path) => _regex.IsMatch(path.Replace('\\', '/'));
}
