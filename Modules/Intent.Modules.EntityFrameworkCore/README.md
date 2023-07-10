# Intent.EntityFrameworkCore

This module provides patterns for working with the Entity Framework Core. Entity Framework Core is an open-source ORM framework by Microsoft that simplifies data access in .NET applications, mapping relational databases to .NET objects and providing LINQ support for querying. It supports multiple database providers, offers migration capabilities, and is cross-platform, making it a versatile and efficient tool for developers.

This module consumes your `Domain Model`, which you build in the `Domain Designer` and generates the corresponding Entity Framework Core implementation:-

* [Database context](#template--database-context).
* [Entity Type Configurations](#template--entity-type-configuration).
* [DB Migrations Cheat sheet](#template--db-migrations-readme).
* [`app.settings` configuration](#factory-extension--dependency-injection).
* [Dependency Injection wiring](#factory-extension--dependency-injection).

For more information on Entity Framework Core, check out their [official documentation](https://learn.microsoft.com/en-us/ef/core/).

## Module Settings

### Database Settings - `Database Provider`

This setting allows you to configure which `Database Provider` you want Entity Framework to use to connect to your database. The available options are as follows:

* In Memory
* SQL Server
* PostgresSQL
* My SQL
* Cosmos DB

![Database Settings - `Database Provider`](/docs//images//database-provider.png)

## Code Generation Artifacts (Templates, Decorators, Factory Extensions)

### Template : Database context

Generates the Application Specific DBContext file. The file will contain a `DBSet` for every aggregational `Entity` in your domain model and registers up their `EntityTypeConfiguration`s.

```csharp

    public class ApplicationDbContext : IdentityDbContext<ApplicationIdentityUser>
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new BasketConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
    }
```

### Template : Entity Type Configuration

Generates a `EntityTypeConfiguration` file for every aggregational entity. This file contains all the technical database mappings for a specific domain entity.

```csharp

    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.OwnsMany(x => x.BasketItems, ConfigureBasketItems);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }

        public void ConfigureBasketItems(OwnedNavigationBuilder<Basket, BasketItem> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.BasketId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BasketId)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

```

### Template : DB Migrations Readme

Generates a cheat sheet of commonly used migration commands pre-configured for your application.

```text

See https://learn.microsoft.com/ef/core/managing-schemas/migrations for information about
migrations using EF Core. You can perform these commands in the Visual Studio IDE using the
Package Manager Console (View > Other Windows > Package Manager Console) or using the dotnet
Command Line Interface (CLI) instructions. Substitute the {Keywords} below with the appropriate
migration name when executing these commands.

-------------------------------------------------------------------------------------------------------------------------------------------------------
Create a new migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Add-Migration -Name {ChangeName} -StartupProject "SimplifiedEShopTutorial.Api" -Project "SimplifiedEShopTutorial.Infrastructure"

CLI:
dotnet ef migrations add {ChangeName} --startup-project "SimplifiedEShopTutorial.Api" --project "SimplifiedEShopTutorial.Infrastructure"

-------------------------------------------------------------------------------------------------------------------------------------------------------
Remove last migration:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Remove-Migration -StartupProject "SimplifiedEShopTutorial.Api" -Project "SimplifiedEShopTutorial.Infrastructure"

CLI:
dotnet ef migrations remove --startup-project "SimplifiedEShopTutorial.Api" --project "SimplifiedEShopTutorial.Infrastructure"

-------------------------------------------------------------------------------------------------------------------------------------------------------
Update schema to the latest version:
-------------------------------------------------------------------------------------------------------------------------------------------------------
From the Visual Studio Package Manager Console:
Update-Database -StartupProject "SimplifiedEShopTutorial.Api" -Project "SimplifiedEShopTutorial.Infrastructure"

CLI:
dotnet ef database update --startup-project "SimplifiedEShopTutorial.Api" --project "SimplifiedEShopTutorial.Infrastructure" 

...

```

### Factory Extension : Dependency Injection  

```json
{
  ...

  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Initial Catalog={Application Name};Integrated Security=true;MultipleActiveResultSets=True;"
  },

  ...

}
```

```csharp

namespace {{Application Name}}.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });

            ...
 
        }
    }
}

```

## Related Modules

### Intent.Metadata.RDBMS

### Intent.Entities

### Intent.EntityFrameworkCore.Repositories
