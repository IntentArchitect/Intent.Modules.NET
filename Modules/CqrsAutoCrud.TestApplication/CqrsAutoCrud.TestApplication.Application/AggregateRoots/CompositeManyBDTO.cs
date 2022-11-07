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

    public class CompositeManyBDTO : IMapFrom<CompositeManyB>
    {
        public CompositeManyBDTO()
        {
        }

        public static CompositeManyBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aAggregaterootId)
        {
            return new CompositeManyBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                AAggregaterootId = aAggregaterootId,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid AAggregaterootId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyB, CompositeManyBDTO>();
        }
    }
}