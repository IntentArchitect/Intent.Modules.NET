using EntityFrameworkCore.SqlServer.EF11.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Enums
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAddressInfoRepository : IEFRepository<AddressInfo, AddressInfo>
    {
        [IntentManaged(Mode.Fully)]
        Task<AddressInfo?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AddressInfo?> FindByIdAsync(Guid id, Func<IQueryable<AddressInfo>, IQueryable<AddressInfo>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AddressInfo>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}