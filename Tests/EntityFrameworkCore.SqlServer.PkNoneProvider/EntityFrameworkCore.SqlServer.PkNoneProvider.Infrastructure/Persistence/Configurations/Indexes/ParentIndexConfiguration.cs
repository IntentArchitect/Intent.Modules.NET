using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence.Configurations.Indexes
{
    public class ParentIndexConfiguration : IEntityTypeConfiguration<ParentIndex>
    {
        public void Configure(EntityTypeBuilder<ParentIndex> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.OwnsMany(x => x.MultiChildIndices, ConfigureMultiChildIndices);

            builder.OwnsOne(x => x.SingleChildIndex, ConfigureSingleChildIndex);
        }

        public static void ConfigureMultiChildIndices(OwnedNavigationBuilder<ParentIndex, MultiChildIndex> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.ParentIndexId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.ParentIndexId)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Surname)
                .IsRequired();

            builder.HasIndex(x => x.Surname);

            builder.HasIndex(x => x.Name)
                .HasDatabaseName("IX_Child_Name");
        }

        public static void ConfigureSingleChildIndex(OwnedNavigationBuilder<ParentIndex, SingleChildIndex> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Surname)
                .IsRequired();

            builder.HasIndex(x => x.Surname);

            builder.HasIndex(x => x.Name)
                .HasDatabaseName("IX_Child_Name");
        }
    }
}