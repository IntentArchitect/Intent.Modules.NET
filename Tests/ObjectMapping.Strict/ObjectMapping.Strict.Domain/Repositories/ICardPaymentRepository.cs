using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Strict.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace ObjectMapping.Strict.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICardPaymentRepository : IEFRepository<CardPayment, CardPayment>
    {
        [IntentManaged(Mode.Fully)]
        Task<CardPayment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CardPayment?> FindByIdAsync(Guid id, Func<IQueryable<CardPayment>, IQueryable<CardPayment>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CardPayment>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}