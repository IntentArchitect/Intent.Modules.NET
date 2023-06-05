using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAggregateRoot1Repository : IEFRepository<AggregateRoot1, AggregateRoot1>
    {

        [IntentManaged(Mode.Fully)]
        Task<AggregateRoot1> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateRoot1>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
        Task<SpResult> Sp_params0_returnsD_collection0_schemaName0(CancellationToken cancellationToken = default);
        Task<SpResult> Sp_params0_returnsD_collection0_schemaName1(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<SpResult>> Sp_params0_returnsD_collection1_schemaName0(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<SpResult>> Sp_params0_returnsD_collection1_schemaName1(CancellationToken cancellationToken = default);
        Task<AggregateRoot1> Sp_params0_returnsE_collection0_schemaName0(CancellationToken cancellationToken = default);
        Task<AggregateRoot1> Sp_params0_returnsE_collection0_schemaName1(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<AggregateRoot1>> Sp_params0_returnsE_collection1_schemaName0(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<AggregateRoot1>> Sp_params0_returnsE_collection1_schemaName1(CancellationToken cancellationToken = default);
        Task Sp_params0_returnsV_collection0_schemaName0(CancellationToken cancellationToken = default);
        Task Sp_params0_returnsV_collection0_schemaName1(CancellationToken cancellationToken = default);
        Task<SpResult> Sp_params1_returnsD_collection0_schemaName0(string param1, CancellationToken cancellationToken = default);
        Task<SpResult> Sp_params1_returnsD_collection0_schemaName1(string param1, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<SpResult>> Sp_params1_returnsD_collection1_schemaName0(string param1, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<SpResult>> Sp_params1_returnsD_collection1_schemaName1(string param1, CancellationToken cancellationToken = default);
        Task<AggregateRoot1> Sp_params1_returnsE_collection0_schemaName0(string param1, CancellationToken cancellationToken = default);
        Task<AggregateRoot1> Sp_params1_returnsE_collection0_schemaName1(string param1, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<AggregateRoot1>> Sp_params1_returnsE_collection1_schemaName0(string param1, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<AggregateRoot1>> Sp_params1_returnsE_collection1_schemaName1(string param1, CancellationToken cancellationToken = default);
        Task Sp_params1_returnsV_collection0_schemaName0(string param1, CancellationToken cancellationToken = default);
        Task Sp_params1_returnsV_collection0_schemaName1(string param1, CancellationToken cancellationToken = default);
        Task<SpResult> Sp_params2_returnsD_collection0_schemaName0(string param1, string param2, CancellationToken cancellationToken = default);
        Task<SpResult> Sp_params2_returnsD_collection0_schemaName1(string param1, string param2, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<SpResult>> Sp_params2_returnsD_collection1_schemaName0(string param1, string param2, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<SpResult>> Sp_params2_returnsD_collection1_schemaName1(string param1, string param2, CancellationToken cancellationToken = default);
        Task<AggregateRoot1> Sp_params2_returnsE_collection0_schemaName0(string param1, string param2, CancellationToken cancellationToken = default);
        Task<AggregateRoot1> Sp_params2_returnsE_collection0_schemaName1(string param1, string param2, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<AggregateRoot1>> Sp_params2_returnsE_collection1_schemaName0(string param1, string param2, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<AggregateRoot1>> Sp_params2_returnsE_collection1_schemaName1(string param1, string param2, CancellationToken cancellationToken = default);
        Task Sp_params2_returnsV_collection0_schemaName0(string param1, string param2, CancellationToken cancellationToken = default);
        Task Sp_params2_returnsV_collection0_schemaName1(string param1, string param2, CancellationToken cancellationToken = default);
    }
}