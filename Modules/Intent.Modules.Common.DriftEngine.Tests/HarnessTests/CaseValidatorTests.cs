using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.HarnessTests;

/// <summary>The integrity gate rejects what it should, not only accepts the suite's own cases.</summary>
public class CaseValidatorTests
{
    private const string Text = """
        ## broken

        ```json case
        { "phase": 1, "kinds": ["table.nmae"], "cases": ["scope/nope"], "traces": ["ESC-DOM-99"], "packs": ["house.shop"], "colour": "red" }
        ```

        ```json model
        {
          "designers": {
            "Domain": {
              "packages": [{ "id": "domain", "name": "Domain", "elements": [
                { "id": "order", "type": "Class", "name": "Order", "generalization": "entity" },
                { "id": "order", "type": "Class", "name": "Order2" }
              ] }]
            }
          }
        }
        ```

        ```csharp file src/Order.cs
        public class Order
        {
        }
        ```

        ```json expect
        {
          "claims": [
            { "kind": "type.exists", "element": "customer", "outcome": "verified", "at": "src/Order.cs:4" },
            { "kind": "table.name", "element": "order", "outcome": "verified" },
            { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "scopeRow": "root#1" }
          ],
          "findings": [ { "rule": "claim/type.exists", "subject": "src/Missing.cs:1" } ]
        }
        ```
        """;

    [Fact]
    public void Every_broken_reference_is_reported()
    {
        var errors = CaseValidator.Validate(CaseFileParser.Parse("structure/broken.cases.md", Text));

        errors.ShouldContain(x => x.Contains("against case.schema.json") && x.Contains("colour"));
        errors.ShouldContain(x => x.Contains("names kind 'table.nmae'"));
        errors.ShouldContain(x => x.Contains("names case 'scope/nope'"));
        errors.ShouldContain(x => x.Contains("traces 'ESC-DOM-99'"));
        errors.ShouldContain(x => x.Contains("uses pack 'house.shop'"));
        errors.ShouldContain(x => x.Contains("model id 'order' is used twice"));
        errors.ShouldContain(x => x.Contains("refers to 'entity', which isn't in the model"));
        errors.ShouldContain(x => x.Contains("expects a claim on 'customer'"));
        errors.ShouldContain(x => x.Contains("'src/Order.cs:4' is past the end of src/Order.cs, which has 3 lines"));
        errors.ShouldContain(x => x.Contains("is phase 1, but expects table.name (phase 4) to be verified"));
        errors.ShouldContain(x => x.Contains("against expect.schema.json") && x.Contains("reason"));
        errors.ShouldContain(x => x.Contains("scope row 'root#1' names host 'root'"));
        errors.ShouldContain(x => x.Contains("'src/Missing.cs:1' names no file block"));
    }
}
