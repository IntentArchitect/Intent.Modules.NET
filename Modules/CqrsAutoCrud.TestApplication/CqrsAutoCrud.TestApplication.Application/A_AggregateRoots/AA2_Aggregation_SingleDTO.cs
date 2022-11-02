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

    public class AA2_Aggregation_SingleDTO : IMapFrom<AA2_Aggregation_Single>
    {
        public AA2_Aggregation_SingleDTO()
        {
        }

        public static AA2_Aggregation_SingleDTO Create(
            Guid id,
            string aggregationAttr)
        {
            return new AA2_Aggregation_SingleDTO
            {
                Id = id,
                AggregationAttr = aggregationAttr,
            };
        }

        public Guid Id { get; set; }

        public string AggregationAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AA2_Aggregation_Single, AA2_Aggregation_SingleDTO>();
        }
    }
}