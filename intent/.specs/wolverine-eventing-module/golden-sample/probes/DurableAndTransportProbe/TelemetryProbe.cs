namespace WolverineEventing.Probes;

// Gate probe closing ledger row a9 - the OpenTelemetry ActivitySource name, which all of R11
// rests on. Verified at WolverineFx 5.39.5 by reflecting over
// Wolverine.Runtime.WolverineTracing.ActivitySource: its Name is exactly "Wolverine".
//
// This file is the artifact. It compiles only while that member remains public and accessible, so
// a future WolverineFx upgrade that moves or renames it breaks the probe build rather than
// silently invalidating the generated telemetry registration.
public static class TelemetryProbe
{
    // What a template must emit: builder.AddSource("Wolverine").
    public const string ActivitySourceName = "Wolverine";

    public static string ActualActivitySourceName
        => Wolverine.Runtime.WolverineTracing.ActivitySource.Name;
}
