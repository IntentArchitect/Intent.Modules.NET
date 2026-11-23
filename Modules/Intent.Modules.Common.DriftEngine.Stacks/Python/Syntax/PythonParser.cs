using System.Globalization;
using System.Text;

namespace Intent.Modules.Common.DriftEngine.Stacks.Python.Syntax;

/// <summary>
/// The engine's own Python parser: recursive descent over <see cref="PythonLexer"/>'s tokens, by the language reference's
/// grammar (Python 3.13: <c>match</c>, type parameters and <c>type</c> aliases, nested f-string quotes), syntax only. A
/// statement it can't read is reported and skipped with its block, and the statements after it are still read.
/// </summary>
internal sealed class PythonParser
{
    private static readonly HashSet<string> Keywords =
    [
        "False", "None", "True", "and", "as", "assert", "async", "await", "break", "class", "continue", "def", "del", "elif", "else", "except",
        "finally", "for", "from", "global", "if", "import", "in", "is", "lambda", "nonlocal", "not", "or", "pass", "raise", "return", "try",
        "while", "with", "yield"
    ];

    private static readonly HashSet<string> Augmented = ["+=", "-=", "*=", "/=", "//=", "%=", "@=", "&=", "|=", "^=", ">>=", "<<=", "**="];

    private static readonly HashSet<string> Comparisons = ["==", "!=", "<", "<=", ">", ">="];

    private readonly string _source;
    private readonly PythonLines _lines;
    private readonly List<PythonToken> _tokens;
    private int _position;

    private PythonParser(string source, PythonLines lines, List<PythonToken> tokens)
    {
        _source = source;
        _lines = lines;
        _tokens = tokens;
    }

    private List<PythonIssue> Issues { get; } = [];

    /// <summary>A file's tree, where it couldn't be read, and its tokens in order (an f-string's code is inside its string's).</summary>
    public static (PythonModule Module, IReadOnlyList<PythonIssue> Issues, IReadOnlyList<PythonToken> Tokens) Parse(string source)
    {
        var lines = new PythonLines(source);
        var lexer = new PythonLexer(source, lines);
        var parser = new PythonParser(source, lines, lexer.Tokenize());
        parser.Issues.AddRange(lexer.Issues);
        var first = parser.Current;
        var body = parser.Statements(topLevel: true);
        var module = new PythonModule { Body = body, First = first, Last = parser.LastWritten() };
        return (module, parser.Issues.Distinct().ToList(), parser._tokens);
    }

    /// <summary>The expression a string is, as a string annotation's forward reference (<c>"Order"</c>) is; null when it isn't one.</summary>
    public static PythonExpression? Expression(string text)
    {
        var lines = new PythonLines(text);
        var lexer = new PythonLexer(text, lines, 0, text.Length, expression: true);
        var parser = new PythonParser(text, lines, lexer.Tokenize());
        try
        {
            var expression = parser.Expression();
            return parser.AtEnd && lexer.Issues.Count == 0 && parser.Issues.Count == 0 ? expression : null;
        }
        catch (ParseException)
        {
            return null;
        }
    }

    private PythonToken Current => _tokens[Math.Min(_position, _tokens.Count - 1)];

    private PythonToken Peek(int ahead) => _tokens[Math.Min(_position + ahead, _tokens.Count - 1)];

    private bool AtEnd => Current.Kind == PythonTokenKind.EndOfFile;

    private PythonToken Advance()
    {
        var token = Current;
        if (_position < _tokens.Count - 1)
        {
            _position++;
        }

        return token;
    }

    private bool Match(string text)
    {
        if (!Current.Is(text))
        {
            return false;
        }

        Advance();
        return true;
    }

    private PythonToken Expect(string text) => Current.Is(text) ? Advance() : throw Error($"expected '{text}'");

    private string ExpectName() =>
        Current.Kind == PythonTokenKind.Name && !Keywords.Contains(Current.Text) ? Advance().Text : throw Error("expected a name");

    private bool IsKeyword(PythonToken token, string keyword) => token.Kind == PythonTokenKind.Name && token.Text == keyword;

    private bool IsKeyword(string keyword) => IsKeyword(Current, keyword);

    /// <summary>The last token the code writes before the current one: a node's last, past the new lines and dedents after a block.</summary>
    private PythonToken LastWritten()
    {
        for (var i = Math.Min(_position, _tokens.Count) - 1; i >= 0; i--)
        {
            if (_tokens[i].IsWritten)
            {
                return _tokens[i];
            }
        }

        return _tokens[0];
    }

    private ParseException Error(string message) => new(Current, Current.Kind switch
    {
        PythonTokenKind.Newline => $"{message} before the end of the line",
        PythonTokenKind.EndOfFile => $"{message} before the end of the file",
        PythonTokenKind.Indent => $"{message}, not an indent",
        PythonTokenKind.Dedent => $"{message} before the block ends",
        _ => $"{message}, not '{Current.Text}'"
    });

    // Statements

    private List<PythonStatement> Statements(bool topLevel)
    {
        var statements = new List<PythonStatement>();
        while (!AtEnd && !(Current.Kind == PythonTokenKind.Dedent && !topLevel))
        {
            switch (Current.Kind)
            {
                case PythonTokenKind.Newline or PythonTokenKind.Dedent:
                    Advance();
                    continue;
                case PythonTokenKind.Indent:
                    Issues.Add(new PythonIssue(Current.Line, Current.Column, "an indent no statement opens"));
                    SkipBlock();
                    continue;
            }

            var start = _position;
            try
            {
                statements.AddRange(Statement());
            }
            catch (ParseException error)
            {
                Issues.Add(new PythonIssue(error.At.Line, error.At.Column, error.Message));
                Recover();
                if (_position == start)
                {
                    Advance();
                }
            }
        }

        return statements;
    }

    /// <summary>Skips the rest of the logical line, and the block it opens.</summary>
    private void Recover()
    {
        while (Current.Kind is not (PythonTokenKind.Newline or PythonTokenKind.Dedent or PythonTokenKind.EndOfFile))
        {
            Advance();
        }

        if (Current.Kind == PythonTokenKind.Newline)
        {
            Advance();
        }

        if (Current.Kind == PythonTokenKind.Indent)
        {
            SkipBlock();
        }
    }

    private void SkipBlock()
    {
        var depth = 0;
        do
        {
            depth += Current.Kind switch { PythonTokenKind.Indent => 1, PythonTokenKind.Dedent => -1, _ => 0 };
            Advance();
        } while (depth > 0 && !AtEnd);
    }

    private IReadOnlyList<PythonStatement> Statement()
    {
        if (Current.Is("@"))
        {
            return [Decorated()];
        }

        if (Current.Kind == PythonTokenKind.Name)
        {
            switch (Current.Text)
            {
                case "def":
                    return [Function([], null)];
                case "class":
                    return [Class([], null)];
                case "if":
                    return [If()];
                case "while":
                    return [While()];
                case "for":
                    return [For(null)];
                case "try":
                    return [Try()];
                case "with":
                    return [With(null)];
                case "async" when IsKeyword(Peek(1), "def"):
                    return [Function([], Advance())];
                case "async" when IsKeyword(Peek(1), "for"):
                    return [For(Advance())];
                case "async" when IsKeyword(Peek(1), "with"):
                    return [With(Advance())];
                case "match" when IsMatch():
                    return [MatchStatement()];
            }
        }

        return SimpleStatements();
    }

    /// <summary>A body: its indented block, or the simple statements on the line after its colon.</summary>
    private IReadOnlyList<PythonStatement> Block()
    {
        Expect(":");
        if (Current.Kind != PythonTokenKind.Newline)
        {
            return SimpleStatements();
        }

        Advance();
        if (Current.Kind != PythonTokenKind.Indent)
        {
            throw Error("expected an indented block");
        }

        Advance();
        var body = Statements(topLevel: false);
        if (Current.Kind == PythonTokenKind.Dedent)
        {
            Advance();
        }

        return body;
    }

    private PythonStatement Decorated()
    {
        var first = Current;
        var decorators = new List<PythonDecorator>();
        while (Current.Is("@"))
        {
            var at = Advance();
            var expression = NamedExpression();
            decorators.Add(new PythonDecorator { Expression = expression, First = at, Last = LastWritten() });
            EndOfStatement();
        }

        if (IsKeyword("def"))
        {
            return Function(decorators, first);
        }

        if (IsKeyword("class"))
        {
            return Class(decorators, first);
        }

        if (IsKeyword("async") && IsKeyword(Peek(1), "def"))
        {
            Advance();
            return Function(decorators, first, isAsync: true);
        }

        throw Error("expected a function or class after its decorators");
    }

    /// <param name="first">Its first decorator's <c>@</c>, or its <c>async</c>; its <c>def</c> when null.</param>
    private PythonFunction Function(IReadOnlyList<PythonDecorator> decorators, PythonToken? first, bool isAsync = false)
    {
        isAsync |= first is not null && IsKeyword(first, "async");
        first ??= Current;
        Expect("def");
        var name = ExpectName();
        var typeParameters = TypeParameters();
        Expect("(");
        var parameters = Parameters(")", annotated: true);
        Expect(")");
        var returns = Match("->") ? Expression() : null;
        var body = Block();
        return new PythonFunction
        {
            Name = name, IsAsync = isAsync, TypeParameters = typeParameters, Parameters = parameters, Returns = returns, Decorators = decorators, Body = body,
            First = first, Last = LastWritten()
        };
    }

    private PythonClass Class(IReadOnlyList<PythonDecorator> decorators, PythonToken? first)
    {
        first ??= Current;
        Expect("class");
        var name = ExpectName();
        var typeParameters = TypeParameters();
        IReadOnlyList<PythonArgument> bases = [];
        if (Match("("))
        {
            bases = Arguments(")");
            Expect(")");
        }

        var body = Block();
        return new PythonClass { Name = name, TypeParameters = typeParameters, Bases = bases, Decorators = decorators, Body = body, First = first, Last = LastWritten() };
    }

    /// <summary><c>[T, *Ts, **P, N: int = 0]</c>: their names.</summary>
    private IReadOnlyList<string> TypeParameters()
    {
        if (!Match("["))
        {
            return [];
        }

        var names = new List<string>();
        while (!Current.Is("]"))
        {
            if (!Match("**"))
            {
                Match("*");
            }

            names.Add(ExpectName());
            if (Match(":"))
            {
                Expression();
            }

            if (Match("="))
            {
                Expression();
            }

            if (!Match(","))
            {
                break;
            }
        }

        Expect("]");
        return names;
    }

    /// <param name="closing">What ends them: <c>)</c> for a function's, <c>:</c> for a lambda's.</param>
    /// <param name="annotated">A function's parameters can have annotations; a lambda's can't.</param>
    private IReadOnlyList<PythonParameter> Parameters(string closing, bool annotated)
    {
        var parameters = new List<PythonParameter>();
        var kind = PythonParameterKind.Normal;
        var positionalOnly = 0;
        while (!Current.Is(closing))
        {
            var first = Current;
            if (Match("/"))
            {
                positionalOnly = parameters.Count;
            }
            else if (Match("**"))
            {
                var name = ExpectName();
                var annotation = annotated && Match(":") ? Expression() : null;
                parameters.Add(new PythonParameter { Name = name, Kind = PythonParameterKind.VarKeyword, Annotation = annotation, First = first, Last = LastWritten() });
            }
            else if (Match("*"))
            {
                if (!Current.Is(",") && !Current.Is(closing))
                {
                    var name = ExpectName();
                    var annotation = annotated && Match(":") ? StarExpression() : null;
                    parameters.Add(new PythonParameter { Name = name, Kind = PythonParameterKind.VarPositional, Annotation = annotation, First = first, Last = LastWritten() });
                }

                kind = PythonParameterKind.KeywordOnly;
            }
            else
            {
                var name = ExpectName();
                var annotation = annotated && Match(":") ? Expression() : null;
                var @default = Match("=") ? Expression() : null;
                parameters.Add(new PythonParameter { Name = name, Kind = kind, Annotation = annotation, Default = @default, First = first, Last = LastWritten() });
            }

            if (!Match(","))
            {
                break;
            }
        }

        // The parameters before a / are positional only.
        for (var i = 0; i < positionalOnly; i++)
        {
            var parameter = parameters[i];
            parameters[i] = new PythonParameter
            {
                Name = parameter.Name, Kind = PythonParameterKind.PositionalOnly, Annotation = parameter.Annotation, Default = parameter.Default, First = parameter.First, Last = parameter.Last
            };
        }

        return parameters;
    }

    /// <summary>An <c>if</c>, or an <c>elif</c>, whose <c>elif</c> or <c>else</c> is its own.</summary>
    private PythonIf If()
    {
        var first = Advance();
        var test = NamedExpression();
        var body = Block();
        IReadOnlyList<PythonStatement> orElse = IsKeyword("elif") ? [If()] : ElseBlock();
        return new PythonIf { Test = test, Body = body, OrElse = orElse, First = first, Last = LastWritten() };
    }

    private IReadOnlyList<PythonStatement> ElseBlock() => Match("else") ? Block() : [];

    private PythonWhile While()
    {
        var first = Advance();
        var test = NamedExpression();
        var body = Block();
        return new PythonWhile { Test = test, Body = body, OrElse = ElseBlock(), First = first, Last = LastWritten() };
    }

    private PythonFor For(PythonToken? async)
    {
        var first = async ?? Current;
        Expect("for");
        var target = Targets();
        Expect("in");
        var iterable = StarExpressions();
        var body = Block();
        return new PythonFor { IsAsync = async is not null, Target = target, Iterable = iterable, Body = body, OrElse = ElseBlock(), First = first, Last = LastWritten() };
    }

    private PythonTry Try()
    {
        var first = Advance();
        var body = Block();
        var handlers = new List<PythonExceptHandler>();
        while (IsKeyword("except"))
        {
            var start = Advance();
            Match("*");
            PythonExpression? type = null;
            string? name = null;
            if (!Current.Is(":"))
            {
                type = Expression();
                if (Current.Is(","))
                {
                    var items = new List<PythonExpression> { type };
                    while (Match(",") && !Current.Is(":") && !IsKeyword("as"))
                    {
                        items.Add(Expression());
                    }

                    type = new PythonSequence { Kind = "tuple", Items = items, First = items[0].First, Last = LastWritten() };
                }

                if (Match("as"))
                {
                    name = ExpectName();
                }
            }

            var handled = Block();
            handlers.Add(new PythonExceptHandler { Type = type, Name = name, Body = handled, First = start, Last = LastWritten() });
        }

        var orElse = ElseBlock();
        var @finally = Match("finally") ? Block() : [];
        return new PythonTry { Body = body, Handlers = handlers, OrElse = orElse, Finally = @finally, First = first, Last = LastWritten() };
    }

    private PythonWith With(PythonToken? async)
    {
        var first = async ?? Current;
        Expect("with");
        IReadOnlyList<PythonWithItem>? items = null;
        if (Current.Is("("))
        {
            // with (open(a) as x, open(b) as y): unless the parentheses are an expression's.
            var start = _position;
            try
            {
                Advance();
                items = WithItems(")");
                Expect(")");
                if (!Current.Is(":"))
                {
                    throw Error("expected ':'");
                }
            }
            catch (ParseException)
            {
                _position = start;
                items = null;
            }
        }

        items ??= WithItems(":");
        var body = Block();
        return new PythonWith { IsAsync = async is not null, Items = items, Body = body, First = first, Last = LastWritten() };
    }

    private List<PythonWithItem> WithItems(string closing)
    {
        var items = new List<PythonWithItem>();
        do
        {
            if (Current.Is(closing))
            {
                break;
            }

            var first = Current;
            var context = Expression();
            var target = Match("as") ? Target() : null;
            items.Add(new PythonWithItem { Context = context, Target = target, First = first, Last = LastWritten() });
        } while (Match(","));

        return items;
    }

    /// <summary>A soft keyword's <c>match</c> starts a statement when its line ends in a colon that opens a block.</summary>
    private bool IsMatch()
    {
        if (Peek(1).Is("=") || Peek(1).Is(".") || Peek(1).Is(":") || Peek(1).Kind is PythonTokenKind.Newline or PythonTokenKind.EndOfFile || Augmented.Contains(Peek(1).Text))
        {
            return false;
        }

        for (var i = _position + 1; i < _tokens.Count - 1; i++)
        {
            if (_tokens[i].Kind == PythonTokenKind.Newline)
            {
                return _tokens[i - 1].Is(":") && _tokens[i + 1].Kind == PythonTokenKind.Indent;
            }
        }

        return false;
    }

    private PythonMatch MatchStatement()
    {
        var first = Advance();
        var subject = StarExpressions();
        Expect(":");
        EndOfStatement();
        if (Current.Kind != PythonTokenKind.Indent)
        {
            throw Error("expected an indented block of cases");
        }

        Advance();
        var cases = new List<PythonCase>();
        while (Current.Kind is not (PythonTokenKind.Dedent or PythonTokenKind.EndOfFile))
        {
            var start = _position;
            try
            {
                if (!(Current.Kind == PythonTokenKind.Name && Current.Text == "case"))
                {
                    throw Error("expected a case");
                }

                cases.Add(Case());
            }
            catch (ParseException error)
            {
                Issues.Add(new PythonIssue(error.At.Line, error.At.Column, error.Message));
                Recover();
                if (_position == start)
                {
                    Advance();
                }
            }
        }

        if (Current.Kind == PythonTokenKind.Dedent)
        {
            Advance();
        }

        return new PythonMatch { Subject = subject, Cases = cases, First = first, Last = LastWritten() };
    }

    private PythonCase Case()
    {
        var first = Advance();
        var start = _position;
        PythonExpression? pattern;
        try
        {
            pattern = Pattern();
        }
        catch (ParseException)
        {
            // A pattern no expression is written like: skip to its colon.
            _position = start;
            pattern = null;
            var depth = 0;
            while (!AtEnd && Current.Kind != PythonTokenKind.Newline && !(depth == 0 && (Current.Is(":") || IsKeyword("if"))))
            {
                depth += Current.Text switch { "(" or "[" or "{" => 1, ")" or "]" or "}" => -1, _ => 0 };
                Advance();
            }
        }

        var guard = Match("if") ? NamedExpression() : null;
        var body = Block();
        return new PythonCase { Pattern = pattern, Guard = guard, Body = body, First = first, Last = LastWritten() };
    }

    /// <summary>A case's pattern, as the expression it is written like; <c>as name</c> binds a name, so is read past.</summary>
    private PythonExpression Pattern()
    {
        var first = Current;
        var items = new List<PythonExpression> { PatternItem() };
        while (Match(",") && !Current.Is(":") && !IsKeyword("if"))
        {
            items.Add(PatternItem());
        }

        return items.Count == 1 && !_tokens[_position - 1].Is(",") ? items[0] : new PythonSequence { Kind = "tuple", Items = items, First = first, Last = LastWritten() };
    }

    /// <summary>A pattern's item: no conditional expression, as an <c>if</c> after it is the case's guard.</summary>
    private PythonExpression PatternItem()
    {
        var item = Current.Is("*") ? StarExpression() : Disjunction();
        if (Match("as"))
        {
            ExpectName();
        }

        return item;
    }

    private IReadOnlyList<PythonStatement> SimpleStatements()
    {
        var statements = new List<PythonStatement> { SimpleStatement() };
        while (Match(";") && Current.Kind is not (PythonTokenKind.Newline or PythonTokenKind.EndOfFile or PythonTokenKind.Dedent))
        {
            statements.Add(SimpleStatement());
        }

        EndOfStatement();
        return statements;
    }

    private void EndOfStatement()
    {
        if (Current.Kind == PythonTokenKind.Newline)
        {
            Advance();
        }
        else if (Current.Kind is not (PythonTokenKind.EndOfFile or PythonTokenKind.Dedent))
        {
            throw Error("expected the end of the statement");
        }
    }

    private bool AtStatementEnd => Current.Kind is PythonTokenKind.Newline or PythonTokenKind.EndOfFile or PythonTokenKind.Dedent || Current.Is(";");

    private PythonStatement SimpleStatement()
    {
        var first = Current;
        if (Current.Kind == PythonTokenKind.Name)
        {
            switch (Current.Text)
            {
                case "pass" or "break" or "continue":
                    Advance();
                    return new PythonKeywordStatement { Keyword = first.Text, First = first, Last = first };
                case "return":
                {
                    Advance();
                    var value = AtStatementEnd ? null : StarExpressions();
                    return new PythonReturn { Value = value, First = first, Last = LastWritten() };
                }
                case "import":
                    return Import();
                case "from":
                    return ImportFrom();
                case "raise":
                {
                    Advance();
                    var values = new List<PythonExpression>();
                    if (!AtStatementEnd)
                    {
                        values.Add(Expression());
                        if (Match("from"))
                        {
                            values.Add(Expression());
                        }
                    }

                    return new PythonKeywordStatement { Keyword = "raise", Values = values, First = first, Last = LastWritten() };
                }
                case "global" or "nonlocal":
                {
                    Advance();
                    var names = new List<PythonExpression>();
                    do
                    {
                        var name = Current;
                        names.Add(new PythonName { Id = ExpectName(), First = name, Last = name });
                    } while (Match(","));

                    return new PythonKeywordStatement { Keyword = first.Text, Values = names, First = first, Last = LastWritten() };
                }
                case "del":
                {
                    Advance();
                    var targets = StarExpressions();
                    return new PythonKeywordStatement { Keyword = "del", Values = targets is PythonSequence { Kind: "tuple" } tuple ? tuple.Items : [targets], First = first, Last = LastWritten() };
                }
                case "assert":
                {
                    Advance();
                    var values = new List<PythonExpression> { Expression() };
                    if (Match(","))
                    {
                        values.Add(Expression());
                    }

                    return new PythonKeywordStatement { Keyword = "assert", Values = values, First = first, Last = LastWritten() };
                }
                case "type" when Peek(1).Kind == PythonTokenKind.Name && !Keywords.Contains(Peek(1).Text) && (Peek(2).Is("=") || Peek(2).Is("[")):
                {
                    Advance();
                    var name = ExpectName();
                    var typeParameters = TypeParameters();
                    Expect("=");
                    var value = Expression();
                    return new PythonTypeAlias { Name = name, TypeParameters = typeParameters, Value = value, First = first, Last = LastWritten() };
                }
            }
        }

        var target = YieldOrStarExpressions();
        if (Match(":"))
        {
            var annotation = Expression();
            var value = Match("=") ? YieldOrStarExpressions() : null;
            return new PythonAnnotatedAssign { Target = target, Annotation = annotation, Value = value, First = first, Last = LastWritten() };
        }

        if (Current.Kind == PythonTokenKind.Operator && Augmented.Contains(Current.Text))
        {
            var op = Advance().Text;
            var value = YieldOrStarExpressions();
            return new PythonAugmentedAssign { Target = target, Operator = op, Value = value, First = first, Last = LastWritten() };
        }

        if (Current.Is("="))
        {
            var targets = new List<PythonExpression> { target };
            while (Match("="))
            {
                targets.Add(YieldOrStarExpressions());
            }

            return new PythonAssign { Targets = targets.Take(targets.Count - 1).ToList(), Value = targets[^1], First = first, Last = LastWritten() };
        }

        return new PythonExpressionStatement { Value = target, First = first, Last = LastWritten() };
    }

    private PythonImport Import()
    {
        var first = Advance();
        var names = new List<PythonAlias>();
        do
        {
            var start = Current;
            var name = DottedName();
            var alias = Match("as") ? ExpectName() : null;
            names.Add(new PythonAlias { Name = name, AsName = alias, First = start, Last = LastWritten() });
        } while (Match(","));

        return new PythonImport { Names = names, First = first, Last = LastWritten() };
    }

    private PythonImportFrom ImportFrom()
    {
        var first = Advance();
        var level = 0;
        while (Current.Is(".") || Current.Is("..."))
        {
            level += Advance().Text.Length;
        }

        var module = Current.Kind == PythonTokenKind.Name && !IsKeyword("import") ? DottedName() : null;
        if (module is null && level == 0)
        {
            throw Error("expected a module");
        }

        Expect("import");
        if (Match("*"))
        {
            return new PythonImportFrom { Module = module, Level = level, Names = [], IsWildcard = true, First = first, Last = LastWritten() };
        }

        var parenthesized = Match("(");
        var names = new List<PythonAlias>();
        do
        {
            if (parenthesized && Current.Is(")"))
            {
                break;
            }

            var start = Current;
            var name = ExpectName();
            var alias = Match("as") ? ExpectName() : null;
            names.Add(new PythonAlias { Name = name, AsName = alias, First = start, Last = LastWritten() });
        } while (Match(","));

        if (parenthesized)
        {
            Expect(")");
        }

        return new PythonImportFrom { Module = module, Level = level, Names = names, First = first, Last = LastWritten() };
    }

    private string DottedName()
    {
        var name = new StringBuilder(ExpectName());
        while (Current.Is(".") && Peek(1).Kind == PythonTokenKind.Name)
        {
            Advance();
            name.Append('.').Append(ExpectName());
        }

        return name.ToString();
    }

    // Expressions

    private bool StartsExpression(PythonToken token) => token.Kind switch
    {
        PythonTokenKind.Name => !Keywords.Contains(token.Text) || token.Text is "True" or "False" or "None" or "lambda" or "not" or "await" or "yield",
        PythonTokenKind.Number or PythonTokenKind.String => true,
        PythonTokenKind.Operator => token.Text is "(" or "[" or "{" or "-" or "+" or "~" or "*" or "**" or "...",
        _ => false
    };

    private PythonExpression YieldOrStarExpressions() => IsKeyword("yield") ? Yield() : StarExpressions();

    /// <summary>Expressions separated by commas, a tuple when there is a comma: <c>a, *rest</c>.</summary>
    private PythonExpression StarExpressions()
    {
        var first = Current;
        var item = StarExpression();
        if (!Current.Is(","))
        {
            return item;
        }

        var items = new List<PythonExpression> { item };
        while (Match(",") && StartsExpression(Current))
        {
            items.Add(StarExpression());
        }

        return new PythonSequence { Kind = "tuple", Items = items, First = first, Last = LastWritten() };
    }

    private PythonExpression StarExpression()
    {
        if (!Current.Is("*"))
        {
            return NamedExpression();
        }

        var first = Advance();
        var value = BitOr();
        return new PythonStarred { Value = value, First = first, Last = LastWritten() };
    }

    /// <summary>The targets of a <c>for</c> or a comprehension, up to its <c>in</c>.</summary>
    private PythonExpression Targets()
    {
        var first = Current;
        var item = Target();
        if (!Current.Is(","))
        {
            return item;
        }

        var items = new List<PythonExpression> { item };
        while (Match(",") && !IsKeyword("in") && !Current.Is("="))
        {
            items.Add(Target());
        }

        return new PythonSequence { Kind = "tuple", Items = items, First = first, Last = LastWritten() };
    }

    private PythonExpression Target()
    {
        if (!Current.Is("*"))
        {
            return BitOr();
        }

        var first = Advance();
        var value = BitOr();
        return new PythonStarred { Value = value, First = first, Last = LastWritten() };
    }

    private PythonExpression NamedExpression()
    {
        if (Current.Kind != PythonTokenKind.Name || Keywords.Contains(Current.Text) || !Peek(1).Is(":="))
        {
            return Expression();
        }

        var name = Advance();
        Advance();
        var value = Expression();
        return new PythonNamedExpression { Target = new PythonName { Id = name.Text, First = name, Last = name }, Value = value, First = name, Last = LastWritten() };
    }

    private PythonExpression Expression()
    {
        if (IsKeyword("lambda"))
        {
            return Lambda();
        }

        var first = Current;
        var body = Disjunction();
        if (!Match("if"))
        {
            return body;
        }

        var test = Disjunction();
        Expect("else");
        var orElse = Expression();
        return new PythonConditional { Body = body, Test = test, OrElse = orElse, First = first, Last = LastWritten() };
    }

    private PythonLambda Lambda()
    {
        var first = Advance();
        var parameters = Parameters(":", annotated: false);
        Expect(":");
        var body = Expression();
        return new PythonLambda { Parameters = parameters, Body = body, First = first, Last = LastWritten() };
    }

    private PythonExpression Disjunction() => Binary(Conjunction, "or");

    private PythonExpression Conjunction() => Binary(Inversion, "and");

    private PythonExpression Inversion()
    {
        if (!IsKeyword("not"))
        {
            return Comparison();
        }

        var first = Advance();
        var operand = Inversion();
        return new PythonUnary { Operator = "not", Operand = operand, First = first, Last = LastWritten() };
    }

    private PythonExpression Comparison()
    {
        var first = Current;
        var left = BitOr();
        var operators = new List<string>();
        var comparators = new List<PythonExpression>();
        while (true)
        {
            string op;
            if (Current.Kind == PythonTokenKind.Operator && Comparisons.Contains(Current.Text))
            {
                op = Advance().Text;
            }
            else if (IsKeyword("in"))
            {
                op = Advance().Text;
            }
            else if (IsKeyword("not") && IsKeyword(Peek(1), "in"))
            {
                Advance();
                Advance();
                op = "not in";
            }
            else if (IsKeyword("is"))
            {
                Advance();
                op = Match("not") ? "is not" : "is";
            }
            else
            {
                break;
            }

            operators.Add(op);
            comparators.Add(BitOr());
        }

        return operators.Count == 0 ? left : new PythonCompare { Left = left, Operators = operators, Comparators = comparators, First = first, Last = LastWritten() };
    }

    private PythonExpression BitOr() => Binary(BitXor, "|");

    private PythonExpression BitXor() => Binary(BitAnd, "^");

    private PythonExpression BitAnd() => Binary(Shift, "&");

    private PythonExpression Shift() => Binary(Sum, "<<", ">>");

    private PythonExpression Sum() => Binary(Term, "+", "-");

    private PythonExpression Term() => Binary(Factor, "*", "/", "//", "%", "@");

    /// <summary>A left-associative level of binary operators; <c>and</c> and <c>or</c> are keywords, the rest operators.</summary>
    private PythonExpression Binary(Func<PythonExpression> operand, params string[] operators)
    {
        var first = Current;
        var left = operand();
        while (Current.Kind is PythonTokenKind.Operator or PythonTokenKind.Name && operators.Contains(Current.Text))
        {
            var op = Advance().Text;
            var right = operand();
            left = new PythonBinary { Left = left, Operator = op, Right = right, First = first, Last = LastWritten() };
        }

        return left;
    }

    private PythonExpression Factor()
    {
        if (!(Current.Is("-") || Current.Is("+") || Current.Is("~")))
        {
            return Power();
        }

        var first = Advance();
        var operand = Factor();
        return new PythonUnary { Operator = first.Text, Operand = operand, First = first, Last = LastWritten() };
    }

    private PythonExpression Power()
    {
        var first = Current;
        var value = AwaitPrimary();
        if (!Match("**"))
        {
            return value;
        }

        var exponent = Factor();
        return new PythonBinary { Left = value, Operator = "**", Right = exponent, First = first, Last = LastWritten() };
    }

    private PythonExpression AwaitPrimary()
    {
        if (!IsKeyword("await"))
        {
            return Primary();
        }

        var first = Advance();
        var value = Primary();
        return new PythonAwait { Value = value, First = first, Last = LastWritten() };
    }

    private PythonExpression Primary()
    {
        var first = Current;
        var value = Atom();
        while (true)
        {
            if (Match("."))
            {
                var name = Current.Kind == PythonTokenKind.Name ? Advance().Text : throw Error("expected a name");
                value = new PythonAttribute { Value = value, Name = name, First = first, Last = LastWritten() };
            }
            else if (Match("("))
            {
                var arguments = Arguments(")");
                Expect(")");
                value = new PythonCall { Function = value, Arguments = arguments, First = first, Last = LastWritten() };
            }
            else if (Match("["))
            {
                var index = Slices();
                Expect("]");
                value = new PythonSubscript { Value = value, Index = index, First = first, Last = LastWritten() };
            }
            else
            {
                return value;
            }
        }
    }

    private PythonExpression Atom()
    {
        var token = Current;
        switch (token.Kind)
        {
            case PythonTokenKind.Name when token.Text is "True" or "False" or "None":
                Advance();
                return new PythonConstant { Value = token.Text, First = token, Last = token };
            case PythonTokenKind.Name when !Keywords.Contains(token.Text):
                Advance();
                return new PythonName { Id = token.Text, First = token, Last = token };
            case PythonTokenKind.Number:
                Advance();
                return new PythonNumber { Text = token.Text, First = token, Last = token };
            case PythonTokenKind.String:
                return Strings();
            case PythonTokenKind.Operator when token.Text == "...":
                Advance();
                return new PythonConstant { Value = "...", First = token, Last = token };
            case PythonTokenKind.Operator when token.Text == "(":
                return Parenthesized();
            case PythonTokenKind.Operator when token.Text == "[":
                return Bracketed();
            case PythonTokenKind.Operator when token.Text == "{":
                return Braced();
            default:
                throw Error("expected an expression");
        }
    }

    private PythonExpression StarNamedExpression() => Current.Is("*") ? StarExpression() : NamedExpression();

    private bool StartsComprehension => IsKeyword("for") || IsKeyword("async") && IsKeyword(Peek(1), "for");

    private PythonExpression Parenthesized()
    {
        var first = Advance();
        if (Match(")"))
        {
            return new PythonSequence { Kind = "tuple", Items = [], First = first, Last = LastWritten() };
        }

        if (IsKeyword("yield"))
        {
            var yielded = Yield();
            Expect(")");
            return yielded;
        }

        var item = StarNamedExpression();
        if (StartsComprehension)
        {
            var clauses = Clauses();
            Expect(")");
            return new PythonComprehension { Kind = "generator", Element = item, Clauses = clauses, First = first, Last = LastWritten() };
        }

        if (!Current.Is(","))
        {
            Expect(")");
            return item;
        }

        var items = new List<PythonExpression> { item };
        while (Match(",") && !Current.Is(")"))
        {
            items.Add(StarNamedExpression());
        }

        Expect(")");
        return new PythonSequence { Kind = "tuple", Items = items, First = first, Last = LastWritten() };
    }

    private PythonExpression Bracketed()
    {
        var first = Advance();
        var items = new List<PythonExpression>();
        if (!Current.Is("]"))
        {
            items.Add(StarNamedExpression());
            if (StartsComprehension)
            {
                var clauses = Clauses();
                Expect("]");
                return new PythonComprehension { Kind = "list", Element = items[0], Clauses = clauses, First = first, Last = LastWritten() };
            }

            while (Match(",") && !Current.Is("]"))
            {
                items.Add(StarNamedExpression());
            }
        }

        Expect("]");
        return new PythonSequence { Kind = "list", Items = items, First = first, Last = LastWritten() };
    }

    private PythonExpression Braced()
    {
        var first = Advance();
        if (Match("}"))
        {
            return new PythonDict { Entries = [], First = first, Last = LastWritten() };
        }

        if (Current.Is("**") || !Current.Is("*") && IsDictEntry())
        {
            var entries = new List<PythonDictEntry>();
            do
            {
                if (Current.Is("}"))
                {
                    break;
                }

                if (Match("**"))
                {
                    entries.Add(new PythonDictEntry(null, BitOr()));
                    continue;
                }

                var key = Expression();
                Expect(":");
                var value = Expression();
                if (entries.Count == 0 && StartsComprehension)
                {
                    var clauses = Clauses();
                    Expect("}");
                    return new PythonComprehension { Kind = "dict", Element = key, Value = value, Clauses = clauses, First = first, Last = LastWritten() };
                }

                entries.Add(new PythonDictEntry(key, value));
            } while (Match(","));

            Expect("}");
            return new PythonDict { Entries = entries, First = first, Last = LastWritten() };
        }

        var items = new List<PythonExpression> { StarNamedExpression() };
        if (StartsComprehension)
        {
            var clauses = Clauses();
            Expect("}");
            return new PythonComprehension { Kind = "set", Element = items[0], Clauses = clauses, First = first, Last = LastWritten() };
        }

        while (Match(",") && !Current.Is("}"))
        {
            items.Add(StarNamedExpression());
        }

        Expect("}");
        return new PythonSequence { Kind = "set", Items = items, First = first, Last = LastWritten() };
    }

    /// <summary>A brace display is a dict when its first item has a colon after it.</summary>
    private bool IsDictEntry()
    {
        var start = _position;
        try
        {
            Expression();
            return Current.Is(":");
        }
        catch (ParseException)
        {
            return false;
        }
        finally
        {
            _position = start;
        }
    }

    private List<PythonComprehensionClause> Clauses()
    {
        var clauses = new List<PythonComprehensionClause>();
        while (StartsComprehension)
        {
            var first = Current;
            var isAsync = Match("async");
            Expect("for");
            var target = Targets();
            Expect("in");
            var iterable = Disjunction();
            var conditions = new List<PythonExpression>();
            while (Match("if"))
            {
                conditions.Add(Disjunction());
            }

            clauses.Add(new PythonComprehensionClause { IsAsync = isAsync, Target = target, Iterable = iterable, Conditions = conditions, First = first, Last = LastWritten() });
        }

        return clauses;
    }

    private PythonYield Yield()
    {
        var first = Advance();
        if (Match("from"))
        {
            var from = Expression();
            return new PythonYield { Value = from, IsFrom = true, First = first, Last = LastWritten() };
        }

        var value = StartsExpression(Current) ? StarExpressions() : null;
        return new PythonYield { Value = value, First = first, Last = LastWritten() };
    }

    /// <summary>A call's arguments, or a class's bases: positional, <c>name=value</c>, <c>*items</c> and <c>**options</c>; a generator alone.</summary>
    private IReadOnlyList<PythonArgument> Arguments(string closing)
    {
        var arguments = new List<PythonArgument>();
        while (!Current.Is(closing))
        {
            var first = Current;
            if (Match("*") || Match("**"))
            {
                var value = Expression();
                arguments.Add(new PythonArgument { Value = value, Stars = first.Text.Length, First = first, Last = LastWritten() });
            }
            else if (Current.Kind == PythonTokenKind.Name && !Keywords.Contains(Current.Text) && Peek(1).Is("="))
            {
                var name = Advance().Text;
                Advance();
                var value = Expression();
                arguments.Add(new PythonArgument { Name = name, Value = value, First = first, Last = LastWritten() });
            }
            else
            {
                var value = NamedExpression();
                if (StartsComprehension)
                {
                    var clauses = Clauses();
                    value = new PythonComprehension { Kind = "generator", Element = value, Clauses = clauses, First = first, Last = LastWritten() };
                }

                arguments.Add(new PythonArgument { Value = value, First = first, Last = LastWritten() });
            }

            if (!Match(","))
            {
                break;
            }
        }

        return arguments;
    }

    private PythonExpression Slices()
    {
        var first = Current;
        var item = Slice();
        if (!Current.Is(","))
        {
            return item;
        }

        var items = new List<PythonExpression> { item };
        while (Match(",") && !Current.Is("]"))
        {
            items.Add(Slice());
        }

        return new PythonSequence { Kind = "tuple", Items = items, First = first, Last = LastWritten() };
    }

    private PythonExpression Slice()
    {
        if (Current.Is("*"))
        {
            return StarExpression();
        }

        var first = Current;
        var lower = Current.Is(":") ? null : NamedExpression();
        if (!Match(":"))
        {
            return lower!;
        }

        var upper = Current.Is(":") || Current.Is(",") || Current.Is("]") ? null : Expression();
        PythonExpression? step = null;
        if (Match(":") && !Current.Is(",") && !Current.Is("]"))
        {
            step = Expression();
        }

        return new PythonSlice { Lower = lower, Upper = upper, Step = step, First = first, Last = LastWritten() };
    }

    // Strings

    /// <summary>A string and the strings written next to it, joined: their text and their f-strings' fields, in order.</summary>
    private PythonString Strings()
    {
        var first = Current;
        var parts = new List<PythonStringPart>();
        var bytes = false;
        var formatted = false;
        while (Current.Kind == PythonTokenKind.String)
        {
            var token = Advance();
            var quote = token.Text.IndexOfAny(['"', '\'']);
            var prefix = token.Text[..quote].ToLowerInvariant();
            var raw = prefix.Contains('r');
            bytes |= prefix.Contains('b');
            var isFormatted = prefix.Contains('f') || prefix.Contains('t');
            formatted |= isFormatted;

            var mark = token.Text[quote];
            var triple = token.Text.Length >= quote + 6 && token.Text[quote + 1] == mark && token.Text[quote + 2] == mark;
            var open = token.Start + quote + (triple ? 3 : 1);
            var closed = token.Text.Length >= quote + (triple ? 6 : 2) && token.Text.EndsWith(new string(mark, triple ? 3 : 1), StringComparison.Ordinal);
            var close = token.End - (closed ? triple ? 3 : 1 : 0);
            if (isFormatted)
            {
                Formatted(open, close, raw, triple, parts);
            }
            else
            {
                Text(parts, raw ? Lines(_source[open..close]) : Unescape(Lines(_source[open..close]), bytes));
            }
        }

        return new PythonString { Parts = parts, IsBytes = bytes, IsFormatted = formatted, First = first, Last = LastWritten() };
    }

    /// <summary>A string's line breaks as Python reads its source: each <c>\r\n</c> or <c>\r</c> is <c>\n</c>.</summary>
    private static string Lines(string text) => text.Replace("\r\n", "\n").Replace('\r', '\n');

    private static void Text(List<PythonStringPart> parts, string text)
    {
        if (text.Length == 0)
        {
            return;
        }

        if (parts.Count > 0 && parts[^1].Text is { } before)
        {
            parts[^1] = new PythonStringPart(before + text);
        }
        else
        {
            parts.Add(new PythonStringPart(text));
        }
    }

    /// <summary>An f-string's body: its text, with <c>{{</c> and <c>}}</c> as braces, and each replacement field's code parsed.</summary>
    private void Formatted(int open, int close, bool raw, bool triple, List<PythonStringPart> parts)
    {
        var scanner = new PythonLexer(_source, _lines, 0, close, expression: true);
        var text = new StringBuilder();
        var i = open;
        while (i < close)
        {
            var c = _source[i];
            if (c == '\\' && i + 1 < close)
            {
                text.Append(_source, i, 2);
                i += 2;
            }
            else if (c is '{' or '}' && i + 1 < close && _source[i + 1] == c)
            {
                Text(parts, raw ? Lines(text.ToString()) : Unescape(Lines(text.ToString()), false));
                text.Clear();
                Text(parts, c.ToString());
                i += 2;
            }
            else if (c == '{')
            {
                Text(parts, raw ? Lines(text.ToString()) : Unescape(Lines(text.ToString()), false));
                text.Clear();
                var end = scanner.FieldEnd(i, triple);
                if (end < 0)
                {
                    break;
                }

                parts.Add(Field(i + 1, end - 1));
                i = end;
            }
            else
            {
                text.Append(c);
                i++;
            }
        }

        Text(parts, raw ? Lines(text.ToString()) : Unescape(Lines(text.ToString()), false));
    }

    /// <summary>A replacement field: its code, up to a <c>!</c> conversion or a <c>:</c> format spec, without a trailing <c>=</c>.</summary>
    private PythonStringPart Field(int start, int end)
    {
        var scanner = new PythonLexer(_source, _lines, 0, end, expression: true);
        var depth = 0;
        var i = start;
        while (i < end)
        {
            var c = _source[i];
            if (c is '"' or '\'')
            {
                var after = scanner.StringEnd(i, false);
                i = after < 0 ? end : after;
                continue;
            }

            if (c is '(' or '[' or '{')
            {
                depth++;
            }
            else if (c is ')' or ']' or '}')
            {
                depth--;
            }
            else if (depth == 0 && (c == ':' || c == '!' && i + 1 < end && _source[i + 1] != '='))
            {
                break;
            }

            i++;
        }

        var code = i;
        string? conversion = null;
        string? spec = null;
        if (code < end && _source[code] == '!')
        {
            var colon = _source.IndexOf(':', code, end - code);
            conversion = _source[(code + 1)..(colon < 0 ? end : colon)];
            spec = colon < 0 ? null : _source[(colon + 1)..end];
        }
        else if (code < end)
        {
            spec = _source[(code + 1)..end];
        }

        // {total=} shows the code before its value.
        var trimmed = code;
        while (trimmed > start && char.IsWhiteSpace(_source[trimmed - 1]))
        {
            trimmed--;
        }

        if (trimmed > start && _source[trimmed - 1] == '=' && !(trimmed - 2 >= start && _source[trimmed - 2] is '=' or '!' or '<' or '>'))
        {
            code = trimmed - 1;
        }

        var lexer = new PythonLexer(_source, _lines, start, code, expression: true);
        var parser = new PythonParser(_source, _lines, lexer.Tokenize());
        Issues.AddRange(lexer.Issues);
        try
        {
            var expression = parser.YieldOrStarExpressions();
            if (!parser.AtEnd)
            {
                throw parser.Error("expected the end of the replacement field");
            }

            return new PythonStringPart(null, expression, conversion, spec);
        }
        catch (ParseException error)
        {
            Issues.Add(new PythonIssue(error.At.Line, error.At.Column, error.Message));
            return new PythonStringPart(null, new PythonName { Id = _source[start..code].Trim(), First = parser._tokens[0], Last = parser._tokens[0] }, conversion, spec);
        }
    }

    /// <summary>
    /// A string's escapes resolved: <c>\n</c> and the other single characters, octal, <c>\x</c>, and for text <c>\u</c>,
    /// <c>\U</c> and <c>\N{...}</c>, which stays as written; a backslash before a line break joins the lines. An unknown
    /// escape keeps its backslash, as Python does.
    /// </summary>
    internal static string Unescape(string text, bool bytes)
    {
        if (!text.Contains('\\'))
        {
            return text;
        }

        var result = new StringBuilder(text.Length);
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] != '\\' || i + 1 >= text.Length)
            {
                result.Append(text[i]);
                continue;
            }

            var c = text[++i];
            switch (c)
            {
                case '\n':
                    break;
                case '\r':
                    if (i + 1 < text.Length && text[i + 1] == '\n')
                    {
                        i++;
                    }

                    break;
                case '\\' or '\'' or '"':
                    result.Append(c);
                    break;
                case 'a': result.Append('\a'); break;
                case 'b': result.Append('\b'); break;
                case 'f': result.Append('\f'); break;
                case 'n': result.Append('\n'); break;
                case 'r': result.Append('\r'); break;
                case 't': result.Append('\t'); break;
                case 'v': result.Append('\v'); break;
                case >= '0' and <= '7':
                {
                    var length = 1;
                    while (length < 3 && i + length < text.Length && text[i + length] is >= '0' and <= '7')
                    {
                        length++;
                    }

                    result.Append((char)Convert.ToInt32(text.Substring(i, length), 8));
                    i += length - 1;
                    break;
                }
                case 'x' when Hex(text, i + 1, 2) is { } code:
                    result.Append((char)code);
                    i += 2;
                    break;
                case 'u' when !bytes && Hex(text, i + 1, 4) is { } code:
                    result.Append((char)code);
                    i += 4;
                    break;
                case 'U' when !bytes && Hex(text, i + 1, 8) is { } code && code <= 0x10FFFF:
                    result.Append(char.ConvertFromUtf32(code));
                    i += 8;
                    break;
                default:
                    result.Append('\\').Append(c);
                    break;
            }
        }

        return result.ToString();
    }

    private static int? Hex(string text, int start, int length) =>
        start + length <= text.Length && int.TryParse(text.AsSpan(start, length), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var value) ? value : null;

    private sealed class ParseException(PythonToken at, string message) : Exception(message)
    {
        public PythonToken At { get; } = at;
    }
}
