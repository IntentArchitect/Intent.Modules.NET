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

    public class AA3_Composite_SingleDTO : IMapFrom<AA3_Composite_Single>
    {
        public AA3_Composite_SingleDTO()
        {
        }

        public static AA3_Composite_SingleDTO Create(
            Guid id,
            string compositeAttr)
        {
            return new AA3_Composite_SingleDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AA3_Composite_Single, AA3_Composite_SingleDTO>();
        }
    }
}