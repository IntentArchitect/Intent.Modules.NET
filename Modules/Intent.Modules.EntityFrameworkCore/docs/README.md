# Intent.EntityFrameworkCore

This module provides patterns for working with the Entity Framework Core. Entity Framework Core is an open-source ORM framework by Microsoft that simplifies data access in .NET applications, mapping relational databases to .NET objects and providing LINQ support for querying. It supports multiple database providers, offers migration capabilities, and is cross-platform, making it a versatile and efficient tool for developers.

This module consumes your `Domain Model`, which you build in the `Domain Designer` and generates the corresponding Entity Framework Core implementation:

* [Database context](#template--database-context).
* [Entity Type Configurations](#template--entity-type-configuration).
* [DB Migrations Cheat sheet](#template--db-migrations-readme).
* [`app.settings` configuration](#factory-extension--dependency-injection).
* [Dependency Injection wiring](#factory-extension--dependency-injection).

For more information on Entity Framework Core, check out their [official documentation](https://learn.microsoft.com/ef/core/).

## Module Settings

### Database Settings - `Database Provider`

This setting allows you to configure which `Database Provider` you want Entity Framework to use to connect to your database. The available options are as follows:

* In Memory
* SQL Server
* PostgresSQL
* My SQL
* SQL Lite
* Cosmos DB

![Database Settings - `Database Provider`](images/database-provider.png)

### Database Settings - `Table naming convention`

This setting allows you to configure a convention for your SQL table name. The available options are as follows:

* Pluralized, SQL table name will be the pluralized version of the domain model `Class`'s name.
* Singularized, SQL table name will be the singularized version of the domain model `Class`'s name.
* None, SQL table name will be the same as domain model `Class`'s name.

### Database Settings - `Decimal precision and scale`

This setting allows you to configure a default Precision and Scale for your SQL decimal types.
The value of this setting is a string as follows : {Precision},{Scale}

For example 18,4 would be 18 precision, 4 scale.

For more info on decimal types check out [SQL Server decimal](https://learn.microsoft.com/sql/t-sql/data-types/decimal-and-numeric-transact-sql).

### Database Settings - `Lazy loading with proxies`

This setting allows you to configure whether you would like to use Entity Frameworks, Lazy loading with proxies feature.
This setting is on by default, but can be turned off if you don't want this behaviour.

For more info on lazy loading with proxies check out [the official documentation](https://learn.microsoft.com/ef/core/querying/related-data/lazy#lazy-loading-with-proxies).

### Database Settings - `Generate DbContext interface`

When enabled, an `IApplicationDbContext` will be generated in the "Application" layer. The `IApplicationDbContext` exposes all the [`DbSet<TEntity>`](https://learn.microsoft.com/dotnet/api/microsoft.entityframeworkcore.dbset-1) properties of the DbContext.

> [!NOTE]
> A NuGet package reference to `Microsoft.EntityFrameworkCore` is added to the "Application" layer project's `.csproj` as this is the assembly containing the `DbSet<TEntity>` type. This should be considered if you're adhering to the Clean Architecture principle of keeping your "application" layer as clean as possible of references.

### Database Settings - `Enable split queries globally`

When enabled, configures the DbContext to have [split queries enabled globally](https://learn.microsoft.com/ef/core/querying/single-split-queries#enabling-split-queries-globally).

### Database Settings - `Maintain column ordering`

When enabled, the EF configuration will be set up to preserve the column ordering as per your Domain model ordering. Base classes will be ordered because inherited classes.
The ordering is achieved using EF Core's `HasColumnOrder` functionality.

```csharp
    builder.Property(x => x.Id)
        .HasColumnOrder(0);

    builder.Property(x => x.Name)
        .IsRequired()
        .HasColumnOrder(1);
```

### Database Settings - `Enum check constraints`

When enabled, the EF configuration will set up a SQL table check constraints to ensure the data stored in the underlying column adheres to the `Enum`.

```csharp
    var customerTypeEnumValues = Enum.GetValuesAsUnderlyingType<CustomerType>()
        .Cast<object>()
        .Select(value => value.ToString());

    builder.ToTable(tb => tb.HasCheckConstraint("customer_customer_type_check", $"\"CustomerType\" IN ({string.Join(",", customerTypeEnumValues)})"));
```

If you are using the `Store enums as strings` setting you will get the following

```csharp
    var customerTypeEnumValues = Enum.GetNames<CustomerType>()
        .Select(e => $"'{e}'");

    builder.ToTable(tb => tb.HasCheckConstraint("customer_customer_type_check", $"\"CustomerType\" IN ({string.Join(",", customerTypeEnumValues)})"));
```

Intent will automatically do the Column ordering but if you want to get very specific you can use the `DataColumn` stereotype to explicitly set the ordering.
This can also be using for forcing columns in base class to go to the end by assigning them a arbitrary large number.

## Domain Designer modeling

The `Domain Designer` has been extended with many stereotypes for modeling RDBMS technology specific concepts in your domain.

Please see the [RDBMS README](https://docs.intentarchitect.com/articles/modules-common/intent-metadata-rdbms/intent-metadata-rdbms.html) for additional information on modeling RDBMS concepts in the `Domain Designer`, including, but not limited to:

* **Modifying Table defaults**
* **Modifying Column defaults**
* **Primary Keys**
* **Foreign Keys**
* **Text Constraints**
* **Decimal Constraints**
* **Computed Value**
* **Default Constraints**
* **Table Join Constraints**
* **Indexes**
* **Schemas**
* **Views**

### Row Version - Attribute stereotype

The `Row Version` stereotype when applied to a `binary` `Attribute`, denotes that it should map to a database type that provides automatic row-versioning, such as the SQL Server `rowversion` type.

The `Row Version` stereotype can be manually applied. This stereotype is visualized by the time stamp icon.

![Column visual](images/row-version-stereotype.png)

### Embedded vs. Separate Tables

Designing a 1 -> 1 relationship between two Entities (as illustrated below) communicates that `AccountDetails` being **owned** by `Account`.

![Embedded one-to-one relationship](images/embedded-one-to-one.png)

This will result in Entity Framework embedding `AccountDetails` into the `Accounts` table like this:

![Embedded table](images/embedded-table.png)

Learn more about this by visiting this [article](https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities).

To "split" the Entities into different tables, apply the `Table` stereotype on the `AccountDetails` entity.

![Split with Table Stereotype](images/split-table-stereotype.png)

This will result in Entity Framework making separate tables for both Entities:

![Split Entities tables](images/split-entities-table.png)

### Many-to-Many Relationship Modeling

When modeling a `many-to-many` relationship, it is not always necessary to explicitly define the **joining table**. Entity Framework Core can automatically create and manage this table behind the scenes - see the [official documentation](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many) for more details.

However, the joining table can also be explicitly modeled if preferred. Both approaches are functionally equivalent in many scenarios.

#### Implicit Join Table

When not modeling the joining table explicitly, Entity Framework will generate the necessary intermediate table automatically:

![Implicit Many-To-Many](images/many-to-many-no-join.png)

#### Explicit Join Table

You can also model the join table directly in the domain:

![Explicit Many-To-Many](images/many-to-many-with-join.png)

#### When to Model the Join Table

You should explicitly model the join table when:

* You need to add additional columns to the joining table (beyond the default foreign keys).
* You need to establish relationships between the joining table and other tables not part of the original many-to-many relationship.

Example:

![Many-To-Many Additions](images/many-to-many-additions.png)

#### Naming the Join Table

When using an implicit many-to-many relationship, you can still customize the generated join table’s name using the `Join Table` stereotype. This allows for control over the table name without needing to fully model the table.

> For more information on configuring the join table using the `Join Table` stereotype, refer to the [Intent.Metadata.RDBMS module documentation](https://docs.intentarchitect.com/articles/modules-common/intent-metadata-rdbms/intent-metadata-rdbms.html#create-join-table-constraint).

### Modeling Inheritance

In Entity Framework Core there are 3 ways to model inheritance, namely:

* Table per hierarchy (TPH).
* Table per type (TPT).
* Table per concrete type (TPC).

For more information on modeling inheritance with Entity Framework Core, see the [documentation](https://learn.microsoft.com/ef/core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration).

#### Table per hierarchy

Modeling:

![Table per hierarchy Model](images/tph-model.png)

Resulting database structure:

![Table per hierarchy database tables](images/tph-db.png)

If you wish to make the base class abstract, while still maintaining TPH, simply add a `Table` stereotype to the base class.

#### Table per type

Modeling:

Note the `Table` stereotypes, you don't need to fill in the name schema if you are happy with the defaults.

![Table per type Model](images/tpt-model.png)

Resulting database structure:

![Table per type database tables](images/tpt-db.png)

#### Table per concrete type

Modeling:

Note the base class is marked as abstract.

![Table per concrete type Model](images/tpc-model.png)

Resulting database structure:

![Table per concrete type database tables](images/tpc-db.png)

## Database Settings

### Multiple Database support

Applying the `Database Settings` stereotype on a Domain package will allow you to specify a Connection String Name as well as a Database Provider that will make a DbContext type that will contain all the Classes in that Domain package as DbSets.

![Database Settings Stereotype](images/database-settings-stereotype.png)

Having a `(default)` Connection String Name will make use of the connection string `DefaultConnection` and will generate the `ApplicationDbContext` type. The `Default` Database Provider will defer to the Module Database setting to determine which Database Provider to use.

Changing the Connection String name will allow you to specify a connection string for connecting to another database and you may alter the Database Provider by choose the specific one in the dropdown menu. This will also generate a DbContext type specifically for that Connection String.

> [!NOTE]
>
> For this release, the unit of work pattern still only applies to the main `ApplicationDbContext`, for the additional `DbContext`s the `SaveChanges` methods will need to be called manually. Should you have a project which requires the unit of work pattern to apply to additional `DbContext`s, please each out to us at [Intent Architect Support](https://github.com/IntentArchitect/Support).

## Code Generation Artifacts (Templates, Factory Extensions)

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

Generates a cheat sheet of commonly used migration commands pre-configured for your application. These commands cover things like:

* Creating a new Migration.
* Updating the database schema.
* Generating SQL Scripts to upgrade databases.

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

For more information on EF Migrations check out [Microsoft's official documentation](https://learn.microsoft.com/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli).

### Factory Extension : Dependency Injection  

This extension does the following handles various Entity Framework Core configuration concerns.

Adds a default connection string to your `app.settings.config` file, if the connection is not present.

```json
{
  ...

  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Initial Catalog={Application Name};Integrated Security=true;MultipleActiveResultSets=True;"
  },

  ...

}
```

Wires up the dependency injection registrations and EF application configuration.

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

## Primitive Collection Modelling

Primitive data types (examples include `string`, `int`, `bool` etc) can now be modelled as collections in the `Domain Designer` and will leverage Entity Framework's [Primitive collection properties](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#primitive-collection-properties) functionality to persist the data as a JSON column in the database.

![Primitive Collection](images/primitive-collection.png)

The collection can be mapped using the `Services Designer` using existing mapping functionality:

![Primitive Collection Mapping](images/collection-mapping.png)

When persisted to the database, the primitive collection property saved in a single column serialized to JSON.

![Primitive Collection Mapping](images/primitrive-collection-database.png)

## Related Modules

### Intent.Metadata.RDBMS

This module provides RDBMS related stereotypes for extending the Domain Designer with RDBMS technology specific data.

### Intent.Entities

This module generated domain entities as C# classes, which are used by this model.

### Intent.EntityFrameworkCore.Repositories

This module provides an Entity Framework repository pattern implementation.
