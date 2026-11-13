using System.Collections;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.DriftEngine.Vocabulary;

/// <summary>
/// The vocabulary's expression language (vocabulary.md, Expressions), evaluated against one element. This is the subset
/// the structure kinds need: fields, member access, stereotypes, association ends (<c>associations('Association')</c>,
/// <c>.isSource</c>, <c>.multiplicity</c>), numbers, <c>??</c>, comparison and boolean operators.
/// </summary>
internal sealed class Expression
{
    private readonly string _text;
    private readonly IElement _self;
    private int _position;

    private Expression(string text, IElement self)
    {
        _text = text;
        _self = self;
    }

    /// <summary>Text, a bool, an element, a type reference, a stereotype, or null for nothing.</summary>
    public static object? Evaluate(string text, IElement self)
    {
        var expression = new Expression(text, self);
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
        var value = Unary();
        if (Accept("=="))
        {
            return Text(value) == Text(Unary());
        }

        if (Accept("!="))
        {
            return Text(value) != Text(Unary());
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
                value = Member(value, Identifier());
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
                ? list.Where(x => Truthy(Evaluate(each, x))).ToList()
                : list.Select(x => Evaluate(each, x)).ToList();
        }

        if (Accept("("))
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

            return Function(name, arguments);
        }

        return name switch
        {
            "true" => true,
            "false" => false,
            _ => Member(_self, name)
        };
    }

    private object? Function(string name, IReadOnlyList<object?> arguments) => name switch
    {
        "exists" => Text(arguments[0]) is not null,
        "stereotype" => Stereotypes(_self).FirstOrDefault(x => x.Name == Text(arguments[0])),
        "hasStereotype" => Stereotypes(_self).Any(x => x.Name == Text(arguments[0])),
        "children" => _self.ChildElements.Where(x => x.SpecializationType == Text(arguments[0])).ToList(),
        // The association ends leading away from it, of associations of a specialization: associations('Generalization').
        "associations" => (_self.AssociatedElements ?? []).Where(x => x.Association?.SpecializationType == Text(arguments[0])).ToList(),
        "join" => arguments[0] is IEnumerable list and not string
            ? string.Join(Text(arguments[1]) ?? ", ", list.Cast<object?>().Select(Text).OfType<string>())
            : Text(arguments[0]),
        "count" => arguments[0] is IEnumerable items and not string ? items.Cast<object?>().Count().ToString() : null,
        _ => throw new FormatException($"Unknown function '{name}' in expression '{_text}'.")
    };

    private static object? Member(object? owner, string name) => owner switch
    {
        null => null,
        IAssociationEnd end when name is "association" or "isSource" or "isTarget" or "otherEnd" or "multiplicity" or "isNavigable" or "element" => name switch
        {
            "association" => end.Association,
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
            "type" => element.TypeReference?.Element is null ? null : element.TypeReference,
            "abstract" => element.IsAbstract,
            "static" => element.IsStatic,
            "value" => element.Value,
            _ => throw new FormatException($"An element has no '{name}'.")
        },
        ITypeReference type => name switch
        {
            "name" => type.Element?.Name,
            "element" => type.Element,
            "isCollection" => type.IsCollection,
            "isNullable" => type.IsNullable,
            _ => throw new FormatException($"A type reference has no '{name}'.")
        },
        // A property holding an element gives the element (a Foreign Key's Association is its association end).
        IStereotype stereotype => stereotype.Properties.FirstOrDefault(x => x.Key == name) switch
        {
            IStereotypeProperty<IElement> { Value: { } element } => element,
            var property => property?.Value
        },
        _ => throw new FormatException($"'{name}' can't be read from {owner.GetType().Name}.")
    };

    private static IEnumerable<IStereotype> Stereotypes(IElement element) => element.Stereotypes ?? [];

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
