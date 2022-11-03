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
            CompositeOfAggrLongDTO? compositeOfAggrLong,
            string attribute)
        {
            return new AggregateRootLongDTO
            {
                Id = id,
                CompositeOfAggrLong = compositeOfAggrLong,
                Attribute = attribute,
            };
        }

        public long Id { get; set; }

        public CompositeOfAggrLongDTO? CompositeOfAggrLong { get; set; }

        public string Attribute { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateRootLong, AggregateRootLongDTO>();
        }
    }
}