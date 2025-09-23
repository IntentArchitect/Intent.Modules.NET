using System;
using System.Linq;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Enums;
using EntityFrameworkCore.SqlServer.EF8.Domain.Enums;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence.Configurations.Enums
{
    public class AddressInfoConfiguration : IEntityTypeConfiguration<AddressInfo>
    {
        public void Configure(EntityTypeBuilder<AddressInfo> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.AddressType)
                .IsRequired();

            builder.Property(x => x.BuildingType);

            builder.ToTable(tb => tb.HasCheckConstraint("address_info_address_type_check", $"\"AddressType\" IN ({string.Join(",", Enum.GetValues<AddressType>().Select(e => $"{e:D}"))})"));

            builder.ToTable(tb => tb.HasCheckConstraint("address_info_building_type_check", $"\"BuildingType\" IS NULL OR \"BuildingType\" IN ({string.Join(",", Enum.GetValues<BuildingType>().Select(e => $"{e:D}"))})"));
        }
    }
}