using Entities.Constants.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Entities.Constants.TestApplication.Infrastructure.Persistence.Configurations
{
    public class TestClassConfiguration : IEntityTypeConfiguration<TestClass>
    {
        public void Configure(EntityTypeBuilder<TestClass> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Att100)
                .IsRequired()
                .HasMaxLength(TestClass.Att100MaxLength);

            builder.Property(x => x.VarChar200)
                .IsRequired()
                .HasColumnType($"varchar({TestClass.VarChar200MaxLength})");

            builder.Property(x => x.NVarChar300)
                .IsRequired()
                .HasColumnType($"nvarchar({TestClass.NVarChar300MaxLength})");

            builder.Property(x => x.AttMax)
                .IsRequired();

            builder.Property(x => x.VarCharMax)
                .IsRequired()
                .HasColumnType("varchar(max)");

            builder.Property(x => x.NVarCharMax)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}