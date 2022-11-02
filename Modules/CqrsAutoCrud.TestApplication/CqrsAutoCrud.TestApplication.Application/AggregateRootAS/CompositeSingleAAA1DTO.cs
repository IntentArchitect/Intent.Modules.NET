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

    public class CompositeSingleAAA1DTO : IMapFrom<CompositeSingleAAA1>
    {
        public CompositeSingleAAA1DTO()
        {
        }

        public static CompositeSingleAAA1DTO Create(
            Guid id,
            string compositeAttr)
        {
            return new CompositeSingleAAA1DTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleAAA1, CompositeSingleAAA1DTO>();
        }
    }
}