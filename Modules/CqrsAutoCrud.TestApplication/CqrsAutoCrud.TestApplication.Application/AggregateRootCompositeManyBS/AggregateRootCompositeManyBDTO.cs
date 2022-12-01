using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeManyBS
{

    public class AggregateRootCompositeManyBDTO : IMapFrom<CompositeManyB>
    {
        public AggregateRootCompositeManyBDTO()
        {
        }

        public static AggregateRootCompositeManyBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aggregateRootId,
            DateTime? someDate)
        {
            return new AggregateRootCompositeManyBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                AggregateRootId = aggregateRootId,
                SomeDate = someDate,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid AggregateRootId { get; set; }

        public DateTime? SomeDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyB, AggregateRootCompositeManyBDTO>();
        }
    }
}