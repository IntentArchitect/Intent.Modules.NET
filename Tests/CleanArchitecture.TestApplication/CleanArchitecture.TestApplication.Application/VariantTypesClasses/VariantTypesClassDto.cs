using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.VariantTypesClasses
{

    public class VariantTypesClassDto : IMapFrom<VariantTypesClass>
    {
        public VariantTypesClassDto()
        {
        }

        public static VariantTypesClassDto Create(
            Guid id,
            IEnumerable<string> strCollection,
            IEnumerable<int> intCollection,
            IEnumerable<string>? strNullCollection,
            IEnumerable<int>? intNullCollection,
            string? nullStr,
            int? nullInt)
        {
            return new VariantTypesClassDto
            {
                Id = id,
                StrCollection = strCollection,
                IntCollection = intCollection,
                StrNullCollection = strNullCollection,
                IntNullCollection = intNullCollection,
                NullStr = nullStr,
                NullInt = nullInt,
            };
        }

        public Guid Id { get; set; }

        public IEnumerable<string> StrCollection { get; set; }

        public IEnumerable<int> IntCollection { get; set; }

        public IEnumerable<string>? StrNullCollection { get; set; }

        public IEnumerable<int>? IntNullCollection { get; set; }

        public string? NullStr { get; set; }

        public int? NullInt { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<VariantTypesClass, VariantTypesClassDto>();
        }
    }
}