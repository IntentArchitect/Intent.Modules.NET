using System;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns
{
    public class AggregateTestNoIdReturnDto : IMapFrom<AggregateTestNoIdReturn>
    {
        public AggregateTestNoIdReturnDto()
        {
            Attribute = null!;
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; }

        public static AggregateTestNoIdReturnDto Create(Guid id, string attribute)
        {
            return new AggregateTestNoIdReturnDto
            {
                Id = id,
                Attribute = attribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateTestNoIdReturn, AggregateTestNoIdReturnDto>();
        }
    }
}