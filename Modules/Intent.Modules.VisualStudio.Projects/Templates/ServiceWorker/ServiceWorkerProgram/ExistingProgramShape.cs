#nullable enable
using System.Text.RegularExpressions;

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceWorker.ServiceWorkerProgram;

internal enum ExistingProgramShapeKind
{
    /// <summary>There is no existing <c>Program.cs</c>.</summary>
    None,

    /// <summary>
    /// The builder, <c>Build()</c> and run statements are each in a form the template generates, so it
    /// can re-emit them and Merge mode adds nothing that duplicates the developer's code.
    /// </summary>
    Recognised,

    /// <summary>
    /// Any other shape, e.g. <c>Host.CreateDefaultBuilder(args).ConfigureServices(...)</c> or other
    /// variable names. Merging into it would duplicate statements, so it is left untouched.
    /// </summary>
    Unrecognised
}

/// <summary>
/// The parts of an existing Worker <c>Program.cs</c> that the template adapts to: its builder
/// declaration and its run call. Statement style (top-level or <c>Main</c>) is not detected: it
/// comes from the modelled <c>Use top-level statements</c> setting.
/// </summary>
internal sealed record ExistingProgramShape(
    ExistingProgramShapeKind Kind,
    string? BuilderStatement,
    bool UsesWebApplicationBuilder,
    bool RunsAsync)
{
    public const string DefaultBuilderStatement = "HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);";
    public const string BuildStatement = "var app = builder.Build();";
    public const string RunStatement = "app.Run();";
    public const string RunAsyncStatement = "await app.RunAsync();";

    public static readonly ExistingProgramShape None = new(ExistingProgramShapeKind.None, null, false, false);

    private static readonly ExistingProgramShape Unrecognised = new(ExistingProgramShapeKind.Unrecognised, null, false, false);

    private static readonly Regex CommentsAndStrings = new(
        @"(?<string>""(?:\\.|[^""\\])*"")|//[^\r\n]*|/\*[\s\S]*?\*/",
        RegexOptions.Compiled);

    private static readonly Regex Builder = new(
        @"^[ \t]*(?<statement>(?:var|WebApplicationBuilder|HostApplicationBuilder)\s+builder\s*=\s*(?<factory>WebApplication\s*\.\s*CreateBuilder|Host\s*\.\s*CreateApplicationBuilder)\s*\(\s*args\s*\)\s*;)",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex Build = new(
        @"^[ \t]*var\s+app\s*=\s*builder\s*\.\s*Build\s*\(\s*\)\s*;",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex Run = new(
        @"^[ \t]*(?:(?<async>await\s+app\s*\.\s*RunAsync\s*\(\s*\))|app\s*\.\s*Run\s*\(\s*\))\s*;",
        RegexOptions.Compiled | RegexOptions.Multiline);

    public string RunCallStatement => RunsAsync ? RunAsyncStatement : RunStatement;

    public static ExistingProgramShape Detect(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return None;
        }

        var code = CommentsAndStrings.Replace(content.TrimStart('\uFEFF'), m => m.Groups["string"].Success ? m.Value : string.Empty);

        var builders = Builder.Matches(code);
        var runs = Run.Matches(code);
        if (builders.Count != 1 || Build.Matches(code).Count != 1 || runs.Count != 1)
        {
            return Unrecognised;
        }

        var builder = builders[0];
        return new ExistingProgramShape(
            Kind: ExistingProgramShapeKind.Recognised,
            BuilderStatement: builder.Groups["statement"].Value.Trim(),
            UsesWebApplicationBuilder: builder.Groups["factory"].Value.StartsWith("WebApplication"),
            RunsAsync: runs[0].Groups["async"].Success);
    }
}
