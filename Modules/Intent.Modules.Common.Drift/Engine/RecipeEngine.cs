using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.Drift
{
    /// <summary>What a step of a find path locates.</summary>
    internal enum CodeKind
    {
        File,
        Class,
        Method,
        Property,
        Parameter,
        Statement,
        Chain
    }

    internal enum StepMode
    {
        /// <summary><c>method</c>: every one inside the previous step's code.</summary>
        All,

        /// <summary><c>class:Name</c>: declared with that name.</summary>
        Name,

        /// <summary><c>statement:text</c>: the smallest one containing the text.</summary>
        Anchor,

        /// <summary><c>file:path/*.cs</c>: the files matching the pattern.</summary>
        Glob,

        /// <summary><c>method with regex</c>: the smallest one containing a match.</summary>
        Regex
    }

    internal sealed record FindStep(CodeKind Kind, StepMode Mode, string Argument);

    /// <summary>
    /// Steps separated by <c> &gt; </c>, each searching inside the code the previous step found, e.g.
    /// <c>class:{Service} &gt; method:{Operation}</c> or <c>statement:Entity&lt;{Entity}&gt; &gt; chain:Property(e =&gt; e.{Attribute})</c>.
    /// </summary>
    internal sealed class FindPath
    {
        private static readonly Dictionary<string, CodeKind> Kinds = new(StringComparer.Ordinal)
        {
            ["file"] = CodeKind.File,
            ["class"] = CodeKind.Class,
            ["method"] = CodeKind.Method,
            ["property"] = CodeKind.Property,
            ["parameter"] = CodeKind.Parameter,
            ["statement"] = CodeKind.Statement,
            ["chain"] = CodeKind.Chain
        };

        private FindPath(string text, IReadOnlyList<FindStep> steps)
        {
            Text = text;
            Steps = steps;
        }

        public string Text { get; }
        public IReadOnlyList<FindStep> Steps { get; }

        public static IReadOnlyCollection<string> KindNames => Kinds.Keys;

        public static bool TryParseKind(string text, out CodeKind kind) => Kinds.TryGetValue(text, out kind);

        public static FindPath Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new RecipeException("a find path can't be empty");
            }

            var steps = new List<FindStep>();
            foreach (var part in text.Split(" > "))
            {
                var step = part.Trim();
                var kindEnd = 0;
                while (kindEnd < step.Length && char.IsLetter(step[kindEnd]))
                {
                    kindEnd++;
                }

                var kindText = step[..kindEnd];
                if (!Kinds.TryGetValue(kindText, out var kind))
                {
                    throw new RecipeException($"'{step}' doesn't start with one of {string.Join(", ", Kinds.Keys)}");
                }

                var rest = step[kindEnd..];
                FindStep parsed;
                if (rest.Length == 0)
                {
                    if (kind == CodeKind.Chain)
                    {
                        throw new RecipeException("chain needs the text it contains, e.g. chain:Property(e => e.Name)");
                    }

                    parsed = new FindStep(kind, StepMode.All, null);
                }
                else if (rest.StartsWith(":", StringComparison.Ordinal))
                {
                    var argument = rest[1..].Trim();
                    if (argument.Length == 0)
                    {
                        throw new RecipeException($"'{step}' has nothing after the colon");
                    }

                    var mode = kind switch
                    {
                        CodeKind.File => StepMode.Glob,
                        CodeKind.Statement or CodeKind.Chain => StepMode.Anchor,
                        _ => StepMode.Name
                    };
                    parsed = new FindStep(kind, mode, argument);
                }
                else if (rest.StartsWith(" with ", StringComparison.Ordinal))
                {
                    var argument = rest[" with ".Length..].Trim();
                    if (argument.Length == 0)
                    {
                        throw new RecipeException($"'{step}' has no regular expression after 'with'");
                    }

                    parsed = new FindStep(kind, StepMode.Regex, argument);
                }
                else
                {
                    throw new RecipeException($"'{step}': write {kindText}, {kindText}:<name or text> or {kindText} with <regular expression>");
                }

                if (kind == CodeKind.File && steps.Count > 0)
                {
                    throw new RecipeException("file can only be the first step");
                }

                steps.Add(parsed);
            }

            return new FindPath(text.Trim(), steps);
        }

        public override string ToString() => Text;
    }

    internal sealed class RecipeException : Exception
    {
        public RecipeException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// <c>{Name}</c> placeholders in recipe text, optionally filtered: <c>{Service|trimEnd:Controller|lower}</c>.
    /// A brace written as <c>\{</c> is never a placeholder.
    /// </summary>
    internal static class Placeholders
    {
        private static readonly Regex Pattern = new(@"(?<!\\)\{(?<name>[A-Z][A-Za-z]*)(?<filters>(?:\|[A-Za-z]+(?::[^|}]*)?)*)\}", RegexOptions.CultureInvariant);
        private static readonly HashSet<string> Filters = new(StringComparer.Ordinal) { "lower", "upper", "camel", "pascal", "trimStart", "trimEnd" };

        /// <summary>The problem with the template's placeholders, or <see langword="null"/>.</summary>
        public static string Validate(string template, IReadOnlyCollection<string> allowed)
        {
            foreach (Match match in Pattern.Matches(template ?? string.Empty))
            {
                var name = match.Groups["name"].Value;
                if (!allowed.Contains(name))
                {
                    return $"{{{name}}} isn't available here (available: {string.Join(", ", allowed.Select(x => $"{{{x}}}"))}); write a literal brace as \\{{";
                }

                foreach (var filter in ParseFilters(match.Groups["filters"].Value))
                {
                    if (!Filters.Contains(filter.Name))
                    {
                        return $"'{filter.Name}' in {match.Value} isn't a filter (filters: lower, upper, camel, pascal, trimStart:<text>, trimEnd:<text>)";
                    }
                }
            }

            return null;
        }

        public static string Render(string template, IReadOnlyDictionary<string, string> values, Func<string, string> escape = null)
        {
            if (template == null)
            {
                return null;
            }

            return Pattern.Replace(template, match =>
            {
                if (!values.TryGetValue(match.Groups["name"].Value, out var value))
                {
                    return match.Value;
                }

                value ??= string.Empty;
                foreach (var filter in ParseFilters(match.Groups["filters"].Value))
                {
                    value = Apply(filter.Name, filter.Argument, value);
                }

                return escape == null ? value : escape(value);
            });
        }

        public static IReadOnlyDictionary<string, string> Samples(IEnumerable<string> names) => names.ToDictionary(x => x, _ => "Sample");

        private static string Apply(string filter, string argument, string value) => filter switch
        {
            "lower" => value.ToLowerInvariant(),
            "upper" => value.ToUpperInvariant(),
            "camel" => value.Length == 0 ? value : char.ToLowerInvariant(value[0]) + value[1..],
            "pascal" => value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..],
            "trimStart" => argument.Length > 0 && value.StartsWith(argument, StringComparison.Ordinal) ? value[argument.Length..] : value,
            "trimEnd" => argument.Length > 0 && value.EndsWith(argument, StringComparison.Ordinal) ? value[..^argument.Length] : value,
            _ => value
        };

        private static IEnumerable<(string Name, string Argument)> ParseFilters(string filters)
        {
            foreach (var filter in filters.Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                var colon = filter.IndexOf(':');
                yield return colon < 0 ? (filter, string.Empty) : (filter[..colon], filter[(colon + 1)..]);
            }
        }
    }

    internal sealed class ReadCase
    {
        public string Match { get; init; }
        public string Value { get; init; }
        public string Case { get; init; }
        public CodeKind? In { get; init; }

        /// <summary><c>declaration</c>: only the code before the body (attributes and signature); <c>body</c>: only the body.</summary>
        public string Part { get; init; }

        /// <summary>
        /// The code states the fact, but in a form the recipe can't read (a route from a constant or a helper), so
        /// the fact can't be verified, rather than taking the recipe's <c>default</c>.
        /// </summary>
        public bool Unreadable { get; init; }
    }

    internal sealed record Replacement(string Match, string With);

    /// <summary>How to find one kind of fact in the code and read its value.</summary>
    internal sealed class Recipe
    {
        public string Key { get; init; }
        public FactKind Kind { get; init; }
        public IReadOnlyList<FindPath> Find { get; init; }
        public IReadOnlyList<ReadCase> Read { get; init; }
        public string Default { get; init; }
        public string Absent { get; init; }

        /// <summary>
        /// What the code saying nothing means, in the codebase's own terms (<c>no IsRequired() in EF Core, which maps a
        /// string as nullable</c>), quoted on a drift row whose value is the default or absent one.
        /// </summary>
        public string Unstated { get; init; }

        public IReadOnlyList<string> Inherit { get; init; }
        public IReadOnlyList<Replacement> Replace { get; init; }
        public string Case { get; init; }
        public IReadOnlyList<Glob> Files { get; init; }
    }

    /// <summary>A kind of code that the model must describe, e.g. every action of every controller.</summary>
    internal sealed class OwnRecipe
    {
        public string Kind { get; init; }
        public IReadOnlyList<FindPath> Find { get; init; }
        public string Name { get; init; }
        public IReadOnlyList<Glob> Files { get; init; }
    }

    /// <summary>Code that may stay unmodelled: a file pattern, a type or a <c>Type.Member</c>.</summary>
    internal sealed class DriftException
    {
        public string Pattern { get; init; }
        public string Reason { get; init; }
        public Glob Glob { get; init; }
    }

    internal sealed record RecipeProblem(string Subject, string Message);

    /// <summary>
    /// A designer's recipe file, <c>guardrails/&lt;designer&gt;.drift.json</c>: per kind of fact, where it is and
    /// how to read its value; what code the model must describe; and the exceptions to that. Every problem with
    /// the file is collected, never thrown, so each one can be reported.
    /// </summary>
    internal sealed class RecipeFile
    {
        public static readonly IReadOnlyList<Glob> DefaultScope = new[] { new Glob("**/*.cs") };

        private static readonly string[] OwnPlaceholders = { "Class", "Method", "Member", "Name", "File" };
        private static readonly HashSet<string> RelativeInheritTargets = new(StringComparer.Ordinal) { "class", "base", "method", "property", "statement", "file" };

        public IReadOnlyList<Glob> Files { get; private set; } = DefaultScope;
        public Dictionary<string, Recipe> Facts { get; } = new(StringComparer.Ordinal);
        public List<OwnRecipe> Own { get; } = new();
        public List<DriftException> Exceptions { get; } = new();
        public List<RecipeProblem> Problems { get; } = new();

        /// <summary>Recipe kinds that had problems; their facts are reported through the problem, not one by one.</summary>
        public HashSet<string> BrokenKinds { get; } = new(StringComparer.Ordinal);

        public static RecipeFile Parse(string json, string designer, IReadOnlyList<FactKind> kinds)
        {
            var file = new RecipeFile();
            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(json, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
            }
            catch (JsonException e)
            {
                file.Problems.Add(new RecipeProblem(null, $"isn't valid JSON: {e.Message}"));
                return file;
            }

            using (document)
            {
                var root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                {
                    file.Problems.Add(new RecipeProblem(null, "must be a JSON object with facts, own and exceptions"));
                    return file;
                }

                foreach (var property in root.EnumerateObject())
                {
                    switch (property.Name)
                    {
                        case "files":
                            file.Files = file.Globs(property.Value, null) ?? DefaultScope;
                            break;
                        case "facts":
                            file.ParseFacts(property.Value, designer, kinds);
                            break;
                        case "own":
                            file.ParseOwn(property.Value);
                            break;
                        case "exceptions":
                            file.ParseExceptions(property.Value);
                            break;
                        case "note":
                            break;
                        default:
                            file.Problems.Add(new RecipeProblem(null, $"has an unknown property '{property.Name}' (expected files, facts, own, exceptions or note)"));
                            break;
                    }
                }
            }

            return file;
        }

        private void ParseFacts(JsonElement facts, string designer, IReadOnlyList<FactKind> kinds)
        {
            if (facts.ValueKind != JsonValueKind.Object)
            {
                Problems.Add(new RecipeProblem("facts", "must be an object keyed by fact kind"));
                return;
            }

            var byKey = kinds.ToDictionary(x => x.Key, StringComparer.Ordinal);
            foreach (var property in facts.EnumerateObject())
            {
                if (!byKey.TryGetValue(property.Name, out var kind))
                {
                    Problems.Add(new RecipeProblem(property.Name, $"isn't a fact the {designer} designer states (its facts: {string.Join(", ", byKey.Keys)})"));
                    continue;
                }

                var problemCount = Problems.Count;
                var recipe = ParseRecipe(kind, property.Value);
                if (recipe == null || Problems.Count > problemCount)
                {
                    BrokenKinds.Add(kind.Key);
                    continue;
                }

                Facts[kind.Key] = recipe;
            }
        }

        private Recipe ParseRecipe(FactKind kind, JsonElement element)
        {
            var subject = kind.Key;
            if (element.ValueKind != JsonValueKind.Object)
            {
                Problems.Add(new RecipeProblem(subject, "must be an object with find and read"));
                return null;
            }

            IReadOnlyList<FindPath> find = null;
            var read = new List<ReadCase>();
            string @default = null, absent = null, unstated = null, @case = null;
            IReadOnlyList<string> inherit = Array.Empty<string>();
            var replace = new List<Replacement>();
            IReadOnlyList<Glob> files = null;
            foreach (var property in element.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "find":
                        find = Paths(property.Value, subject, kind.Placeholders);
                        break;
                    case "read":
                        if (property.Value.ValueKind != JsonValueKind.Array)
                        {
                            Problems.Add(new RecipeProblem(subject, "read must be an array of cases"));
                            break;
                        }

                        foreach (var item in property.Value.EnumerateArray())
                        {
                            var readCase = ParseCase(item, subject, kind.Placeholders);
                            if (readCase != null)
                            {
                                read.Add(readCase);
                            }
                        }

                        break;
                    case "default":
                        @default = Template(property.Value, subject, "default", kind.Placeholders);
                        break;
                    case "absent":
                        absent = Template(property.Value, subject, "absent", kind.Placeholders);
                        break;
                    case "unstated":
                        unstated = Template(property.Value, subject, "unstated", kind.Placeholders);
                        break;
                    case "inherit":
                        inherit = Strings(property.Value, subject, "inherit");
                        var absoluteTargets = inherit.Where(x => !RelativeInheritTargets.Contains(x)).ToList();
                        if (absoluteTargets.Count > 0)
                        {
                            ParsePaths(absoluteTargets, subject, kind.Placeholders);
                        }

                        break;
                    case "replace":
                        if (property.Value.ValueKind != JsonValueKind.Array)
                        {
                            Problems.Add(new RecipeProblem(subject, "replace must be an array of { \"match\": …, \"with\": … }"));
                            break;
                        }

                        foreach (var item in property.Value.EnumerateArray())
                        {
                            var match = item.ValueKind == JsonValueKind.Object && item.TryGetProperty("match", out var m) ? Scalar(m) : null;
                            var with = item.ValueKind == JsonValueKind.Object && item.TryGetProperty("with", out var w) ? Scalar(w) : null;
                            if (match == null || with == null)
                            {
                                Problems.Add(new RecipeProblem(subject, "each replace needs match and with"));
                                continue;
                            }

                            if (CheckRegex(match, subject, kind.Placeholders) && CheckPlaceholders(with, subject, kind.Placeholders))
                            {
                                replace.Add(new Replacement(match, with));
                            }
                        }

                        break;
                    case "case":
                        @case = CaseOf(property.Value, subject);
                        break;
                    case "files":
                        files = Globs(property.Value, subject);
                        break;
                    case "note":
                        break;
                    default:
                        Problems.Add(new RecipeProblem(subject, $"has an unknown property '{property.Name}' (expected find, read, default, absent, unstated, inherit, replace, case, files or note)"));
                        break;
                }
            }

            if (find == null)
            {
                Problems.Add(new RecipeProblem(subject, "has no find"));
                return null;
            }

            if (read.Count == 0 && @default == null)
            {
                Problems.Add(new RecipeProblem(subject, "needs read cases, a default, or both"));
                return null;
            }

            if (unstated != null && @default == null && absent == null)
            {
                Problems.Add(new RecipeProblem(subject, "has unstated but no default or absent for it to explain"));
            }

            return new Recipe
            {
                Key = kind.Key,
                Kind = kind,
                Find = find,
                Read = read,
                Default = @default,
                Absent = absent,
                Unstated = unstated,
                Inherit = inherit,
                Replace = replace,
                Case = @case,
                Files = files
            };
        }

        private ReadCase ParseCase(JsonElement item, string subject, IReadOnlyCollection<string> placeholders)
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                Problems.Add(new RecipeProblem(subject, "each read case must be an object with match"));
                return null;
            }

            string match = null, value = null, @case = null, part = null;
            CodeKind? @in = null;
            var unreadable = false;
            foreach (var property in item.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "match":
                        match = Scalar(property.Value);
                        break;
                    case "value":
                        value = Template(property.Value, subject, "value", placeholders);
                        break;
                    case "case":
                        @case = CaseOf(property.Value, subject);
                        break;
                    case "in":
                        var text = Scalar(property.Value);
                        if (text != null && FindPath.TryParseKind(text, out var kind) && kind != CodeKind.Chain)
                        {
                            @in = kind;
                        }
                        else
                        {
                            Problems.Add(new RecipeProblem(subject, $"in must be file, class, method, property, parameter or statement, not '{text}'"));
                        }

                        break;
                    case "part":
                        part = Scalar(property.Value);
                        if (part is not ("declaration" or "body"))
                        {
                            Problems.Add(new RecipeProblem(subject, $"part must be declaration or body, not '{part}'"));
                            part = null;
                        }

                        break;
                    case "unreadable":
                        if (property.Value.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
                        {
                            Problems.Add(new RecipeProblem(subject, $"unreadable must be true or false, not '{Scalar(property.Value)}'"));
                            break;
                        }

                        unreadable = property.Value.GetBoolean();
                        break;
                    case "note":
                        break;
                    default:
                        Problems.Add(new RecipeProblem(subject, $"a read case has an unknown property '{property.Name}' (expected match, value, case, in, part, unreadable or note)"));
                        break;
                }
            }

            if (match == null)
            {
                Problems.Add(new RecipeProblem(subject, "a read case has no match"));
                return null;
            }

            if (unreadable && value != null)
            {
                Problems.Add(new RecipeProblem(subject, "a read case can't have both unreadable and value: an unreadable case reads no value"));
                return null;
            }

            return CheckRegex(match, subject, placeholders) ? new ReadCase { Match = match, Value = value, Case = @case, In = @in, Part = part, Unreadable = unreadable } : null;
        }

        private void ParseOwn(JsonElement own)
        {
            if (own.ValueKind != JsonValueKind.Array)
            {
                Problems.Add(new RecipeProblem("own", "must be an array"));
                return;
            }

            var index = 0;
            foreach (var item in own.EnumerateArray())
            {
                var subject = $"own[{index++}]";
                if (item.ValueKind != JsonValueKind.Object)
                {
                    Problems.Add(new RecipeProblem(subject, "must be an object with kind and find"));
                    continue;
                }

                string kind = null, name = null;
                IReadOnlyList<FindPath> find = null;
                IReadOnlyList<Glob> files = null;
                var problemCount = Problems.Count;
                foreach (var property in item.EnumerateObject())
                {
                    switch (property.Name)
                    {
                        case "kind":
                            kind = Scalar(property.Value);
                            break;
                        case "find":
                            find = Paths(property.Value, subject, Array.Empty<string>());
                            break;
                        case "name":
                            name = Template(property.Value, subject, "name", OwnPlaceholders);
                            break;
                        case "files":
                            files = Globs(property.Value, subject);
                            break;
                        case "note":
                            break;
                        default:
                            Problems.Add(new RecipeProblem(subject, $"has an unknown property '{property.Name}' (expected kind, find, name, files or note)"));
                            break;
                    }
                }

                if (string.IsNullOrWhiteSpace(kind) || find == null)
                {
                    Problems.Add(new RecipeProblem(subject, "needs kind and find"));
                    continue;
                }

                if (Problems.Count == problemCount)
                {
                    Own.Add(new OwnRecipe { Kind = kind.Trim(), Find = find, Name = name, Files = files });
                }
            }
        }

        private void ParseExceptions(JsonElement exceptions)
        {
            if (exceptions.ValueKind != JsonValueKind.Array)
            {
                Problems.Add(new RecipeProblem("exceptions", "must be an array"));
                return;
            }

            foreach (var item in exceptions.EnumerateArray())
            {
                string pattern = null, reason = null;
                if (item.ValueKind == JsonValueKind.String)
                {
                    pattern = item.GetString();
                }
                else if (item.ValueKind == JsonValueKind.Object)
                {
                    pattern = item.TryGetProperty("match", out var m) ? Scalar(m) : null;
                    reason = item.TryGetProperty("reason", out var r) ? Scalar(r) : null;
                }

                if (string.IsNullOrWhiteSpace(pattern))
                {
                    Problems.Add(new RecipeProblem("exceptions", "each exception is a pattern, or { \"match\": …, \"reason\": … }"));
                    continue;
                }

                pattern = pattern.Trim();
                Exceptions.Add(new DriftException
                {
                    Pattern = pattern,
                    Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
                    Glob = Glob.LooksLikeGlob(pattern) ? new Glob(pattern) : null
                });
            }
        }

        private IReadOnlyList<FindPath> Paths(JsonElement value, string subject, IReadOnlyCollection<string> placeholders)
        {
            var texts = Strings(value, subject, "find");
            if (texts.Count == 0)
            {
                Problems.Add(new RecipeProblem(subject, "find must be a path, or an array of alternative paths"));
                return null;
            }

            return ParsePaths(texts, subject, placeholders);
        }

        private IReadOnlyList<FindPath> ParsePaths(IReadOnlyList<string> texts, string subject, IReadOnlyCollection<string> placeholders)
        {
            var paths = new List<FindPath>();
            foreach (var text in texts)
            {
                try
                {
                    var path = FindPath.Parse(text);
                    foreach (var step in path.Steps.Where(x => x.Argument != null))
                    {
                        if (!CheckPlaceholders(step.Argument, subject, placeholders))
                        {
                            return null;
                        }

                        if (step.Mode == StepMode.Regex && !CheckRegex(step.Argument, subject, placeholders))
                        {
                            return null;
                        }
                    }

                    paths.Add(path);
                }
                catch (RecipeException e)
                {
                    Problems.Add(new RecipeProblem(subject, $"find '{text}': {e.Message}"));
                    return null;
                }
            }

            return paths;
        }

        private bool CheckRegex(string pattern, string subject, IReadOnlyCollection<string> placeholders)
        {
            if (!CheckPlaceholders(pattern, subject, placeholders))
            {
                return false;
            }

            try
            {
                _ = new Regex(Placeholders.Render(pattern, Placeholders.Samples(placeholders), Regex.Escape), RegexOptions.CultureInvariant);
                return true;
            }
            catch (ArgumentException e)
            {
                Problems.Add(new RecipeProblem(subject, $"'{pattern}' isn't a valid regular expression: {e.Message}"));
                return false;
            }
        }

        private bool CheckPlaceholders(string template, string subject, IReadOnlyCollection<string> placeholders)
        {
            var problem = Placeholders.Validate(template, placeholders);
            if (problem == null)
            {
                return true;
            }

            Problems.Add(new RecipeProblem(subject, problem));
            return false;
        }

        private string Template(JsonElement value, string subject, string name, IReadOnlyCollection<string> placeholders)
        {
            var text = Scalar(value);
            if (text == null)
            {
                Problems.Add(new RecipeProblem(subject, $"{name} must be a string"));
                return null;
            }

            return CheckPlaceholders(text, subject, placeholders) ? text : null;
        }

        private string CaseOf(JsonElement value, string subject)
        {
            var text = Scalar(value);
            if (text is "upper" or "lower")
            {
                return text;
            }

            Problems.Add(new RecipeProblem(subject, $"case must be upper or lower, not '{text}'"));
            return null;
        }

        private IReadOnlyList<Glob> Globs(JsonElement value, string subject)
        {
            var patterns = Strings(value, subject ?? "files", "files");
            return patterns.Count == 0 ? null : patterns.Select(x => new Glob(x)).ToList();
        }

        private IReadOnlyList<string> Strings(JsonElement value, string subject, string name)
        {
            if (value.ValueKind == JsonValueKind.String)
            {
                return new[] { value.GetString() };
            }

            if (value.ValueKind == JsonValueKind.Array && value.EnumerateArray().All(x => x.ValueKind == JsonValueKind.String))
            {
                return value.EnumerateArray().Select(x => x.GetString()).ToList();
            }

            Problems.Add(new RecipeProblem(subject, $"{name} must be a string or an array of strings"));
            return Array.Empty<string>();
        }

        private static string Scalar(JsonElement value) => value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => null
        };
    }

    /// <summary>A place a find step found: a node of a C# file, or a whole file.</summary>
    internal sealed class CodeHit
    {
        public CodeFile File { get; init; }

        /// <summary>The node, the file's root for a whole C# file, or <see langword="null"/> for any other file.</summary>
        public SyntaxNode Node { get; init; }

        /// <summary>The last text or regular-expression match that led here, for <c>$1</c> in ownership names.</summary>
        public Match Capture { get; init; }

        public int Line => Node == null || Node is CompilationUnitSyntax ? 1 : File.LineOf(Node.SpanStart);

        public string Location => $"{File.RelativePath}:{Line}";
    }

    /// <summary>Where a verified fact's value came from.</summary>
    internal enum ValueSource
    {
        /// <summary>A read case matched the code.</summary>
        Code,

        /// <summary>The code was found, but no read case matched it.</summary>
        Default,

        /// <summary>No code was found.</summary>
        Absent
    }

    /// <summary>What a read case matched, and in which file's text, for the recipe probe to change it.</summary>
    internal sealed class ReadMatch
    {
        public CodeFile File { get; init; }

        /// <summary>Where the text the case was matched against starts in the file's <see cref="CodeFile.Canonical"/> text.</summary>
        public int Offset { get; init; }

        public Match Match { get; init; }
    }

    /// <summary>What reading one fact from the code gave.</summary>
    internal sealed class FactReading
    {
        public bool Verified { get; init; }
        public string Value { get; init; }

        /// <summary>Where a verified value came from.</summary>
        public ValueSource From { get; init; }

        /// <summary>The recipe's find path that found the code, by index; <see langword="null"/> when none did.</summary>
        public int? Path { get; init; }

        /// <summary>The read case that gave the value, or the problem (an unreadable case), by index; <see langword="null"/> when none did.</summary>
        public int? Case { get; init; }

        /// <summary>Where that case matched: 0 in the code the find found, 1 in the first inherit target, and so on.</summary>
        public int Level { get; init; }

        /// <summary>What that case matched.</summary>
        public ReadMatch Match { get; init; }

        public string Location { get; init; }

        /// <summary>
        /// When the find found no code: the one place its path reached before it stopped matching (an entity's
        /// configuration with no line for the property), where the code would state the fact.
        /// </summary>
        public string Near { get; init; }

        /// <summary>
        /// When the value is the recipe's default or absent one, because the code doesn't state the fact: what that
        /// means, in the recipe's words when it has them. <see langword="null"/> when the code states the fact.
        /// </summary>
        public string Unstated { get; init; }

        /// <summary>Why the fact can't be verified, without its <see cref="Location"/>, which the report shows beside it.</summary>
        public string Problem { get; init; }

        /// <summary>The same reading, with <see cref="Location"/> and <see cref="Near"/> mapped, e.g. from code held in memory to the file on disk.</summary>
        public FactReading Relocated(Func<string, string> locate) => new()
        {
            Verified = Verified,
            Value = Value,
            From = From,
            Path = Path,
            Case = Case,
            Level = Level,
            Match = Match,
            Location = locate(Location),
            Near = locate(Near),
            Unstated = Unstated,
            Problem = Problem
        };
    }

    /// <summary>A piece of code an ownership recipe found.</summary>
    internal sealed class OwnedCode
    {
        public string Kind { get; init; }
        public string Name { get; init; }
        public string TypeName { get; init; }
        public string MemberName { get; init; }
        public CodeHit Hit { get; init; }
    }

    /// <summary>
    /// Runs recipes over the code. It knows C# syntax and nothing about any framework or database: a recipe says
    /// where a fact is, and how to read its value.
    /// </summary>
    internal sealed class RecipeEngine
    {
        private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(2);

        private readonly CodeFiles _code;
        private readonly IReadOnlyList<Glob> _defaultScope;
        private readonly Dictionary<CodeFile, Dictionary<CodeKind, List<SyntaxNode>>> _nodes;
        private readonly Dictionary<string, Regex> _regexes;
        private readonly Dictionary<string, IReadOnlyList<CodeHit>> _firstSteps = new(StringComparer.Ordinal);
        private readonly Dictionary<string, IReadOnlyList<string>> _scopes;
        private readonly List<CodeHit> _located = new();
        private readonly GeneratedTypes _generated;
        private readonly RecipeEngine _base;
        private readonly IReadOnlySet<string> _changed;

        public RecipeEngine(CodeFiles code, IReadOnlyList<Glob> defaultScope, GeneratedTypes generated = null)
        {
            _code = code;
            _defaultScope = defaultScope ?? RecipeFile.DefaultScope;
            _generated = generated ?? new GeneratedTypes();
            _nodes = new Dictionary<CodeFile, Dictionary<CodeKind, List<SyntaxNode>>>();
            _regexes = new Dictionary<string, Regex>(StringComparer.Ordinal);
            _scopes = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
        }

        private RecipeEngine(RecipeEngine engine, IReadOnlyDictionary<string, string> files)
        {
            _code = engine._code.WithFiles(files);
            _defaultScope = engine._defaultScope;
            _generated = engine._generated;
            _nodes = engine._nodes;
            _regexes = engine._regexes;
            _base = engine;
            _changed = new HashSet<string>(files.Keys.Select(engine._code.ToRelative), StringComparer.OrdinalIgnoreCase);
            _scopes = _changed.All(x => engine._code.Get(x) != null) ? engine._scopes : new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
        }

        /// <summary>
        /// An engine over the same codebase with some files' text changed, or new files added, in memory, sharing what this
        /// one has already read of the others. What it finds starts afresh.
        /// </summary>
        public RecipeEngine WithFiles(IReadOnlyDictionary<string, string> files) => new(this, files);

        /// <summary>An engine over the same codebase with one file's text changed in memory: the recipe probe's.</summary>
        public RecipeEngine WithText(string relativePath, string text) => WithFiles(new Dictionary<string, string> { [relativePath] = text });

        /// <summary>
        /// Hands what this engine's finds located in files it didn't change to the engine it was made from, where that
        /// code (the same syntax nodes) then counts as described.
        /// </summary>
        public void HandBackLocated() => _base?._located.AddRange(_located.Where(x => !_changed.Contains(x.File.RelativePath)));

        /// <summary>Finds the fact's code and reads its value.</summary>
        public FactReading Read(Recipe recipe, ModelFact fact)
        {
            var scope = recipe.Files ?? _defaultScope;
            try
            {
                var hits = Find(recipe.Find, fact.Placeholders, fact.ParameterNames, scope, firstMatchOnly: true, out var near, out var path);
                if (hits.Count == 0)
                {
                    return recipe.Absent != null
                        ? new FactReading { Verified = true, Value = Placeholders.Render(recipe.Absent, fact.Placeholders).Trim(), From = ValueSource.Absent, Near = near?.Location, Unstated = Unstated(recipe, fact) }
                        : new FactReading { Problem = $"no code found ({Describe(recipe.Find, fact.Placeholders)})", Near = near?.Location };
                }

                var parts = AsOneUnit(hits);
                if (parts == null)
                {
                    var places = string.Join(", ", hits.Take(3).Select(x => $"`{x.Location}`"));
                    return new FactReading { Problem = $"{Describe(recipe.Find, fact.Placeholders)} matches {hits.Count} places ({places}{(hits.Count > 3 ? ", …" : string.Empty)}); narrow the find", Path = path };
                }

                _located.AddRange(hits);
                var level = 0;
                var reading = ReadCases(recipe, parts, fact, scope, path, level);
                if (reading != null)
                {
                    return reading;
                }

                foreach (var target in recipe.Inherit)
                {
                    foreach (var targetParts in InheritTargets(target, parts, fact, scope))
                    {
                        reading = ReadCases(recipe, targetParts, fact, scope, path, ++level);
                        if (reading != null)
                        {
                            return reading;
                        }
                    }
                }

                if (recipe.Default != null)
                {
                    return new FactReading
                    {
                        Verified = true,
                        Value = Placeholders.Render(recipe.Default, fact.Placeholders).Trim(),
                        From = ValueSource.Default,
                        Path = path,
                        Location = parts[0].Location,
                        Unstated = Unstated(recipe, fact)
                    };
                }

                return new FactReading { Problem = "no read case matches, and the recipe has no default", Path = path, Location = parts[0].Location };
            }
            catch (RegexMatchTimeoutException)
            {
                return new FactReading { Problem = "a regular expression took too long; simplify it" };
            }
        }

        private static string Unstated(Recipe recipe, ModelFact fact) =>
            recipe.Unstated == null ? "the code doesn't state it" : Placeholders.Render(recipe.Unstated, fact.Placeholders).Trim();

        /// <summary>Every piece of code an ownership recipe finds.</summary>
        public IReadOnlyList<OwnedCode> Own(OwnRecipe own)
        {
            var scope = own.Files ?? _defaultScope;
            var hits = Find(own.Find, new Dictionary<string, string>(), null, scope, firstMatchOnly: false);
            return hits.Select(hit =>
            {
                var type = hit.Node?.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
                var member = hit.Node?.AncestorsAndSelf().FirstOrDefault(x => x is MemberDeclarationSyntax and not BaseTypeDeclarationSyntax and not BaseNamespaceDeclarationSyntax and not GlobalStatementSyntax);
                var typeName = type == null ? null : CodeLocator.TypePath(type);
                var memberName = member == null ? null : NamesOf(member).FirstOrDefault();
                var values = new Dictionary<string, string>
                {
                    ["Class"] = typeName ?? string.Empty,
                    ["Method"] = hit.Node?.AncestorsAndSelf().FirstOrDefault(x => IsKind(x, CodeKind.Method)) is { } method ? NamesOf(method).FirstOrDefault() : string.Empty,
                    ["Member"] = memberName ?? string.Empty,
                    ["Name"] = hit.Node == null ? hit.File.RelativePath : NamesOf(hit.Node).FirstOrDefault() ?? string.Empty,
                    ["File"] = hit.File.RelativePath
                };
                var name = own.Name != null
                    ? RenderWithCapture(own.Name, hit.Capture, values)
                    : DefaultName(hit, typeName, memberName);
                return new OwnedCode { Kind = own.Kind, Name = name, TypeName = typeName, MemberName = memberName, Hit = hit };
            }).ToList();
        }

        /// <summary>
        /// Whether the model describes this code: a fact's find located it or code inside it, or it is a type the
        /// contract templates generate (a whole file, when it declares one).
        /// </summary>
        public bool IsDescribed(CodeHit owned)
        {
            if (owned.Node is BaseTypeDeclarationSyntax type
                ? _generated.Contains(type)
                : owned.Node is CompilationUnitSyntax root && CodeLocator.TopLevelTypes(root).Any(_generated.Contains))
            {
                return true;
            }

            return _located.Any(x => x.File == owned.File &&
                (owned.Node == null || owned.Node is CompilationUnitSyntax ||
                x.Node != null && x.Node.SyntaxTree == owned.Node.SyntaxTree && owned.Node.Span.Contains(x.Node.Span)));
        }

        private static string DefaultName(CodeHit hit, string typeName, string memberName)
        {
            if (hit.Node == null || hit.Node is CompilationUnitSyntax)
            {
                return hit.File.RelativePath;
            }

            if (hit.Capture is { Groups.Count: > 1 } && hit.Capture.Groups[1].Success)
            {
                return hit.Capture.Groups[1].Value;
            }

            if (hit.Node is BaseTypeDeclarationSyntax)
            {
                return typeName;
            }

            return memberName != null && typeName != null ? $"{typeName}.{memberName}" : typeName ?? hit.Location;
        }

        private FactReading ReadCases(Recipe recipe, IReadOnlyList<CodeHit> parts, ModelFact fact, IReadOnlyList<Glob> scope, int? path, int level)
        {
            for (var caseIndex = 0; caseIndex < recipe.Read.Count; caseIndex++)
            {
                var readCase = recipe.Read[caseIndex];
                var regex = Compile(Placeholders.Render(readCase.Match, fact.Placeholders, Regex.Escape));
                foreach (var part in parts)
                {
                    var target = readCase.In == null ? part : AncestorOrSelf(part, readCase.In.Value, scope).FirstOrDefault();
                    if (target == null)
                    {
                        continue;
                    }

                    foreach (var unit in target is { Node: BaseTypeDeclarationSyntax type } && readCase.In == CodeKind.Class ? ClassParts(type, target.File, scope) : new[] { target })
                    {
                        var span = SpanOf(unit, readCase.Part);
                        var match = regex.Match(span.Text);
                        if (!match.Success)
                        {
                            continue;
                        }

                        var location = $"{unit.File.RelativePath}:{span.LineAt(match.Index)}";
                        var matched = new ReadMatch { File = unit.File, Offset = span.Start, Match = match };
                        if (readCase.Unreadable)
                        {
                            return new FactReading
                            {
                                Problem = $"stated in a form the recipe can't read: {DriftCheck.Code(match.Value)}",
                                Path = path,
                                Case = caseIndex,
                                Level = level,
                                Match = matched,
                                Location = location
                            };
                        }

                        var value = readCase.Value != null
                            ? RenderWithCapture(readCase.Value, match, fact.Placeholders)
                            : match.Groups.Count > 1 ? match.Groups[1].Value : match.Value;
                        foreach (var replacement in recipe.Replace)
                        {
                            value = Compile(Placeholders.Render(replacement.Match, fact.Placeholders, Regex.Escape))
                                .Replace(value, Placeholders.Render(replacement.With, fact.Placeholders, x => x.Replace("$", "$$")));
                        }

                        value = (readCase.Case ?? recipe.Case) switch
                        {
                            "upper" => value.ToUpperInvariant(),
                            "lower" => value.ToLowerInvariant(),
                            _ => value
                        };
                        return new FactReading
                        {
                            Verified = true,
                            Value = value.Trim(),
                            From = ValueSource.Code,
                            Path = path,
                            Case = caseIndex,
                            Level = level,
                            Match = matched,
                            Location = location
                        };
                    }
                }
            }

            return null;
        }

        private IEnumerable<IReadOnlyList<CodeHit>> InheritTargets(string target, IReadOnlyList<CodeHit> parts, ModelFact fact, IReadOnlyList<Glob> scope)
        {
            switch (target)
            {
                case "base":
                    var visited = new HashSet<string>(StringComparer.Ordinal);
                    var level = parts.Select(x => x.Node?.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault())
                        .Where(x => x != null)
                        .ToList();
                    for (var depth = 0; depth < 8 && level.Count > 0; depth++)
                    {
                        var baseNames = level.SelectMany(BaseNames).Where(visited.Add).ToList();
                        var bases = baseNames.SelectMany(name => TypesNamed(name, scope)).ToList();
                        if (bases.Count == 0)
                        {
                            yield break;
                        }

                        yield return bases;
                        level = bases.Select(x => (BaseTypeDeclarationSyntax)x.Node).ToList();
                    }

                    yield break;
                case "class":
                case "method":
                case "property":
                case "statement":
                case "file":
                    FindPath.TryParseKind(target, out var kind);
                    var ancestors = parts.SelectMany(x => Ancestor(x, kind, scope)).ToList();
                    if (ancestors.Count > 0)
                    {
                        yield return ancestors;
                    }

                    yield break;
                default:
                    var hits = Find(new[] { FindPath.Parse(target) }, fact.Placeholders, fact.ParameterNames, scope, firstMatchOnly: true);
                    if (hits.Count > 0)
                    {
                        yield return hits;
                    }

                    yield break;
            }
        }

        /// <summary>The nearest enclosing code of a kind, not the code itself. A partial class gives all of its parts.</summary>
        private IEnumerable<CodeHit> Ancestor(CodeHit hit, CodeKind kind, IReadOnlyList<Glob> scope)
        {
            if (kind == CodeKind.File)
            {
                return hit.Node == null || hit.Node is CompilationUnitSyntax ? Array.Empty<CodeHit>() : new[] { WholeFile(hit.File) };
            }

            var node = hit.Node?.Parent?.AncestorsAndSelf().FirstOrDefault(x => IsKind(x, kind));
            if (node == null)
            {
                return Array.Empty<CodeHit>();
            }

            return node is BaseTypeDeclarationSyntax type ? ClassParts(type, hit.File, scope) : new[] { new CodeHit { File = hit.File, Node = node } };
        }

        private IEnumerable<CodeHit> AncestorOrSelf(CodeHit hit, CodeKind kind, IReadOnlyList<Glob> scope)
        {
            if (kind == CodeKind.File)
            {
                return new[] { WholeFile(hit.File) };
            }

            var node = hit.Node?.AncestorsAndSelf().FirstOrDefault(x => IsKind(x, kind));
            return node == null ? Array.Empty<CodeHit>() : new[] { new CodeHit { File = hit.File, Node = node } };
        }

        /// <summary>Every declaration of a partial type in scope, or just this one.</summary>
        private IReadOnlyList<CodeHit> ClassParts(BaseTypeDeclarationSyntax type, CodeFile file, IReadOnlyList<Glob> scope)
        {
            if (!type.Modifiers.Any(x => x.Text == "partial"))
            {
                return new[] { new CodeHit { File = file, Node = type } };
            }

            var path = CodeLocator.FullTypePath(type);
            var parts = TypesNamed(type.Identifier.Text, scope)
                .Where(x => CodeLocator.FullTypePath((BaseTypeDeclarationSyntax)x.Node) == path)
                .ToList();
            return parts.Count > 0 ? parts : new[] { new CodeHit { File = file, Node = type } };
        }

        private IEnumerable<CodeHit> TypesNamed(string name, IReadOnlyList<Glob> scope)
        {
            foreach (var file in FilesIn(scope).Select(_code.Get).Where(x => x?.Syntax != null))
            {
                foreach (var node in NodesOf(file, CodeKind.Class).Where(x => NamesOf(x).Contains(name)))
                {
                    yield return new CodeHit { File = file, Node = node };
                }
            }
        }

        private static IEnumerable<string> BaseNames(BaseTypeDeclarationSyntax type)
        {
            return type.BaseList?.Types.Select(x =>
            {
                var name = x.Type.ToString();
                var generic = name.IndexOf('<');
                name = generic >= 0 ? name[..generic] : name;
                return name[(name.LastIndexOf('.') + 1)..].Trim();
            }) ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// One unit to read: a single place, or the partial declarations of one type. Anything else is ambiguous
        /// (<see langword="null"/>).
        /// </summary>
        private static IReadOnlyList<CodeHit> AsOneUnit(IReadOnlyList<CodeHit> hits)
        {
            if (hits.Count == 1)
            {
                return hits;
            }

            var types = hits.Select(x => x.Node as BaseTypeDeclarationSyntax).ToList();
            if (types.All(x => x != null) && types.Select(CodeLocator.FullTypePath).Distinct(StringComparer.Ordinal).Count() == 1)
            {
                return hits;
            }

            return null;
        }

        private IReadOnlyList<CodeHit> Find(
            IReadOnlyList<FindPath> alternatives,
            IReadOnlyDictionary<string, string> placeholders,
            IReadOnlyList<string> parameterNames,
            IReadOnlyList<Glob> scope,
            bool firstMatchOnly)
        {
            return Find(alternatives, placeholders, parameterNames, scope, firstMatchOnly, out _, out _);
        }

        /// <param name="near">
        /// When nothing is found: the deepest place a path reached before a later step matched nothing, if exactly one
        /// place was reached at that step.
        /// </param>
        /// <param name="found">With <paramref name="firstMatchOnly"/>, the path that found code, by index.</param>
        private IReadOnlyList<CodeHit> Find(
            IReadOnlyList<FindPath> alternatives,
            IReadOnlyDictionary<string, string> placeholders,
            IReadOnlyList<string> parameterNames,
            IReadOnlyList<Glob> scope,
            bool firstMatchOnly,
            out CodeHit near,
            out int? found)
        {
            near = null;
            found = null;
            var nearDepth = 0;
            var all = new List<CodeHit>();
            for (var pathIndex = 0; pathIndex < alternatives.Count; pathIndex++)
            {
                var path = alternatives[pathIndex];
                IReadOnlyList<CodeHit> hits = null;
                for (var i = 0; i < path.Steps.Count; i++)
                {
                    var step = path.Steps[i];
                    var argument = step.Mode == StepMode.Regex
                        ? Placeholders.Render(step.Argument, placeholders, Regex.Escape)
                        : Placeholders.Render(step.Argument, placeholders);
                    hits = i == 0
                        ? FirstStep(step, argument, parameterNames, scope)
                        : hits.SelectMany(context => StepWithin(step, argument, context, parameterNames)).ToList();
                    hits = Distinct(hits);
                    if (hits.Count == 0)
                    {
                        break;
                    }

                    if (i < path.Steps.Count - 1 && i + 1 > nearDepth && hits.Count == 1)
                    {
                        (near, nearDepth) = (hits[0], i + 1);
                    }
                }

                if (hits is { Count: > 0 })
                {
                    if (firstMatchOnly)
                    {
                        found = pathIndex;
                        return hits;
                    }

                    all.AddRange(hits);
                }
            }

            return Distinct(all);
        }

        private IReadOnlyList<CodeHit> FirstStep(FindStep step, string argument, IReadOnlyList<string> parameterNames, IReadOnlyList<Glob> scope)
        {
            var key = $"{string.Join("|", scope.Select(x => x.Pattern))}\u0001{step.Kind}\u0001{step.Mode}\u0001{argument}\u0001{string.Join(",", parameterNames ?? Array.Empty<string>())}";
            if (_firstSteps.TryGetValue(key, out var cached))
            {
                return cached;
            }

            List<CodeHit> result;
            if (step.Kind == CodeKind.File)
            {
                var paths = step.Mode == StepMode.Glob ? _code.Matching(new[] { new Glob(argument) }) : FilesIn(scope);
                result = new List<CodeHit>();
                foreach (var file in paths.Select(_code.Get).Where(x => x != null))
                {
                    if (step.Mode == StepMode.Regex)
                    {
                        var match = Compile(argument).Match(file.Canonical.Text);
                        if (match.Success)
                        {
                            result.Add(new CodeHit { File = file, Node = file.Syntax, Capture = match });
                        }

                        continue;
                    }

                    result.Add(WholeFile(file));
                }
            }
            else
            {
                result = FilesIn(scope)
                    .Select(_code.Get)
                    .Where(x => x?.Syntax != null)
                    .SelectMany(file => StepWithin(step, argument, WholeFile(file), parameterNames))
                    .ToList();
            }

            return _firstSteps[key] = result;
        }

        private IEnumerable<CodeHit> StepWithin(FindStep step, string argument, CodeHit context, IReadOnlyList<string> parameterNames)
        {
            var file = context.File;
            if (file.Syntax == null || step.Kind == CodeKind.File)
            {
                return Array.Empty<CodeHit>();
            }

            var contextNode = context.Node ?? file.Syntax;
            switch (step.Mode)
            {
                case StepMode.All:
                    return Within(file, step.Kind, contextNode).Select(x => new CodeHit { File = file, Node = x });
                case StepMode.Name:
                    var name = CodeLocator.NameOf(argument);
                    var signature = step.Kind == CodeKind.Method ? CodeLocator.SignatureOf(argument) : null;
                    var named = Within(file, step.Kind, contextNode)
                        .Where(x => NamesOf(x).Contains(name, StringComparer.Ordinal))
                        .Where(x => signature == null || CodeLocator.ParameterKeys(CodeLocator.ParametersOf(x)) == signature)
                        .ToList();
                    if (step.Kind == CodeKind.Method && signature == null && named.Count > 1 && parameterNames != null)
                    {
                        var byParameterNames = named
                            .Where(x => (CodeLocator.ParametersOf(x)?.Parameters.Select(p => p.Identifier.Text) ?? Enumerable.Empty<string>()).SequenceEqual(parameterNames, StringComparer.Ordinal))
                            .ToList();
                        if (byParameterNames.Count > 0)
                        {
                            named = byParameterNames;
                        }
                    }

                    return named.Select(x => new CodeHit { File = file, Node = x });
                case StepMode.Anchor:
                    var anchor = CodeText.Canonicalize(argument);
                    return TextMatches(context, anchor, null, step.Kind);
                case StepMode.Regex:
                    return TextMatches(context, null, Compile(argument), step.Kind);
                default:
                    return Array.Empty<CodeHit>();
            }
        }

        /// <summary>The smallest code of a kind around each occurrence of the anchor or regular expression.</summary>
        private IEnumerable<CodeHit> TextMatches(CodeHit context, string anchor, Regex regex, CodeKind kind)
        {
            var span = SpanOf(context);
            var text = span.Text;
            var contextNode = context.Node ?? context.File.Syntax;
            var results = new List<CodeHit>();
            var matches = regex != null
                ? regex.Matches(text).Cast<Match>().Where(x => x.Success)
                : AnchorMatches(text, anchor);
            foreach (var match in matches)
            {
                if (!span.TryGetTokens(match.Index, Math.Max(1, match.Length), out var first, out var last))
                {
                    continue;
                }

                var node = Enclosing(first, last, kind, contextNode);
                if (node != null)
                {
                    results.Add(new CodeHit { File = context.File, Node = node, Capture = match });
                }
            }

            return results;
        }

        private static IEnumerable<Match> AnchorMatches(string text, string anchor)
        {
            if (string.IsNullOrEmpty(anchor))
            {
                yield break;
            }

            var regex = new Regex(Regex.Escape(anchor), RegexOptions.CultureInvariant);
            var index = 0;
            while (index <= text.Length)
            {
                var match = regex.Match(text, index);
                if (!match.Success)
                {
                    yield break;
                }

                yield return match;
                index = match.Index + Math.Max(1, match.Length);
            }
        }

        private static SyntaxNode Enclosing(SyntaxToken first, SyntaxToken last, CodeKind kind, SyntaxNode context)
        {
            var node = first.Parent;
            while (node != null && !node.Span.Contains(last.Span))
            {
                node = node.Parent;
            }

            if (kind == CodeKind.Chain)
            {
                while (node != null && node is not InvocationExpressionSyntax)
                {
                    if (node == context)
                    {
                        return null;
                    }

                    node = node.Parent;
                }

                if (node == null || !Contains(context, node))
                {
                    return null;
                }

                while (node.Parent != null && node.Parent != context)
                {
                    if (node.Parent is MemberAccessExpressionSyntax access && access.Expression == node ||
                        node.Parent is InvocationExpressionSyntax invocation && invocation.Expression == node ||
                        node.Parent is ConditionalAccessExpressionSyntax conditional && conditional.Expression == node)
                    {
                        node = node.Parent;
                        continue;
                    }

                    break;
                }

                return node;
            }

            while (node != null && !IsKind(node, kind))
            {
                if (node == context)
                {
                    return null;
                }

                node = node.Parent;
            }

            return node != null && Contains(context, node) ? node : null;
        }

        private static bool Contains(SyntaxNode context, SyntaxNode node)
        {
            return context is CompilationUnitSyntax || context.Span.Contains(node.Span);
        }

        private IEnumerable<SyntaxNode> Within(CodeFile file, CodeKind kind, SyntaxNode context)
        {
            var nodes = NodesOf(file, kind);
            return context is CompilationUnitSyntax
                ? nodes
                : nodes.Where(x => x != context && context.Span.Contains(x.Span));
        }

        private List<SyntaxNode> NodesOf(CodeFile file, CodeKind kind)
        {
            if (!_nodes.TryGetValue(file, out var byKind))
            {
                byKind = new Dictionary<CodeKind, List<SyntaxNode>>();
                foreach (var node in file.Syntax.DescendantNodes())
                {
                    foreach (var candidate in new[] { CodeKind.Class, CodeKind.Method, CodeKind.Property, CodeKind.Parameter, CodeKind.Statement })
                    {
                        if (IsKind(node, candidate))
                        {
                            if (!byKind.TryGetValue(candidate, out var list))
                            {
                                byKind[candidate] = list = new List<SyntaxNode>();
                            }

                            list.Add(node);
                        }
                    }
                }

                _nodes[file] = byKind;
            }

            return byKind.TryGetValue(kind, out var result) ? result : new List<SyntaxNode>();
        }

        public static bool IsKind(SyntaxNode node, CodeKind kind) => kind switch
        {
            CodeKind.File => node is CompilationUnitSyntax,
            CodeKind.Class => node is BaseTypeDeclarationSyntax,
            CodeKind.Method => node is MethodDeclarationSyntax or ConstructorDeclarationSyntax or LocalFunctionStatementSyntax,
            CodeKind.Property => node is PropertyDeclarationSyntax or IndexerDeclarationSyntax or EventDeclarationSyntax or BaseFieldDeclarationSyntax,
            CodeKind.Parameter => node is ParameterSyntax,
            CodeKind.Statement => node is StatementSyntax,
            CodeKind.Chain => node is InvocationExpressionSyntax,
            _ => false
        };

        public static IReadOnlyList<string> NamesOf(SyntaxNode node) => node switch
        {
            BaseTypeDeclarationSyntax x => new[] { x.Identifier.Text },
            MethodDeclarationSyntax x => new[] { x.Identifier.Text },
            ConstructorDeclarationSyntax x => new[] { x.Identifier.Text },
            LocalFunctionStatementSyntax x => new[] { x.Identifier.Text },
            PropertyDeclarationSyntax x => new[] { x.Identifier.Text },
            EventDeclarationSyntax x => new[] { x.Identifier.Text },
            IndexerDeclarationSyntax => new[] { "this" },
            BaseFieldDeclarationSyntax x => x.Declaration.Variables.Select(v => v.Identifier.Text).ToList(),
            ParameterSyntax x => new[] { x.Identifier.Text },
            DelegateDeclarationSyntax x => new[] { x.Identifier.Text },
            _ => Array.Empty<string>()
        };

        private CodeSpan SpanOf(CodeHit hit)
        {
            if (hit.Node == null || hit.Node is CompilationUnitSyntax)
            {
                return new CodeSpan(hit.File.Canonical, 0, hit.File.Canonical.Text.Length);
            }

            var (start, end) = hit.File.Canonical.SliceOf(hit.Node.Span);
            return new CodeSpan(hit.File.Canonical, start, end);
        }

        /// <summary>
        /// The whole code, or only its <c>declaration</c> (what comes before the body: attributes, modifiers,
        /// signature and base types) or its <c>body</c> (a type's members, a method's block or expression, a
        /// property's accessors and initializer). Code with no body is all declaration.
        /// </summary>
        private CodeSpan SpanOf(CodeHit hit, string part)
        {
            if (part == null || hit.Node == null || hit.Node is CompilationUnitSyntax)
            {
                return SpanOf(hit);
            }

            var node = hit.Node;
            var bodyStart = node switch
            {
                TypeDeclarationSyntax x when !x.OpenBraceToken.IsMissing && x.OpenBraceToken.Span.Length > 0 => x.OpenBraceToken.SpanStart,
                EnumDeclarationSyntax x when !x.OpenBraceToken.IsMissing => x.OpenBraceToken.SpanStart,
                BaseMethodDeclarationSyntax x => ((SyntaxNode)x.Body ?? x.ExpressionBody)?.SpanStart,
                LocalFunctionStatementSyntax x => ((SyntaxNode)x.Body ?? x.ExpressionBody)?.SpanStart,
                PropertyDeclarationSyntax x => ((SyntaxNode)x.AccessorList ?? x.ExpressionBody)?.SpanStart,
                IndexerDeclarationSyntax x => ((SyntaxNode)x.AccessorList ?? x.ExpressionBody)?.SpanStart,
                EventDeclarationSyntax x => x.AccessorList?.SpanStart,
                _ => null
            } ?? node.Span.End;
            var span = part == "declaration"
                ? Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(node.SpanStart, bodyStart)
                : Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(bodyStart, node.Span.End);
            var (start, end) = hit.File.Canonical.SliceOf(span);
            return new CodeSpan(hit.File.Canonical, start, end);
        }

        private static CodeHit WholeFile(CodeFile file) => new() { File = file, Node = file.Syntax };

        private IReadOnlyList<string> FilesIn(IReadOnlyList<Glob> scope)
        {
            var key = string.Join("|", scope.Select(x => x.Pattern));
            if (!_scopes.TryGetValue(key, out var files))
            {
                _scopes[key] = files = _code.Matching(scope.ToList());
            }

            return files;
        }

        private Regex Compile(string pattern)
        {
            if (!_regexes.TryGetValue(pattern, out var regex))
            {
                _regexes[pattern] = regex = new Regex(pattern, RegexOptions.CultureInvariant, MatchTimeout);
            }

            return regex;
        }

        private static IReadOnlyList<CodeHit> Distinct(IEnumerable<CodeHit> hits)
        {
            var seen = new HashSet<(CodeFile, SyntaxNode)>();
            return hits.Where(x => seen.Add((x.File, x.Node))).ToList();
        }

        private static string RenderWithCapture(string template, Match capture, IReadOnlyDictionary<string, string> values)
        {
            var rendered = Placeholders.Render(template, values, x => x.Replace("$", "$$"));
            return capture != null ? capture.Result(rendered) : rendered.Replace("$$", "$");
        }

        private static string Describe(IReadOnlyList<FindPath> paths, IReadOnlyDictionary<string, string> placeholders)
        {
            return string.Join(" or ", paths.Select(x => $"find `{Placeholders.Render(x.Text, placeholders)}`"));
        }

        private sealed class CodeSpan
        {
            private readonly CodeText _text;
            private readonly int _start;
            private string _value;

            public CodeSpan(CodeText text, int start, int end)
            {
                _text = text;
                _start = start;
                Length = Math.Max(0, end - start);
            }

            public int Length { get; }

            /// <summary>Where the span starts in the file's <see cref="CodeFile.Canonical"/> text.</summary>
            public int Start => _start;

            public string Text => _value ??= _text.Text.Substring(_start, Length);

            public int LineAt(int offset) => _text.LineAt(_start + offset);

            public bool TryGetTokens(int offset, int length, out SyntaxToken first, out SyntaxToken last)
            {
                return _text.TryGetTokens(_start + offset, length, out first, out last);
            }
        }
    }
}
