using System.Collections;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.DriftEngine.Vocabulary;

/// <summary>
/// The vocabulary's expression language (vocabulary.md, Expressions), evaluated against one element. This is the subset
/// the packs need: fields, member access, stereotypes, association ends (<c>associations('Association')</c>,
/// <c>.isSource</c>, <c>.multiplicity</c>), numbers, <c>??</c>, comparison and boolean operators.
/// </summary>
internal sealed class Expression
{
    private readonly string _text;
    private readonly IElement _self;

    /// <summary>The item of an entry's <c>each</c> list this claim is for, as <c>item</c>; null outside one.</summary>
    private readonly object? _item;
    private int _position;

    private Expression(string text, IElement self, object? item)
    {
        _text = text;
        _self = self;
        _item = item;
    }

    /// <summary>Text, a bool, an element, a type reference, a stereotype, or null for nothing.</summary>
    public static object? Evaluate(string text, IElement self) => Evaluate(text, self, null);

    /// <summary>As <see cref="Evaluate(string, IElement)"/>, with <c>item</c> the entry's <c>each</c> item it is evaluated for.</summary>
    public static object? Evaluate(string text, IElement self, object? item)
    {
        var expression = new Expression(text, self, item);
        var value = expression.Coalesce();
        expression.SkipSpace();
        if (expression._position != text.Length)
        {
            throw new FormatException($"Unexpected '{text[expression._position..]}' in expression '{text}'.");
        }

        return value;
    }

    /// <summary>A value as canonical text (claims-and-findings.md, Values); empty text is nothing.</summary>
    public static string? Text(object? value) => value switch
    {
        null => null,
        string s => s.Length == 0 ? null : s,
        bool b => b ? "true" : "false",
        ITypeReference type => TypeText(type),
        IElement element => element.Name,
        // A list is its items' text in order, joined as a list's canonical text is; an empty one is nothing.
        IEnumerable items => items.Cast<object?>().Select(Text).OfType<string>().ToList() is { Count: > 0 } texts ? string.Join(", ", texts) : null,
        _ => value.ToString()
    };

    /// <summary>A type reference written with Intent's type names: <c>guid?</c>, <c>OrderItem[]</c>.</summary>
    public static string TypeText(ITypeReference type)
    {
        var name = type.Element?.Name ?? "";
        return $"{name}{(type.IsCollection ? "[]" : "")}{(type.IsNullable ? "?" : "")}";
    }

    private object? Coalesce()
    {
        var value = Or();
        while (Accept("??"))
        {
            var right = Or();
            value = Text(value) is null ? right : value;
        }

        return value;
    }

    private object? Or()
    {
        var value = And();
        while (Accept("||"))
        {
            var right = And();
            value = Truthy(value) || Truthy(right);
        }

        return value;
    }

    private object? And()
    {
        var value = Equality();
        while (Accept("&&"))
        {
            var right = Equality();
            value = Truthy(value) && Truthy(right);
        }

        return value;
    }

    private object? Equality()
    {
        var value = Additive();
        if (Accept("=="))
        {
            return Text(value) == Text(Additive());
        }

        if (Accept("!="))
        {
            return Text(value) != Text(Additive());
        }

        return value;
    }

    /// <summary>Text joined: <c>stereotype('gRPC Service')['Proto Service'] + 'Base'</c>. Nothing joined with anything is nothing.</summary>
    private object? Additive()
    {
        var value = Unary();
        while (Accept("+"))
        {
            var right = Unary();
            value = Text(value) is { } left && Text(right) is { } added ? left + added : null;
        }

        return value;
    }

    private object? Unary() => Accept("!") ? !Truthy(Unary()) : Postfix();

    private object? Postfix()
    {
        var value = Primary();
        while (true)
        {
            if (Accept("."))
            {
                // A function applies to what it follows, as to the element itself: parent.hasStereotype('Table').
                var name = Identifier();
                value = Accept("(") ? Function(name, Arguments(), value) : Member(value, name);
            }
            else if (Accept("["))
            {
                var key = StringLiteral();
                Expect("]");
                value = Member(value, key);
            }
            else
            {
                return value;
            }
        }
    }

    private object? Primary()
    {
        SkipSpace();
        if (Accept("("))
        {
            var inner = Coalesce();
            Expect(")");
            return inner;
        }

        if (Peek() == '\'')
        {
            return StringLiteral();
        }

        if (char.IsAsciiDigit(Peek()))
        {
            // A number is its text, so count(...) == 0 compares as text.
            var start = _position;
            while (_position < _text.Length && char.IsAsciiDigit(_text[_position]))
            {
                _position++;
            }

            return _text[start.._position];
        }

        var name = Identifier();
        if (name is "where" or "map" && Accept("("))
        {
            // The second argument is evaluated per item, with the item as self: map(children('Attribute'), name).
            var list = Coalesce() as IEnumerable<IElement> ?? [];
            Expect(",");
            var each = Argument();
            Expect(")");
            return name == "where"
                ? list.Where(x => Truthy(Evaluate(each, x, _item))).ToList()
                : list.Select(x => Evaluate(each, x, _item)).ToList();
        }

        if (Accept("("))
        {
            return Function(name, Arguments(), _self);
        }

        return name switch
        {
            "true" => true,
            "false" => false,
            // An entry's each item: item.element.name names the message a handler's subscription is to.
            "item" => _item,
            _ => Member(_self, name)
        };
    }

    /// <summary>A call's arguments, its <c>(</c> read.</summary>
    private List<object?> Arguments()
    {
        var arguments = new List<object?>();
        if (!Accept(")"))
        {
            do
            {
                arguments.Add(Coalesce());
            } while (Accept(","));

            Expect(")");
        }

        return arguments;
    }

    /// <summary>A function, of <paramref name="self"/> where it reads an element, package or association end.</summary>
    private object? Function(string name, IReadOnlyList<object?> arguments, object? self) => name switch
    {
        "exists" => Text(arguments[0]) is not null,
        "stereotype" => Stereotypes(self).FirstOrDefault(x => x.Name == Text(arguments[0])),
        "hasStereotype" => Stereotypes(self).Any(x => x.Name == Text(arguments[0])),
        // The stereotype on the nearest folder holding it, else its package, that has it: enclosing('Schema').Name.
        "enclosing" => self is IElement element ? Enclosing(element, Text(arguments[0])) : null,
        "children" => (self as IElement)?.ChildElements.Where(x => x.SpecializationType == Text(arguments[0])).ToList() ?? (self as IPackage)?.ChildElements.Where(x => x.SpecializationType == Text(arguments[0])).ToList(),
        // The association ends leading away from it, of associations of a specialization: associations('Generalization').
        "associations" => ((self as IElement)?.AssociatedElements ?? []).Where(x => x.Association?.SpecializationType == Text(arguments[0])).ToList(),
        "if" => Truthy(arguments[0]) ? arguments[1] : arguments.ElementAtOrDefault(2),
        "split" => Text(arguments[0]) is { } whole
            ? whole.Split(Text(arguments[1]) ?? ",").Select(x => x.Trim()).Where(x => x.Length > 0).Cast<object?>().ToList()
            : null,
        "lower" => Text(arguments[0])?.ToLowerInvariant(),
        "upper" => Text(arguments[0])?.ToUpperInvariant(),
        "camel" => Text(arguments[0]) is { Length: > 0 } camel ? char.ToLowerInvariant(camel[0]) + camel[1..] : null,
        "pascal" => Text(arguments[0]) is { Length: > 0 } pascal ? char.ToUpperInvariant(pascal[0]) + pascal[1..] : null,
        "kebab" => Text(arguments[0]) is { } kebab ? System.Text.RegularExpressions.Regex.Replace(kebab, "(?<=[a-z0-9])([A-Z])", "-$1").ToLowerInvariant() : null,
        "trimStart" => Text(arguments[0]) is { } start && Text(arguments[1]) is { Length: > 0 } prefix && start.StartsWith(prefix, StringComparison.Ordinal) ? start[prefix.Length..] : Text(arguments[0]),
        "trimEnd" => Text(arguments[0]) is { } end && Text(arguments[1]) is { Length: > 0 } suffix && end.EndsWith(suffix, StringComparison.Ordinal) ? end[..^suffix.Length] : Text(arguments[0]),
        "replace" => Text(arguments[0])?.Replace(Text(arguments[1]) ?? "", Text(arguments[2]) ?? "", StringComparison.Ordinal),
        "join" => arguments[0] is IEnumerable list and not string
            ? string.Join(Text(arguments[1]) ?? ", ", list.Cast<object?>().Select(Text).OfType<string>())
            : Text(arguments[0]),
        "count" => arguments[0] is IEnumerable items and not string ? items.Cast<object?>().Count().ToString() : null,
        // The number a text starts with, as an option list writes a code before its name: number('201 (Created)') is 201.
        "number" => Text(arguments[0]) is { } text && System.Text.RegularExpressions.Regex.Match(text, @"^\s*-?\d+(\.\d+)?") is { Success: true } number ? number.Value.Trim() : null,
        _ => throw new FormatException($"Unknown function '{name}' in expression '{_text}'.")
    };

    private static object? Member(object? owner, string name) => owner switch
    {
        null => null,
        // An association is selected as its target end, so source and target read its ends from either.
        IAssociationEnd end when name is "association" or "isSource" or "isTarget" or "otherEnd" or "multiplicity" or "isNavigable" or "element" or "source" or "target" => name switch
        {
            "association" => end.Association,
            "source" => end.Association?.SourceEnd,
            "target" => end.Association?.TargetEnd,
            "isSource" => end.Association is { } association && !IsTarget(association, end),
            "isTarget" => end.Association is { } association && IsTarget(association, end),
            "otherEnd" => end.OtherEnd(),
            "multiplicity" => end.TypeReference is not { } multiplicity ? null
                : multiplicity.IsCollection ? multiplicity.IsNullable ? "*" : "1..*"
                : multiplicity.IsNullable ? "0..1" : "1",
            "isNavigable" => end.IsNavigable,
            _ => end.TypeReference?.Element
        },
        IElement element => name switch
        {
            "self" or "it" => element,
            "name" => element.Name,
            "id" => element.Id,
            "specialization" => element.SpecializationType,
            "comment" => element.Comment,
            "parent" => element.ParentElement,
            "package" => element.Package,
            "folderPath" => FolderPath(element),
            "type" => element.TypeReference?.Element is null ? null : element.TypeReference,
            "abstract" => element.IsAbstract,
            "static" => element.IsStatic,
            "value" => element.Value,
            // Its generic type parameters' names, in order.
            "genericTypes" => (element.GenericTypes ?? []).Select(x => x.Name).ToList(),
            _ => throw new FormatException($"An element has no '{name}'.")
        },
        IPackage package => name switch
        {
            "self" or "it" => package,
            "name" => package.Name,
            "id" => package.Id,
            "specialization" => package.SpecializationType,
            "comment" => package.Comment,
            _ => throw new FormatException($"A package has no '{name}'.")
        },
        ITypeReference type => name switch
        {
            "name" => type.Element?.Name,
            "element" => type.Element,
            "isCollection" => type.IsCollection,
            "isNullable" => type.IsNullable,
            _ => throw new FormatException($"A type reference has no '{name}'.")
        },
        // A property holding an element gives the element (a Foreign Key's Association is its association end), and one
        // holding several the elements (Secured's Security Roles).
        IStereotype stereotype => stereotype.Properties.FirstOrDefault(x => x.Key == name) switch
        {
            IStereotypeProperty<IElement> { Value: { } element } => element,
            IStereotypeProperty<IElement[]> { Value: { } elements } => elements,
            var property => property?.Value
        },
        _ => throw new FormatException($"'{name}' can't be read from {owner.GetType().Name}.")
    };

    private static IEnumerable<IStereotype> Stereotypes(object? owner) => (owner as IHasStereotypes)?.Stereotypes ?? [];

    /// <summary>The designer folders holding it, outermost first, joined by <c>/</c>; nothing when none do.</summary>
    private static string? FolderPath(IElement element)
    {
        var folders = new List<string>();
        for (var parent = element.ParentElement; parent is not null; parent = parent.ParentElement)
        {
            if (parent.SpecializationType == "Folder")
            {
                folders.Insert(0, parent.Name);
            }
        }

        return folders.Count == 0 ? null : string.Join("/", folders);
    }

    private static IStereotype? Enclosing(IElement element, string? name)
    {
        for (var parent = element.ParentElement; parent is not null; parent = parent.ParentElement)
        {
            if (Stereotypes(parent).FirstOrDefault(x => x.Name == name) is { } stereotype)
            {
                return stereotype;
            }
        }

        return (element.Package?.Stereotypes ?? []).FirstOrDefault(x => x.Name == name);
    }

    private static bool IsTarget(IAssociation association, IAssociationEnd end) =>
        ReferenceEquals(association.TargetEnd, end) || association.TargetEnd?.Id == end.Id && association.SourceEnd?.Id != end.Id;

    private static bool Truthy(object? value) => value switch
    {
        bool b => b,
        _ => Text(value) is { } text && text != "false"
    };

    private string Identifier()
    {
        SkipSpace();
        var start = _position;
        while (_position < _text.Length && (char.IsLetterOrDigit(_text[_position]) || _text[_position] == '_'))
        {
            _position++;
        }

        if (start == _position)
        {
            throw new FormatException($"Expected a name at {start} in expression '{_text}'.");
        }

        return _text[start.._position];
    }

    /// <summary>An argument's text, unevaluated: up to the <c>,</c> or <c>)</c> that closes it.</summary>
    private string Argument()
    {
        var start = _position;
        var depth = 0;
        for (; _position < _text.Length; _position++)
        {
            switch (_text[_position])
            {
                case '\'':
                    _position = _text.IndexOf('\'', _position + 1) is var end and >= 0 ? end : throw new FormatException($"Unterminated text in expression '{_text}'.");
                    break;
                case '(':
                    depth++;
                    break;
                case ')' when depth == 0:
                case ',' when depth == 0:
                    return _text[start.._position].Trim();
                case ')':
                    depth--;
                    break;
            }
        }

        throw new FormatException($"Expected ')' in expression '{_text}'.");
    }

    private string StringLiteral()
    {
        SkipSpace();
        Expect("'");
        var end = _text.IndexOf('\'', _position);
        if (end < 0)
        {
            throw new FormatException($"Unterminated text in expression '{_text}'.");
        }

        var value = _text[_position..end];
        _position = end + 1;
        return value;
    }

    private bool Accept(string token)
    {
        SkipSpace();
        if (string.CompareOrdinal(_text, _position, token, 0, token.Length) != 0)
        {
            return false;
        }

        // '!' must not swallow the first character of '!='.
        if (token == "!" && Peek(1) == '=')
        {
            return false;
        }

        _position += token.Length;
        return true;
    }

    private void Expect(string token)
    {
        if (!Accept(token))
        {
            throw new FormatException($"Expected '{token}' at {_position} in expression '{_text}'.");
        }
    }

    private char Peek(int offset = 0) => _position + offset < _text.Length ? _text[_position + offset] : '\0';

    private void SkipSpace()
    {
        while (_position < _text.Length && char.IsWhiteSpace(_text[_position]))
        {
            _position++;
        }
    }
}
