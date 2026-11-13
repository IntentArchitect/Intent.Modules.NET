using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.EfCore;

/// <summary>
/// EF Core's fluent configuration, read from the in-scope C# syntax: <c>modelBuilder.Entity&lt;T&gt;(entity =&gt; { ... })</c>
/// and an <c>IEntityTypeConfiguration&lt;T&gt;</c>'s <c>Configure(EntityTypeBuilder&lt;T&gt; builder)</c>. Each statement on
/// the builder is a call chain; a chain that starts with <c>Property(e =&gt; e.X)</c> configures that property.
/// </summary>
internal sealed class EfCoreReader : ICodeReader
{
    /// <summary>The calls with an overload that names members by string: <c>Property("_alias")</c>, <c>HasForeignKey("BuyerId")</c>.</summary>
    private static readonly HashSet<string> NamingCalls =
    [
        "Property", "HasKey", "HasAlternateKey", "HasForeignKey", "HasPrincipalKey", "HasIndex", "HasOne", "HasMany", "WithOne", "WithMany", "Ignore", "Navigation",
        "OwnsOne", "OwnsMany"
    ];

    public void Read(CodeGraph graph)
    {
        if (CSharpSyntax.Of(graph) is not { } csharp)
        {
            return;
        }

        var model = new EfCoreModel();
        foreach (var (file, tree) in csharp.Trees)
        {
            Read(tree.GetRoot(), file, model, csharp);
        }

        // EF Core is in the code when it configures an entity or declares a DbContext; otherwise it reads nothing, so its
        // claim readers leave every claim to another framework's, or to can't verify.
        if (model.Entities.Count > 0 || graph.Types.Any(x => graph.Supertypes(x).Any(s => s.EndsWith("DbContext", StringComparison.Ordinal))))
        {
            graph.Set(model);
        }
    }

    private static void Read(SyntaxNode root, string file, EfCoreModel model, CSharpSyntax csharp)
    {
        foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (invocation.Expression is MemberAccessExpressionSyntax { Name: GenericNameSyntax { Identifier.Text: "Entity", TypeArgumentList.Arguments: [var entityType] } }
                && invocation.ArgumentList.Arguments is [{ Expression: SimpleLambdaExpressionSyntax lambda }])
            {
                Configure(model, Name(entityType), lambda.Parameter.Identifier.Text, lambda.Body, file, csharp);
            }
        }

        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            if (method.Identifier.Text == "Configure"
                && method.ParameterList.Parameters is [{ Type: GenericNameSyntax { Identifier.Text: "EntityTypeBuilder", TypeArgumentList.Arguments: [var entityType] } } parameter]
                && ((SyntaxNode?)method.Body ?? method.ExpressionBody) is { } body)
            {
                Configure(model, Name(entityType), parameter.Identifier.Text, body, file, csharp);
            }
        }
    }

    private static void Configure(EfCoreModel model, string entity, string builder, SyntaxNode body, string file, CSharpSyntax csharp)
    {
        if (!model.Entities.TryGetValue(entity, out var config))
        {
            config = model.Entities[entity] = new EntityConfig { Entity = entity, At = SpanOf(body, file) };
        }

        foreach (var chain in body.DescendantNodes().OfType<InvocationExpressionSyntax>().Where(x => x.Parent is not MemberAccessExpressionSyntax))
        {
            var calls = Calls(chain, file, csharp);
            if (calls.Count == 0 || calls[0].Receiver != builder)
            {
                continue;
            }

            if (calls[0].Name == "Property" && calls[0].Selected is [var property])
            {
                if (!config.Properties.TryGetValue(property, out var list))
                {
                    list = config.Properties[property] = [];
                }

                list.AddRange(calls);
                if (calls[0].TypeArguments is [var type] && calls[0].Arguments is [var argument] && argument == property)
                {
                    config.Declared[property] = (type, calls[0].At);
                }
            }
            else
            {
                config.Calls.AddRange(calls);
                config.Chains.Add(calls);
            }
        }
    }

    /// <summary>A call chain, first call first: <c>entity.Property(e =&gt; e.X).HasMaxLength(50)</c>.</summary>
    private static List<FluentCall> Calls(InvocationExpressionSyntax last, string file, CSharpSyntax csharp)
    {
        var calls = new List<FluentCall>();
        ExpressionSyntax current = last;
        while (current is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax access } invocation)
        {
            var arguments = invocation.ArgumentList.Arguments;
            var lambdas = arguments.Select(x => x.Expression).OfType<LambdaExpressionSyntax>().ToList();

            // A call names members by its lambdas, or else by strings when every argument is one: HasIndex(e => e.A, "IX_A")
            // names its index, not a member.
            var named = lambdas.Count == 0 && NamingCalls.Contains(access.Name.Identifier.Text) && arguments.Count > 0
                        && arguments.All(x => x.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression));
            calls.Insert(0, new FluentCall
            {
                Name = access.Name.Identifier.Text,
                Arguments = arguments.Select(x => Value(x.Expression)).ToList(),
                Written = arguments.Select(x => x.Expression.ToString()).ToList(),
                Selected = named ? arguments.Select(x => Value(x.Expression)).ToList() : lambdas.SelectMany(Selected).ToList(),
                TypeArguments = access.Name is GenericNameSyntax generic ? generic.TypeArgumentList.Arguments.Select(x => csharp.TypeOf(x, file)).ToList() : [],
                At = SpanOf(access.Name, file)
            });
            current = access.Expression;
        }

        if (calls.Count > 0 && current is IdentifierNameSyntax receiver)
        {
            calls[0] = calls[0] with { Receiver = receiver.Identifier.Text };
        }

        return calls;
    }

    /// <summary>The members a selector lambda names: <c>e =&gt; e.Id</c>, <c>e =&gt; new { e.A, e.B }</c>.</summary>
    private static IEnumerable<string> Selected(LambdaExpressionSyntax lambda) => lambda.Body switch
    {
        MemberAccessExpressionSyntax access => [access.Name.Identifier.Text],
        AnonymousObjectCreationExpressionSyntax anonymous => anonymous.Initializers.Select(x => x.Expression).OfType<MemberAccessExpressionSyntax>().Select(x => x.Name.Identifier.Text),
        _ => []
    };

    /// <summary>A string literal's value, otherwise the argument as written.</summary>
    private static string Value(ExpressionSyntax expression) =>
        expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression) ? literal.Token.ValueText : expression.ToString();

    private static string Name(TypeSyntax type) => type switch
    {
        QualifiedNameSyntax qualified => qualified.Right.Identifier.Text,
        SimpleNameSyntax simple => simple.Identifier.Text,
        _ => type.ToString()
    };

    private static Span SpanOf(SyntaxNode node, string file) => CSharpSyntax.SpanOf(node, file);
}
