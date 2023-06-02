using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreGenericRepositoryInterface", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Domain.Repositories
{
    /// <summary>
    /// A generic repository for working with values by key against an underlying key/value state store.
    /// </summary>
    public interface IDaprStateStoreGenericRepository
    {

        /// <summary>
        /// Provides access to the <see cref="IDaprStateStoreUnitOfWork"/>.
        /// </summary>
        IDaprStateStoreUnitOfWork UnitOfWork { get; }
        /// <summary>
        /// Upserts the provided <paramref name="value" /> associated with the provided <paramref name="key" /> to the state
        /// store.
        /// </summary>
        /// <remarks>
        /// The implementation of this interface follows a unit of work pattern. Calling this
        /// method internally queues up a work action which is only executed when
        /// <see cref="UnitOfWork"/>'s <see cref="IDaprStateStoreUnitOfWork.SaveChangesAsync"/> is called.
        /// </remarks>
        /// <typeparam name="TValue">The type of the data that will be JSON serialized and stored in the state store.</typeparam>
        /// <param name="key">The state key.</param>
        /// <param name="value">The data that will be JSON serialized and stored in the state store.</param>
        void Upsert<TValue>(string key, TValue value);

        /// <summary>
        /// Gets the current value associated with the <paramref name="key" /> from the state store.
        /// </summary>
        /// <typeparam name="TValue">The data type of the value to read.</typeparam>
        /// <param name="key">The state key.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}" /> that will return the value when the operation has completed.</returns>
        Task<TValue> GetAsync<TValue>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the value associated with the provided <paramref name="key" /> in the state store.
        /// </summary>
        /// <remarks>
        /// The implementation of this interface follows a unit of work pattern. Calling this
        /// method internally queues up a work action which is only executed when
        /// <see cref="UnitOfWork"/>'s <see cref="IDaprStateStoreUnitOfWork.SaveChangesAsync"/> is called.
        /// </remarks>
        /// <param name="key">The state key.</param>
        /// <returns>A <see cref="Task" /> that will complete when the operation has completed.</returns>
        void Delete(string key);
    }
}