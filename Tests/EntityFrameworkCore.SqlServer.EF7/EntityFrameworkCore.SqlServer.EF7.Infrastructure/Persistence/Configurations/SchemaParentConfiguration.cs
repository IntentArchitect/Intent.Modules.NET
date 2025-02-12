using EntityFrameworkCore.SqlServer.EF7.Application.Common.Interfaces;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Interceptors;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence.Configurations
{
    public class SchemaParentConfiguration : IEntityTypeConfiguration<SchemaParent>
    {
        private readonly ICurrentUserService _currentUserService;

        public SchemaParentConfiguration(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public void Configure(EntityTypeBuilder<SchemaParent> builder)
        {
            builder.ToTable("SchemaParents", "myapp");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasConversion(DataMaskConverter.VariableLength(_currentUserService, '*', roles: ["admin"], policies: ["user"]));

            builder.OwnsOne(x => x.SchemaInLineChild, ConfigureSchemaInLineChild)
                .Navigation(x => x.SchemaInLineChild).IsRequired();
        }

        public static void ConfigureSchemaInLineChild(OwnedNavigationBuilder<SchemaParent, SchemaInLineChild> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}