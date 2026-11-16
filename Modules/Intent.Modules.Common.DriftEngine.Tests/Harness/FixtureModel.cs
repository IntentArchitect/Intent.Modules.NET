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
    private readonly Dictionary<string, IAssociationEnd> _targetEnds = new(StringComparer.Ordinal);

    // An element's "generalization" is a Generalization link from it to its base, built with the other links.
    private readonly List<JsonNode> _generalizations = [];
    private readonly List<(IAssociationEnd End, ITypeReference Reference, string Element, string From)> _ends = [];
    private readonly List<(IStereotypeProperty<IElement> Property, string Id)> _lookups = [];

    private readonly List<(IStereotypeProperty<IElement[]> Property, IReadOnlyList<string> Ids)> _multiLookups = [];

    public static IReadOnlyDictionary<string, IReadOnlyList<IPackage>> Build(JsonNode model) => Build(model, out _);

    /// <param name="references">
    /// Each designer's packages another application owns (the model's <c>references</c>), built with the rest so this
    /// application's links can point into them.
    /// </param>
    public static IReadOnlyDictionary<string, IReadOnlyList<IPackage>> Build(JsonNode model, out IReadOnlyDictionary<string, IReadOnlyList<IPackage>> references)
    {
        var fixture = new FixtureModel();
        var designers = new Dictionary<string, IReadOnlyList<IPackage>>(StringComparer.Ordinal);
        foreach (var (name, designer) in model["designers"]?.AsObject() ?? [])
        {
            designers[name] = designer.Items("packages").Select(fixture.Package).ToList();
        }

        var referenced = new Dictionary<string, IReadOnlyList<IPackage>>(StringComparer.Ordinal);
        foreach (var (name, designer) in model["references"]?.AsObject() ?? [])
        {
            referenced[name] = designer.Items("packages").Select(fixture.Package).ToList();
        }

        references = referenced;

        // Associations link elements anywhere in the model, so they're built once every element exists.
        var sections = (model["designers"]?.AsObject() ?? []).Concat(model["references"]?.AsObject() ?? []);
        foreach (var association in sections.SelectMany(x => x.Value!.Items("associations")).Concat(fixture._generalizations).ToList())
        {
            fixture.Association(association);
        }

        // Type references point at elements anywhere in the model, so they resolve once every element exists.
        foreach (var (reference, data) in fixture._typeRefs.ToList())
        {
            var target = data.Text("element") is { } id ? fixture._elements.GetValueOrDefault(id) : fixture.TypeDefinition(data.Text("type")!);
            reference.Element.Returns(target);
        }

        // An end is a navigation of the element at its other end, so it's among that element's associated elements. Its
        // element may be an association's id, standing for that association's target end.
        foreach (var (end, reference, element, from) in fixture._ends)
        {
            var target = fixture._elements.GetValueOrDefault(element) ?? fixture._targetEnds.GetValueOrDefault(element);
            var package = fixture._elements.GetValueOrDefault(from)?.Package;
            reference.Element.Returns(target);
            end.Package.Returns(package);
        }

        // A stereotype's lookup gives the element it names: an element, or an association's id for its target end.
        foreach (var (property, id) in fixture._lookups)
        {
            property.Value.Returns(fixture._elements.GetValueOrDefault(id) ?? fixture._targetEnds.GetValueOrDefault(id));
        }

        foreach (var (property, ids) in fixture._multiLookups)
        {
            property.Value.Returns(ids.Select(x => fixture._elements.GetValueOrDefault(x)).OfType<IElement>().ToArray());
        }

        // A target end is an element too, so an association may start from it, as a publish does from a handler's subscription.
        foreach (var (id, element) in fixture._elements.Select(x => (x.Key, x.Value)).Concat(fixture._targetEnds.Select(x => (x.Key, (IElement)x.Value))))
        {
            var associated = fixture._ends.Where(x => x.From == id).Select(x => x.End).ToList();
            element.AssociatedElements.Returns(associated);
        }

        return designers;
    }

    private void Association(JsonNode data)
    {
        // A substitute is never created inside another's Returns(), so every one is built first.
        var association = Substitute.For<IAssociation>();
        var source = Substitute.For<IAssociationEnd>();
        var target = Substitute.For<IAssociationEnd>();
        var id = data.Text("id")!;
        association.SpecializationType.Returns(data.Text("type"));
        association.SourceEnd.Returns(source);
        association.TargetEnd.Returns(target);
        // Intent gives a target end its association's id.
        End(source, $"{id}:source", data["source"]!, data["target"]!, association, target, false);
        End(target, id, data["target"]!, data["source"]!, association, source, true);
        _targetEnds[id] = target;
    }

    private void End(IAssociationEnd end, string id, JsonNode data, JsonNode other, IAssociation association, IAssociationEnd otherEnd, bool isTarget)
    {
        var stereotypes = Stereotypes(data);
        var reference = Substitute.For<ITypeReference>();
        var multiplicity = data.Text("multiplicity") ?? "1";
        reference.IsCollection.Returns(multiplicity is "*" or "1..*");
        reference.IsNullable.Returns(multiplicity is "*" or "0..1");
        reference.GenericTypeParameters.Returns([]);
        end.Id.Returns(id);
        end.Name.Returns(data.Text("name") ?? "");
        end.SpecializationType.Returns(isTarget ? "Association Target End" : "Association Source End");
        end.Stereotypes.Returns(stereotypes);
        end.TypeReference.Returns(reference);
        end.ChildElements.Returns([]);
        end.ParentElement.Returns((IElement?)null);
        end.Association.Returns(association);
        end.IsNavigable.Returns(data["navigable"]?.GetValue<bool>() ?? isTarget);
        end.OtherEnd().Returns(otherEnd);
        end.IsTargetEnd().Returns(isTarget);
        end.IsSourceEnd().Returns(!isTarget);
        _ends.Add((end, reference, data.Text("element")!, other.Text("element")!));
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
        if (data.Text("generalization") is { } generalization)
        {
            _generalizations.Add(new JsonObject
            {
                ["id"] = $"{id}:generalization",
                ["type"] = "Generalization",
                ["source"] = new JsonObject { ["element"] = id, ["navigable"] = false },
                ["target"] = new JsonObject { ["element"] = generalization }
            });
        }

        var stereotypes = Stereotypes(data);
        var typeReference = data["typeRef"] is { } typeRef ? TypeReference(typeRef) : null;
        var children = data.Items("children").Select(x => Element(x, element, package)).ToList();
        var genericTypes = data.Texts("genericTypes").Select(GenericType).ToList();
        element.GenericTypes.Returns(genericTypes);
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

    private static IGenericType GenericType(string name)
    {
        var type = Substitute.For<IGenericType>();
        type.Name.Returns(name);
        return type;
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
    /// Each stereotype property as Intent hands it over: an element reference <c>{ "ref": id }</c> as a lookup, its text the
    /// element's id, as Intent stores it, and its typed value the element; an item list (an array of objects) as one
    /// stereotype per row; a list of texts as one per line.
    /// </summary>
    private IReadOnlyList<IStereotype> Stereotypes(JsonNode data) =>
        (data["stereotypes"]?.AsObject() ?? []).Select(x => Stereotype(x.Key, x.Value?.AsObject() ?? [])).ToList();

    private IStereotype Stereotype(string name, JsonObject properties)
    {
        var values = properties.Select(x => Property(x.Key, x.Value)).ToList();
        var stereotype = Substitute.For<IStereotype>();
        stereotype.Name.Returns(name);
        stereotype.Properties.Returns(values);
        return stereotype;
    }

    private IStereotypeProperty Property(string key, JsonNode? value)
    {
        // Several element references (Api Version Settings' Applicable Versions), as Intent hands them over: the elements,
        // its text their ids.
        if (value is JsonArray refs && refs.Count > 0 && refs.All(x => x is JsonObject { Count: 1 } item && item.Text("ref") is not null))
        {
            var ids = refs.Select(x => x!.AsObject().Text("ref")!).ToList();
            var elements = Substitute.For<IStereotypeProperty<IElement[]>>();
            elements.Key.Returns(key);
            ((IStereotypeProperty)elements).Value.Returns(string.Join(",", ids));
            _multiLookups.Add((elements, ids));
            return elements;
        }

        if (value is JsonArray rows && rows.All(x => x is JsonObject))
        {
            var items = rows.Select(x => Stereotype(key, x!.AsObject())).ToArray();
            var list = Substitute.For<IStereotypeProperty<IStereotype[]>>();
            list.Key.Returns(key);
            list.Value.Returns(items);
            return list;
        }

        // A lookup resolves once every element exists, as a type reference does.
        if (value is JsonObject lookup && lookup.Text("ref") is { } referenced)
        {
            var element = Substitute.For<IStereotypeProperty<IElement>>();
            element.Key.Returns(key);
            ((IStereotypeProperty)element).Value.Returns(referenced);
            _lookups.Add((element, referenced));
            return element;
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
