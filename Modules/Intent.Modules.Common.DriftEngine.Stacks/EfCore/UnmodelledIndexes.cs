using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.EfCore;

/// <summary>
/// <c>unmodelled.index</c>: an index the EF Core configuration declares, <c>HasIndex</c> in checked code, that no index claim
/// located: one no Index element or Index stereotype describes. It is named by its database name (<c>HasDatabaseName</c>, or
/// a named <c>HasIndex(e =&gt; e.A, "IX_A")</c>), else by its entity and columns.
/// </summary>
internal sealed class UnmodelledIndexes : IOptInRule
{
    public string Id => "unmodelled.index";

    public IEnumerable<UnmodelledCode> Find(RuleContext context)
    {
        if (context.Code.Get<EfCoreModel>() is not { } model)
        {
            yield break;
        }

        foreach (var entity in model.Entities.Values)
        {
            var type = context.Code.Types.FirstOrDefault(x => x.Name == entity.Entity && model.Of(x, context.Code) == entity);
            var owner = type is null ? null : context.ElementOf(type);
            foreach (var chain in entity.Chains.Where(x => x[0].Name == "HasIndex"))
            {
                var at = chain[0].At;
                if (!context.Scope.Checks(at.File) || context.IsLocated(at))
                {
                    continue;
                }

                var name = chain.LastOrDefault(x => x.Name is "HasDatabaseName" or "HasName")?.Arguments.FirstOrDefault() ?? chain[0].Arguments.ElementAtOrDefault(1);
                var shown = name ?? $"{entity.Entity}({string.Join(", ", chain[0].Selected)})";
                yield return new UnmodelledCode(owner?.Designer ?? "Domain", at, shown,
                    $"The index `{shown}` on `{entity.Entity}` isn't in the model. Model it as an Index on `{entity.Entity}`, or add an exception with a reason to the Root Folder's Drift Check.",
                    owner?.Element.Id, entity.Entity);
            }
        }
    }
}
