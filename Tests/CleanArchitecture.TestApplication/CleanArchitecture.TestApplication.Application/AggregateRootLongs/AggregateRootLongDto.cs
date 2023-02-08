using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRootLongs
{

    public class AggregateRootLongDto : IMapFrom<AggregateRootLong>
    {
        public AggregateRootLongDto()
        {
        }

        public static AggregateRootLongDto Create(
            long id,
            string attribute,
            AggregateRootLongCompositeOfAggrLongDto? compositeOfAggrLong)
        {
            return new AggregateRootLongDto
            {
                Id = id,
                Attribute = attribute,
                CompositeOfAggrLong = compositeOfAggrLong,
            };
        }

        public long Id { get; set; }

        public string Attribute { get; set; }

        public AggregateRootLongCompositeOfAggrLongDto? CompositeOfAggrLong { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateRootLong, AggregateRootLongDto>();
        }
    }
}