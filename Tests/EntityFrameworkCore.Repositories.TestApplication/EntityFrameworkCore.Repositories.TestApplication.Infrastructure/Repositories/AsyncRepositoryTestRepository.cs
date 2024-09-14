using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories
{
    public class AsyncRepositoryTestRepository : IAsyncRepositoryTestRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AsyncRepositoryTestRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Operation(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation (AsyncRepositoryTestRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}