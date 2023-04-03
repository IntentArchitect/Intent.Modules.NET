using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class AggregateRootAggregateSingleCDto : IMapFrom<AggregateSingleC>
    {
        public AggregateRootAggregateSingleCDto()
        {
        }

        public static AggregateRootAggregateSingleCDto Create(string aggregationAttr, Guid id)
        {
            return new AggregateRootAggregateSingleCDto
            {
                AggregationAttr = aggregationAttr,
                Id = id,
            };
        }

        public string AggregationAttr { get; set; }

        public Guid Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateSingleC, AggregateRootAggregateSingleCDto>();
        }
    }
}