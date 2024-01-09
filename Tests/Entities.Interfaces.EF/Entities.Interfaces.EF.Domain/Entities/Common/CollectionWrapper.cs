using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.CollectionWrapper", Version = "1.0")]

namespace Entities.Interfaces.EF.Domain.Entities.Common
{
    /// <summary>
    /// Provides a wrapper over an <see cref="ICollection{TImplementation}"/> to make it behave as an <see cref="ICollection{TInterface}"/>.  
    /// </summary>
    /// <typeparam name="TInterface">The interface type the collection should be exposed as.</typeparam>
    /// <typeparam name="TImplementation">The actual type of the items in the collection. Must implement <typeparamref name="TInterface"/>.</typeparam>
    public class CollectionWrapper<TInterface, TImplementation> : ICollection<TInterface>
        where TImplementation : TInterface
    {
        private readonly ICollection<TImplementation> _wrappedCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionWrapper{TInterface, TImplementation}"/> class.
        /// </summary>
        /// <param name="wrappedCollection">The collection to be wrapped.</param>
        public CollectionWrapper(ICollection<TImplementation> wrappedCollection)
        {
            _wrappedCollection = wrappedCollection;
        }

        /// <inheritdoc />
        public IEnumerator<TInterface> GetEnumerator()
        {
            return _wrappedCollection.Cast<TInterface>().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(TInterface item)
        {
            _wrappedCollection.Add((TImplementation)item!);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _wrappedCollection.Clear();
        }

        /// <inheritdoc />
        public bool Contains(TInterface item)
        {
            return _wrappedCollection.Contains((TImplementation)item!);
        }

        /// <inheritdoc />
        public void CopyTo(TInterface[] array, int arrayIndex)
        {
            _wrappedCollection.Cast<TInterface>().ToArray().CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(TInterface item)
        {
            return _wrappedCollection.Remove((TImplementation)item!);
        }

        /// <inheritdoc />
        public int Count => _wrappedCollection.Count;

        /// <inheritdoc />
        public bool IsReadOnly => _wrappedCollection.IsReadOnly;
    }

    /// <summary>
    /// Provides extension methods for <see cref="ICollection{T}"/>.
    /// </summary>
    public static class CollectionWrapperExtensions
    {
        /// <summary>
        /// Creates a wrapper for a collection to expose it as a different interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface type the collection should be exposed as.</typeparam>
        /// <typeparam name="TImplementation">The actual type of the items in the collection. Must implement <typeparamref name="TInterface"/>.</typeparam>
        /// <param name="collection">The collection to be wrapped.</param>
        /// <returns>An <see cref="ICollection{TInterface}"/> that wraps the provided collection.</returns>
        public static ICollection<TInterface> CreateWrapper<TInterface, TImplementation>(this ICollection<TImplementation> collection)
            where TImplementation : TInterface
        {
            return new CollectionWrapper<TInterface, TImplementation>(collection);
        }
    }
}