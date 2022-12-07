using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs
{

    public class AggregateRootLongDTO : IMapFrom<AggregateRootLong>
    {
        public AggregateRootLongDTO()
        {
        }

        public static AggregateRootLongDTO Create(
            long id,
            string attribute,
            AggregateRootLongCompositeOfAggrLongDTO? compositeOfAggrLong)
        {
            return new AggregateRootLongDTO
            {
                Id = id,
                Attribute = attribute,
                CompositeOfAggrLong = compositeOfAggrLong,
            };
        }

        public long Id { get; set; }

        public string Attribute { get; set; }

        public AggregateRootLongCompositeOfAggrLongDTO? CompositeOfAggrLong { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateRootLong, AggregateRootLongDTO>();
        }
    }
}