using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.UpdateHelper", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Domain.Common
{
    public static class UpdateHelper
    {
        public static ICollection<TOriginal> CreateOrUpdateCollection<TChanged, TOriginal>(
            ICollection<TOriginal> baseCollection,
            ICollection<TChanged>? changedCollection,
            Func<TOriginal, TChanged, bool> equalityCheck,
            Func<TOriginal, TChanged, TOriginal> assignmentAction)
            where TOriginal : class, new()
        {
            if (changedCollection == null)
            {
                return new List<TOriginal>();
            }

            baseCollection ??= new List<TOriginal>()!;

            var result = baseCollection.CompareCollections(changedCollection, equalityCheck);
            foreach (var elementToAdd in result.ToAdd)
            {
                var newEntity = new TOriginal();
                assignmentAction(newEntity, elementToAdd);

                baseCollection.Add(newEntity);
            }

            foreach (var elementToRemove in result.ToRemove)
            {
                baseCollection.Remove(elementToRemove);
            }

            foreach (var elementToEdit in result.PossibleEdits)
            {
                assignmentAction(elementToEdit.Original, elementToEdit.Changed);
            }

            return baseCollection;
        }
    }
}