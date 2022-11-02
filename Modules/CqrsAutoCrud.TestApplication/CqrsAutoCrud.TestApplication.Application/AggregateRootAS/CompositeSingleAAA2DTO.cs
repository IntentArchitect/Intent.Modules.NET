using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS
{

    public class CompositeSingleAAA2DTO : IMapFrom<CompositeSingleAAA2>
    {
        public CompositeSingleAAA2DTO()
        {
        }

        public static CompositeSingleAAA2DTO Create(
            Guid id,
            string compositeAttr)
        {
            return new CompositeSingleAAA2DTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleAAA2, CompositeSingleAAA2DTO>();
        }
    }
}