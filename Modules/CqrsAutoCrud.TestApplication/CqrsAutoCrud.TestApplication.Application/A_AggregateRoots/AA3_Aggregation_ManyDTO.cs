using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots
{

    public class AA3_Aggregation_ManyDTO : IMapFrom<AA3_Aggregation_Many>
    {
        public AA3_Aggregation_ManyDTO()
        {
        }

        public static AA3_Aggregation_ManyDTO Create(
            Guid id,
            string aggregationAttr)
        {
            return new AA3_Aggregation_ManyDTO
            {
                Id = id,
                AggregationAttr = aggregationAttr,
            };
        }

        public Guid Id { get; set; }

        public string AggregationAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AA3_Aggregation_Many, AA3_Aggregation_ManyDTO>();
        }
    }
}