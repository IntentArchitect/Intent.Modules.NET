using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.PkDataSources;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.PkDataSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICompKeyDefaultDataSourceEntityRepository : IEFRepository<CompKeyDefaultDataSourceEntity, CompKeyDefaultDataSourceEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<CompKeyDefaultDataSourceEntity?> FindByIdAsync((long Id1, long Id2) id, CancellationToken cancellationToken = default);
    }
}