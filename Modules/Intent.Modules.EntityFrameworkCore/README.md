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

![Database Settings - `Database Provider`](./docs/images/database-provider.png)

## Domain Designer modelling

The `Domain Designer` has been extended with many stereotypes which enable you to model RDBMS technology specific concepts in your domain.

### Primary Key - Attribute stereotype

The `Primary Key` stereotype indicates that an `Attribute` is the database table's primary key.

By default any `Class`'s added to your domain will have an `Attribute` added name `Id` with the `Primary Key` stereotype applied. The type of this attribute will default to the configured 'Database Settings > Key Type' which can be configured in your application settings.

This stereotype can be manually applied to `Attribute`s and can be applied to multiple `Attribute`s in the case of composite primary keys. This stereotype is visualizes as a golden key icon.

![Primary Key visual](./docs/images/primary-key-stereotype.png)

### Foreign Key - Attribute stereotype

The `Foreign Key` stereotype indicates an `Attribute` has been introduced to an Entity as a result of a modelled `Association`, for example:

![Foreign Key visual](./docs/images/foreign-key-stereotype.png)

In this diagram you can see the `CustomerId` attribute has been introduced, with the `Foreign Key` stereotype, as a result of the many-to-one relationship between `Basket` and `Customer`.

The `Foreign Key` stereotype's are automatically managed when modelling associations. This stereotype is visualizes as a silver key icon.

### Text Constraint - Attribute Stereotype

The `Text Constraint` stereotype allows you to configure the specifics of how the `string` type should be realized in the database.

This stereotype can be used to specify:

* Maxlength, the maximum storage size of the string.
* SQL Datatype, the SQL Datatype for the database.

By default `strings` are realized in SQL as `nvarchar(max)`. The `Text Constraint` stereotype is automatically applied to any attributes of type `string`. This stereotype is visualized by the `[{size}]` text after the `string` type.

![Text Constraint visual](./docs/images/text-constraint-stereotype.png)

### Decimal Constraint - Attribute stereotype

The `Decimal Constraint` stereotype allows you to configure the precision and scale for your `decimal` type.

The `Decimal Constraint` stereotype can be manually applied to any attributes of type `decimal`. This stereotype is visualized by the `({precision},{scale})` text after the `decimal` type.

![Decimal Constraint visual](./docs/images/decimal-constraint-stereotype.png)

### Computed Value - Attribute stereotype

The `Computed Value` stereotype allows you to configure SQL computed columns in your model.

The `Computed Value` stereotype can be manually applied to an attribute, allowing you to specify the formula for the calculation and whether or nopt the calculated result is persisted in the database. This stereotype is visualized by the blue computed column icon.

![Computed Value visual](./docs/images/computed-value-stereotype.png)

### Column - Attribute stereotype

The `Column` stereotype allows you to override the SQL column details from your model, if required.

The `Column` stereotype can be manually applied to an attribute, allowing you to specify the SQL column name and / or SQL column type. This stereotype is visualized by the orange and blue column icon.

![Column visual](./docs/images/column-stereotype.png)

### Row Version - Attribute stereotype

The `Row Version` stereotype when applied to a byte[] Attribute, denotes that the property should map to a database type that provides automatic row-versioning, such as the SQL Server `rowversion`` type.

The `Row Version` stereotype can be manually applied. This stereotype is visualized by the time stamp icon.

![Column visual](./docs/images/row-version-stereotype.png)

### Default Constraint - Attribute stereotype

The `Default Constraint` stereotype allows you to specify SQL column defaults from your model, if required.

The `Default Constraint` stereotype can be manually applied to an attribute, allowing you to specify either a default value or a default sql expression. This stereotype is visualized by stereotype's icon.

![Default Constraint visual](./docs/images/default-constraint-stereotype.png)

### Table - Entity stereotype

The `Table` stereotype allows you to specify a SQL tables name and/or schema name , if required.

By default SQL table names will be the pluralized version of the `Class` name, and go in the `dbo` schema.

The `Table` stereotype can be manually applied to an `Class`. If `Name` or `Schema` are not populated the default value will be used. This stereotype is visualized by stereotype's icon on the top right of the `Class`.

![Table visual](./docs/images/table-stereotype.png)

### View - Entity stereotype

If you have SQL Views in you database which you want to reference in you domain, you can model those views as `Class`s and apply the `View` stereotype to them.
This will allow you to access these views in code, through Entity Framework.

On the `View` stereotype you can specify the `Name` and `Schema` for the view, if they are not specified they will default to the pluralized version of the `Class` name, and the `dbo` schema. The SQL view must exist in the database for this to work.

The `View` stereotype can be manually applied to a `Class`. This stereotype is visualized by stereotype's icon on the top right of the `Class`.

![View visual](./docs/images/view-stereotype.png)

### Creating SQL Indexes

You can also model your SQL indexes in the `Domain Designer`.

* Find the `Class` you want to add an Index to, in the `Domain Designer` tree panel.
* Right-click the `Class` and select `Add Index`.

![Select Add Index](./docs/images/index-add-context.png)

* In dialog box, select the attributes you want to include in your index.

![Select Attributes which make up your Index](./docs/images/index-choose-attributes.png)

* Click `Done`.

![See Index Added](./docs/images/index-created.png)

You will see an `Index` has been added to the `Class`. If the order of the attributes in the index is not correct ,you can re-order them by dragging them around.

### Modelling Inheritance

In Entity Framework Core there are 3 ways to model inheritance namely:

* Table per hierarchy (TPH).
* Table per type (TPT).
* Table per concrete type (TPC).

For more information on modelling inheritance with Entity Framework Core see the [documentation](https://learn.microsoft.com/en-us/ef/core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration).

#### Table per hierarchy

Modelling:

![Table per hierarchy Model](./docs/images/tph-model.png)

Resulting database structure:

![Table per hierarchy database tables](./docs/images/tph-db.png)

#### Table per type

Modelling:

![Table per type Model](./docs/images/tpt-model.png)

Resulting database structure:

![Table per type database tables](./docs/images/tpt-db.png)

#### Table per concrete type

Modelling:

Note : the base class is marked as abstract.

![Table per concrete type Model](./docs/images/tpc-model.png)

Resulting database structure:

![Table per concrete type database tables](./docs/images/tpc-db.png)

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

## Related Modules

### Intent.Metadata.RDBMS

This modules provides RDBMS related stereotypes for extending the Domain Designer with RDBMS technology specific data.

### Intent.Entities

This module realized Domain entities as C# classes, which are used by this model.

### Intent.EntityFrameworkCore.Repositories

This module provides an Entity Framework repository pattern implementation.
