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

    public class AggregateRootLongCompositeOfAggrLongDTO : IMapFrom<CompositeOfAggrLong>
    {
        public AggregateRootLongCompositeOfAggrLongDTO()
        {
        }

        public static AggregateRootLongCompositeOfAggrLongDTO Create(
            long id,
            string attribute)
        {
            return new AggregateRootLongCompositeOfAggrLongDTO
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public long Id { get; set; }

        public string Attribute { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeOfAggrLong, AggregateRootLongCompositeOfAggrLongDTO>();
        }
    }
}