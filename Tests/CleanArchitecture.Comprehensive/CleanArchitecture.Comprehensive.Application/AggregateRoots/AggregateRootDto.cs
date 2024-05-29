using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{

    public class AggregateRootDto : IMapFrom<AggregateRoot>
    {
        public AggregateRootDto()
        {
            AggregateAttr = null!;
            Composites = null!;
            LimitedDomain = null!;
            LimitedService = null!;
        }

        public static AggregateRootDto Create(
            Guid id,
            string aggregateAttr,
            List<AggregateRootCompositeManyBDto> composites,
            AggregateRootCompositeSingleADto? composite,
            AggregateRootAggregateSingleCDto? aggregate,
            EnumWithoutValues enumType1,
            EnumWithDefaultLiteral enumType2,
            EnumWithoutDefaultLiteral enumType3,
            string limitedDomain,
            string limitedService)
        {
            return new AggregateRootDto
            {
                Id = id,
                AggregateAttr = aggregateAttr,
                Composites = composites,
                Composite = composite,
                Aggregate = aggregate,
                EnumType1 = enumType1,
                EnumType2 = enumType2,
                EnumType3 = enumType3,
                LimitedDomain = limitedDomain,
                LimitedService = limitedService
            };
        }

        public Guid Id { get; set; }

        public string AggregateAttr { get; set; }

        public List<AggregateRootCompositeManyBDto> Composites { get; set; }

        public AggregateRootCompositeSingleADto? Composite { get; set; }

        public AggregateRootAggregateSingleCDto? Aggregate { get; set; }
        public EnumWithoutValues EnumType1 { get; set; }
        public EnumWithDefaultLiteral EnumType2 { get; set; }
        public EnumWithoutDefaultLiteral EnumType3 { get; set; }
        public string LimitedDomain { get; set; }
        public string LimitedService { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateRoot, AggregateRootDto>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}