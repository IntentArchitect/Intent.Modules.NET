using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Mappings;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches
{
    public class TestAddressDCDto : IMapFrom<AddressDC>
    {
        public TestAddressDCDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }

        public static TestAddressDCDto Create(string line1, string line2, string city)
        {
            return new TestAddressDCDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AddressDC, TestAddressDCDto>();
        }
    }
}