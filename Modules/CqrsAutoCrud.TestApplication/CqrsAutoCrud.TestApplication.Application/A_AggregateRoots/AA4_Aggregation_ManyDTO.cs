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

    public class AA4_Aggregation_ManyDTO : IMapFrom<AA4_Aggregation_Many>
    {
        public AA4_Aggregation_ManyDTO()
        {
        }

        public static AA4_Aggregation_ManyDTO Create(
            Guid id,
            string aggregationAttr)
        {
            return new AA4_Aggregation_ManyDTO
            {
                Id = id,
                AggregationAttr = aggregationAttr,
            };
        }

        public Guid Id { get; set; }

        public string AggregationAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AA4_Aggregation_Many, AA4_Aggregation_ManyDTO>();
        }
    }
}