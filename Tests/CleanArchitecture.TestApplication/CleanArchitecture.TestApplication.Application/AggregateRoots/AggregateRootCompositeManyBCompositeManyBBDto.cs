using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class AggregateRootCompositeManyBCompositeManyBBDto : IMapFrom<CompositeManyBB>
    {
        public AggregateRootCompositeManyBCompositeManyBBDto()
        {
        }

        public static AggregateRootCompositeManyBCompositeManyBBDto Create(string compositeAttr, Guid compositeManyBId, Guid id)
        {
            return new AggregateRootCompositeManyBCompositeManyBBDto
            {
                CompositeAttr = compositeAttr,
                CompositeManyBId = compositeManyBId,
                Id = id,
            };
        }

        public string CompositeAttr { get; set; }

        public Guid CompositeManyBId { get; set; }

        public Guid Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyBB, AggregateRootCompositeManyBCompositeManyBBDto>();
        }
    }
}