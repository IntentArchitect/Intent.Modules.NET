using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntityMultiParents
{
    public class CompoundIndexEntityMultiChildDto : IMapFrom<CompoundIndexEntityMultiChild>
    {
        public CompoundIndexEntityMultiChildDto()
        {
            CompoundOne = null!;
            CompoundTwo = null!;
        }

        public string CompoundOne { get; set; }
        public string CompoundTwo { get; set; }
        public Guid Id { get; set; }

        public static CompoundIndexEntityMultiChildDto Create(string compoundOne, string compoundTwo, Guid id)
        {
            return new CompoundIndexEntityMultiChildDto
            {
                CompoundOne = compoundOne,
                CompoundTwo = compoundTwo,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompoundIndexEntityMultiChild, CompoundIndexEntityMultiChildDto>();
        }
    }
}