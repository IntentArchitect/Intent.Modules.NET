using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Integrity;

/// <summary>Integrity check 1: every schema is a valid draft 2020-12 JSON Schema, and the set resolves.</summary>
public class SchemaTests
{
    public static TheoryData<string> SchemaFiles => new(SchemaSet.Instance.Documents.Keys);

    [Theory]
    [MemberData(nameof(SchemaFiles))]
    public void Schema_is_valid_against_draft_2020_12(string schemaFile)
    {
        SchemaSet.Instance.MetaValidate(schemaFile).ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(SchemaFiles))]
    public void Schema_id_is_its_file_name_under_the_base_uri(string schemaFile)
    {
        SchemaSet.Instance.Documents[schemaFile].Text("$id").ShouldBe(SchemaSet.BaseUri + schemaFile);
    }

    [Theory]
    [MemberData(nameof(SchemaFiles))]
    public void Every_reference_resolves_within_the_set(string schemaFile)
    {
        SchemaSet.Instance.UnresolvedReferences(schemaFile).ShouldBeEmpty();
    }

    [Fact]
    public void The_spec_documents_name_every_schema()
    {
        var specText = string.Concat(Directory.GetFiles(Paths.Spec, "*.md").Select(File.ReadAllText));
        var unnamed = SchemaSet.Instance.Documents.Keys.Where(x => !specText.Contains(x, StringComparison.Ordinal)).ToList();
        unnamed.ShouldBeEmpty("Every schema is described by a spec document; these are named by none.");
    }

    [Theory]
    [InlineData("checks.json", "checks.schema.json")]
    [InlineData("catalog.json", "catalog.schema.json")]
    [InlineData("conversion-map.json", "conversion-map.schema.json")]
    public void Acceptance_data_file_is_valid_against_its_schema(string dataFile, string schemaFile)
    {
        SchemaSet.Instance.Validate(schemaFile, JsonText.Load(Path.Combine(Paths.Acceptance, dataFile))).ShouldBeEmpty();
    }
}
