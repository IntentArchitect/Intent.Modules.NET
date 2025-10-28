using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Customer> Customer { get; }
        DbSet<Order> Order { get; }
        DbSet<Product> Product { get; }
        DbSet<User> User { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}