using System;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Infrastructure.Persistence.Configurations
{
    public class AA2_Aggregation_SingleConfiguration : IEntityTypeConfiguration<AA2_Aggregation_Single>
    {
        public void Configure(EntityTypeBuilder<AA2_Aggregation_Single> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AggregationAttr)
                .IsRequired();
        }
    }
}