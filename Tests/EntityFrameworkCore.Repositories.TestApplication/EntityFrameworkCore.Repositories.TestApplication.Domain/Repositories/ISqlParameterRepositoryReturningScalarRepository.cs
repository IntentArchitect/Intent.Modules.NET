using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories
{
    public interface ISqlParameterRepositoryReturningScalarRepository
    {
        Task<int> Sp_params0_collection0_schemaName0(CancellationToken cancellationToken = default);
        Task<int> Sp_params0_collection0_schemaName1(CancellationToken cancellationToken = default);
        Task<int> Sp_params1_collection0_schemaName0(string param1, CancellationToken cancellationToken = default);
        Task<int> Sp_params1_collection0_schemaName1(string param1, CancellationToken cancellationToken = default);
        Task<int> Sp_params2_collection0_schemaName0(string param1, string param2, CancellationToken cancellationToken = default);
        Task<int> Sp_params2_collection0_schemaName1(string param1, string param2, CancellationToken cancellationToken = default);
    }
}