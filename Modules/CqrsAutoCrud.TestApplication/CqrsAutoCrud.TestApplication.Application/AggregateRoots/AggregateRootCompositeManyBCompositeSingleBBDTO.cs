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

    public class AggregateRootCompositeManyBCompositeSingleBBDTO : IMapFrom<CompositeSingleBB>
    {
        public AggregateRootCompositeManyBCompositeSingleBBDTO()
        {
        }

        public static AggregateRootCompositeManyBCompositeSingleBBDTO Create(
            Guid id,
            string compositeAttr)
        {
            return new AggregateRootCompositeManyBCompositeSingleBBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleBB, AggregateRootCompositeManyBCompositeSingleBBDTO>();
        }
    }
}