using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftEngine.Code;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.CSharp;

/// <summary>
/// What a method's or constructor's body does, read from its syntax once every type is read: each call, the type of what
/// it is made on, and what its arguments are (<see cref="ValueNode"/>), as far as the code says without running it.
/// </summary>
internal sealed class CSharpBody(SyntaxNode member, TypeNode owner, string file, CodeGraph graph)
{
    /// <summary>How many locals deep a value is followed (<c>var a = b; var b = new X();</c>).</summary>
    private const int MaxDepth = 3;

    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.CultureInvariant);

    private static readonly (List<TypeRefNode> Types, bool Known) Unknown = ([], false);

    public IReadOnlyList<CallNode> Calls() =>
        member.DescendantNodes().OfType<InvocationExpressionSyntax>().Select(Call).OfType<CallNode>().ToList();

    private CallNode? Call(InvocationExpressionSyntax invocation)
    {
        var (name, receiver) = invocation.Expression switch
        {
            SimpleNameSyntax simple => (simple.Identifier.Text, null),
            MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } access => (access.Name.Identifier.Text, null),
            MemberAccessExpressionSyntax access => (access.Name.Identifier.Text, access.Expression),
            _ => ((string?)null, (ExpressionSyntax?)null)
        };
        if (name is null)
        {
            return null;
        }

        var receiverType = receiver is null ? Unknown : TypesOf(receiver, invocation.SpanStart, 0);
        return new CallNode
        {
            Name = name,
            Receiver = receiver is null ? null : Normalise(receiver),
            ReceiverType = receiverType is { Known: true, Types: [var only] } ? only : null,
            Arguments = invocation.ArgumentList.Arguments.Select(x => Value(x.Expression, invocation.SpanStart)).ToList(),
            Text = Normalise(invocation),
            At = CSharpReader.SpanOf(invocation, file)
        };
    }

    private ValueNode Value(ExpressionSyntax expression, int before)
    {
        var (types, known) = TypesOf(expression, before, 0);
        return new ValueNode { Text = Normalise(expression), Types = types, IsKnown = known && types.Count > 0, At = CSharpReader.SpanOf(expression, file) };
    }

    /// <summary>The types an expression can be, and whether every way it is given is known (<see cref="ValueNode"/>).</summary>
    private (List<TypeRefNode> Types, bool Known) TypesOf(ExpressionSyntax expression, int before, int depth)
    {
        switch (expression)
        {
            case ParenthesizedExpressionSyntax parenthesized:
                return TypesOf(parenthesized.Expression, before, depth);
            case ObjectCreationExpressionSyntax creation:
                return ([CSharpReader.TypeRef(creation.Type)], true);
            case CastExpressionSyntax cast:
                return OrDeclared(TypesOf(cast.Expression, before, depth), cast.Type);
            case BinaryExpressionSyntax { Right: TypeSyntax type } binary when binary.IsKind(SyntaxKind.AsExpression):
                return OrDeclared(TypesOf(binary.Left, before, depth), type);
            case ConditionalExpressionSyntax conditional:
            {
                var whenTrue = TypesOf(conditional.WhenTrue, before, depth);
                var whenFalse = TypesOf(conditional.WhenFalse, before, depth);
                return ([..whenTrue.Types, ..whenFalse.Types], whenTrue.Known && whenFalse.Known);
            }
            case ThisExpressionSyntax:
                return ([OwnerRef()], true);
            case IdentifierNameSyntax identifier when depth < MaxDepth:
                return Named(identifier, before, depth);
            case MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } access:
                return FieldOf(owner, access.Name.Identifier.Text);
            case MemberAccessExpressionSyntax access when depth < MaxDepth:
            {
                // A property path through in-scope types: services.Mediator is OrderServices' Mediator.
                var (types, known) = TypesOf(access.Expression, before, depth + 1);
                return known && types is [var only] && graph.TypeFor(only, owner) is { } type ? FieldOf(type, access.Name.Identifier.Text) : Unknown;
            }
            default:
                return Unknown;
        }
    }

    /// <summary>What a cast or a typed local wraps says more than its declared type, which is what it is when that isn't known.</summary>
    private static (List<TypeRefNode> Types, bool Known) OrDeclared((List<TypeRefNode> Types, bool Known) value, TypeSyntax declared) =>
        value.Known && value.Types.Count > 0 ? value : ([CSharpReader.TypeRef(declared)], true);

    /// <summary>
    /// A name in the body: a local declared before the use, then a parameter (the type's primary constructor's included), then
    /// a field or property of the type.
    /// </summary>
    private (List<TypeRefNode> Types, bool Known) Named(IdentifierNameSyntax identifier, int before, int depth)
    {
        var name = identifier.Identifier.Text;
        var local = member.DescendantNodes().OfType<VariableDeclaratorSyntax>().LastOrDefault(x => x.Identifier.Text == name && x.SpanStart < before);
        if (local is not null)
        {
            // Its initializer and every plain assignment to it before the use (one in each branch of an if is both).
            var values = new List<ExpressionSyntax>();
            if (local.Initializer?.Value is { } initializer)
            {
                values.Add(initializer);
            }

            values.AddRange(member.DescendantNodes().OfType<AssignmentExpressionSyntax>()
                .Where(x => x.IsKind(SyntaxKind.SimpleAssignmentExpression) && x.Left is IdentifierNameSyntax { Identifier.Text: var assigned } && assigned == name
                            && x.SpanStart > local.SpanStart && x.SpanStart < before)
                .Select(x => x.Right));
            var given = values.Select(x => TypesOf(x, x.SpanStart, depth + 1)).ToList();
            (List<TypeRefNode> Types, bool Known) value = (given.SelectMany(x => x.Types).ToList(), given.Count > 0 && given.All(x => x.Known));
            return local.Parent is VariableDeclarationSyntax { Type.IsVar: false } declaration ? OrDeclared(value, declaration.Type) : value;
        }

        var parameter = identifier.Ancestors().SelectMany(Parameters).FirstOrDefault(x => x.Identifier.Text == name);
        if (parameter is not null)
        {
            return parameter.Type is null ? Unknown : ([CSharpReader.TypeRef(parameter.Type)], true);
        }

        return FieldOf(owner, name);
    }

    private static IEnumerable<ParameterSyntax> Parameters(SyntaxNode node) => node switch
    {
        BaseMethodDeclarationSyntax method => method.ParameterList.Parameters,
        LocalFunctionStatementSyntax function => function.ParameterList.Parameters,
        ParenthesizedLambdaExpressionSyntax lambda => lambda.ParameterList.Parameters,
        SimpleLambdaExpressionSyntax lambda => [lambda.Parameter],
        AnonymousMethodExpressionSyntax { ParameterList: { } list } => list.Parameters,
        TypeDeclarationSyntax { ParameterList: { } list } => list.Parameters,
        _ => []
    };

    /// <summary>A field or property of a type, or one it inherits (a positional record's parameters are its properties).</summary>
    private (List<TypeRefNode> Types, bool Known) FieldOf(TypeNode type, string name) =>
        graph.MembersOf(type).FirstOrDefault(x => x.Name == name && x.Kind is "field" or "property") is { Type: { } declared } ? ([declared], true) : Unknown;

    private TypeRefNode OwnerRef() => new() { Text = owner.Name, Name = owner.Name, QualifiedName = owner.Namespace.Length == 0 ? owner.Name : $"{owner.Namespace}.{owner.Name}" };

    private static string Normalise(SyntaxNode node) => Whitespace.Replace(node.ToString(), " ").Trim();
}
