using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.CollectionWrapper", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{
    public class CollectionWrapper<TInterface, TImplementation> : ICollection<TInterface>
        where TImplementation : TInterface
    {
        private readonly ICollection<TImplementation> _wrappedCollection;

        public CollectionWrapper(ICollection<TImplementation> wrappedCollection)
        {
            _wrappedCollection = wrappedCollection;
        }
        public IEnumerator<TInterface> GetEnumerator()
        {
            return _wrappedCollection.Cast<TInterface>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TInterface item)
        {
            _wrappedCollection.Add((TImplementation)item!);
        }

        public void Clear()
        {
            _wrappedCollection.Clear();
        }

        public bool Contains(TInterface item)
        {
            return _wrappedCollection.Contains((TImplementation)item!);
        }

        public void CopyTo(TInterface[] array, int arrayIndex)
        {
            _wrappedCollection.Cast<TInterface>().ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(TInterface item)
        {
            return _wrappedCollection.Remove((TImplementation)item!);
        }

        public int Count => _wrappedCollection.Count;
        public bool IsReadOnly => _wrappedCollection.IsReadOnly;
    }

    public static class CollectionWrapperExtensions
    {
        public static ICollection<TInterface> CreateWrapper<TInterface, TImplementation>(this ICollection<TImplementation> collection)
            where TImplementation : TInterface
        {
            return new CollectionWrapper<TInterface, TImplementation>(collection);
        }
    }
}