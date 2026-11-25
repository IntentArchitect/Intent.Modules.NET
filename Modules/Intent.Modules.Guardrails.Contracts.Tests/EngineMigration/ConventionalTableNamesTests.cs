using System.Collections.Generic;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Drift.EngineMigration;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests.EngineMigration
{
    /// <summary>
    /// Which modelled Table Names the migration clears: those the code states only as EF Core's convention does, which v1's
    /// recipes could read and the drift engine doesn't.
    /// </summary>
    public class ConventionalTableNamesTests
    {
        private const string Context = """
            public class ShopContext : DbContext
            {
                public virtual DbSet<Order> Orders { get; set; }
                public virtual DbSet<Customer> Customers { get; set; }
                public virtual DbSet<Model.Invoice> Invoices { get; set; }
                public virtual DbSet<Log> Log { get; set; }
                public virtual DbSet<Product> Products { get; set; }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<Order>(entity =>
                    {
                        entity.HasKey(e => e.Id);
                    });

                    modelBuilder.Entity<Customer>(entity =>
                    {
                        entity.ToTable("Customers");
                    });

                    modelBuilder.Entity<Model.Invoice>().ToTable("Invoices", "billing");
                }
            }

            public class ProductConfiguration : IEntityTypeConfiguration<Product>
            {
                public void Configure(EntityTypeBuilder<Product> builder)
                {
                    builder.ToView("Products");
                }
            }

            [Table("Shipments")]
            public class Shipment { }
            """;

        [Fact]
        public void A_name_only_the_DbSet_or_the_class_gives_is_cleared()
        {
            Find(("Order", "Orders"), ("Log", "Log"), ("Note", "Note")).ShouldBe(new[] { "Order", "Log", "Note" });
        }

        [Fact]
        public void A_name_a_table_statement_gives_stays()
        {
            Find(("Customer", "Customers"), ("Invoice", "Invoices"), ("Product", "Products"), ("Shipment", "Shipments")).ShouldBeEmpty();
        }

        [Fact]
        public void A_name_EF_Core_wouldnt_use_stays_for_the_engine_to_report()
        {
            Find(("Order", "Purchases")).ShouldBeEmpty();
        }

        [Fact]
        public void A_table_statement_whose_entity_cant_be_told_clears_nothing()
        {
            var notes = new List<string>();
            var code = new CodeFiles(new Dictionary<string, string>
            {
                ["Data/ShopContext.cs"] = Context,
                ["Data/Mapping.cs"] = "public static class Mapping { public static void Map(dynamic builder) { builder.ToTable(\"Orders\"); } }"
            });

            ConventionalTableNames.Find(code, new[] { ("Order", "Orders") }, notes).ShouldBeEmpty();
            notes.ShouldHaveSingleItem().ShouldContain("builder.ToTable(\"Orders\")");
        }

        [Fact]
        public void A_migrations_snapshot_states_no_table_name()
        {
            var notes = new List<string>();
            var code = new CodeFiles(new Dictionary<string, string>
            {
                ["Data/ShopContext.cs"] = Context,
                ["Migrations/ShopContextModelSnapshot.cs"] = """
                    [DbContext(typeof(ShopContext))]
                    partial class ShopContextModelSnapshot : ModelSnapshot
                    {
                        protected override void BuildModel(ModelBuilder modelBuilder)
                        {
                            modelBuilder.Entity("Shop.Order", b =>
                            {
                                b.Property<int>("Id");
                                b.HasKey("Id");
                                b.ToTable("Orders");
                            });

                            modelBuilder.Entity("Shop.Order", b =>
                            {
                                b.OwnsOne("Shop.Address", "Address", b1 =>
                                {
                                    b1.ToTable("Orders");
                                });
                            });
                        }
                    }
                    """
            });

            ConventionalTableNames.Find(code, new[] { ("Order", "Orders") }, notes).ShouldBe(new[] { "Order" });
            notes.ShouldBeEmpty();
        }

        private static IReadOnlyList<string> Find(params (string Entity, string Table)[] modelled)
        {
            var notes = new List<string>();
            var result = ConventionalTableNames.Find(new CodeFiles(new Dictionary<string, string> { ["Data/ShopContext.cs"] = Context }), modelled, notes);
            notes.ShouldBeEmpty();
            return result;
        }
    }
}
