using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class AggregateRootAggregateSingleCDto : IMapFrom<AggregateSingleC>
    {
        public AggregateRootAggregateSingleCDto()
        {
            AggregationAttr = null!;
        }

        public string AggregationAttr { get; set; }
        public Guid Id { get; set; }

        public static AggregateRootAggregateSingleCDto Create(string aggregationAttr, Guid id)
        {
            return new AggregateRootAggregateSingleCDto
            {
                AggregationAttr = aggregationAttr,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateSingleC, AggregateRootAggregateSingleCDto>();
        }
    }
}