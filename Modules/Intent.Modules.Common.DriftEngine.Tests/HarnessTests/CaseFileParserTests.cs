using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.HarnessTests;

public class CaseFileParserTests
{
    private const string Text = """
        # Table name

        ## verified

        The model and the code agree.

        ```json case
        { "phase": 4, "kinds": ["table.name"] }
        ```

        ```json model
        { "designers": {} }
        ```

        ```csharp file src/Order.cs
        public class Order
        {
        }
        ```

        ```csharp file src/OrderConfiguration.cs
        builder.ToTable("Orders");
        ```

        ```json expect
        { "claims": [] }
        ```

        ## contradicted

        Same model; the configuration names another table.

        ```json case
        { "phase": 4, "base": "verified" }
        ```

        ```csharp file src/OrderConfiguration.cs
        builder.ToTable("Order");
        ```

        ## without-configuration

        ```json case
        { "phase": 4, "base": "verified", "without": ["src/OrderConfiguration.cs"] }
        ```
        """;

    [Fact]
    public void A_case_inherits_its_base_and_replaces_blocks_of_the_same_role_and_path()
    {
        var file = CaseFileParser.Parse("persistence/table.name.cases.md", Text);

        file.Errors.ShouldBeEmpty();
        var contradicted = file.Cases.Single(x => x.Id == "contradicted");
        contradicted.FullId.ShouldBe("persistence/table.name#contradicted");
        contradicted.Files.Keys.Order().ShouldBe(["src/Order.cs", "src/OrderConfiguration.cs"]);
        contradicted.Files["src/OrderConfiguration.cs"].Content.ShouldBe("builder.ToTable(\"Order\");");
        contradicted.Model.ShouldNotBeNull();
        contradicted.Expect.ShouldNotBeNull();
        contradicted.Meta!["phase"]!.GetValue<int>().ShouldBe(4);
        contradicted.Meta["kinds"].ShouldBeNull("the case block is replaced, not merged");
    }

    [Fact]
    public void Without_drops_a_base_file()
    {
        var file = CaseFileParser.Parse("persistence/table.name.cases.md", Text);

        file.Cases.Single(x => x.Id == "without-configuration").Files.Keys.ShouldBe(["src/Order.cs"]);
    }

    [Fact]
    public void A_file_block_counts_lines_from_its_first_content_line()
    {
        var file = CaseFileParser.Parse("persistence/table.name.cases.md", Text);
        var order = file.Cases[0].Files["src/Order.cs"];

        order.Lines.Count.ShouldBe(3);
        order.Lines[0].ShouldBe("public class Order");
    }

    [Theory]
    [InlineData("## a\n\n```json\n{}\n```\n", "isn't '<language> <role> [<argument>]'")]
    [InlineData("## a\n\n```csharp file\nx\n```\n", "a file block names its path")]
    [InlineData("## a\n\n```json case\n{ \"phase\": 1, \"base\": \"b\" }\n```\n", "base 'b' isn't a case in the same file")]
    [InlineData("## Bad Id\n", "isn't kebab case")]
    [InlineData("## a\n\n```json case\n{\n", "the fence is never closed")]
    [InlineData("```json case\n{}\n```\n", "belongs to no case")]
    [InlineData("## a\n\n```json case\n{ \"phase\": 1 }\n```\n", "has no model block")]
    [InlineData("## a\n\n```json case\n{ \"phase\": 1, \"base\": \"a\" }\n```\n", "loops back")]
    public void A_malformed_file_reports_what_is_wrong(string text, string error)
    {
        CaseFileParser.Parse("area/name.cases.md", text).Errors.ShouldContain(x => x.Contains(error));
    }
}
