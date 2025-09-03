using EntityFrameworkCore.SqlServer.PkNoneProvider.Application.Common.Interfaces;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Application.Security;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Interceptors;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence.Configurations
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

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasConversion(DataMaskConverter.VariableLength(_currentUserService, '*', roles: [Permissions.RoleAdmin], policies: [Permissions.PolicyUser]));

            builder.OwnsOne(x => x.SchemaInLineChild, ConfigureSchemaInLineChild)
                .Navigation(x => x.SchemaInLineChild).IsRequired();
        }

        public static void ConfigureSchemaInLineChild(OwnedNavigationBuilder<SchemaParent, SchemaInLineChild> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}