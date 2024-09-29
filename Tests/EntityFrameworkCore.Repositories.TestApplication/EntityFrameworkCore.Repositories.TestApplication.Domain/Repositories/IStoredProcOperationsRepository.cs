using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories
{
    public interface IStoredProcOperationsRepository
    {
        /// <summary>
        /// Method comment.
        /// </summary>
        Task<SpResult> MyProc(IEnumerable<SpParameter> param, CancellationToken cancellationToken = default);
    }
}