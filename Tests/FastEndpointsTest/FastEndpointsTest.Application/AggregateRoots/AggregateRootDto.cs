using System;
using AutoMapper;
using FastEndpointsTest.Application.Common.Mappings;
using FastEndpointsTest.Domain.Entities.CRUD;
using FastEndpointsTest.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record AggregateRootDto : IMapFrom<AggregateRoot>
    {
        public AggregateRootDto()
        {
            AggregateAttr = null!;
            LimitedDomain = null!;
            LimitedService = null!;
        }

        public Guid Id { get; init; }
        public string AggregateAttr { get; init; }
        public string LimitedDomain { get; init; }
        public string LimitedService { get; init; }
        public EnumWithoutValues EnumType1 { get; init; }
        public EnumWithDefaultLiteral EnumType2 { get; init; }
        public EnumWithoutDefaultLiteral EnumType3 { get; init; }
        public Guid? AggregateId { get; init; }

        public static AggregateRootDto Create(
            Guid id,
            string aggregateAttr,
            string limitedDomain,
            string limitedService,
            EnumWithoutValues enumType1,
            EnumWithDefaultLiteral enumType2,
            EnumWithoutDefaultLiteral enumType3,
            Guid? aggregateId)
        {
            return new AggregateRootDto
            {
                Id = id,
                AggregateAttr = aggregateAttr,
                LimitedDomain = limitedDomain,
                LimitedService = limitedService,
                EnumType1 = enumType1,
                EnumType2 = enumType2,
                EnumType3 = enumType3,
                AggregateId = aggregateId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateRoot, AggregateRootDto>();
        }
    }
}