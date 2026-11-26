using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>
/// Checks a run result (run-result.schema.json) against a case's <c>expect</c> block (expect.schema.json), and
/// reports every difference, not only the first.
/// </summary>
internal static class ExpectComparer
{
    private static readonly Regex Location = new(@"^(?<path>.+):(?<line>\d+)$", RegexOptions.CultureInvariant);

    public static IReadOnlyList<string> Compare(JsonNode expect, JsonNode result)
    {
        var differences = new List<string>();
        Claims(expect, result, differences);
        Findings(expect.Items("findings").Concat(expect.Items("config")).ToList(), expect["noOtherFindings"]?.GetValue<bool>() == true, result, differences);
        FilesRead(expect["filesRead"], result, differences);
        Nodes(expect.Items("nodes").ToList(), result["graph"], differences);
        return differences;
    }

    private static void Claims(JsonNode expect, JsonNode result, List<string> differences)
    {
        var actual = result.Items("claims").ToList();
        var matched = new HashSet<JsonNode>(ReferenceEqualityComparer.Instance);
        foreach (var expected in expect.Items("claims"))
        {
            var kind = expected.Text("kind");
            var element = expected.Text("element");
            var key = expected.Text("key");
            var label = $"claim {kind} on '{element}'{(key is null ? "" : $" ({key})")}";
            var candidates = actual
                .Where(x => x.Text("kind") == kind && x.Text("element") == element)
                .Where(x => key is null || x.Text("discriminator") == key)
                .ToList();
            if (expected["absent"]?.GetValue<bool>() == true)
            {
                if (candidates.Count > 0)
                {
                    differences.Add($"{label}: must not exist, but the run produced {candidates.Count}, {string.Join(", ", candidates.Select(x => x.Text("outcome")))}");
                }

                continue;
            }

            // Without a key, the claim with no discriminator, where the element's others of the kind each have one: a
            // subscription's message.copy beside those of the types its message carries.
            if (key is null && candidates.Count > 1 && candidates.Where(x => x.Text("discriminator") is null).ToList() is [var undiscriminated])
            {
                candidates = [undiscriminated];
            }

            if (candidates.Count == 0)
            {
                differences.Add($"{label}: the run produced no such claim");
                continue;
            }

            if (candidates.Count > 1)
            {
                differences.Add($"{label}: the run produced {candidates.Count} such claims ({string.Join(", ", candidates.Select(x => x.Text("discriminator") ?? "no discriminator"))}); give the expected claim its key");
                continue;
            }

            var claim = candidates[0];
            matched.Add(claim);
            var reading = claim["reading"];
            Same(label, "outcome", expected["outcome"], claim["outcome"], differences);
            if (expected["reason"] is not null || claim.Text("outcome") != "cantVerify")
            {
                Same(label, "reason", expected["reason"], claim["reason"], differences);
            }

            if (expected.AsObject().ContainsKey("expected"))
            {
                Same(label, "expected value", expected["expected"], claim["expected"], differences);
            }

            if (expected.AsObject().ContainsKey("actual"))
            {
                Same(label, "value read", expected["actual"], reading?["value"], differences);
            }

            if (expected.Text("at") is { } at)
            {
                var actualAt = reading?["at"] is { } span ? $"{span.Text("file")}:{span["line"]}" : null;
                Same(label, "location", at, actualAt, differences);
            }

            if (expected["fromSilence"] is { } fromSilence)
            {
                var silent = reading.Text("from") is "default" or "absent";
                Same(label, "from silence", fromSilence.GetValue<bool>(), silent, differences);
            }

            if (expected.Text("scopeRow") is { } scopeRow)
            {
                Same(label, "excluding scope row", scopeRow, reading.Text("scopeRow"), differences);
            }

            if (expected.Text("blockedBy") is { } blockedBy)
            {
                Same(label, "blocking claim", blockedBy, claim["dependsOn"].Text("kind"), differences);
            }
        }

        var unmatched = actual.Where(x => !matched.Contains(x)).ToList();
        switch (expect.Text("others") ?? "ignore")
        {
            case "verified":
                foreach (var claim in unmatched.Where(x => x.Text("outcome") != "verified"))
                {
                    differences.Add($"claim {claim.Text("kind")} on '{claim.Text("element")}': every claim not listed must be verified, but it is {claim.Text("outcome")}{(claim.Text("reason") is { } reason ? $" ({reason})" : "")}");
                }

                break;
            case "absent":
                foreach (var claim in unmatched)
                {
                    differences.Add($"claim {claim.Text("kind")} on '{claim.Text("element")}': the case allows no claim it doesn't list");
                }

                break;
        }
    }

    private static void Findings(IReadOnlyList<JsonNode> expectedFindings, bool noOthers, JsonNode result, List<string> differences)
    {
        var actual = result.Items("findings").ToList();
        var matched = new HashSet<JsonNode>(ReferenceEqualityComparer.Instance);
        foreach (var expected in expectedFindings)
        {
            var rule = expected.Text("rule");
            var subject = expected.Text("subject");
            var label = $"finding {rule}{(subject is null ? "" : $" on '{subject}'")}";
            var candidates = actual
                .Where(x => x.Text("rule") == rule)
                .Where(x => subject is null || SubjectIs(x["subject"], subject))
                .Where(x => expected.Text("severity") is not { } severity || x.Text("severity") == severity)
                .Where(x => expected.Text("messageContains") is not { } text || (x.Text("message") ?? "").Contains(text, StringComparison.Ordinal))
                .ToList();
            var count = expected["count"]?.GetValue<int>() ?? 1;
            if (candidates.Count != count)
            {
                var near = actual.Where(x => x.Text("rule") == rule).Select(Describe).ToList();
                differences.Add($"{label}: expected {count}, the run reported {candidates.Count}{(near.Count > 0 ? $"; findings of that rule: {string.Join("; ", near)}" : "")}");
            }

            foreach (var candidate in candidates)
            {
                matched.Add(candidate);
            }
        }

        if (noOthers)
        {
            foreach (var finding in actual.Where(x => !matched.Contains(x)))
            {
                differences.Add($"finding not expected: {Describe(finding)}");
            }
        }
    }

    private static void FilesRead(JsonNode? filesRead, JsonNode result, List<string> differences)
    {
        if (filesRead is null)
        {
            return;
        }

        var read = result.Items("files").Select(x => x.Text("path")!).ToList();
        foreach (var pattern in filesRead.Texts("include"))
        {
            var glob = new Glob(pattern);
            if (!read.Any(glob.IsMatch))
            {
                differences.Add($"files read: nothing read matches '{pattern}'");
            }
        }

        foreach (var pattern in filesRead.Texts("exclude"))
        {
            var glob = new Glob(pattern);
            foreach (var path in read.Where(glob.IsMatch))
            {
                differences.Add($"files read: '{path}' was read, but matches excluded '{pattern}'");
            }
        }
    }

    private static void Nodes(IReadOnlyList<JsonNode> expectedNodes, JsonNode? graph, List<string> differences)
    {
        if (expectedNodes.Count == 0)
        {
            return;
        }

        if (graph is null)
        {
            differences.Add("nodes: the run result has no code graph");
            return;
        }

        var nodes = graph.Items("nodes").ToList();
        foreach (var expected in expectedNodes)
        {
            var kind = expected.Text("kind");
            var name = expected.Text("name");
            var label = $"node {kind}{(name is null ? "" : $" '{name}'")}";
            var candidates = nodes
                .Where(x => x.Text("kind") == kind && (name is null || x.Text("name") == name))
                .Where(x => expected.Text("at") is not { } at || $"{x.Text("file")}:{x["span"]?["line"]}" == at)
                .ToList();
            if (expected["absent"]?.GetValue<bool>() == true)
            {
                if (candidates.Count > 0)
                {
                    differences.Add($"{label}: must not exist, but the graph has {candidates.Count}");
                }

                continue;
            }

            if (candidates.Count != 1)
            {
                differences.Add($"{label}: expected one, the graph has {candidates.Count}");
                continue;
            }

            foreach (var field in new[] { "text", "declaration", "body" })
            {
                if (expected.Text(field) is { } value)
                {
                    Same(label, field, value, candidates[0].Text(field), differences);
                }
            }
        }
    }

    private static bool SubjectIs(JsonNode? subject, string expected)
    {
        // An element id never holds a colon (fixture-model.schema.json), so path:line is a location.
        var location = Location.Match(expected);
        if (location.Success)
        {
            return subject.Text("file") == location.Groups["path"].Value && subject?["line"]?.ToString() == location.Groups["line"].Value;
        }

        return subject.Text("element") == expected;
    }

    private static string Describe(JsonNode finding)
    {
        var subject = finding["subject"];
        var on = subject.Text("element") ?? (subject.Text("file") is { } file ? $"{file}:{subject?["line"]}" : subject.Text("text"));
        return $"{finding.Text("rule")} {finding.Text("severity")} on '{on}': {finding.Text("message")}";
    }

    private static void Same(string label, string what, JsonNode? expected, JsonNode? actual, List<string> differences)
    {
        if (!JsonNode.DeepEquals(expected, actual))
        {
            differences.Add($"{label}: {what} expected {Show(expected)}, was {Show(actual)}");
        }
    }

    private static void Same<T>(string label, string what, T expected, T actual, List<string> differences)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            differences.Add($"{label}: {what} expected {expected?.ToString() ?? "none"}, was {actual?.ToString() ?? "none"}");
        }
    }

    private static string Show(JsonNode? node) => node?.ToJsonString() ?? "none";
}
