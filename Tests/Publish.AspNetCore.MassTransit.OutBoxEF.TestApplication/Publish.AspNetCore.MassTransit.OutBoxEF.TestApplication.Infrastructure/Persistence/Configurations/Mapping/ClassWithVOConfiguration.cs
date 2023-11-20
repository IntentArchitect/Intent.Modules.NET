using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities.Mapping;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Persistence.Configurations.Mapping
{
    public class ClassWithVOConfiguration : IEntityTypeConfiguration<ClassWithVO>
    {
        public void Configure(EntityTypeBuilder<ClassWithVO> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.TestVO, ConfigureTestVO)
                .Navigation(x => x.TestVO).IsRequired();
        }

        public void ConfigureTestVO(OwnedNavigationBuilder<ClassWithVO, TestVO> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}