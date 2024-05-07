using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Repositories
{
    public class CustomAlternateRepository : ICustomAlternateRepository
    {
        private readonly AlternateConnStrDefaultDbDbContext _dbContext;

        public CustomAlternateRepository(AlternateConnStrDefaultDbDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task TestProc(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE TestProc", cancellationToken);
        }
    }
}