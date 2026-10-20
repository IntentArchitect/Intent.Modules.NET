using System.Text.RegularExpressions;

namespace Intent.Modules.AspNetCore.Templates.Program;

internal enum ExistingProgramRunCallKind
{
    /// <summary>There is no existing <c>Program.cs</c>.</summary>
    None,

    /// <summary>
    /// The file has exactly one run call in a form the template can re-emit verbatim, so Merge mode
    /// doesn't add a second one alongside it.
    /// </summary>
    Recognised,

    /// <summary>
    /// No run call, several of them (e.g. one per <c>if</c> branch), a different receiver variable, or
    /// a form such as <c>StartAsync</c> + <c>WaitForShutdownAsync</c>. The default run call is used.
    /// </summary>
    Unrecognised
}

/// <summary>
/// The host run call of an existing ASP.NET Core <c>Program.cs</c>, so that a developer's change from
/// <c>app.Run();</c> to e.g. <c>await app.RunAsync();</c> is kept rather than the default being re-added.
/// </summary>
/// <remarks>
/// A web-specific counterpart of the Worker template's <c>ExistingProgramShape</c> in
/// Intent.Modules.VisualStudio.Projects, which is internal to that module.
/// </remarks>
internal sealed record ExistingProgramRunCall(
    ExistingProgramRunCallKind Kind,
    string? Statement,
    bool IsAwaited,
    bool DeclaresAsyncMain)
{
    public const string MinimalHostingReceiver = "app";
    public const string GenericHostingReceiver = "CreateHostBuilder(args).Build()";

    public static readonly ExistingProgramRunCall None = new(ExistingProgramRunCallKind.None, null, false, false);

    private static readonly Regex CommentsAndStrings = new(
        @"(?<string>""(?:\\.|[^""\\])*"")|//[^\r\n]*|/\*[\s\S]*?\*/",
        RegexOptions.Compiled);

    private static readonly Regex AsyncTaskMain = new(
        @"\basync\s+Task\s+Main\s*\(",
        RegexOptions.Compiled);

    /// <summary>
    /// Whether a <c>Main</c> method must be generated as <c>async Task</c>: the run call is awaited, or the
    /// developer has already declared it that way (e.g. for their own <c>await</c>s). Generating <c>void</c>
    /// for the latter would have Merge mode keep <c>async</c> and produce <c>async void Main</c> (CS4009).
    /// </summary>
    public bool UsesAsyncMain => IsAwaited || DeclaresAsyncMain;

    private static readonly Regex MinimalHostingRun = CreateRunRegex(@"app");

    private static readonly Regex GenericHostingRun = CreateRunRegex(@"CreateHostBuilder\s*\(\s*args\s*\)\s*\.\s*Build\s*\(\s*\)");

    public static string DefaultStatement(bool usesMinimalHostingModel) =>
        $"{(usesMinimalHostingModel ? MinimalHostingReceiver : GenericHostingReceiver)}.Run();";

    public string GetStatementOrDefault(bool usesMinimalHostingModel) =>
        Kind == ExistingProgramRunCallKind.Recognised ? Statement! : DefaultStatement(usesMinimalHostingModel);

    public static ExistingProgramRunCall Detect(string? content, bool usesMinimalHostingModel)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return None;
        }

        var code = CommentsAndStrings.Replace(content.TrimStart('\uFEFF'), m => m.Groups["string"].Success ? m.Value : string.Empty);

        var declaresAsyncMain = AsyncTaskMain.IsMatch(code);
        var unrecognised = new ExistingProgramRunCall(ExistingProgramRunCallKind.Unrecognised, null, false, declaresAsyncMain);

        var matches = (usesMinimalHostingModel ? MinimalHostingRun : GenericHostingRun).Matches(code);
        if (matches.Count != 1)
        {
            return unrecognised;
        }

        var match = matches[0];
        var isAwait = match.Groups["await"].Success;
        var isRunAsync = match.Groups["method"].Value == "RunAsync";
        var isBlocking = match.Groups["block"].Success;
        var isConfigureAwait = match.Groups["configureAwait"].Success;

        var isValid = isRunAsync
            // await app.RunAsync()[.ConfigureAwait(..)];  or  app.RunAsync().GetAwaiter().GetResult();  /  .Wait();
            ? isAwait ? !isBlocking : isBlocking && !isConfigureAwait
            // app.Run(...);
            : !isAwait && !isBlocking && !isConfigureAwait;

        return isValid
            ? new ExistingProgramRunCall(ExistingProgramRunCallKind.Recognised, match.Groups["statement"].Value.Trim(), isAwait, declaresAsyncMain)
            : unrecognised;
    }

    // Arguments can't contain "=>" so that terminal middleware, e.g. app.Run(context => ...), isn't
    // mistaken for the host run call.
    private static Regex CreateRunRegex(string receiverPattern) => new(
        @"^[ \t]*(?<statement>(?<await>await\s+)?" + receiverPattern + @"\s*\.\s*(?<method>RunAsync|Run)\s*\((?:(?!=>)[^;])*?\)" +
        @"(?<configureAwait>\s*\.\s*ConfigureAwait\s*\(\s*(?:true|false)\s*\))?" +
        @"(?<block>\s*\.\s*GetAwaiter\s*\(\s*\)\s*\.\s*GetResult\s*\(\s*\)|\s*\.\s*Wait\s*\(\s*\))?" +
        @"\s*;)",
        RegexOptions.Compiled | RegexOptions.Multiline);
}
