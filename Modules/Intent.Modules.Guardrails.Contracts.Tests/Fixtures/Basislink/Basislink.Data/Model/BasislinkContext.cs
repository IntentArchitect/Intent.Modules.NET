// Excerpt of Basislink's database-first context (2026-10-19): the sapPlants block the authority audit found
// wrong, and its neighbour sapPlantsMapping, whose name starts the same way.
using Basislink.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Basislink.Data.Model
{
    public partial class BasislinkContext : DbContext
    {
        public BasislinkContext()
        {
        }

        public virtual DbSet<sapPlants> sapPlants { get; set; }
        public virtual DbSet<sapPlantsMapping> sapPlantsMapping { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<sapPlants>(entity =>
            {
                entity.HasKey(e => e.code)
                    .HasName("PK_SAPPlants");

                entity.ToTable("sapPlants", "grain");

                entity.Property(e => e.code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyCode)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.PurchaseOrg)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SalesOrg)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.costcenter)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.description)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.differential)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.enabled)
                    .IsRequired()
                    .HasDefaultValueSql("(1)");

                entity.Property(e => e.profitcenter)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<sapPlantsMapping>(entity =>
            {
                entity.ToTable("sapPlantsMapping", "grain");

                entity.Property(e => e.defaultCompanyCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.mappingName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });
        }
    }
}
