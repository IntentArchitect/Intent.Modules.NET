using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.CollectionExtensions", Version = "1.0")]

namespace Application.Identity.MSAL.TestApplication.Domain.Common
{
    public static class CollectionExtensions
    {
        public static ComparisonResult<TChanged, TOriginal> CompareCollections<TChanged, TOriginal>(
            this ICollection<TOriginal> baseCollection,
            ICollection<TChanged> changedCollection,
            Func<TOriginal, TChanged, bool> equalityCheck)
        {
            if (changedCollection == null)
            {
                changedCollection = new List<TChanged>();
            }

            var toRemove = baseCollection.Where(baseElement => changedCollection.All(changedElement => !equalityCheck(baseElement, changedElement))).ToList();
            var toAdd = changedCollection.Where(changedElement => baseCollection.All(baseElement => !equalityCheck(baseElement, changedElement))).ToList();

            var possibleEdits = new List<Match<TChanged, TOriginal>>();
            foreach (var changedElement in changedCollection)
            {
                var match = baseCollection.FirstOrDefault(baseElement => equalityCheck(baseElement, changedElement));
                if (match != null)
                {
                    possibleEdits.Add(new Match<TChanged, TOriginal>(changedElement, match));
                }
            }

            return new ComparisonResult<TChanged, TOriginal>(toAdd, toRemove, possibleEdits);
        }

        public class ComparisonResult<TChanged, TOriginal>
        {
            public ComparisonResult(ICollection<TChanged> toAdd, ICollection<TOriginal> toRemove, ICollection<Match<TChanged, TOriginal>> possibleEdits)
            {
                ToAdd = toAdd;
                ToRemove = toRemove;
                PossibleEdits = possibleEdits;
            }

            public ICollection<TChanged> ToAdd { get; }
            public ICollection<TOriginal> ToRemove { get; }
            public ICollection<Match<TChanged, TOriginal>> PossibleEdits { get; }
            public bool HasChanges()
            {
                return ToAdd.Any() || ToRemove.Any() || PossibleEdits.Any();
            }
        }

        public class Match<TChanged, TOriginal>
        {
            public Match(TChanged changed, TOriginal original)
            {
                Changed = changed;
                Original = original;
            }

            public TChanged Changed { get; private set; }
            public TOriginal Original { get; private set; }
        }
    }
}