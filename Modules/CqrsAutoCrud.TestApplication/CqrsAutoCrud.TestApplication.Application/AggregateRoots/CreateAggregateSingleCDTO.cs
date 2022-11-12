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

    public class CreateAggregateSingleCDTO : IMapFrom<AggregateSingleC>
    {
        public CreateAggregateSingleCDTO()
        {
        }

        public static CreateAggregateSingleCDTO Create(
            Guid id,
            string aggregationAttr)
        {
            return new CreateAggregateSingleCDTO
            {
                Id = id,
                AggregationAttr = aggregationAttr,
            };
        }

        public Guid Id { get; set; }

        public string AggregationAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateSingleC, CreateAggregateSingleCDTO>();
        }
    }
}