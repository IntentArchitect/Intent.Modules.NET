using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;
using static Intent.Modules.Guardrails.Contracts.Tests.Guides;
using DomainGuides = Intent.Modules.Domain.Guardrails.Contracts.Templates.DriftRecipeGuides;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The Domain guides' recipe files, run against code in each style a guide covers and a model that agrees with it:
    /// no drift, no warnings from validate-drift-recipes, values from where the guide says they come from, and each planted
    /// change reported as exactly its rows.
    /// </summary>
    public class DomainGuideTests
    {
        private const string Domain = FactKinds.Domain;

        // EF Core: one entity configured in OnModelCreating, one in a configuration class, and two by annotations and
        // conventions; a view; nullable reference types on.
        private static readonly Dictionary<string, string> EfCore = new()
        {
            ["src/Acme.Domain/Entities/Customer.cs"] = """
                namespace Acme.Domain.Entities;

                public class Customer
                {
                    public int Id { get; set; }
                    public string Name { get; set; } = null!;
                    public string? Email { get; set; }
                    public string Code { get; set; } = null!;
                    public decimal CreditLimit { get; set; }
                    public Address Address { get; set; } = null!;
                    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
                }

                public class Address
                {
                    public string Street { get; set; } = null!;
                    public string? City { get; set; }
                }
                """,
            ["src/Acme.Domain/Entities/Order.cs"] = """
                namespace Acme.Domain.Entities;

                public class Order
                {
                    public Guid Id { get; set; }
                    public int CustomerId { get; set; }
                    public string Reference { get; set; } = null!;
                    public string? Notes { get; set; }
                    public decimal Total { get; set; }
                    public DateTime PlacedAt { get; set; }
                    public virtual Customer Customer { get; set; } = null!;
                    public virtual ICollection<OrderLine> Lines { get; set; } = new List<OrderLine>();
                }
                """,
            ["src/Acme.Domain/Entities/OrderLine.cs"] = """
                namespace Acme.Domain.Entities;

                [Table("OrderLines")]
                [Index(nameof(ProductCode), nameof(OrderId), Name = "IX_OrderLines_Product", IsUnique = true)]
                public class OrderLine
                {
                    [Key]
                    public int LineId { get; set; }

                    public Guid OrderId { get; set; }

                    [Required]
                    [StringLength(30)]
                    [Unicode(false)]
                    public string ProductCode { get; set; } = null!;

                    [Column("qty")]
                    public int Quantity { get; set; }

                    [Precision(10, 3)]
                    public decimal UnitPrice { get; set; }

                    [MaxLength(200)]
                    public string? Description { get; set; }

                    public int? ProductId { get; set; }

                    public virtual Order Order { get; set; } = null!;

                    public virtual Product? Product { get; set; }
                }
                """,
            ["src/Acme.Domain/Entities/Product.cs"] = """
                namespace Acme.Domain.Entities;

                public class Product
                {
                    public int Id { get; set; }

                    [MaxLength(50)]
                    public string Name { get; set; } = null!;

                    [Column(TypeName = "varchar(20)")]
                    public string? Sku { get; set; }
                }

                public class CustomerSummary
                {
                    public int CustomerId { get; set; }
                    public string Name { get; set; } = null!;
                    public int OrderCount { get; set; }
                }
                """,
            ["src/Acme.Infrastructure/Persistence/AcmeDbContext.cs"] = """
                namespace Acme.Infrastructure.Persistence;

                public class AcmeDbContext : DbContext
                {
                    public DbSet<Customer> Customers => Set<Customer>();
                    public DbSet<Order> Orders { get; set; } = null!;
                    public DbSet<OrderLine> OrderLines { get; set; } = null!;
                    public DbSet<Product> Products { get; set; } = null!;
                    public DbSet<CustomerSummary> CustomerSummaries { get; set; } = null!;

                    protected override void OnModelCreating(ModelBuilder modelBuilder)
                    {
                        modelBuilder.HasDefaultSchema("dbo");
                        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcmeDbContext).Assembly);

                        modelBuilder.Entity<Customer>(entity =>
                        {
                            entity.ToTable("Customers", "sales");
                            entity.HasKey(e => e.Id);
                            entity.Property(e => e.Name).HasMaxLength(100).IsUnicode(false);
                            entity.Property(e => e.Email).HasMaxLength(256).HasColumnName("email_address");
                            entity.Property(e => e.Code)
                                .HasMaxLength(5)
                                .IsUnicode(false)
                                .IsFixedLength();
                            entity.Property(e => e.CreditLimit).HasPrecision(18, 2).HasDefaultValue(0m);
                            entity.HasIndex(e => e.Code, "IX_Customers_Code").IsUnique();
                            entity.OwnsOne(e => e.Address);
                        });

                        modelBuilder.Entity<CustomerSummary>(entity =>
                        {
                            entity.HasNoKey();
                            entity.ToView("vw_CustomerSummary");
                        });
                    }
                }
                """,
            ["src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs"] = """
                namespace Acme.Infrastructure.Persistence.Configurations;

                public class OrderConfiguration : IEntityTypeConfiguration<Order>
                {
                    public void Configure(EntityTypeBuilder<Order> builder)
                    {
                        builder.ToTable("Orders");
                        builder.HasKey(o => o.Id);

                        builder.Property(o => o.Reference).IsRequired().HasMaxLength(20);
                        builder.Property(o => o.Notes).HasColumnType("nvarchar(max)");
                        builder.Property(o => o.Total).HasColumnType("decimal(18, 2)");
                        builder.Property(o => o.PlacedAt).HasColumnType("datetime2").HasDefaultValueSql("getutcdate()");

                        builder.HasOne(o => o.Customer).WithMany(c => c.Orders).HasForeignKey(o => o.CustomerId);
                        builder.HasMany(o => o.Lines).WithOne(l => l.Order).HasForeignKey(l => l.OrderId);
                        builder.HasIndex(o => new { o.CustomerId, o.PlacedAt }).HasDatabaseName("IX_Orders_Customer_Placed");
                    }
                }
                """
        };

        private static List<ModelFact> EfCoreModel() => new List<ModelFact>()
            .Concat(Entity("Customer", "Id", "Customers", "sales"))
            .Concat(Plain("Customer", "Id"))
            .Concat(Text("Customer", "Name", "VARCHAR", "100"))
            .Concat(Text("Customer", "Email", maxLength: "256", required: "false", name: "email_address"))
            .Concat(Text("Customer", "Code", type: "char(5)"))
            .Concat(Decimal("Customer", "CreditLimit", "18", "2", @default: "0m"))
            .Concat(Entity("Order", "Id", "Orders", "dbo"))
            .Concat(Plain("Order", "Id"))
            .Concat(Plain("Order", "CustomerId"))
            .Concat(Text("Order", "Reference", maxLength: "20"))
            .Concat(Text("Order", "Notes", "NVARCHAR", required: "false"))
            .Concat(Decimal("Order", "Total", "18", "2"))
            .Concat(Plain("Order", "PlacedAt", type: "datetime2", @default: "getutcdate()"))
            .Concat(Entity("OrderLine", "LineId", "OrderLines", "dbo"))
            .Concat(Plain("OrderLine", "LineId"))
            .Concat(Plain("OrderLine", "OrderId"))
            .Concat(Text("OrderLine", "ProductCode", "VARCHAR", "30"))
            .Concat(Plain("OrderLine", "Quantity", name: "qty"))
            .Concat(Decimal("OrderLine", "UnitPrice", "10", "3"))
            .Concat(Text("OrderLine", "Description", maxLength: "200", required: "false"))
            .Concat(Plain("OrderLine", "ProductId"))
            .Concat(Entity("Product", "Id", "Products", "dbo"))
            .Concat(Plain("Product", "Id"))
            .Concat(Text("Product", "Name", maxLength: "50"))
            .Concat(Text("Product", "Sku", "VARCHAR", "20", "false"))
            .Concat(Entity("CustomerSummary", "", "vw_CustomerSummary", "dbo", "View"))
            .Concat(Plain("CustomerSummary", "CustomerId"))
            .Concat(Text("CustomerSummary", "Name"))
            .Concat(Plain("CustomerSummary", "OrderCount"))
            .Concat(new[]
            {
                Relationship(FactKinds.RelationshipMultiplicity, "Order", "Customer", "Orders", "Customer", "*->1"),
                Relationship(FactKinds.RelationshipForeignKey, "Order", "Customer", "Orders", "Customer", "CustomerId"),
                Relationship(FactKinds.RelationshipMultiplicity, "Order", "OrderLine", "Order", "Lines", "1->*"),
                Relationship(FactKinds.RelationshipForeignKey, "Order", "OrderLine", "Order", "Lines", "OrderId"),
                Relationship(FactKinds.RelationshipMultiplicity, "Customer", "Address", "", "Address", "1->1"),
                Relationship(FactKinds.RelationshipForeignKey, "Customer", "Address", "", "Address", ""),
                Relationship(FactKinds.RelationshipMultiplicity, "OrderLine", "Product", "", "Product", "*->0..1"),
                Relationship(FactKinds.RelationshipForeignKey, "OrderLine", "Product", "", "Product", "ProductId"),
                Index(FactKinds.IndexColumns, "Customer", "IX_Customers_Code", "Code"),
                Index(FactKinds.IndexUnique, "Customer", "IX_Customers_Code", "true"),
                Index(FactKinds.IndexColumns, "Order", "IX_Orders_Customer_Placed", "CustomerId,PlacedAt"),
                Index(FactKinds.IndexUnique, "Order", "IX_Orders_Customer_Placed", "false"),
                Index(FactKinds.IndexColumns, "OrderLine", "IX_OrderLines_Product", "ProductCode,OrderId"),
                Index(FactKinds.IndexUnique, "OrderLine", "IX_OrderLines_Product", "true")
            })
            .ToList();

        private static string EfCoreRecipe => RecipeFile(DomainGuides.EfCore);

        [Fact]
        public void The_EF_Core_recipe_file_reads_every_style_with_no_drift()
        {
            Check(Domain, EfCoreRecipe, EfCoreModel(), EfCore).ShouldBeClean();
        }

        [Fact]
        public void The_EF_Core_recipe_file_validates_and_reads_values_where_the_guide_says()
        {
            var validation = ShouldValidate(Domain, EfCoreRecipe, EfCoreModel(), EfCore);

            validation.Sources(FactKinds.TableName).ShouldBe((5, 0, 0));
            validation.Sources(FactKinds.Key).ShouldBe((5, 0, 0));
            validation.Sources(FactKinds.TableKind).ShouldBe((1, 4, 0));
            validation.Sources(FactKinds.RelationshipMultiplicity).ShouldBe((4, 0, 0));
            validation.Sources(FactKinds.RelationshipForeignKey).ShouldBe((3, 1, 0));
            validation.Sources(FactKinds.IndexColumns).ShouldBe((3, 0, 0));
            validation.Sources(FactKinds.ColumnMaxLength).ShouldBe((8, 2, 0));
            validation.Sources(FactKinds.ColumnRequired).ShouldBe((6, 4, 0));
        }

        [Theory]
        [InlineData("src/Acme.Infrastructure/Persistence/AcmeDbContext.cs", "e.Name).HasMaxLength(100)", "e.Name).HasMaxLength(120)",
            "`Customer.Name` max length: model `100`, code `120`")]
        [InlineData("src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "o.Reference).IsRequired()", "o.Reference).IsRequired(false)",
            "`Order.Reference` required: model `true`, code `false`")]
        [InlineData("src/Acme.Domain/Entities/Customer.cs", "public string Name", "public string? Name",
            "`Customer.Name` required: model `true`, code `false`")]
        [InlineData("src/Acme.Domain/Entities/OrderLine.cs", "[StringLength(30)]", "[StringLength(40)]",
            "`OrderLine.ProductCode` max length: model `30`, code `40`")]
        [InlineData("src/Acme.Domain/Entities/Product.cs", "[MaxLength(50)]", "",
            "`Product.Name` max length: model `50`, code (none)")]
        [InlineData("src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "ToTable(\"Orders\")", "ToTable(\"SalesOrders\")",
            "`Order` table name: model `Orders`, code `SalesOrders`")]
        [InlineData("src/Acme.Infrastructure/Persistence/AcmeDbContext.cs", "\"Customers\", \"sales\"", "\"Customers\", \"crm\"",
            "`Customer` schema: model `sales`, code `crm`")]
        [InlineData("src/Acme.Infrastructure/Persistence/AcmeDbContext.cs", "HasPrecision(18, 2)", "HasPrecision(18, 4)",
            "`Customer.CreditLimit` scale: model `2`, code `4`")]
        [InlineData("src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "decimal(18, 2)", "decimal(19, 2)",
            "`Order.Total` precision: model `18`, code `19`")]
        [InlineData("src/Acme.Infrastructure/Persistence/AcmeDbContext.cs", "HasColumnName(\"email_address\")", "HasColumnName(\"email\")",
            "`Customer.Email` column name: model `email_address`, code `email`")]
        [InlineData("src/Acme.Domain/Entities/OrderLine.cs", "[Column(\"qty\")]", "[Column(\"quantity\")]",
            "`OrderLine.Quantity` column name: model `qty`, code `quantity`")]
        [InlineData("src/Acme.Infrastructure/Persistence/AcmeDbContext.cs", "HasMaxLength(100).IsUnicode(false)", "HasMaxLength(100)",
            "`Customer.Name` text type: model `VARCHAR`, code `DEFAULT`")]
        [InlineData("src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "nvarchar(max)", "varchar(max)",
            "`Order.Notes` text type: model `NVARCHAR`, code `VARCHAR`")]
        [InlineData("src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "WithMany(c => c.Orders)", "WithMany(c => c.Orders).IsRequired(false)",
            "`Order.Customer` multiplicity: model `*->1`, code `*->0..1`")]
        [InlineData("src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "HasForeignKey(o => o.CustomerId)", "HasForeignKey(o => o.CustomerRef)",
            "`Order.Customer` foreign key: model `CustomerId`, code `CustomerRef`")]
        [InlineData("src/Acme.Domain/Entities/OrderLine.cs", "public virtual Product? Product", "public virtual Product Product",
            "`OrderLine.Product` multiplicity: model `*->0..1`, code `*->1`")]
        [InlineData("src/Acme.Infrastructure/Persistence/AcmeDbContext.cs", "\"IX_Customers_Code\").IsUnique()", "\"IX_Customers_Code\")",
            "`Customer.IX_Customers_Code` unique: model `true`, code `false`")]
        [InlineData("src/Acme.Domain/Entities/OrderLine.cs", "IsUnique = true", "IsUnique = false",
            "`OrderLine.IX_OrderLines_Product` unique: model `true`, code `false`")]
        [InlineData("src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "new { o.CustomerId, o.PlacedAt }", "new { o.PlacedAt, o.CustomerId }",
            "`Order.IX_Orders_Customer_Placed` columns: model `CustomerId,PlacedAt`, code `PlacedAt,CustomerId`")]
        [InlineData("src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "getutcdate()", "sysutcdatetime()",
            "`Order.PlacedAt` default: model `getutcdate()`, code `sysutcdatetime()`")]
        [InlineData("src/Acme.Infrastructure/Persistence/AcmeDbContext.cs", "ToView(\"vw_CustomerSummary\")", "ToTable(\"vw_CustomerSummary\")",
            "`CustomerSummary` table or view: model `View`, code `Table`")]
        [InlineData("src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "HasKey(o => o.Id)", "HasKey(o => new { o.Id, o.CustomerId })",
            "`Order` primary key: model `Id`, code `Id,CustomerId`")]
        [InlineData("src/Acme.Domain/Entities/Product.cs", "public int Id { get; set; }", "public int ProductNo { get; set; }",
            "`Product` primary key: model `Id`, code (none)")]
        [InlineData("src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "HasMany(o => o.Lines).WithOne(l => l.Order)", "HasMany(o => o.Lines).WithMany(l => l.Orders)",
            "`Order.Lines` multiplicity: model `1->*`, code `*->*`")]
        public void An_EF_Core_change_is_reported_as_its_one_row(string path, string from, string to, string row)
        {
            Check(Domain, EfCoreRecipe, EfCoreModel(), Mutate(EfCore, path, from, to)).ShouldReport(row);
        }

        [Fact]
        public void Making_a_column_fixed_length_changes_its_type_text_type_and_length()
        {
            var files = Mutate(EfCore, "src/Acme.Infrastructure/Persistence/AcmeDbContext.cs", ".IsFixedLength();", ";");

            Check(Domain, EfCoreRecipe, EfCoreModel(), files).ShouldReport(
                "`Customer.Code` column type: model `char(5)`, code (none)",
                "`Customer.Code` text type: model `DEFAULT`, code `VARCHAR`",
                "`Customer.Code` max length: model (none), code `5`");
        }

        [Fact]
        public void An_unmodelled_EF_Core_entity_configuration_is_reported()
        {
            var files = Mutate(EfCore, "src/Acme.Infrastructure/Persistence/AcmeDbContext.cs",
                "modelBuilder.Entity<CustomerSummary>(", "modelBuilder.Entity<AuditLog>(entity => entity.ToTable(\"AuditLogs\"));\n        modelBuilder.Entity<CustomerSummary>(");

            Check(Domain, EfCoreRecipe, EfCoreModel(), files).ShouldReport("entity configuration `AuditLog`: not modelled");
        }

        [Fact]
        public void A_table_name_from_a_constant_can_not_be_verified()
        {
            var files = Mutate(EfCore, "src/Acme.Infrastructure/Persistence/Configurations/OrderConfiguration.cs", "ToTable(\"Orders\")", "ToTable(TableNames.Orders)");

            Check(Domain, EfCoreRecipe, EfCoreModel(), files).Wrong(DriftSection.Unverifiable).ShouldHaveSingleItem().ShouldStartWith("`Order` table name: stated in a form the recipe can't read: `.ToTable(TableNames.Orders)`");
        }

        [Fact]
        public void The_EF_Core_recipe_for_nullable_reference_types_off_reads_strings_as_optional_unless_they_say_otherwise()
        {
            var recipe = WithSnippet(EfCoreRecipe, DomainGuides.EfCore, "with nullable reference types off");
            var files = new Dictionary<string, string>
            {
                ["src/Legacy/Supplier.cs"] = """
                    public class Supplier
                    {
                        public int Id { get; set; }
                        [Required]
                        public string Name { get; set; }
                        public string Notes { get; set; }
                        public string Phone { get; set; }
                    }
                    """,
                ["src/Legacy/LegacyContext.cs"] = """
                    public class LegacyContext : DbContext
                    {
                        public DbSet<Supplier> Suppliers { get; set; }

                        protected override void OnModelCreating(ModelBuilder modelBuilder)
                        {
                            modelBuilder.Entity<Supplier>(entity =>
                            {
                                entity.Property(e => e.Phone).IsRequired();
                            });
                        }
                    }
                    """
            };
            var model = new List<ModelFact>
            {
                Column(FactKinds.ColumnRequired, "Supplier", "Name", "true"),
                Column(FactKinds.ColumnRequired, "Supplier", "Notes", "false"),
                Column(FactKinds.ColumnRequired, "Supplier", "Phone", "true")
            };

            Check(Domain, recipe, model, files).WrongLines.ShouldBeEmpty();
            ShouldValidate(Domain, recipe, model, files);
            Check(Domain, recipe, model, Mutate(files, "src/Legacy/Supplier.cs", "[Required]\n    public string Name", "public string Name"))
                .ShouldReport("`Supplier.Name` required: model `true`, code `false`");
        }

        // MongoDB: one document mapped by attributes, one by a class map; an embedded document and a reference; an index
        // created with a name.
        private static readonly Dictionary<string, string> Mongo = new()
        {
            ["src/Shop.Domain/Documents/Customer.cs"] = """
                namespace Shop.Domain.Documents;

                public class Customer
                {
                    [BsonId]
                    [BsonRepresentation(BsonType.ObjectId)]
                    public string Id { get; set; } = null!;

                    [BsonElement("name")]
                    [BsonRequired]
                    public string Name { get; set; } = null!;

                    public string? Email { get; set; }

                    [BsonDefaultValue("bronze")]
                    public string Tier { get; set; } = "bronze";

                    public Address Address { get; set; } = null!;
                }

                public class Address
                {
                    public string Street { get; set; } = null!;
                }
                """,
            ["src/Shop.Domain/Documents/Order.cs"] = """
                namespace Shop.Domain.Documents;

                public class Order
                {
                    public string Id { get; set; } = null!;
                    public string CustomerId { get; set; } = null!;
                    public string? SalesRepId { get; set; }
                    public List<OrderLine> Lines { get; set; } = new();
                    public decimal Total { get; set; }
                }

                public class OrderLine
                {
                    public string ProductName { get; set; } = null!;
                    public int Quantity { get; set; }
                }
                """,
            ["src/Shop.Infrastructure/Mongo/MongoMappings.cs"] = """
                namespace Shop.Infrastructure.Mongo;

                public static class MongoMappings
                {
                    public static void Register()
                    {
                        BsonClassMap.RegisterClassMap<Order>(map =>
                        {
                            map.AutoMap();
                            map.MapIdMember(o => o.Id);
                            map.MapMember(o => o.Total).SetElementName("total");
                            map.MapMember(o => o.CustomerId).SetElementName("customerId");
                        });
                    }
                }
                """,
            ["src/Shop.Infrastructure/Mongo/MongoIndexes.cs"] = """
                namespace Shop.Infrastructure.Mongo;

                public static class MongoIndexes
                {
                    public static async Task CreateAsync(IMongoDatabase database)
                    {
                        var orders = database.GetCollection<Order>("orders");
                        await orders.Indexes.CreateOneAsync(new CreateIndexModel<Order>(
                            Builders<Order>.IndexKeys.Ascending(o => o.CustomerId).Descending(o => o.Total),
                            new CreateIndexOptions { Name = "IX_Orders_Customer_Total", Unique = true }));
                    }
                }
                """
        };

        private static List<ModelFact> MongoModel() => new List<ModelFact>()
            .Concat(Entity("Customer", "Id"))
            .Concat(Text("Customer", "Id", key: true))
            .Concat(Text("Customer", "Name", name: "name"))
            .Concat(Text("Customer", "Email", required: "false"))
            .Concat(Text("Customer", "Tier", @default: "\"bronze\""))
            .Concat(Entity("Order", "Id"))
            .Concat(Text("Order", "Id", key: true))
            .Concat(Text("Order", "CustomerId", name: "customerId"))
            .Concat(Text("Order", "SalesRepId", required: "false"))
            .Concat(Decimal("Order", "Total", name: "total"))
            .Concat(new[]
            {
                Relationship(FactKinds.RelationshipMultiplicity, "Customer", "Address", "", "Address", "1->1"),
                Relationship(FactKinds.RelationshipForeignKey, "Customer", "Address", "", "Address", ""),
                Relationship(FactKinds.RelationshipMultiplicity, "Order", "OrderLine", "", "Lines", "1->*"),
                Relationship(FactKinds.RelationshipForeignKey, "Order", "OrderLine", "", "Lines", ""),
                Relationship(FactKinds.RelationshipMultiplicity, "Order", "Customer", "", "Customer", "*->1"),
                Relationship(FactKinds.RelationshipForeignKey, "Order", "Customer", "", "Customer", "CustomerId"),
                Index(FactKinds.IndexColumns, "Order", "IX_Orders_Customer_Total", "CustomerId,Total"),
                Index(FactKinds.IndexUnique, "Order", "IX_Orders_Customer_Total", "true")
            })
            .ToList();

        private static string MongoRecipe => RecipeFile(DomainGuides.MongoDb);

        [Fact]
        public void The_MongoDB_recipe_file_reads_attributes_and_class_maps_with_no_drift()
        {
            Check(Domain, MongoRecipe, MongoModel(), Mongo).ShouldBeClean();
        }

        [Fact]
        public void The_MongoDB_recipe_file_validates_and_reads_values_where_the_guide_says()
        {
            var validation = ShouldValidate(Domain, MongoRecipe, MongoModel(), Mongo);

            validation.Sources(FactKinds.Key).ShouldBe((2, 0, 0));
            validation.Sources(FactKinds.ColumnName).ShouldBe((3, 5, 0));
            validation.Sources(FactKinds.RelationshipMultiplicity).ShouldBe((3, 0, 0));
            validation.Sources(FactKinds.RelationshipForeignKey).ShouldBe((1, 0, 2));
            validation.Sources(FactKinds.IndexColumns).ShouldBe((1, 0, 0));
        }

        [Theory]
        [InlineData("src/Shop.Domain/Documents/Customer.cs", "[BsonElement(\"name\")]", "[BsonElement(\"fullName\")]",
            "`Customer.Name` column name: model `name`, code `fullName`")]
        [InlineData("src/Shop.Infrastructure/Mongo/MongoMappings.cs", "SetElementName(\"total\")", "SetElementName(\"amount\")",
            "`Order.Total` column name: model `total`, code `amount`")]
        [InlineData("src/Shop.Domain/Documents/Customer.cs", "public string? Email", "public string Email",
            "`Customer.Email` required: model `false`, code `true`")]
        [InlineData("src/Shop.Domain/Documents/Customer.cs", "[BsonDefaultValue(\"bronze\")]", "[BsonDefaultValue(\"silver\")]",
            "`Customer.Tier` default: model `\"bronze\"`, code `\"silver\"`")]
        [InlineData("src/Shop.Infrastructure/Mongo/MongoIndexes.cs", "Unique = true", "Unique = false",
            "`Order.IX_Orders_Customer_Total` unique: model `true`, code `false`")]
        [InlineData("src/Shop.Infrastructure/Mongo/MongoIndexes.cs", ".Descending(o => o.Total)", "",
            "`Order.IX_Orders_Customer_Total` columns: model `CustomerId,Total`, code `CustomerId`")]
        [InlineData("src/Shop.Domain/Documents/Order.cs", "public List<OrderLine> Lines { get; set; } = new();", "public OrderLine Lines { get; set; } = new();",
            "`Order.Lines` multiplicity: model `1->*`, code `1->1`")]
        [InlineData("src/Shop.Infrastructure/Mongo/MongoMappings.cs", "map.MapIdMember(o => o.Id);", "map.MapIdMember(o => o.CustomerId);",
            "`Order` primary key: model `Id`, code `CustomerId`")]
        public void A_MongoDB_change_is_reported_as_its_one_row(string path, string from, string to, string row)
        {
            Check(Domain, MongoRecipe, MongoModel(), Mutate(Mongo, path, from, to)).ShouldReport(row);
        }

        [Fact]
        public void A_MongoDB_reference_made_optional_changes_its_multiplicity_and_its_attribute()
        {
            var files = Mutate(Mongo, "src/Shop.Domain/Documents/Order.cs", "public string CustomerId", "public string? CustomerId");

            Check(Domain, MongoRecipe, MongoModel(), files).ShouldReport(
                "`Order.Customer` multiplicity: model `*->1`, code `*->0..1`",
                "`Order.CustomerId` required: model `true`, code `false`");
        }

        [Fact]
        public void An_unmodelled_MongoDB_class_map_is_reported()
        {
            var files = Mutate(Mongo, "src/Shop.Infrastructure/Mongo/MongoMappings.cs", "BsonClassMap.RegisterClassMap<Order>(",
                "BsonClassMap.RegisterClassMap<Invoice>(map => map.AutoMap());\n        BsonClassMap.RegisterClassMap<Order>(");

            Check(Domain, MongoRecipe, MongoModel(), files).ShouldReport("class map `Invoice`: not modelled");
        }

        // Azure Cosmos DB through its SDK: Newtonsoft.Json names on one document, System.Text.Json names on another.
        private static readonly Dictionary<string, string> Cosmos = new()
        {
            ["src/Billing.Domain/Invoice.cs"] = """
                namespace Billing.Domain;

                public class Invoice
                {
                    [JsonProperty("id")]
                    public string Id { get; set; } = null!;

                    [JsonProperty("customerId")]
                    public string CustomerId { get; set; } = null!;

                    [JsonProperty(PropertyName = "number", Required = Required.Always)]
                    public string Number { get; set; } = null!;

                    public string? Memo { get; set; }

                    public List<InvoiceLine> Lines { get; set; } = new();
                }

                public class InvoiceLine
                {
                    public string Description { get; set; } = null!;
                }
                """,
            ["src/Billing.Domain/Customer.cs"] = """
                namespace Billing.Domain;

                public class Customer
                {
                    [JsonPropertyName("id")]
                    public string Id { get; set; } = null!;

                    [JsonPropertyName("displayName")]
                    public string Name { get; set; } = null!;
                }
                """
        };

        private static List<ModelFact> CosmosModel() => new List<ModelFact>()
            .Concat(Entity("Invoice", "Id"))
            .Concat(Text("Invoice", "Id", name: "id", key: true))
            .Concat(Text("Invoice", "CustomerId", name: "customerId"))
            .Concat(Text("Invoice", "Number", name: "number"))
            .Concat(Text("Invoice", "Memo", required: "false"))
            .Concat(Entity("Customer", "Id"))
            .Concat(Text("Customer", "Id", name: "id", key: true))
            .Concat(Text("Customer", "Name", name: "displayName"))
            .Concat(new[]
            {
                Relationship(FactKinds.RelationshipMultiplicity, "Invoice", "InvoiceLine", "", "Lines", "1->*"),
                Relationship(FactKinds.RelationshipForeignKey, "Invoice", "InvoiceLine", "", "Lines", ""),
                Relationship(FactKinds.RelationshipMultiplicity, "Invoice", "Customer", "", "Customer", "*->1"),
                Relationship(FactKinds.RelationshipForeignKey, "Invoice", "Customer", "", "Customer", "CustomerId")
            })
            .ToList();

        private static string CosmosRecipe => RecipeFile(DomainGuides.CosmosDb);

        [Fact]
        public void The_Cosmos_DB_recipe_file_reads_both_serializers_names_with_no_drift()
        {
            Check(Domain, CosmosRecipe, CosmosModel(), Cosmos).ShouldBeClean();
        }

        [Fact]
        public void The_Cosmos_DB_recipe_file_validates_and_reads_values_where_the_guide_says()
        {
            var validation = ShouldValidate(Domain, CosmosRecipe, CosmosModel(), Cosmos);

            validation.Sources(FactKinds.Key).ShouldBe((2, 0, 0));
            validation.Sources(FactKinds.ColumnName).ShouldBe((5, 1, 0));
            validation.Sources(FactKinds.ColumnRequired).ShouldBe((2, 2, 0));
        }

        [Theory]
        [InlineData("src/Billing.Domain/Invoice.cs", "[JsonProperty(\"customerId\")]", "[JsonProperty(\"customer\")]",
            "`Invoice.CustomerId` column name: model `customerId`, code `customer`")]
        [InlineData("src/Billing.Domain/Customer.cs", "[JsonPropertyName(\"displayName\")]", "[JsonPropertyName(\"name\")]",
            "`Customer.Name` column name: model `displayName`, code `name`")]
        [InlineData("src/Billing.Domain/Invoice.cs", "public string? Memo", "public string Memo",
            "`Invoice.Memo` required: model `false`, code `true`")]
        [InlineData("src/Billing.Domain/Invoice.cs", "public List<InvoiceLine> Lines", "public InvoiceLine? Lines",
            "`Invoice.Lines` multiplicity: model `1->*`, code `1->0..1`")]
        [InlineData("src/Billing.Domain/Invoice.cs", "public string CustomerId", "public string? CustomerId",
            "`Invoice.Customer` multiplicity: model `*->1`, code `*->0..1`", "`Invoice.CustomerId` required: model `true`, code `false`")]
        public void A_Cosmos_DB_change_is_reported_as_its_rows(string path, string from, string to, params string[] rows)
        {
            Check(Domain, CosmosRecipe, CosmosModel(), Mutate(Cosmos, path, from, to)).ShouldReport(rows);
        }

        [Fact]
        public void The_Cosmos_DB_guides_EF_Core_provider_recipe_reads_ToJsonProperty()
        {
            var recipe = WithSnippet(EfCoreRecipe, DomainGuides.CosmosDb, "ToJsonProperty");
            var files = new Dictionary<string, string>
            {
                ["src/Cosmos/Order.cs"] = """
                    public class Order
                    {
                        public Guid Id { get; set; }
                        public string Reference { get; set; } = null!;
                        public string? Notes { get; set; }
                    }
                    """,
                ["src/Cosmos/OrdersContext.cs"] = """
                    public class OrdersContext : DbContext
                    {
                        protected override void OnModelCreating(ModelBuilder modelBuilder)
                        {
                            modelBuilder.Entity<Order>(entity =>
                            {
                                entity.ToContainer("Orders").HasPartitionKey(o => o.Id).HasNoDiscriminator();
                                entity.Property(o => o.Reference).ToJsonProperty("ref");
                            });
                        }
                    }
                    """
            };
            var model = new List<ModelFact>
            {
                Column(FactKinds.ColumnName, "Order", "Reference", "ref"),
                Column(FactKinds.ColumnName, "Order", "Notes", "")
            };

            Check(Domain, recipe, model, files).WrongLines.ShouldBeEmpty();
            Check(Domain, recipe, model, Mutate(files, "src/Cosmos/OrdersContext.cs", "ToJsonProperty(\"ref\")", "ToJsonProperty(\"reference\")"))
                .ShouldReport("`Order.Reference` column name: model `ref`, code `reference`");
        }
    }
}
