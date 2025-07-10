using CleanArchitecture.IdentityService.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationIdentityUser> ApplicationIdentityUsers { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}