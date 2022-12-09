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

    public class AggregateRootCompositeManyBCompositeManyBBDTO : IMapFrom<CompositeManyBB>
    {
        public AggregateRootCompositeManyBCompositeManyBBDTO()
        {
        }

        public static AggregateRootCompositeManyBCompositeManyBBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aCompositeManyId)
        {
            return new AggregateRootCompositeManyBCompositeManyBBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                ACompositeManyId = aCompositeManyId,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid ACompositeManyId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyBB, AggregateRootCompositeManyBCompositeManyBBDTO>();
        }
    }
}