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

    public class AggregateRootCompositeSingleACompositeManyAADto : IMapFrom<CompositeManyAA>
    {
        public AggregateRootCompositeSingleACompositeManyAADto()
        {
        }

        public static AggregateRootCompositeSingleACompositeManyAADto Create(string compositeAttr, Guid compositeSingleAId, Guid id)
        {
            return new AggregateRootCompositeSingleACompositeManyAADto
            {
                CompositeAttr = compositeAttr,
                CompositeSingleAId = compositeSingleAId,
                Id = id,
            };
        }

        public string CompositeAttr { get; set; }

        public Guid CompositeSingleAId { get; set; }

        public Guid Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyAA, AggregateRootCompositeSingleACompositeManyAADto>();
        }
    }
}