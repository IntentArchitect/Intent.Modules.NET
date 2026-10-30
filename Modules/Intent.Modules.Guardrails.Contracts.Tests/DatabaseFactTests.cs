using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>Database facts read from EF Core configuration, with a recipe written as the AI would.</summary>
    public class DatabaseFactTests
    {
        private const string Recipe = """
        {
          "files": "src/**/*.cs",
          "facts": {
            "table.name": {
              "find": "statement:Entity<{Entity}>",
              "read": [
                { "match": "To(?:Table|View)\\(\"([^\"]+)\"" },
                { "match": "DbSet<{Entity}>(\\w+)\\{" }
              ],
              "inherit": "property with DbSet<{Entity}>"
            },
            "table.schema": {
              "find": "statement:Entity<{Entity}>",
              "read": [{ "match": "To(?:Table|View)\\(\"[^\"]+\",\"([^\"]+)\"" }],
              "default": ""
            },
            "table.kind": {
              "find": "statement:Entity<{Entity}>",
              "read": [{ "match": "\\.ToView\\(", "value": "View" }],
              "default": "Table"
            },
            "key": {
              "find": "statement:Entity<{Entity}>",
              "read": [
                { "match": "HasKey\\(\\w+=>new\\{([^}]*)\\}" },
                { "match": "HasKey\\(\\w+=>\\w+\\.(\\w+)\\)" },
                { "match": "HasNoKey\\(\\)", "value": "" }
              ],
              "replace": [{ "match": "\\b\\w+\\.", "with": "" }],
              "default": "Id"
            },
            "column.name": {
              "find": "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
              "read": [{ "match": "HasColumnName\\(\"([^\"]+)\"" }],
              "default": "",
              "absent": ""
            },
            "column.type": {
              "find": "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
              "read": [{ "match": "HasColumnType\\(\"(?!decimal|numeric)([^\"]+)\"" }],
              "default": "",
              "absent": ""
            },
            "column.textType": {
              "find": "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
              "read": [
                { "match": "IsUnicode\\(false\\).*IsFixedLength\\(\\)|IsFixedLength\\(\\).*IsUnicode\\(false\\)", "value": "CHAR" },
                { "match": "IsFixedLength\\(\\)", "value": "NCHAR" },
                { "match": "HasColumnType\\(\"(n?(?:var)?char|n?text)", "case": "upper" },
                { "match": "IsUnicode\\(false\\)", "value": "VARCHAR" }
              ],
              "default": "DEFAULT",
              "absent": "DEFAULT"
            },
            "column.maxLength": {
              "find": "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
              "read": [{ "match": "HasMaxLength\\((\\d+)\\)" }],
              "default": "",
              "absent": ""
            },
            "column.required": {
              "find": "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
              "read": [{ "match": "IsRequired\\(\\)", "value": "true" }],
              "default": "false",
              "absent": "false"
            },
            "column.precision": {
              "find": "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
              "read": [
                { "match": "HasColumnType\\(\"(?:decimal|numeric)\\((\\d+)," },
                { "match": "HasPrecision\\((\\d+)" }
              ],
              "default": "",
              "absent": ""
            },
            "column.scale": {
              "find": "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
              "read": [
                { "match": "HasColumnType\\(\"(?:decimal|numeric)\\(\\d+,\\s?(\\d+)\\)" },
                { "match": "HasPrecision\\(\\d+,(\\d+)\\)" }
              ],
              "default": "",
              "absent": ""
            },
            "column.default": {
              "find": "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
              "read": [
                { "match": "HasDefaultValueSql\\(\"([^\"]*)\"" },
                { "match": "HasDefaultValue\\((.*?)\\)(?:\\.|$)" }
              ],
              "default": "",
              "absent": ""
            },
            "relationship.multiplicity": {
              "find": [
                "statement:Entity<{Source}> > chain with HasOne\\(\\w+=>\\w+\\.{TargetEnd}\\)",
                "statement:Entity<{Target}> > chain with HasMany\\(\\w+=>\\w+\\.{SourceEnd}\\)"
              ],
              "read": [
                { "match": "HasOne\\([^)]*\\)\\.WithMany\\(", "value": "*->1" },
                { "match": "HasMany\\([^)]*\\)\\.WithOne\\(", "value": "*->1" },
                { "match": "HasOne\\([^)]*\\)\\.WithOne\\(", "value": "1->1" }
              ]
            },
            "relationship.foreignKey": {
              "find": [
                "statement:Entity<{Source}> > chain with HasOne\\(\\w+=>\\w+\\.{TargetEnd}\\)",
                "statement:Entity<{Target}> > chain with HasMany\\(\\w+=>\\w+\\.{SourceEnd}\\)"
              ],
              "read": [{ "match": "HasForeignKey\\(\\w+=>\\w+\\.(\\w+)\\)" }],
              "default": ""
            }
          },
          "own": [
            { "kind": "entity configuration", "find": "statement with modelBuilder\\.Entity<(\\w+)>", "name": "$1" }
          ]
        }
        """;

        private const string Context = """
        using Microsoft.EntityFrameworkCore;

        namespace Acme.Infrastructure
        {
            public class AppDbContext : DbContext
            {
                public DbSet<Customer> Customers { get; set; }
                public DbSet<Order> Orders { get; set; }
                public DbSet<Product> Products { get; set; }

                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<Customer>(entity =>
                    {
                        entity.ToTable("Customers", "sales");
                        entity.HasKey(e => e.CustomerId);
                        entity.Property(e => e.Name)
                            .IsRequired()
                            .HasMaxLength(100)
                            .IsUnicode(false);
                        // A comment, which the check ignores: .HasMaxLength(5)
                        entity.Property(e => e.Code).HasColumnType("char(5)");
                        entity.Property(e => e.CreditLimit).HasColumnType("decimal(18, 2)");
                        entity.Property(e => e.CreatedOn).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
                        entity.Property(e => e.Email).HasColumnName("email_address");
                    });

                    modelBuilder.Entity<Order>(entity =>
                    {
                        entity.HasKey(e => new { e.OrderNo, e.Line });
                        entity.Property(e => e.Notes);
                        entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasForeignKey(d => d.CustomerId);
                    });

                    modelBuilder.Entity<Product>(entity =>
                    {
                        entity.Property(e => e.Price).HasPrecision(10, 3);
                    });
                }
            }
        }
        """;

        private static Dictionary<string, string> Code(string context = Context) => new() { ["src/Infrastructure/AppDbContext.cs"] = context };

        private static List<ModelFact> Model() => new()
        {
            Table(FactKinds.TableName, "Customer", "Customers"),
            Table(FactKinds.TableSchema, "Customer", "sales"),
            Table(FactKinds.TableKind, "Customer", "Table"),
            Table(FactKinds.Key, "Customer", "CustomerId"),
            Column(FactKinds.ColumnTextType, "Customer", "Name", "VARCHAR"),
            Column(FactKinds.ColumnMaxLength, "Customer", "Name", "100"),
            Column(FactKinds.ColumnRequired, "Customer", "Name", "true"),
            Column(FactKinds.ColumnName, "Customer", "Name", ""),
            Column(FactKinds.ColumnType, "Customer", "Code", "char(5)"),
            Column(FactKinds.ColumnPrecision, "Customer", "CreditLimit", "18"),
            Column(FactKinds.ColumnScale, "Customer", "CreditLimit", "2"),
            Column(FactKinds.ColumnType, "Customer", "CreditLimit", ""),
            Column(FactKinds.ColumnType, "Customer", "CreatedOn", "datetime"),
            Column(FactKinds.ColumnDefault, "Customer", "CreatedOn", "(getdate())"),
            Column(FactKinds.ColumnName, "Customer", "Email", "email_address"),
            Column(FactKinds.ColumnTextType, "Customer", "Email", "DEFAULT"),
            Column(FactKinds.ColumnRequired, "Customer", "Email", "false"),
            Table(FactKinds.TableName, "Order", "Orders"),
            Table(FactKinds.TableSchema, "Order", ""),
            Table(FactKinds.Key, "Order", "OrderNo,Line"),
            Column(FactKinds.ColumnTextType, "Order", "Notes", "DEFAULT"),
            Column(FactKinds.ColumnMaxLength, "Order", "Notes", ""),
            Column(FactKinds.ColumnDefault, "Order", "Notes", ""),
            Relationship(FactKinds.RelationshipMultiplicity, "*->1"),
            Relationship(FactKinds.RelationshipForeignKey, "CustomerId"),
            Table(FactKinds.TableName, "Product", "Products"),
            Table(FactKinds.Key, "Product", "Id"),
            Column(FactKinds.ColumnPrecision, "Product", "Price", "10"),
            Column(FactKinds.ColumnScale, "Product", "Price", "3")
        };

        private static ModelFact Relationship(FactKind kind, string value)
        {
            return Fact(kind, "Order.Customer", value, ("Source", "Order"), ("Target", "Customer"), ("SourceEnd", "Orders"), ("TargetEnd", "Customer"));
        }

        private static List<ModelFact> With(params ModelFact[] changed)
        {
            var model = Model();
            foreach (var fact in changed)
            {
                model[model.FindIndex(x => x.Element == fact.Element && x.Kind == fact.Kind)] = fact;
            }

            return model;
        }

        [Fact]
        public void A_model_that_agrees_with_the_configuration_has_no_drift()
        {
            var report = Check("Domain", Recipe, Model(), Code());

            report.WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void Table_schema_and_kind_are_compared()
        {
            var report = Check("Domain", Recipe, With(
                Table(FactKinds.TableName, "Customer", "Client"),
                Table(FactKinds.TableSchema, "Customer", "dbo"),
                Table(FactKinds.TableKind, "Customer", "View")), Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Customer` schema: model `dbo`, code `sales`. Location: `src/Infrastructure/AppDbContext.cs:15`",
                "`Customer` table name: model `Client`, code `Customers`. Location: `src/Infrastructure/AppDbContext.cs:15`",
                "`Customer` table or view: model `View`, code `Table` (the code doesn't state it). Location: `src/Infrastructure/AppDbContext.cs:13`"
            });
        }

        [Fact]
        public void A_table_named_by_convention_is_read_from_its_DbSet()
        {
            var report = Check("Domain", Recipe, With(Table(FactKinds.TableName, "Order", "Order")), Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Order` table name: model `Order`, code `Orders`. Location: `src/Infrastructure/AppDbContext.cs:8`"
            });
        }

        [Fact]
        public void Keys_are_read_explicitly_and_by_convention()
        {
            var report = Check("Domain", Recipe, With(
                Table(FactKinds.Key, "Customer", "Id"),
                Table(FactKinds.Key, "Order", "OrderNo"),
                Table(FactKinds.Key, "Product", "ProductId")), Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Customer` primary key: model `Id`, code `CustomerId`. Location: `src/Infrastructure/AppDbContext.cs:16`",
                "`Order` primary key: model `OrderNo`, code `OrderNo,Line`. Location: `src/Infrastructure/AppDbContext.cs:30`",
                "`Product` primary key: model `ProductId`, code `Id` (the code doesn't state it). Location: `src/Infrastructure/AppDbContext.cs:35`"
            });
        }

        [Fact]
        public void Text_type_max_length_and_required_are_compared()
        {
            var context = Context.Replace(".HasMaxLength(100)", ".HasMaxLength(120)").Replace(".IsRequired()", string.Empty);

            var report = Check("Domain", Recipe, With(Column(FactKinds.ColumnTextType, "Customer", "Email", "NVARCHAR")), Code(context));

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Customer.Email` text type: model `NVARCHAR`, code `DEFAULT` (the code doesn't state it). Location: `src/Infrastructure/AppDbContext.cs:25`",
                "`Customer.Name` max length: model `100`, code `120`. Location: `src/Infrastructure/AppDbContext.cs:19`",
                "`Customer.Name` required: model `true`, code `false` (the code doesn't state it). Location: `src/Infrastructure/AppDbContext.cs:17`"
            });
        }

        [Fact]
        public void A_fixed_length_column_is_a_char_type_the_model_must_state()
        {
            var report = Check("Domain", Recipe, new List<ModelFact> { Column(FactKinds.ColumnTextType, "Customer", "Code", "VARCHAR") }, Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Customer.Code` text type: model `VARCHAR`, code `CHAR`. Location: `src/Infrastructure/AppDbContext.cs:22`"
            });
        }

        [Fact]
        public void Precision_column_type_column_name_and_default_are_compared()
        {
            var report = Check("Domain", Recipe, With(
                Column(FactKinds.ColumnPrecision, "Customer", "CreditLimit", "19"),
                Column(FactKinds.ColumnScale, "Product", "Price", "2"),
                Column(FactKinds.ColumnType, "Customer", "CreatedOn", ""),
                Column(FactKinds.ColumnName, "Customer", "Email", "email"),
                Column(FactKinds.ColumnDefault, "Customer", "CreatedOn", "(sysdatetime())")), Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Customer.CreatedOn` column type: model (none), code `datetime`. Location: `src/Infrastructure/AppDbContext.cs:24`",
                "`Customer.CreatedOn` default: model `(sysdatetime())`, code `(getdate())`. Location: `src/Infrastructure/AppDbContext.cs:24`",
                "`Customer.CreditLimit` precision: model `19`, code `18`. Location: `src/Infrastructure/AppDbContext.cs:23`",
                "`Customer.Email` column name: model `email`, code `email_address`. Location: `src/Infrastructure/AppDbContext.cs:25`",
                "`Product.Price` scale: model `2`, code `3`. Location: `src/Infrastructure/AppDbContext.cs:37`"
            });
        }

        [Fact]
        public void A_column_the_code_does_not_configure_reads_the_absent_value()
        {
            var report = Check("Domain", Recipe, With(Column(FactKinds.ColumnMaxLength, "Order", "Notes", "500")), Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Order.Notes` max length: model `500`, code (none) (the code doesn't state it). Location: `src/Infrastructure/AppDbContext.cs:31`"
            });

            var unconfigured = Check("Domain", Recipe, new List<ModelFact> { Column(FactKinds.ColumnMaxLength, "Order", "Comment", "50") }, Code());
            unconfigured.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Order.Comment` max length: model `50`, code (none) (the code doesn't state it). Near: `src/Infrastructure/AppDbContext.cs:28`"
            });
        }

        [Fact]
        public void Relationships_are_found_on_either_side()
        {
            var report = Check("Domain", Recipe, With(
                Relationship(FactKinds.RelationshipMultiplicity, "1->1"),
                Relationship(FactKinds.RelationshipForeignKey, "CustomerNo")), Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Order.Customer` foreign key: model `CustomerNo`, code `CustomerId`. Location: `src/Infrastructure/AppDbContext.cs:32`",
                "`Order.Customer` multiplicity: model `1->1`, code `*->1`. Location: `src/Infrastructure/AppDbContext.cs:32`"
            });

            var otherSide = Context
                .Replace(
                    "entity.Property(e => e.Email).HasColumnName(\"email_address\");",
                    "entity.Property(e => e.Email).HasColumnName(\"email_address\");\n                entity.HasMany(c => c.Orders).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId);")
                .Replace("entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasForeignKey(d => d.CustomerId);", string.Empty);
            Check("Domain", Recipe, Model(), Code(otherSide)).WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void Required_is_read_from_the_entity_when_nullable_reference_types_are_on()
        {
            const string recipe = """
            {
              "facts": {
                "column.required": {
                  "find": "class:{Entity} > property:{Attribute}",
                  "read": [{ "part": "declaration", "match": "\\?{Attribute}\\b", "value": "false" }],
                  "default": "true"
                }
              }
            }
            """;
            const string entity = """
            #nullable enable
            namespace Acme.Domain
            {
                public class Customer
                {
                    public string Name { get; set; } = null!;
                    public string? Nickname { get; set; }
                }
            }
            """;
            var files = new Dictionary<string, string> { ["src/Domain/Customer.cs"] = entity };

            Check("Domain", recipe, new List<ModelFact>
            {
                Column(FactKinds.ColumnRequired, "Customer", "Name", "true"),
                Column(FactKinds.ColumnRequired, "Customer", "Nickname", "false")
            }, files).WrongLines.ShouldBeEmpty();

            Check("Domain", recipe, new List<ModelFact>
            {
                Column(FactKinds.ColumnRequired, "Customer", "Name", "false"),
                Column(FactKinds.ColumnRequired, "Customer", "Nickname", "true")
            }, files).Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Customer.Name` required: model `false`, code `true` (the code doesn't state it). Location: `src/Domain/Customer.cs:6`",
                "`Customer.Nickname` required: model `true`, code `false`. Location: `src/Domain/Customer.cs:7`"
            });
        }

        [Fact]
        public void An_unmodelled_entity_configuration_is_code_the_model_does_not_describe()
        {
            var context = Context.Replace(
                "modelBuilder.Entity<Product>(entity =>",
                "modelBuilder.Entity<Supplier>(entity => entity.ToTable(\"Suppliers\"));\n            modelBuilder.Entity<Product>(entity =>");

            var report = Check("Domain", Recipe, Model(), Code(context));

            report.Wrong(DriftSection.Undescribed).ShouldBe(new[]
            {
                "entity configuration `Supplier`: not modelled. Location: `src/Infrastructure/AppDbContext.cs:35`"
            });
        }

        [Fact]
        public void A_disagreement_present_at_the_first_check_is_reported()
        {
            // There is no baseline: the first check of a model and code that already disagree reports the difference.
            var report = Check("Domain", Recipe, With(Column(FactKinds.ColumnMaxLength, "Customer", "Name", "50")), Code());

            report.Wrong(DriftSection.Contradicted).Single().ShouldStartWith("`Customer.Name` max length: model `50`, code `100`.");
        }
    }
}
