using System.Linq;
using Intent.Modelers.Domain.Api;

namespace Intent.Entities.Repositories.Api.Api;

public static class ClassModelExtensions
{
    public static bool IsAggregateRoot(this ClassModel classModel)
    {
        var otherSources = classModel.AssociationEnds()
            .Where(p => p.IsSourceEnd())
            .ToArray();
        return !otherSources.Any() || otherSources.All(x => x.IsCollection || x.IsNullable);
    }
}