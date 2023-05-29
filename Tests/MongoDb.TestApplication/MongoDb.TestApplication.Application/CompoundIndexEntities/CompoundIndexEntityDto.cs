using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntities
{
    public class CompoundIndexEntityDto : IMapFrom<CompoundIndexEntity>
    {
        public CompoundIndexEntityDto()
        {
            Id = null!;
            SomeField = null!;
            CompoundOne = null!;
            CompoundTwo = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }
        public string CompoundOne { get; set; }
        public string CompoundTwo { get; set; }

        public static CompoundIndexEntityDto Create(string id, string someField, string compoundOne, string compoundTwo)
        {
            return new CompoundIndexEntityDto
            {
                Id = id,
                SomeField = someField,
                CompoundOne = compoundOne,
                CompoundTwo = compoundTwo
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompoundIndexEntity, CompoundIndexEntityDto>();
        }
    }
}