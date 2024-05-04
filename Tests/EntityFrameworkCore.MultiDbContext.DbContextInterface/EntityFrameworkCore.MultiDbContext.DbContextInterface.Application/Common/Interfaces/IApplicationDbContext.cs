using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<AppDbEntity> AppDbEntities { get; }
        DbSet<DefaultEntity> DefaultEntities { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}