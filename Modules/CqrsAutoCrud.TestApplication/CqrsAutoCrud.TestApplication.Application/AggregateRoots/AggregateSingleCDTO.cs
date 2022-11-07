using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class AggregateSingleCDTO : IMapFrom<AggregateSingleC>
    {
        public AggregateSingleCDTO()
        {
        }

        public static AggregateSingleCDTO Create(
            Guid id,
            string aggregationAttr)
        {
            return new AggregateSingleCDTO
            {
                Id = id,
                AggregationAttr = aggregationAttr,
            };
        }

        public Guid Id { get; set; }

        public string AggregationAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateSingleC, AggregateSingleCDTO>();
        }
    }
}