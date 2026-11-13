using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.HarnessTests;

/// <summary>The comparison works before any engine exists: synthetic run results against expect blocks.</summary>
public class ExpectComparerTests
{
    private const string Result = """
        {
          "claims": [
            { "kind": "table.name", "designer": "Domain", "element": "order", "path": ["Order"], "expected": "Orders", "outcome": "contradicted",
              "reading": { "from": "code", "value": "Order", "at": { "file": "src/OrderConfiguration.cs", "line": 5 } } },
            { "kind": "type.exists", "designer": "Domain", "element": "order", "path": ["Order"], "expected": true, "outcome": "verified" },
            { "kind": "type.abstract", "designer": "Domain", "element": "order", "path": ["Order"], "expected": false, "outcome": "verified",
              "reading": { "from": "default", "value": false, "at": { "file": "src/Order.cs", "line": 1 } } },
            { "kind": "member.type", "designer": "Domain", "element": "order-total", "path": ["Order", "Total"], "expected": "decimal", "outcome": "cantVerify", "reason": "blocked",
              "dependsOn": { "kind": "member.exists", "element": "order-total" } }
          ],
          "findings": [
            { "rule": "claim/table.name", "area": "Domain (DriftEngine)", "category": "ModelCode", "severity": "error", "message": "Order's table name is Orders in the model, Order in the code",
              "subject": { "element": "order" }, "against": [ { "file": "src/OrderConfiguration.cs", "line": 5 } ] }
          ],
          "coverage": { "rows": [], "builtin": {}, "unmatched": {} },
          "packs": [],
          "files": [ { "path": "src/Order.cs", "reader": "csharp" }, { "path": "src/OrderConfiguration.cs", "reader": "csharp" } ]
        }
        """;

    [Fact]
    public void A_matching_expectation_has_no_differences()
    {
        var expect = JsonText.Parse("""
            {
              "claims": [
                { "kind": "table.name", "element": "order", "outcome": "contradicted", "expected": "Orders", "actual": "Order", "at": "src/OrderConfiguration.cs:5" },
                { "kind": "type.abstract", "element": "order", "outcome": "verified", "fromSilence": true },
                { "kind": "member.type", "element": "order-total", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
              ],
              "findings": [ { "rule": "claim/table.name", "severity": "error", "subject": "order", "messageContains": "Orders" } ],
              "noOtherFindings": true,
              "filesRead": { "include": ["src/**/*.cs"], "exclude": ["legacy/**"] }
            }
            """);

        ExpectComparer.Compare(expect, JsonText.Parse(Result)).ShouldBeEmpty();
    }

    [Fact]
    public void Every_difference_is_reported_not_only_the_first()
    {
        var expect = JsonText.Parse("""
            {
              "claims": [
                { "kind": "table.name", "element": "order", "outcome": "verified", "at": "src/OrderConfiguration.cs:6" },
                { "kind": "column.name", "element": "order", "outcome": "verified" }
              ],
              "others": "verified",
              "findings": [ { "rule": "claim/table.name", "severity": "warning" } ],
              "filesRead": { "exclude": ["src/Order.cs"] }
            }
            """);

        var differences = ExpectComparer.Compare(expect, JsonText.Parse(Result));

        differences.ShouldContain(x => x.Contains("claim table.name on 'order': outcome expected \"verified\", was \"contradicted\""));
        differences.ShouldContain(x => x.Contains("claim table.name on 'order': location expected src/OrderConfiguration.cs:6, was src/OrderConfiguration.cs:5"));
        differences.ShouldContain(x => x.Contains("claim column.name on 'order': the run produced no such claim"));
        differences.ShouldContain(x => x.Contains("claim member.type on 'order-total': every claim not listed must be verified"));
        differences.ShouldContain(x => x.Contains("finding claim/table.name: expected 1, the run reported 0"));
        differences.ShouldContain(x => x.Contains("'src/Order.cs' was read"));
        differences.Count.ShouldBe(6);
    }

    [Fact]
    public void An_absent_claim_must_not_be_produced()
    {
        var expect = JsonText.Parse("""
            { "claims": [ { "kind": "table.schema", "element": "order", "absent": true }, { "kind": "table.name", "element": "order", "absent": true } ] }
            """);

        ExpectComparer.Compare(expect, JsonText.Parse(Result)).ShouldHaveSingleItem().ShouldBe("claim table.name on 'order': must not exist, but the run produced 1, contradicted");
    }

    [Fact]
    public void Others_absent_allows_no_unlisted_claim()
    {
        var expect = JsonText.Parse("""{ "claims": [ { "kind": "table.name", "element": "order", "outcome": "contradicted" } ], "others": "absent" }""");

        ExpectComparer.Compare(expect, JsonText.Parse(Result)).Count.ShouldBe(3);
    }

    [Fact]
    public void An_unexpected_finding_fails_only_when_no_others_are_allowed()
    {
        var lenient = JsonText.Parse("""{ "findings": [] }""");
        var strict = JsonText.Parse("""{ "findings": [], "noOtherFindings": true }""");

        ExpectComparer.Compare(lenient, JsonText.Parse(Result)).ShouldBeEmpty();
        ExpectComparer.Compare(strict, JsonText.Parse(Result)).ShouldHaveSingleItem().ShouldStartWith("finding not expected: claim/table.name error on 'order'");
    }

    [Fact]
    public void A_finding_subject_can_be_a_location()
    {
        var result = JsonText.Parse(Result);
        result["findings"]![0]!["subject"] = JsonText.Parse("""{ "file": "src/OrderConfiguration.cs", "line": 5 }""");
        var expect = JsonText.Parse("""{ "findings": [ { "rule": "claim/table.name", "subject": "src/OrderConfiguration.cs:5" } ] }""");

        ExpectComparer.Compare(expect, result).ShouldBeEmpty();
    }
}
