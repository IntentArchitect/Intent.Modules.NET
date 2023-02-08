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

    public class AggregateRootLongCompositeOfAggrLongDto : IMapFrom<CompositeOfAggrLong>
    {
        public AggregateRootLongCompositeOfAggrLongDto()
        {
        }

        public static AggregateRootLongCompositeOfAggrLongDto Create(
            string attribute,
            long id)
        {
            return new AggregateRootLongCompositeOfAggrLongDto
            {
                Attribute = attribute,
                Id = id,
            };
        }

        public string Attribute { get; set; }

        public long Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeOfAggrLong, AggregateRootLongCompositeOfAggrLongDto>();
        }
    }
}