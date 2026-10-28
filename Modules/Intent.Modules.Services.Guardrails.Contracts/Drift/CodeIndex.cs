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

namespace Intent.Modules.Services.Guardrails.Contracts.Drift
{
    /// <summary>A file of the codebase: its text, and its syntax when it is C#.</summary>
    internal sealed class CodeFile
    {
        private CodeText _canonical;
        private int[] _lineStarts;

        public string RelativePath { get; init; }
        public string Text { get; init; }

        /// <summary>The parsed file, or <see langword="null"/> when it isn't C#.</summary>
        public CompilationUnitSyntax Syntax { get; init; }

        /// <summary>The whole file as the recipes see it (see <see cref="CodeText"/>).</summary>
        public CodeText Canonical => _canonical ??= Syntax != null ? CodeText.Of(Syntax, this) : CodeText.OfPlainText(this);

        /// <summary>The 1-based line of a position in the file.</summary>
        public int LineOf(int position)
        {
            if (Syntax != null)
            {
                return Syntax.SyntaxTree.GetLineSpan(new TextSpan(Math.Max(0, position), 0)).StartLinePosition.Line + 1;
            }

            _lineStarts ??= LineStarts(Text);
            var index = Array.BinarySearch(_lineStarts, position);
            return (index >= 0 ? index : ~index - 1) + 1;
        }

        private static int[] LineStarts(string text)
        {
            var starts = new List<int> { 0 };
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    starts.Add(i + 1);
                }
            }

            return starts.ToArray();
        }
    }

    /// <summary>
    /// Reads and parses the codebase's files, each once. Parsing is syntax only: nothing is built. Paths are
    /// relative to the codebase root (the application's output location), with forward slashes.
    /// </summary>
    internal sealed class CodeFiles
    {
        private static readonly CSharpParseOptions ParseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        private static readonly HashSet<string> SkippedFolders = new(StringComparer.OrdinalIgnoreCase) { "bin", "obj", "node_modules" };

        private readonly Dictionary<string, CodeFile> _files = new(StringComparer.OrdinalIgnoreCase);
        private readonly IReadOnlyDictionary<string, string> _inMemory;
        private readonly CodeFiles _base;
        private readonly IReadOnlyDictionary<string, string> _changed;
        private IReadOnlyList<string> _all;

        public CodeFiles(string root)
        {
            Root = Path.GetFullPath(root);
        }

        /// <summary>Files held in memory, keyed by relative path, instead of a folder on disk.</summary>
        public CodeFiles(IReadOnlyDictionary<string, string> files)
        {
            Root = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "in-memory-codebase"));
            _inMemory = files.ToDictionary(x => Normalize(x.Key), x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        private CodeFiles(CodeFiles @base, IReadOnlyDictionary<string, string> changed)
        {
            Root = @base.Root;
            _inMemory = @base._inMemory;
            _base = @base;
            _changed = changed;
        }

        public string Root { get; }

        /// <summary>
        /// The same codebase with some files' text changed, or new files added, in memory: the recipe probe's, and the
        /// code as it will be once a run's pending changes are applied. Every other file is this codebase's own, parsed once.
        /// </summary>
        public CodeFiles WithFiles(IReadOnlyDictionary<string, string> files) =>
            new(this, files.ToDictionary(x => ToRelative(x.Key), x => x.Value, StringComparer.OrdinalIgnoreCase));

        public string ToRelative(string path)
        {
            if (_inMemory != null && !Path.IsPathRooted(path))
            {
                return Normalize(path);
            }

            var fullPath = Path.IsPathRooted(path) ? Path.GetFullPath(path) : Path.GetFullPath(Path.Combine(Root, path));
            return Normalize(Path.GetRelativePath(Root, fullPath));
        }

        /// <summary>The file, or <see langword="null"/> when it doesn't exist.</summary>
        public CodeFile Get(string path)
        {
            var relativePath = ToRelative(path);
            if (_files.TryGetValue(relativePath, out var cached))
            {
                return cached;
            }

            string text = null;
            if (_base != null && !_changed.TryGetValue(relativePath, out text))
            {
                return _base.Get(relativePath);
            }

            if (_base == null && _inMemory != null)
            {
                _inMemory.TryGetValue(relativePath, out text);
            }
            else if (_base == null)
            {
                var fullPath = Path.GetFullPath(Path.Combine(Root, relativePath));
                if (File.Exists(fullPath))
                {
                    text = File.ReadAllText(fullPath);
                }
            }

            var file = text == null
                ? null
                : new CodeFile
                {
                    RelativePath = relativePath,
                    Text = text,
                    Syntax = IsCSharp(relativePath)
                    ? (CompilationUnitSyntax)CSharpSyntaxTree.ParseText(text, ParseOptions, relativePath).GetRoot()
                    : null
                };
            _files[relativePath] = file;
            return file;
        }

        public static bool IsCSharp(string path) => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);

        /// <summary>Every file of the codebase, skipping build output and hidden folders, in ordinal order.</summary>
        public IReadOnlyList<string> AllFiles()
        {
            if (_all != null)
            {
                return _all;
            }

            if (_base != null)
            {
                var added = _changed.Keys.Where(x => _base.Get(x) == null).ToList();
                return _all = added.Count == 0 ? _base.AllFiles() : _base.AllFiles().Concat(added).OrderBy(x => x, StringComparer.Ordinal).ToList();
            }

            if (_inMemory != null)
            {
                return _all = _inMemory.Keys.OrderBy(x => x, StringComparer.Ordinal).ToList();
            }

            var result = new List<string>();
            var pending = new Stack<string>();
            if (Directory.Exists(Root))
            {
                pending.Push(Root);
            }

            while (pending.Count > 0)
            {
                var folder = pending.Pop();
                result.AddRange(Directory.EnumerateFiles(folder).Select(ToRelative));
                foreach (var child in Directory.EnumerateDirectories(folder))
                {
                    var name = Path.GetFileName(child);
                    if (!name.StartsWith(".") && !SkippedFolders.Contains(name))
                    {
                        pending.Push(child);
                    }
                }
            }

            return _all = result.OrderBy(x => x, StringComparer.Ordinal).ToList();
        }

        /// <summary>The files matching any of the globs, in ordinal order.</summary>
        public IReadOnlyList<string> Matching(IReadOnlyCollection<Glob> globs)
        {
            return AllFiles().Where(path => globs.Any(glob => glob.IsMatch(path))).ToList();
        }

        private static string Normalize(string path) => path.Replace('\\', '/').TrimStart('/');
    }

    /// <summary>
    /// A path pattern relative to the codebase root: <c>*</c> matches within one folder, <c>**</c> across folders
    /// and <c>?</c> one character. Matching ignores case.
    /// </summary>
    internal sealed class Glob
    {
        private readonly Regex _regex;

        public Glob(string pattern)
        {
            Pattern = pattern.Trim().Replace('\\', '/').TrimStart('/');
            if (Pattern.StartsWith("./", StringComparison.Ordinal))
            {
                Pattern = Pattern[2..];
            }

            var regex = new StringBuilder("^");
            for (var i = 0; i < Pattern.Length; i++)
            {
                var c = Pattern[i];
                if (c == '*' && i + 1 < Pattern.Length && Pattern[i + 1] == '*')
                {
                    var slash = i + 2 < Pattern.Length && Pattern[i + 2] == '/';
                    regex.Append(slash ? "(?:.*/)?" : ".*");
                    i += slash ? 2 : 1;
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

        public bool IsMatch(string relativePath) => _regex.IsMatch(relativePath);

        /// <summary>
        /// Whether an exception pattern names files rather than a type or member: it has a <c>/</c>, <c>*</c> or
        /// <c>?</c>, or ends in <c>.cs</c>.
        /// </summary>
        public static bool LooksLikeGlob(string pattern)
        {
            return pattern.IndexOfAny(new[] { '/', '\\', '*', '?' }) >= 0 || pattern.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Code as the drift recipes read it: without comments, disabled code or layout. C# is its tokens joined with no
    /// whitespace, except for one space between two tokens that would otherwise run together, so
    /// <c>public async Task&lt;int&gt; GetAsync(int id)</c> reads <c>public async Task&lt;int&gt;GetAsync(int id)</c>.
    /// Any other file is its text with each run of whitespace removed, or reduced to one space between two words.
    /// String contents are kept as written.
    /// </summary>
    internal sealed class CodeText
    {
        private readonly int[] _starts;
        private readonly int[] _positions;
        private readonly SyntaxToken[] _tokens;

        private CodeText(CodeFile file, string text, int[] starts, int[] positions, SyntaxToken[] tokens)
        {
            File = file;
            Text = text;
            _starts = starts;
            _positions = positions;
            _tokens = tokens;
        }

        public CodeFile File { get; }
        public string Text { get; }

        public static CodeText Of(SyntaxNode node, CodeFile file)
        {
            var tokens = node.DescendantTokens()
                .Where(x => !x.IsKind(SyntaxKind.EndOfFileToken) && x.Span.Length > 0)
                .ToArray();
            var text = new StringBuilder();
            var starts = new int[tokens.Length];
            for (var i = 0; i < tokens.Length; i++)
            {
                if (i > 0 && NeedsSpace(tokens[i - 1].Text, tokens[i].Text))
                {
                    text.Append(' ');
                }

                starts[i] = text.Length;
                text.Append(tokens[i].Text);
            }

            return new CodeText(file, text.ToString(), starts, tokens.Select(x => x.SpanStart).ToArray(), tokens);
        }

        public static CodeText OfPlainText(CodeFile file)
        {
            var source = file.Text;
            var text = new StringBuilder();
            var positions = new List<int>();
            for (var i = 0; i < source.Length; i++)
            {
                if (!char.IsWhiteSpace(source[i]))
                {
                    text.Append(source[i]);
                    positions.Add(i);
                    continue;
                }

                var end = i;
                while (end + 1 < source.Length && char.IsWhiteSpace(source[end + 1]))
                {
                    end++;
                }

                if (text.Length > 0 && end + 1 < source.Length && IsWordChar(text[^1]) && IsWordChar(source[end + 1]))
                {
                    text.Append(' ');
                    positions.Add(i);
                }

                i = end;
            }

            var starts = Enumerable.Range(0, text.Length).ToArray();
            return new CodeText(file, text.ToString(), starts, positions.ToArray(), null);
        }

        /// <summary>A fragment of C# (an anchor) written the way <see cref="Of"/> writes code.</summary>
        public static string Canonicalize(string code)
        {
            var tokens = SyntaxFactory.ParseTokens(code)
                .Where(x => !x.IsKind(SyntaxKind.EndOfFileToken) && x.Span.Length > 0)
                .Select(x => x.Text)
                .ToList();
            var text = new StringBuilder();
            for (var i = 0; i < tokens.Count; i++)
            {
                if (i > 0 && NeedsSpace(tokens[i - 1], tokens[i]))
                {
                    text.Append(' ');
                }

                text.Append(tokens[i]);
            }

            return text.ToString();
        }

        /// <summary>The position in the file of a character of <see cref="Text"/>.</summary>
        public int PositionAt(int offset)
        {
            if (_starts.Length == 0)
            {
                return 0;
            }

            var index = Array.BinarySearch(_starts, Math.Max(0, offset));
            index = index >= 0 ? index : Math.Max(0, ~index - 1);
            return _positions[index] + (_tokens == null ? 0 : Math.Max(0, Math.Min(offset - _starts[index], _tokens[index].Span.Length - 1)));
        }

        public int LineAt(int offset) => File.LineOf(PositionAt(offset));

        /// <summary>Where a span of the file (e.g. a node's) is in <see cref="Text"/>, as [start, end).</summary>
        public (int Start, int End) SliceOf(TextSpan span)
        {
            if (_tokens == null || _tokens.Length == 0)
            {
                return (0, Text.Length);
            }

            var first = IndexAtOrAfter(span.Start);
            var last = IndexAtOrAfter(span.End) - 1;
            return first > last || first >= _tokens.Length
                ? (0, 0)
                : (_starts[first], _starts[last] + _tokens[last].Text.Length);
        }

        private int IndexAtOrAfter(int position)
        {
            var index = Array.BinarySearch(_positions, position);
            return index >= 0 ? index : ~index;
        }

        /// <summary>The first and last C# tokens covering <see cref="Text"/>[<paramref name="start"/>, + <paramref name="length"/>).</summary>
        public bool TryGetTokens(int start, int length, out SyntaxToken first, out SyntaxToken last)
        {
            first = default;
            last = default;
            if (_tokens == null || _tokens.Length == 0)
            {
                return false;
            }

            first = _tokens[TokenIndexAt(start)];
            last = _tokens[TokenIndexAt(start + Math.Max(0, length - 1))];
            return true;
        }

        private int TokenIndexAt(int offset)
        {
            var index = Array.BinarySearch(_starts, offset);
            return index >= 0 ? index : Math.Max(0, ~index - 1);
        }

        private static bool NeedsSpace(string previous, string next)
        {
            return previous.Length > 0 && next.Length > 0 && IsWordEnd(previous[^1]) && IsWordStart(next[0]);
        }

        private static bool IsWordEnd(char c) => IsWordChar(c) || c is '"' or '\'';

        private static bool IsWordStart(char c) => IsWordChar(c) || c is '"' or '\'' or '@' or '$';

        private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_';
    }

    /// <summary>
    /// Finds types and members in parsed C#, and describes their members the way the model does: by name, access
    /// level and, for methods, parameter types.
    /// </summary>
    internal static class CodeLocator
    {
        public const string Public = "public";
        public const string Private = "private";

        private static readonly HashSet<string> ParameterModifiers = new(StringComparer.Ordinal) { "ref", "out", "in", "params", "this", "scoped", "readonly" };

        /// <summary>Every declaration of the type named <paramref name="typeName"/>, preferring a top-level one.</summary>
        public static IReadOnlyList<BaseTypeDeclarationSyntax> FindTypes(CompilationUnitSyntax root, string typeName)
        {
            var matches = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
                .Where(x => x.Identifier.Text == typeName)
                .GroupBy(TypePath, StringComparer.Ordinal)
                .ToList();
            var preferred = matches.FirstOrDefault(x => x.Key == typeName) ?? matches.FirstOrDefault();
            return preferred?.ToList() ?? new List<BaseTypeDeclarationSyntax>();
        }

        /// <summary>The type's name, prefixed by the names of the types it is nested in.</summary>
        public static string TypePath(BaseTypeDeclarationSyntax type)
        {
            var names = type.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().Select(x => x.Identifier.Text).Reverse();
            return string.Join(".", names);
        }

        /// <summary>The type's namespace and <see cref="TypePath"/>, which partial declarations of one type share.</summary>
        public static string FullTypePath(BaseTypeDeclarationSyntax type)
        {
            var namespaces = type.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Reverse().Select(x => x.Name.ToString());
            var fileScoped = type.SyntaxTree.GetRoot() is CompilationUnitSyntax root
                ? root.Members.OfType<FileScopedNamespaceDeclarationSyntax>().Select(x => x.Name.ToString())
                : Enumerable.Empty<string>();
            return string.Join(".", fileScoped.Concat(namespaces).Append(TypePath(type)));
        }

        public static IEnumerable<BaseTypeDeclarationSyntax> TopLevelTypes(CompilationUnitSyntax root)
        {
            return root.DescendantNodes(x => x is CompilationUnitSyntax || x is BaseNamespaceDeclarationSyntax)
                .OfType<BaseTypeDeclarationSyntax>();
        }

        /// <summary>
        /// The access level as the model writes it: a member declared without one is private, or public in an
        /// interface.
        /// </summary>
        public static string AccessOf(SyntaxTokenList modifiers, bool isInterface)
        {
            var isProtected = modifiers.Any(SyntaxKind.ProtectedKeyword);
            var isInternal = modifiers.Any(SyntaxKind.InternalKeyword);
            var isPrivate = modifiers.Any(SyntaxKind.PrivateKeyword);
            return modifiers.Any(SyntaxKind.PublicKeyword) ? Public
                : isProtected && isInternal ? "protected internal"
                : isProtected && isPrivate ? "private protected"
                : isProtected ? "protected"
                : isInternal ? "internal"
                : isPrivate ? Private
                : isInterface ? Public : Private;
        }

        /// <summary>The name part of a member key or path segment: <c>GetOrder(int)</c> gives <c>GetOrder</c>.</summary>
        public static string NameOf(string segment)
        {
            var end = segment.IndexOfAny(new[] { '(', '<', '[' });
            return (end < 0 ? segment : segment[..end]).Trim();
        }

        /// <summary>The parameter types of <c>Name(int, string)</c>, or <see langword="null"/> when none are given.</summary>
        public static string SignatureOf(string segment)
        {
            var start = segment.IndexOf('(');
            var end = segment.LastIndexOf(')');
            return start >= 0 && end > start ? NormalizeSignature(segment[(start + 1)..end]) : null;
        }

        /// <summary>A parameter list as a key: modifiers and types without whitespace, e.g. <c>ref int,string</c>.</summary>
        public static string ParameterKeys(BaseParameterListSyntax parameters)
        {
            if (parameters == null)
            {
                return string.Empty;
            }

            return string.Join(",", parameters.Parameters.Select(x => ParameterKey(x.Modifiers.Select(m => m.Text), x.Type?.ToString() ?? string.Empty)));
        }

        public static BaseParameterListSyntax ParametersOf(SyntaxNode member) => member switch
        {
            BaseMethodDeclarationSyntax x => x.ParameterList,
            LocalFunctionStatementSyntax x => x.ParameterList,
            IndexerDeclarationSyntax x => x.ParameterList,
            DelegateDeclarationSyntax x => x.ParameterList,
            TypeDeclarationSyntax x => x.ParameterList,
            _ => null
        };

        private static string NormalizeSignature(string signature)
        {
            return string.Join(",", SplitTopLevel(signature, ',')
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .Select(x =>
                {
                    var words = x.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                    var modifiers = words.TakeWhile(ParameterModifiers.Contains).ToList();
                    return ParameterKey(modifiers, string.Join(" ", words.Skip(modifiers.Count)));
                }));
        }

        private static string ParameterKey(IEnumerable<string> modifiers, string type)
        {
            var typeText = new string(type.Where(c => !char.IsWhiteSpace(c)).ToArray());
            return string.Join(" ", modifiers.Append(typeText));
        }

        public static IEnumerable<string> SplitTopLevel(string text, char separator)
        {
            var depth = 0;
            var start = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c is '(' or '<' or '[' or '{')
                {
                    depth++;
                }
                else if (c is ')' or '>' or ']' or '}')
                {
                    depth--;
                }
                else if (c == separator && depth == 0)
                {
                    yield return text[start..i];
                    start = i + 1;
                }
            }

            yield return text[start..];
        }
    }
}
