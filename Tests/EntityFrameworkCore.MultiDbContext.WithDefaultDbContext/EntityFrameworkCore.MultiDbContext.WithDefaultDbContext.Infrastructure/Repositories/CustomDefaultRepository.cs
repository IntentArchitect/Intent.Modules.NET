using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Repositories
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