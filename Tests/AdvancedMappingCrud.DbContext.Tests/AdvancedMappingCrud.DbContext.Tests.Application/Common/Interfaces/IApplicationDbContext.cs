using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.Tests.Domain.Entities;
using AdvancedMappingCrud.DbContext.Tests.Domain.Entities.ManyToMany;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Customer> Customers { get; }
        DbSet<Order> Orders { get; }
        DbSet<Product> Products { get; }
        DbSet<User> Users { get; }
        DbSet<Category> Categories { get; }
        DbSet<ProductItem> ProductItems { get; }
        DbSet<Tag> Tags { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}