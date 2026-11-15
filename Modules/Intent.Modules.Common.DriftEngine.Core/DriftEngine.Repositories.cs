using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine;

/// <summary>
/// A Domain Repository's structure rules, read from the code graph for any language: the entity its generic repository base
/// names, and the class that implements it. A framework that implements repositories itself (Spring Data) reads them first.
/// </summary>
public sealed partial class DriftEngine
{
    /// <summary>
    /// <c>repository.entity</c>: the first type argument of the repository's generic base whose name says it is a repository
    /// (<c>IRepository&lt;Order&gt;</c>, <c>IEFRepository&lt;Order, Order&gt;</c>), its own or an in-scope base's. A repository with
    /// none doesn't state its entity in a form the rule reads, so the claim can't be verified.
    /// </summary>
    private void RepositoryEntity(Claim claim)
    {
        var type = OwnType(claim);
        if (RepositoryBase(type) is not { } found)
        {
            Cant(claim, "notFound", new Reading { At = type.At, Detail = $"`{type.Name}` has no generic repository base type, such as `IRepository<T>`, whose type argument names its entity." });
            return;
        }

        var (declaring, entity) = found;
        var same = claim.ExpectedValue switch
        {
            ITypeReference model => TypeMatches(model, entity),
            IElement element => element.Name == entity.Name,
            _ => entity.Name == claim.Expected
        };
        Compare(claim, entity.Text, declaring.At, "code", same);
    }

    /// <summary>The nearest generic repository base of a type, with the type that declares it, and its first type argument.</summary>
    private (TypeNode Declaring, TypeRefNode Entity)? RepositoryBase(TypeNode type)
    {
        foreach (var declaring in new[] { type }.Concat(_graph.Bases(type)))
        {
            if (declaring.BaseTypes.FirstOrDefault(x => x.Arguments.Count > 0 && x.Name.Contains("Repository", StringComparison.Ordinal)) is { } repository)
            {
                return (declaring, repository.Arguments[0]);
            }
        }

        return null;
    }

    /// <summary>
    /// <c>repository.implementation</c>: a class in scope implements the repository's interface, the type it is located as or,
    /// for a class, the interface its language names it (C#'s <c>IOrderRepository</c>). A class with no such interface is
    /// its own implementation. A name alone isn't one: an <c>OrderRepository</c> that doesn't implement
    /// <c>IOrderRepository</c> doesn't count.
    /// </summary>
    private void RepositoryImplementation(Claim claim)
    {
        var type = OwnType(claim);
        var contract = type.Kind == "interface"
            ? type
            : _graph.Types.FirstOrDefault(x => x != type && x.Kind == "interface" && x.Name == Naming.InCode(ProfileOf(x), claim.Source.Name, ["interface", "type"]));
        if (contract is null)
        {
            Verify(claim, new Reading { Value = "present", From = "code", At = type.At });
            return;
        }

        var implementations = _graph.Types.Where(x => x.Kind != "interface" && _graph.Bases(x).Contains(contract)).ToList();
        if (implementations.Count == 0)
        {
            Missing(claim, claim.Source.Name, $"a class implementing `{contract.Name}`");
            return;
        }

        // The class of the repository's own name, when it is one of them.
        var implementation = implementations.FirstOrDefault(x => x == type) ?? implementations[0];
        Verify(claim, new Reading { Value = "present", From = "code", At = implementation.At });
    }
}
