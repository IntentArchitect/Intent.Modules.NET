using System;
using System.Collections.Generic;
using AutoMapper;
using Entities.Constants.TestApplication.Application.Common.Mappings;
using Entities.Constants.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses
{
    public class TestClassDto : IMapFrom<TestClass>
    {
        public TestClassDto()
        {
        }

        public Guid Id { get; set; }
        public string Att100 { get; set; } = null!;
        public string VarChar200 { get; set; } = null!;
        public string NVarChar300 { get; set; } = null!;
        public string AttMax { get; set; } = null!;
        public string VarCharMax { get; set; } = null!;
        public string NVarCharMax { get; set; } = null!;

        public static TestClassDto Create(
            Guid id,
            string att100,
            string varChar200,
            string nVarChar300,
            string attMax,
            string varCharMax,
            string nVarCharMax)
        {
            return new TestClassDto
            {
                Id = id,
                Att100 = att100,
                VarChar200 = varChar200,
                NVarChar300 = nVarChar300,
                AttMax = attMax,
                VarCharMax = varCharMax,
                NVarCharMax = nVarCharMax
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TestClass, TestClassDto>();
        }
    }
}