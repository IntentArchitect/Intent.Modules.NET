using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories
{
    public interface ICustomRepository
    {
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