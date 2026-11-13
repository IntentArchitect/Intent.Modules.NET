namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>
/// Where the suite's data and the spec are, in the build output. <c>DRIFTENGINE_ACCEPTANCE</c> points the data at
/// another folder, such as the project's own <c>Acceptance</c> folder while cases are written, without a rebuild.
/// </summary>
internal static class Paths
{
    public static string Acceptance { get; } = Environment.GetEnvironmentVariable("DRIFTENGINE_ACCEPTANCE") is { Length: > 0 } folder
        ? Path.GetFullPath(folder)
        : Path.Combine(AppContext.BaseDirectory, "Acceptance");

    public static string Cases { get; } = Path.Combine(Acceptance, "cases");

    public static string Docs { get; } = Path.Combine(AppContext.BaseDirectory, "Docs");

    public static string Spec { get; } = Path.Combine(Docs, "spec");

    public static string Schemas { get; } = Path.Combine(Spec, "schemas");

    public static string CheckClassification { get; } = Path.Combine(Docs, "check-classification.md");

    public static string Requirements { get; } = Path.Combine(Spec, "requirements.md");

    public static string Conversion { get; } = Path.Combine(Spec, "conversion.md");

    public static string ClaimCatalog { get; } = Path.Combine(Spec, "claim-catalog.md");
}
