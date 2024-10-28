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
    public class AggregateRootDto : IMapFrom<AggregateRoot>
    {
        public AggregateRootDto()
        {
            AggregateAttr = null!;
            LimitedDomain = null!;
            LimitedService = null!;
        }

        public Guid Id { get; set; }
        public string AggregateAttr { get; set; }
        public string LimitedDomain { get; set; }
        public string LimitedService { get; set; }
        public EnumWithoutValues EnumType1 { get; set; }
        public EnumWithDefaultLiteral EnumType2 { get; set; }
        public EnumWithoutDefaultLiteral EnumType3 { get; set; }
        public Guid? AggregateId { get; set; }

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