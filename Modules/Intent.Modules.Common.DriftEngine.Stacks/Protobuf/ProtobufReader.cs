using System.Globalization;
using System.Text;
using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.Protobuf;

/// <summary>
/// The Protobuf reader (code-reader.md, The Protobuf reader): parses each <c>.proto</c> file by the language's grammar
/// (proto2, proto3 and editions) and resolves the type names its fields write as protoc does, innermost scope first. A
/// <c>.proto</c> declares and never runs anything, so there are no bodies. A syntax error is recorded, and reading goes on
/// after the statement it is in.
/// </summary>
internal static class ProtobufReader
{
    private static readonly HashSet<string> Scalars =
        ["double", "float", "int32", "int64", "uint32", "uint64", "sint32", "sint64", "fixed32", "fixed64", "sfixed32", "sfixed64", "bool", "string", "bytes"];

    // The well-known types protoc ships (google/protobuf/*.proto) are messages, but for these enums.
    private static readonly HashSet<string> WellKnownEnums =
        ["google.protobuf.NullValue", "google.protobuf.Syntax", "google.protobuf.Field.Kind", "google.protobuf.Field.Cardinality"];

    /// <param name="root">The output folder.</param>
    /// <param name="files">In-scope paths relative to <paramref name="root"/>, with <c>/</c> separators.</param>
    /// <param name="graph">The graph it adds the files, types and members to, and its <see cref="ProtobufSyntax"/>.</param>
    public static void Read(string root, IReadOnlyList<string> files, CodeGraph graph)
    {
        var syntax = new ProtobufSyntax();
        graph.Set(syntax);
        var inScope = files.ToHashSet(StringComparer.Ordinal);
        var parsed = files.Select(x => new Parser(x, File.ReadAllText(Path.Combine(root, x))).Read()).ToList();
        var packages = parsed.ToDictionary(x => x.Path, x => x.Package, StringComparer.Ordinal);
        var declared = parsed.SelectMany(x => x.Types).ToLookup(x => x.FullName, StringComparer.Ordinal);
        foreach (var file in parsed)
        {
            if (file.Errors.Count > 0)
            {
                graph.Failures.Add((file.Path, string.Join("; ", file.Errors)));
            }

            var imports = file.Imports.Select(x => (Import: x, File: ImportedFile(file.Path, x.Written, inScope))).ToList();
            graph.Files.Add(new FileNode
            {
                Path = file.Path,
                Language = ProtobufLanguage.LanguageName,
                Namespaces = file.Package.Length > 0 ? [file.Package] : [],
                // An import makes every type of the file it names visible, by the names its package gives them.
                Imports = imports.Select(x => new ImportNode
                {
                    Target = x.File is not null && packages[x.File].Length > 0 ? packages[x.File] : x.Import.Written,
                    Text = x.Import.Text,
                    Module = x.Import.Written,
                    File = x.File,
                    Wildcard = true,
                    At = x.Import.At
                }).ToList()
            });
            graph.Types.AddRange(file.Types.Select(x => x.Type));
            syntax.Files[file.Path] = new ProtobufFile { Syntax = file.Syntax, Edition = file.Edition, Package = file.Package, Options = file.Options };

            // Whether a field can be left unset can depend on what its type is, which any file may declare.
            var visible = imports.Select(x => x.File).OfType<string>().Prepend(file.Path).ToList();
            foreach (var field in file.Fields)
            {
                field.Type.Nullable = Presence(field, file.Syntax, KindOf(field.Type, field.Scope, visible, declared));
            }
        }
    }

    /// <summary>
    /// The in-scope file an import names, as protoc finds it on an import path: looked for from the importing file's folder,
    /// then each folder above it, so a path relative to the file, its project or the output folder is found.
    /// </summary>
    private static string? ImportedFile(string importing, string written, HashSet<string> inScope)
    {
        var path = written.Replace('\\', '/');
        var folder = importing.Contains('/') ? importing[..importing.LastIndexOf('/')] : "";
        while (true)
        {
            var candidate = folder.Length == 0 ? path : $"{folder}/{path}";
            if (inScope.Contains(candidate))
            {
                return candidate;
            }

            if (folder.Length == 0)
            {
                return null;
            }

            folder = folder.Contains('/') ? folder[..folder.LastIndexOf('/')] : "";
        }
    }

    /// <summary>
    /// What a field's type is: <c>scalar</c>, or the <c>message</c> or <c>enum</c> its name resolves to as protoc resolves
    /// it, innermost scope first, preferring the field's own file and then the files it imports. Null when it names a type
    /// that isn't in scope, other than one of the well-known types.
    /// </summary>
    private static string? KindOf(TypeRefNode type, string scope, IReadOnlyList<string> visible, ILookup<string, Declared> declared)
    {
        if (Scalars.Contains(type.Text))
        {
            return "scalar";
        }

        foreach (var candidate in Candidates(type.Text, scope))
        {
            if (declared[candidate].ToList() is { Count: > 0 } found)
            {
                return (found.FirstOrDefault(x => x.File == visible[0]) ?? found.FirstOrDefault(x => visible.Contains(x.File)) ?? found[0]).Type.Kind;
            }
        }

        return type.QualifiedName.StartsWith("google.protobuf.", StringComparison.Ordinal)
            ? WellKnownEnums.Contains(type.QualifiedName) ? "enum" : "message"
            : null;
    }

    /// <summary>The full names a type name can mean from a scope, innermost first; a leading <c>.</c> is already full.</summary>
    private static IEnumerable<string> Candidates(string written, string scope)
    {
        if (written.StartsWith('.'))
        {
            yield return written[1..];
            yield break;
        }

        while (true)
        {
            yield return scope.Length == 0 ? written : $"{scope}.{written}";
            if (scope.Length == 0)
            {
                yield break;
            }

            scope = scope.Contains('.') ? scope[..scope.LastIndexOf('.')] : "";
        }
    }

    /// <summary>
    /// A singular field is nullable when it has presence, so it can be left unset: <c>optional</c>, in a <c>oneof</c>, a
    /// message (a well-known wrapper such as <c>google.protobuf.Int32Value</c> included), or any field an edition gives
    /// explicit presence. A proto3 scalar or enum without <c>optional</c>, and proto2's <c>required</c>, are not.
    /// </summary>
    private static string Presence(Pending field, string syntax, string? kind)
    {
        if (field.Label == "optional" || field.InOneof || syntax == "proto2" && field.Label != "required")
        {
            return "true";
        }

        if (field.Label == "required")
        {
            return "false";
        }

        var presence = syntax == "editions" ? field.Presence ?? field.Features.FieldPresence ?? "EXPLICIT" : "IMPLICIT";
        return presence switch
        {
            "LEGACY_REQUIRED" => "false",
            "EXPLICIT" => "true",
            _ => kind switch
            {
                "message" => "true",
                "scalar" or "enum" => "false",
                _ => "unknown"
            }
        };
    }

    private sealed record Declared(string FullName, TypeNode Type, string File);

    private sealed record Imported(string Written, string Text, Span At);

    /// <param name="Scope">The full name of the message it is declared in, which its type name is resolved from.</param>
    /// <param name="Presence">The <c>features.field_presence</c> its own options give it.</param>
    private sealed record Pending(TypeRefNode Type, string Scope, string? Label, bool InOneof, Features Features, string? Presence);

    /// <summary>The <c>features.field_presence</c> an edition's file or message sets, which its fields and messages inherit.</summary>
    private sealed class Features(Features? parent)
    {
        public string? Presence { get; set; }

        public string? FieldPresence => Presence ?? parent?.FieldPresence;
    }

    private sealed class Parsed(string path)
    {
        public string Path => path;

        public string Syntax { get; set; } = "proto2";

        public string? Edition { get; set; }

        public string Package { get; set; } = "";

        public Features Features { get; } = new(null);

        public List<Imported> Imports { get; } = [];

        public List<AttributeNode> Options { get; } = [];

        public List<Declared> Types { get; } = [];

        public List<Pending> Fields { get; } = [];

        public List<string> Errors { get; } = [];
    }

    private enum TokenKind
    {
        Identifier,
        Number,
        String,
        Symbol,
        End
    }

    /// <param name="EndColumn">The column after its last character.</param>
    private readonly record struct Token(TokenKind Kind, string Text, int Line, int Column, int EndLine, int EndColumn);

    private sealed class ParseError(string message) : Exception(message);

    private sealed class Parser
    {
        private readonly string _file;
        private readonly List<Token> _tokens;
        private readonly Parsed _parsed;
        private int _at;

        public Parser(string file, string text)
        {
            _file = file;
            _parsed = new Parsed(file);
            _tokens = Tokenize(text, _parsed.Errors);

            // A package names every type of its file, wherever in the file it is declared.
            _parsed.Package = PackageOf(_tokens);
        }

        public Parsed Read()
        {
            while (!AtEnd)
            {
                Statement(TopLevel);
            }

            return _parsed;
        }

        private void TopLevel()
        {
            if (Is("syntax") && IsAt(1, "="))
            {
                Next();
                Next();
                _parsed.Syntax = StringValue();
                Expect(";");
            }
            else if (Is("edition") && IsAt(1, "="))
            {
                Next();
                Next();
                _parsed.Edition = StringValue();
                _parsed.Syntax = "editions";
                Expect(";");
            }
            else if (Is("package"))
            {
                Next();
                FullName();
                Expect(";");
            }
            else if (Is("import"))
            {
                Import();
            }
            else if (Is("option"))
            {
                _parsed.Options.Add(Option(_parsed.Features));
            }
            else if (Is("message"))
            {
                Message(_parsed.Package, _parsed.Features);
            }
            else if (Is("enum"))
            {
                Enum(_parsed.Package);
            }
            else if (Is("service"))
            {
                Service();
            }
            else if (Is("extend"))
            {
                Skip();
            }
            else if (Is(";"))
            {
                Next();
            }
            else
            {
                throw Error("expected a declaration");
            }
        }

        private void Import()
        {
            var start = _at;
            Next();
            if (Is("public") || Is("weak"))
            {
                Next();
            }

            var written = StringValue();
            Expect(";");
            _parsed.Imports.Add(new Imported(written, Canonical(start, _at - 1), SpanOf(start, _at - 1)));
        }

        private void Message(string scope, Features features)
        {
            var start = _at;
            Next();
            var name = Identifier();
            var type = Declare(name, "message", scope, start, End());
            Body(type, Scoped(scope, name), new Features(features));
        }

        private void Body(TypeNode type, string scope, Features features)
        {
            Expect("{");
            while (!Is("}") && !AtEnd)
            {
                Statement(() => MessageElement(type, scope, features));
            }

            Expect("}");
        }

        private void MessageElement(TypeNode type, string scope, Features features)
        {
            if (Is(";"))
            {
                Next();
            }
            else if (Declares("message"))
            {
                Message(scope, features);
            }
            else if (Declares("enum"))
            {
                Enum(scope);
            }
            else if (Declares("oneof"))
            {
                Oneof(type, scope, features);
            }
            else if (Is("extend") || Is("extensions") || Is("reserved"))
            {
                Skip();
            }
            else if (Is("option"))
            {
                type.Attributes.Add(Option(features));
            }
            else
            {
                Field(type, scope, features, inOneof: false);
            }
        }

        /// <summary>A oneof's fields are its message's; its own options aren't read.</summary>
        private void Oneof(TypeNode type, string scope, Features features)
        {
            Next();
            Identifier();
            Expect("{");
            while (!Is("}") && !AtEnd)
            {
                Statement(() =>
                {
                    if (Is(";"))
                    {
                        Next();
                    }
                    else if (Is("option"))
                    {
                        Skip();
                    }
                    else
                    {
                        Field(type, scope, features, inOneof: true);
                    }
                });
            }

            Expect("}");
        }

        private void Field(TypeNode type, string scope, Features features, bool inOneof)
        {
            var start = _at;
            var label = Is("optional") || Is("required") || Is("repeated") ? Next().Text : null;
            if (Is("group"))
            {
                Group(type, scope, features, label, inOneof, start);
                return;
            }

            var fieldType = Is("map") && IsAt(1, "<") ? Map() : TypeName();
            var name = Identifier();
            Expect("=");
            var number = Number();
            var options = Is("[") ? Options() : [];
            Expect(";");
            AddField(type, name, fieldType, number, options, label, scope, features, inOneof, start, _at - 1);
        }

        /// <summary>
        /// proto2's group: a nested message and a field of it, which protoc names after the message, lower-cased:
        /// <c>repeated group Result = 1 { … }</c> is the message <c>Result</c> and the field <c>result</c>.
        /// </summary>
        private void Group(TypeNode type, string scope, Features features, string? label, bool inOneof, int start)
        {
            Next();
            var name = Identifier();
            Expect("=");
            var number = Number();
            var options = Is("[") ? Options() : [];
            var nested = Declare(name, "message", scope, start, End());
            Body(nested, Scoped(scope, name), new Features(features));
            var fieldType = new TypeRefNode { Text = name, Name = name, QualifiedName = name, Language = ProtobufLanguage.LanguageName };
            AddField(type, name.ToLowerInvariant(), fieldType, number, options, label, scope, features, inOneof, start, _at - 1);
        }

        private void AddField(TypeNode type, string name, TypeRefNode fieldType, ValueNode number, IReadOnlyList<AttributeNode> options, string? label,
            string scope, Features features, bool inOneof, int start, int end)
        {
            if (label == "repeated")
            {
                fieldType.Nullable = "false";
                fieldType = new TypeRefNode
                {
                    Text = $"repeated {fieldType.Text}",
                    Name = fieldType.Name,
                    QualifiedName = fieldType.QualifiedName,
                    Arguments = [fieldType],
                    IsArray = true,
                    Nullable = "false",
                    Language = ProtobufLanguage.LanguageName
                };
            }
            else if (fieldType.Name != "map" || fieldType.Arguments.Count != 2)
            {
                var presence = options.FirstOrDefault(x => x.Name == "features.field_presence")?.Arguments[0];
                _parsed.Fields.Add(new Pending(fieldType, scope, label, inOneof, features, presence));
            }

            type.Members.Add(new MemberNode
            {
                Name = name,
                Kind = "property",
                Type = fieldType,
                Value = number,
                Attributes = options,
                Modifiers = label is null ? [] : [label],
                At = SpanOf(start, end),
                TextOf = () => Canonical(start, end)
            });
        }

        /// <summary>A map is <c>map</c> with its key and value types; it is never null, and neither are its values.</summary>
        private TypeRefNode Map()
        {
            var start = _at;
            Next();
            Next();
            var key = TypeName();
            Expect(",");
            var value = TypeName();
            Expect(">");
            key.Nullable = "false";
            value.Nullable = "false";
            return new TypeRefNode { Text = Canonical(start, _at - 1), Name = "map", QualifiedName = "map", Arguments = [key, value], Nullable = "false", Language = ProtobufLanguage.LanguageName };
        }

        private void Enum(string scope)
        {
            var start = _at;
            Next();
            var name = Identifier();
            var type = Declare(name, "enum", scope, start, End());
            Expect("{");
            while (!Is("}") && !AtEnd)
            {
                Statement(() =>
                {
                    if (Is(";"))
                    {
                        Next();
                    }
                    else if (Peek.Kind == TokenKind.Identifier && IsAt(1, "="))
                    {
                        var at = _at;
                        var value = Next().Text;
                        Next();
                        var number = Number();
                        var options = Is("[") ? Options() : [];
                        Expect(";");
                        var end = _at - 1;
                        type.Members.Add(new MemberNode { Name = value, Kind = "enumMember", Value = number, Attributes = options, At = SpanOf(at, end), TextOf = () => Canonical(at, end) });
                    }
                    else if (Is("option"))
                    {
                        type.Attributes.Add(Option(null));
                    }
                    else if (Is("reserved"))
                    {
                        Skip();
                    }
                    else
                    {
                        throw Error("expected an enum value");
                    }
                });
            }

            Expect("}");
        }

        private void Service()
        {
            var start = _at;
            Next();
            var name = Identifier();
            var type = Declare(name, "service", _parsed.Package, start, End());
            Expect("{");
            while (!Is("}") && !AtEnd)
            {
                Statement(() =>
                {
                    if (Is(";"))
                    {
                        Next();
                    }
                    else if (Is("option"))
                    {
                        type.Attributes.Add(Option(null));
                    }
                    else if (Is("rpc"))
                    {
                        Rpc(type);
                    }
                    else
                    {
                        throw Error("expected an rpc");
                    }
                });
            }

            Expect("}");
        }

        /// <summary>An rpc is a method taking its request as <c>request</c> and returning its response.</summary>
        private void Rpc(TypeNode service)
        {
            var start = _at;
            Next();
            var name = Identifier();
            Expect("(");
            var requestStart = _at;
            var request = Streamed();
            var requestEnd = _at - 1;
            Expect(")");
            Expect("returns");
            Expect("(");
            var response = Streamed();
            Expect(")");
            var options = new List<AttributeNode>();
            if (Is("{"))
            {
                Next();
                while (!Is("}") && !AtEnd)
                {
                    Statement(() =>
                    {
                        if (Is(";"))
                        {
                            Next();
                        }
                        else if (Is("option"))
                        {
                            options.Add(Option(null));
                        }
                        else
                        {
                            throw Error("expected an option");
                        }
                    });
                }

                Expect("}");
            }
            else
            {
                Expect(";");
            }

            var end = _at - 1;
            service.Members.Add(new MemberNode
            {
                Name = name,
                Kind = "method",
                Type = response,
                Parameters = [new ParameterNode { Name = "request", Type = request, At = SpanOf(requestStart, requestEnd), TextOf = () => Canonical(requestStart, requestEnd) }],
                Attributes = options,
                At = SpanOf(start, end),
                TextOf = () => Canonical(start, end)
            });
        }

        /// <summary>
        /// An rpc's request or response type. A streamed one is <c>stream</c> of its message, as a generic type is of its
        /// argument: <c>stream OrderReply</c> is named <c>stream</c>, with <c>OrderReply</c> its argument. Neither is ever null.
        /// </summary>
        private TypeRefNode Streamed()
        {
            var streamed = Is("stream");
            if (streamed)
            {
                Next();
            }

            var type = TypeName();
            type.Nullable = "false";
            return streamed
                ? new TypeRefNode { Text = $"stream {type.Text}", Name = "stream", QualifiedName = "stream", Arguments = [type], Nullable = "false", Language = ProtobufLanguage.LanguageName }
                : type;
        }

        /// <summary>A type name as written: a scalar, or a message's or enum's, qualified or not, <c>.</c> first when full.</summary>
        private TypeRefNode TypeName()
        {
            var written = FullName();
            return new TypeRefNode
            {
                Text = written,
                Name = written[(written.LastIndexOf('.') + 1)..],
                QualifiedName = written.TrimStart('.'),
                IsPredefinedValueType = Scalars.Contains(written) && written is not ("string" or "bytes"),
                Language = ProtobufLanguage.LanguageName
            };
        }

        private string FullName()
        {
            var text = new StringBuilder();
            if (Is("."))
            {
                text.Append(Next().Text);
            }

            text.Append(Identifier());
            while (Is("."))
            {
                text.Append(Next().Text).Append(Identifier());
            }

            return text.ToString();
        }

        /// <summary>An integer, with its sign: a field's number, an enum value's.</summary>
        private ValueNode Number()
        {
            var start = _at;
            var sign = Is("-") || Is("+") ? Next().Text : "";
            if (Peek.Kind != TokenKind.Number)
            {
                throw Error("expected a number");
            }

            var number = Next().Text;
            return new ValueNode
            {
                Text = Canonical(start, _at - 1),
                Literal = new LiteralNode { Kind = "number", Value = sign == "-" ? $"-{number}" : number },
                At = SpanOf(start, _at - 1)
            };
        }

        /// <summary>An <c>option</c> statement, as an annotation; an edition's <c>features.field_presence</c> also sets its scope's.</summary>
        private AttributeNode Option(Features? scope)
        {
            Next();
            var option = OptionAssignment();
            Expect(";");
            if (scope is not null && option.Name == "features.field_presence")
            {
                scope.Presence = option.Arguments[0];
            }

            return option;
        }

        /// <summary>A field's or enum value's options: <c>[deprecated = true, json_name = "id"]</c>.</summary>
        private List<AttributeNode> Options()
        {
            Expect("[");
            var options = new List<AttributeNode> { OptionAssignment() };
            while (Is(","))
            {
                Next();
                options.Add(OptionAssignment());
            }

            Expect("]");
            return options;
        }

        /// <summary>
        /// One option, named as written (<c>deprecated</c>, <c>(google.api.http)</c>), with its value: a literal's value as
        /// its argument, and a message value's fields as the value's items.
        /// </summary>
        private AttributeNode OptionAssignment()
        {
            var start = _at;
            OptionNamePart();
            while (Is("."))
            {
                Next();
                OptionNamePart();
            }

            var name = Canonical(start, _at - 1);
            Expect("=");
            var value = Constant(null);
            return new AttributeNode { Name = name, Arguments = [value.Literal ?? value.Text], Values = [value], At = SpanOf(start, _at - 1) };
        }

        private void OptionNamePart()
        {
            if (Is("("))
            {
                Next();
                FullName();
                Expect(")");
            }
            else
            {
                Identifier();
            }
        }

        /// <summary>A constant, or a message value in the text format (<c>{ get: "/v1/orders" body: "*" }</c>).</summary>
        private AttributeValueNode Constant(string? name)
        {
            var start = _at;
            if (Is("{") || Is("<"))
            {
                return Aggregate(name);
            }

            string? literal;
            if (Peek.Kind == TokenKind.String)
            {
                literal = StringValue();
            }
            else if (Is("-") || Is("+") || Peek.Kind == TokenKind.Number || Is("inf") || Is("nan"))
            {
                var sign = Is("-") || Is("+") ? Next().Text : "";
                var number = Peek.Kind == TokenKind.Number || Is("inf") || Is("nan") ? Next().Text : throw Error("expected a number");
                literal = sign == "-" ? $"-{number}" : number;
            }
            else if (Is("true") || Is("false"))
            {
                literal = Next().Text;
            }
            else
            {
                // An enum value's name, or any other identifier.
                FullName();
                literal = null;
            }

            return new AttributeValueNode { Name = name, Text = Canonical(start, _at - 1), Literal = literal };
        }

        private AttributeValueNode Aggregate(string? name)
        {
            var start = _at;
            var close = Next().Text == "{" ? "}" : ">";
            var items = new List<AttributeValueNode>();
            while (!Is(close) && !AtEnd)
            {
                // An extension's field is written [its.full.name].
                var fieldStart = _at;
                if (Is("["))
                {
                    while (!Is("]") && !AtEnd)
                    {
                        Next();
                    }

                    Expect("]");
                }
                else
                {
                    Identifier();
                }

                var field = Canonical(fieldStart, _at - 1);
                if (Is(":"))
                {
                    Next();
                }

                items.Add(Is("[") ? List(field) : Constant(field));
                if (Is(",") || Is(";"))
                {
                    Next();
                }
            }

            Expect(close);
            return new AttributeValueNode { Name = name, Text = Canonical(start, _at - 1), Items = items };
        }

        private AttributeValueNode List(string name)
        {
            var start = _at;
            Expect("[");
            var items = new List<AttributeValueNode>();
            while (!Is("]") && !AtEnd)
            {
                items.Add(Constant(null));
                if (!Is(","))
                {
                    break;
                }

                Next();
            }

            Expect("]");
            return new AttributeValueNode { Name = name, Text = Canonical(start, _at - 1), Items = items };
        }

        /// <summary>A string's value: adjacent strings are one, as in C.</summary>
        private string StringValue()
        {
            if (Peek.Kind != TokenKind.String)
            {
                throw Error("expected a string");
            }

            var text = new StringBuilder();
            while (Peek.Kind == TokenKind.String)
            {
                text.Append(Unescape(Next().Text));
            }

            return text.ToString();
        }

        private TypeNode Declare(string name, string kind, string scope, int start, int end)
        {
            var type = new TypeNode { Name = name, Namespace = _parsed.Package, Kind = kind, At = SpanOf(start, end), TextOf = () => Canonical(start, end) };
            type.Files.Add(_file);
            _parsed.Types.Add(new Declared(Scoped(scope, name), type, _file));
            return type;
        }

        private static string Scoped(string scope, string name) => scope.Length == 0 ? name : $"{scope}.{name}";

        private bool Declares(string keyword) => Is(keyword) && PeekAt(1).Kind == TokenKind.Identifier && IsAt(2, "{");

        /// <summary>Reads a statement; on a syntax error, records it and skips the rest of the statement, so the file reads on.</summary>
        private void Statement(Action read)
        {
            var start = _at;
            try
            {
                read();
            }
            catch (ParseError e)
            {
                _parsed.Errors.Add(e.Message);
                Recover(start);
            }
        }

        /// <summary>Skips to just past the statement's <c>;</c> or block, or to the <c>}</c> that closes the block it is in.</summary>
        private void Recover(int start)
        {
            var depth = 0;
            for (var i = start; i < _at; i++)
            {
                depth += _tokens[i] switch { { Kind: TokenKind.Symbol, Text: "{" } => 1, { Kind: TokenKind.Symbol, Text: "}" } => -1, _ => 0 };
            }

            if (_at == start)
            {
                // Nothing of it was read: drop the token it can't start with.
                var dropped = Next();
                if (dropped.Text is ";" or "}")
                {
                    return;
                }

                depth += dropped.Text == "{" ? 1 : 0;
            }

            while (!AtEnd)
            {
                if (Is("}"))
                {
                    if (depth == 0)
                    {
                        return;
                    }

                    Next();
                    if (--depth == 0)
                    {
                        return;
                    }

                    continue;
                }

                if (Is(";") && depth == 0)
                {
                    Next();
                    return;
                }

                depth += Is("{") ? 1 : 0;
                Next();
            }
        }

        /// <summary>Skips a statement that declares nothing read: <c>reserved</c>, <c>extensions</c>, <c>extend</c>.</summary>
        private void Skip()
        {
            var depth = 0;
            while (!AtEnd)
            {
                var token = Next();
                if (token.Kind != TokenKind.Symbol)
                {
                    continue;
                }

                switch (token.Text)
                {
                    case "{" or "[" or "(":
                        depth++;
                        break;
                    case "]" or ")":
                        depth--;
                        break;
                    case "}" when --depth <= 0:
                    case ";" when depth == 0:
                        return;
                }
            }
        }

        /// <summary>The <c>}</c> that closes the block opening at the current token; the file's last token when none does.</summary>
        private int End()
        {
            if (!Is("{"))
            {
                throw Error("expected '{'");
            }

            var depth = 0;
            for (var i = _at; i < _tokens.Count - 1; i++)
            {
                if (_tokens[i].Kind != TokenKind.Symbol)
                {
                    continue;
                }

                if (_tokens[i].Text == "{")
                {
                    depth++;
                }
                else if (_tokens[i].Text == "}" && --depth == 0)
                {
                    return i;
                }
            }

            return _tokens.Count - 2;
        }

        private Token Peek => _tokens[_at];

        private Token PeekAt(int ahead) => _tokens[Math.Min(_at + ahead, _tokens.Count - 1)];

        private bool AtEnd => Peek.Kind == TokenKind.End;

        private bool Is(string text) => Peek.Kind is TokenKind.Identifier or TokenKind.Symbol && Peek.Text == text;

        private bool IsAt(int ahead, string text) => PeekAt(ahead) is { Kind: TokenKind.Identifier or TokenKind.Symbol } token && token.Text == text;

        private Token Next()
        {
            var token = Peek;
            if (!AtEnd)
            {
                _at++;
            }

            return token;
        }

        private void Expect(string text)
        {
            if (!Is(text))
            {
                throw Error($"expected '{text}'");
            }

            Next();
        }

        private string Identifier() => Peek.Kind == TokenKind.Identifier ? Next().Text : throw Error("expected a name");

        private ParseError Error(string expected) =>
            new($"line {Peek.Line}:{Peek.Column} {expected}, found {(AtEnd ? "the end of the file" : $"'{Peek.Text}'")}");

        private Span SpanOf(int start, int end) => new(_file, _tokens[start].Line, _tokens[start].Column, _tokens[end].EndLine, _tokens[end].EndColumn);

        /// <summary>Canonical text (code-reader.md): the tokens joined, with only the spaces words need; comments are no tokens.</summary>
        private string Canonical(int start, int end) => CanonicalText.Of(_tokens.Skip(start).Take(end - start + 1).Select(x => (x.Text, false)));

        private static string PackageOf(List<Token> tokens)
        {
            var depth = 0;
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                depth += token switch { { Kind: TokenKind.Symbol, Text: "{" } => 1, { Kind: TokenKind.Symbol, Text: "}" } => -1, _ => 0 };
                if (depth != 0 || token is not { Kind: TokenKind.Identifier, Text: "package" } || i > 0 && tokens[i - 1].Text is not (";" or "}"))
                {
                    continue;
                }

                var name = new StringBuilder();
                for (var j = i + 1; tokens[j].Kind == TokenKind.Identifier || tokens[j] is { Kind: TokenKind.Symbol, Text: "." }; j++)
                {
                    name.Append(tokens[j].Text);
                }

                return name.ToString().TrimStart('.');
            }

            return "";
        }

        private static List<Token> Tokenize(string text, List<string> errors)
        {
            var tokens = new List<Token>();
            var (line, column) = (1, 1);
            var i = 0;
            while (i < text.Length)
            {
                var c = text[i];
                if (c is '\r' or '\n')
                {
                    i += c == '\r' && i + 1 < text.Length && text[i + 1] == '\n' ? 2 : 1;
                    (line, column) = (line + 1, 1);
                    continue;
                }

                if (char.IsWhiteSpace(c) || c == '﻿')
                {
                    (i, column) = (i + 1, column + 1);
                    continue;
                }

                if (c == '/' && i + 1 < text.Length && text[i + 1] == '/')
                {
                    while (i < text.Length && text[i] is not ('\r' or '\n'))
                    {
                        (i, column) = (i + 1, column + 1);
                    }

                    continue;
                }

                if (c == '/' && i + 1 < text.Length && text[i + 1] == '*')
                {
                    var (commentLine, commentColumn) = (line, column);
                    (i, column) = (i + 2, column + 2);
                    while (i < text.Length && !(text[i] == '*' && i + 1 < text.Length && text[i + 1] == '/'))
                    {
                        if (text[i] is '\r' or '\n')
                        {
                            i += text[i] == '\r' && i + 1 < text.Length && text[i + 1] == '\n' ? 2 : 1;
                            (line, column) = (line + 1, 1);
                        }
                        else
                        {
                            (i, column) = (i + 1, column + 1);
                        }
                    }

                    if (i >= text.Length)
                    {
                        errors.Add($"line {commentLine}:{commentColumn} a comment isn't closed");
                    }

                    (i, column) = (Math.Min(i + 2, text.Length), column + 2);
                    continue;
                }

                var start = i;
                TokenKind kind;
                if (char.IsAsciiLetter(c) || c == '_')
                {
                    while (i < text.Length && (char.IsAsciiLetterOrDigit(text[i]) || text[i] == '_'))
                    {
                        i++;
                    }

                    kind = TokenKind.Identifier;
                }
                else if (char.IsAsciiDigit(c) || c == '.' && i + 1 < text.Length && char.IsAsciiDigit(text[i + 1]))
                {
                    var hex = c == '0' && i + 1 < text.Length && text[i + 1] is 'x' or 'X';
                    while (i < text.Length && (char.IsAsciiLetterOrDigit(text[i]) || text[i] is '_' or '.' || text[i] is '+' or '-' && !hex && text[i - 1] is 'e' or 'E'))
                    {
                        i++;
                    }

                    kind = TokenKind.Number;
                }
                else if (c is '"' or '\'')
                {
                    i++;
                    while (i < text.Length && text[i] != c && text[i] is not ('\r' or '\n'))
                    {
                        i += text[i] == '\\' && i + 1 < text.Length ? 2 : 1;
                    }

                    if (i < text.Length && text[i] == c)
                    {
                        i++;
                    }
                    else
                    {
                        errors.Add($"line {line}:{column} a string isn't closed");
                    }

                    kind = TokenKind.String;
                }
                else
                {
                    i++;
                    kind = TokenKind.Symbol;
                }

                tokens.Add(new Token(kind, text[start..i], line, column, line, column + i - start));
                column += i - start;
            }

            tokens.Add(new Token(TokenKind.End, "", line, column, line, column));
            return tokens;
        }

        /// <summary>A string token's value: its quotes removed and its escapes resolved, as protoc resolves them.</summary>
        private static string Unescape(string token)
        {
            var body = token.Length >= 2 && token[^1] == token[0] ? token[1..^1] : token[1..];
            if (!body.Contains('\\'))
            {
                return body;
            }

            var text = new StringBuilder();
            for (var i = 0; i < body.Length; i++)
            {
                if (body[i] != '\\' || i + 1 == body.Length)
                {
                    text.Append(body[i]);
                    continue;
                }

                var c = body[++i];
                switch (c)
                {
                    case 'a': text.Append('\a'); break;
                    case 'b': text.Append('\b'); break;
                    case 'f': text.Append('\f'); break;
                    case 'n': text.Append('\n'); break;
                    case 'r': text.Append('\r'); break;
                    case 't': text.Append('\t'); break;
                    case 'v': text.Append('\v'); break;
                    case 'x' or 'X' or 'u' or 'U':
                    {
                        var length = Digits(body, i + 1, c switch { 'u' => 4, 'U' => 8, _ => 2 }, char.IsAsciiHexDigit);
                        if (length == 0)
                        {
                            text.Append(c);
                            break;
                        }

                        text.Append(char.ConvertFromUtf32(int.Parse(body.AsSpan(i + 1, length), NumberStyles.HexNumber, CultureInfo.InvariantCulture)));
                        i += length;
                        break;
                    }
                    case >= '0' and <= '7':
                    {
                        var length = Digits(body, i, 3, x => x is >= '0' and <= '7');
                        text.Append((char)Convert.ToInt32(body.Substring(i, length), 8));
                        i += length - 1;
                        break;
                    }
                    default:
                        text.Append(c);
                        break;
                }
            }

            return text.ToString();
        }

        private static int Digits(string text, int from, int most, Func<char, bool> isDigit)
        {
            var length = 0;
            while (length < most && from + length < text.Length && isDigit(text[from + length]))
            {
                length++;
            }

            return length;
        }
    }
}
