using System;
using AutoMapper;
using FastEndpointsTest.Application.Common.Mappings;
using FastEndpointsTest.Domain.Entities.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes
{
    public record AggregateWithUniqueConstraintIndexStereotypeDto : IMapFrom<AggregateWithUniqueConstraintIndexStereotype>
    {
        public AggregateWithUniqueConstraintIndexStereotypeDto()
        {
            SingleUniqueField = null!;
            CompUniqueFieldA = null!;
            CompUniqueFieldB = null!;
        }

        public Guid Id { get; init; }
        public string SingleUniqueField { get; init; }
        public string CompUniqueFieldA { get; init; }
        public string CompUniqueFieldB { get; init; }

        public static AggregateWithUniqueConstraintIndexStereotypeDto Create(
            Guid id,
            string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB)
        {
            return new AggregateWithUniqueConstraintIndexStereotypeDto
            {
                Id = id,
                SingleUniqueField = singleUniqueField,
                CompUniqueFieldA = compUniqueFieldA,
                CompUniqueFieldB = compUniqueFieldB
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateWithUniqueConstraintIndexStereotype, AggregateWithUniqueConstraintIndexStereotypeDto>();
        }
    }
}