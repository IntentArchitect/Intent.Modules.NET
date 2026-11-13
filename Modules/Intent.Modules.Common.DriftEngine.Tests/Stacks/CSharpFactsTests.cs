using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// The declaration facts the C# language gives the graph that no claim kind reads yet, so no acceptance case covers them:
/// what house rules (and the Java language, filling the same nodes) are written against.
/// </summary>
public class CSharpFactsTests
{
    private const string Source = """
        namespace Shop;

        public interface IOrders { string Name { get; } }

        public class Paged<T> { }

        class Internal
        {
            public int Count { get; set; }
            int _hidden;
            protected string? Label;
            private readonly int _limit = 10;
            public const int Max = 5;

            public override string ToString() => "";

            public void Get(string name, int pageSize = 10) { }

            class Nested { }
        }

        public class OrderService(IMediator mediator)
        {
            public void Cancel() => mediator.Send(new CancelOrder());
        }

        public record OrderDto(string Name, [property: JsonPropertyName("n")] int Count);

        public enum Status { Draft = 1, Paid }

        [Index(nameof(Code), new[] { "a", "b" }, IsUnique = true)]
        public class Product { public string Code { get; set; } = ""; }
        """;

    private static readonly CodeGraph Graph = Read(Source);

    [Fact]
    public void A_property_or_field_has_its_access_as_written_or_the_language_default()
    {
        var type = Type("Internal");
        Member(type, "Count").Access.ShouldBe("public");
        Member(type, "_hidden").Access.ShouldBe("private");
        Member(type, "Label").Access.ShouldBe("protected");
        Member(Type("IOrders"), "Name").Access.ShouldBe("public");
    }

    [Fact]
    public void A_type_has_its_access_and_its_own_type_parameters()
    {
        Type("Paged").Access.ShouldBe("public");
        Type("Paged").TypeParameters.ShouldBe(["T"]);
        Type("Internal").Access.ShouldBe("internal");
        Type("Nested").Access.ShouldBe("private");
    }

    [Fact]
    public void A_class_primary_constructor_is_a_constructor_not_properties()
    {
        var type = Type("OrderService");
        type.Members.ShouldNotContain(x => x.Kind == "property");
        var constructor = type.Members.Single(x => x.Kind == "constructor");
        constructor.IsPrimary.ShouldBeTrue();
        constructor.Parameters.Select(x => x.Name).ShouldBe(["mediator"]);

        // Its parameters still type what the body calls on them.
        Member(type, "Cancel").Calls.Single().ReceiverType!.Name.ShouldBe("IMediator");
    }

    [Fact]
    public void A_positional_record_has_properties_with_their_attributes_and_a_primary_constructor()
    {
        var type = Type("OrderDto");
        type.Members.Where(x => x.Kind == "property").Select(x => x.Name).ShouldBe(["Name", "Count"]);
        Member(type, "Count").Attributes.Single().Name.ShouldBe("JsonPropertyName");
        type.Members.Single(x => x.Kind == "constructor").IsPrimary.ShouldBeTrue();
    }

    [Fact]
    public void A_member_has_its_other_modifiers_and_its_written_value()
    {
        var type = Type("Internal");
        Member(type, "ToString").Modifiers.ShouldBe(["override"]);
        Member(type, "_limit").Modifiers.ShouldBe(["readonly"]);
        Member(type, "_limit").Value!.Text.ShouldBe("10");
        Member(type, "Max").Modifiers.ShouldBe(["const"]);
        Member(type, "Count").Value.ShouldBeNull();
    }

    [Fact]
    public void An_enum_member_has_its_value_only_when_written()
    {
        var type = Type("Status");
        Member(type, "Draft").Value!.Text.ShouldBe("1");
        Member(type, "Paid").Value.ShouldBeNull();
    }

    [Fact]
    public void A_parameter_has_its_default_as_written()
    {
        var get = Member(Type("Internal"), "Get");
        get.Parameters.Select(x => x.Default).ShouldBe([null, "10"]);
    }

    [Fact]
    public void An_attribute_has_its_arguments_structured()
    {
        var values = Type("Product").Attributes.Single().Values;
        values.Select(x => x.Name).ShouldBe([null, null, "IsUnique"]);
        values[1].Items.Select(x => x.Literal).ShouldBe(["a", "b"]);
        values[2].Literal.ShouldBe("true");
        values[0].Literal.ShouldBeNull();
    }

    private static TypeNode Type(string name) => Graph.Types.Single(x => x.Name == name);

    private static MemberNode Member(TypeNode type, string name) => type.Members.Single(x => x.Name == name);

    private static CodeGraph Read(string source)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-csharp-").FullName;
        try
        {
            File.WriteAllText(Path.Combine(root, "Shop.cs"), source);
            var graph = new CodeGraph();
            new CSharpExtension().Languages.Single().Read(root, ["Shop.cs"], graph);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
