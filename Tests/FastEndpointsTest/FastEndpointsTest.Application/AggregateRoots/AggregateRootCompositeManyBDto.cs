using System;
using AutoMapper;
using FastEndpointsTest.Application.Common.Mappings;
using FastEndpointsTest.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class AggregateRootCompositeManyBDto : IMapFrom<CompositeManyB>
    {
        public AggregateRootCompositeManyBDto()
        {
            CompositeAttr = null!;
        }

        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }

        public static AggregateRootCompositeManyBDto Create(
            Guid aggregateRootId,
            Guid id,
            string compositeAttr,
            DateTime? someDate)
        {
            return new AggregateRootCompositeManyBDto
            {
                AggregateRootId = aggregateRootId,
                Id = id,
                CompositeAttr = compositeAttr,
                SomeDate = someDate
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyB, AggregateRootCompositeManyBDto>();
        }
    }
}