using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.UpdateHelper", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Domain.Common
{
    public static class UpdateHelper
    {
        public static TOriginal UpdateObject<TChanged, TOriginal>(
            this TOriginal baseElement,
            TChanged changedElement,
            Action<TOriginal, TChanged> assignmentAction)
        {
            if (baseElement == null)
            {
                return baseElement;
            }

            assignmentAction(baseElement, changedElement);
            return baseElement;
        }

        public static void UpdateCollection<TChanged, TOriginal>(
            this ICollection<TOriginal> baseCollection,
            ICollection<TChanged> changedCollection,
            Func<TOriginal, TChanged, bool> equalityCheck,
            Action<TOriginal, TChanged> assignmentAction)
            where TOriginal : class, new()
        {
            if (changedCollection == null)
            {
                return;
            }

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
        }
    }
}