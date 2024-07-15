using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories
{
    public interface ISqlOutParameterRepository
    {
        Task<int> Sp_out_params_int(CancellationToken cancellationToken = default);
        Task<string> Sp_out_params_string(CancellationToken cancellationToken = default);
        Task<decimal> Sp_out_params_decimal_default(CancellationToken cancellationToken = default);
        Task<decimal> Sp_out_params_decimal_specific(CancellationToken cancellationToken = default);
        Task<(string Param1Output, DateTime Param2Output, bool Param3Output)> Sp_out_params_multiple(CancellationToken cancellationToken = default);
    }
}