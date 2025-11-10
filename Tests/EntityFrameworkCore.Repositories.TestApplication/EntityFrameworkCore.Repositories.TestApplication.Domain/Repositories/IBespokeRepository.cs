using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories
{
    public interface IBespokeRepository
    {
        Task SpUpdateDataEntries(int? supplierID, IEnumerable<UdttDataEntityModel>? dataEntity, int? userID, CancellationToken cancellationToken = default);
    }
}