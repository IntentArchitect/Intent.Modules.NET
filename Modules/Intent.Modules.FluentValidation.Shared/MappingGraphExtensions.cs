using System;
using Intent.Metadata.Models;

namespace Intent.Modules.FluentValidation.Shared;

internal static class MappingGraphExtensions
{
    public static bool TryWalkMappingGraph<TTarget>(this IElement sourceElement, out TTarget targetElement)
        where TTarget : class, IElement
    {
        if (sourceElement.TryWalkMappingGraph(static candidate => candidate is TTarget, out var candidate) &&
            candidate is TTarget typedTarget)
        {
            targetElement = typedTarget;
            return true;
        }

        targetElement = default!;
        return false;
    }

    public static bool TryWalkMappingGraph(this IElement sourceElement, Func<IElement, bool> predicate, out IElement targetElement)
    {
        var mappedElement = sourceElement?.MappedElement?.Element as IElement;
        while (mappedElement is not null)
        {
            if (predicate(mappedElement))
            {
                targetElement = mappedElement;
                return true;
            }

            mappedElement = mappedElement.MappedElement?.Element as IElement;
        }

        targetElement = default!;
        return false;
    }
}
