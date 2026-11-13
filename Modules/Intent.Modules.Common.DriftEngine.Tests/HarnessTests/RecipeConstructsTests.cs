using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.HarnessTests;

public class RecipeConstructsTests
{
    [Fact]
    public void Keys_name_the_format_never_the_recipe_text()
    {
        var recipe = JsonText.Parse("""
            {
              // comments and trailing commas are allowed, as Common.Drift reads them
              "files": ["src/**/*.cs"],
              "facts": {
                "table.name": {
                  "find": ["class:{Entity}Configuration > statement:ToTable", "class with Secret\\w+ > method:Configure(EntityTypeBuilder)"],
                  "read": [
                    { "match": "(?<=ToTable\\(\")(?<name>[^\"]+)", "value": "${name}", "part": "body", "in": "class", "case": "upper" },
                    { "match": "(?i:totable)\\1", "unreadable": true }
                  ],
                  "inherit": ["class", "file:Shared/*.cs"],
                  "unstated": "{Entity|trimEnd:Entity|camel|lower}",
                },
              },
              "own": [ { "kind": "class", "find": "class with \\w+Handler", "name": "$2" } ],
              "exceptions": [ { "match": "Legacy/*.cs", "reason": "old" }, { "match": "Order.Secret" }, "Customer" ],
            }
            """);

        var keys = RecipeConstructs.Of(recipe).Keys;

        keys.ShouldBe(
            [
                "case.case.upper", "case.in.class", "case.match", "case.part.body", "case.unreadable", "case.value",
                "exception.file", "exception.member", "exception.noReason", "exception.object", "exception.string", "exception.typeOrName",
                "file.exceptions", "file.facts", "file.files", "file.own",
                "files.list",
                "filter.camel", "filter.chain.3", "filter.lower", "filter.trimEnd",
                "kind.table.name",
                "own.find", "own.kind", "own.name", "own.name.group2plus", "own.step.with.class",
                "recipe.find.list", "recipe.inherit.list", "recipe.inherit.value.class", "recipe.inherit.value.findPath", "recipe.read", "recipe.unstated",
                "regex.backreference", "regex.lookbehind", "regex.namedGroup", "regex.scopedIgnoreCase",
                "step.class", "step.file", "step.method.overload", "step.statement", "step.with.class"
            ],
            ignoreOrder: true);
        keys.ShouldAllBe(x => !x.Contains("Secret") && !x.Contains("Legacy") && !x.Contains("Customer"));
    }
}
