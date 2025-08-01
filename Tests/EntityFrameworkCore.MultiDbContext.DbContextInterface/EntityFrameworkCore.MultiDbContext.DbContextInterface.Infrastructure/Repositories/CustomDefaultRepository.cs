using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure.Repositories
{
    public class CustomDefaultRepository : ICustomDefaultRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomDefaultRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task TestProc(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE TestProc", cancellationToken);
        }
    }
}