using System.Text.Json.Nodes;
using Intent.Metadata.Models;
using NSubstitute;

namespace Intent.Modules.Common.DriftEngine.Tests.Harness;

/// <summary>
/// Turns a case's <c>model</c> block (fixture-model.schema.json) into Intent's model objects, so the engine reads the
/// same interfaces the Software Factory gives it (decision 2). Only the tests carry this translation.
/// </summary>
internal sealed class FixtureModel
{
    private readonly Dictionary<string, IElement> _elements = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IElement> _typeDefinitions = new(StringComparer.Ordinal);
    private readonly List<(ITypeReference Reference, JsonNode Data)> _typeRefs = [];

    public static IReadOnlyDictionary<string, IReadOnlyList<IPackage>> Build(JsonNode model)
    {
        var fixture = new FixtureModel();
        var designers = new Dictionary<string, IReadOnlyList<IPackage>>(StringComparer.Ordinal);
        foreach (var (name, designer) in model["designers"]?.AsObject() ?? [])
        {
            designers[name] = designer.Items("packages").Select(fixture.Package).ToList();
        }

        // Type references point at elements anywhere in the model, so they resolve once every element exists.
        foreach (var (reference, data) in fixture._typeRefs.ToList())
        {
            var target = data.Text("element") is { } id ? fixture._elements.GetValueOrDefault(id) : fixture.TypeDefinition(data.Text("type")!);
            reference.Element.Returns(target);
        }

        return designers;
    }

    private IPackage Package(JsonNode data)
    {
        // A substitute is never created inside another's Returns(), so every value is built first.
        var package = Substitute.For<IPackage>();
        var stereotypes = Stereotypes(data);
        var elements = data.Items("elements").Select(x => Element(x, null, package)).ToList();
        package.Id.Returns(data.Text("id"));
        package.Name.Returns(data.Text("name"));
        package.SpecializationType.Returns(data.Text("type") ?? "Package");
        package.Stereotypes.Returns(stereotypes);
        package.ChildElements.Returns(elements);
        return package;
    }

    private IElement Element(JsonNode data, IElement? parent, IPackage package)
    {
        var element = Substitute.For<IElement>();
        var id = data.Text("id")!;
        _elements[id] = element;
        var stereotypes = Stereotypes(data);
        var typeReference = data["typeRef"] is { } typeRef ? TypeReference(typeRef) : null;
        var children = data.Items("children").Select(x => Element(x, element, package)).ToList();
        element.Id.Returns(id);
        element.Name.Returns(data.Text("name"));
        element.SpecializationType.Returns(data.Text("type"));
        element.Comment.Returns(data.Text("comment"));
        element.Value.Returns(data["value"]?.ToString());
        element.IsAbstract.Returns(data["abstract"]?.GetValue<bool>() ?? false);
        element.IsStatic.Returns(data["static"]?.GetValue<bool>() ?? false);
        element.ParentElement.Returns(parent);
        element.Package.Returns(package);
        element.Stereotypes.Returns(stereotypes);
        element.TypeReference.Returns(typeReference);
        element.ChildElements.Returns(children);
        return element;
    }

    private ITypeReference TypeReference(JsonNode data)
    {
        var reference = Substitute.For<ITypeReference>();
        var arguments = data.Items("arguments").Select(TypeReference).ToList();
        reference.IsNullable.Returns(data["nullable"]?.GetValue<bool>() ?? false);
        reference.IsCollection.Returns(data["collection"]?.GetValue<bool>() ?? false);
        reference.GenericTypeParameters.Returns(arguments);
        _typeRefs.Add((reference, data));
        return reference;
    }

    /// <summary>An Intent primitive (<c>guid</c>, <c>string</c>) is a Type-Definition element of that name.</summary>
    private IElement TypeDefinition(string name)
    {
        if (!_typeDefinitions.TryGetValue(name, out var definition))
        {
            definition = Substitute.For<IElement>();
            definition.Id.Returns($"type-definition:{name}");
            definition.Name.Returns(name);
            definition.SpecializationType.Returns("Type-Definition");
            definition.ChildElements.Returns([]);
            _typeDefinitions[name] = definition;
        }

        return definition;
    }

    /// <summary>
    /// Each stereotype property as Intent hands it over: an element reference <c>{ "ref": id }</c> as the element's id, as
    /// Intent stores a lookup; an item list (an array of objects) as one stereotype per row; a list of texts as one per line.
    /// </summary>
    private static IReadOnlyList<IStereotype> Stereotypes(JsonNode data) =>
        (data["stereotypes"]?.AsObject() ?? []).Select(x => Stereotype(x.Key, x.Value?.AsObject() ?? [])).ToList();

    private static IStereotype Stereotype(string name, JsonObject properties)
    {
        var values = properties.Select(x => Property(x.Key, x.Value)).ToList();
        var stereotype = Substitute.For<IStereotype>();
        stereotype.Name.Returns(name);
        stereotype.Properties.Returns(values);
        return stereotype;
    }

    private static IStereotypeProperty Property(string key, JsonNode? value)
    {
        if (value is JsonArray rows && rows.All(x => x is JsonObject))
        {
            var items = rows.Select(x => Stereotype(key, x!.AsObject())).ToArray();
            var list = Substitute.For<IStereotypeProperty<IStereotype[]>>();
            list.Key.Returns(key);
            list.Value.Returns(items);
            return list;
        }

        var property = Substitute.For<IStereotypeProperty>();
        property.Key.Returns(key);
        property.Value.Returns(value switch
        {
            null => null,
            JsonObject reference when reference.Text("ref") is { } id => id,
            JsonArray lines => string.Join('\n', lines.Select(x => x!.ToString())),
            JsonValue scalar => scalar.ToString(),
            _ => value.ToJsonString()
        });
        return property;
    }
}
