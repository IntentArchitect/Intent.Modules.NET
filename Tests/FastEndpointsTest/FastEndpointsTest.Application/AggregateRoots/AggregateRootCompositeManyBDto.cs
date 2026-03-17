using System;
using AutoMapper;
using FastEndpointsTest.Application.Common.Mappings;
using FastEndpointsTest.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record AggregateRootCompositeManyBDto : IMapFrom<CompositeManyB>
    {
        public AggregateRootCompositeManyBDto()
        {
            CompositeAttr = null!;
        }

        public Guid AggregateRootId { get; init; }
        public Guid Id { get; init; }
        public string CompositeAttr { get; init; }
        public DateTime? SomeDate { get; init; }

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