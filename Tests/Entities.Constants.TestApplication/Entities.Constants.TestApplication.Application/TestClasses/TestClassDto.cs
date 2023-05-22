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
            Att100 = null!;
            VarChar200 = null!;
            NVarChar300 = null!;
            AttMax = null!;
            VarCharMax = null!;
            NVarCharMax = null!;
        }

        public Guid Id { get; set; }
        public string Att100 { get; set; }
        public string VarChar200 { get; set; }
        public string NVarChar300 { get; set; }
        public string AttMax { get; set; }
        public string VarCharMax { get; set; }
        public string NVarCharMax { get; set; }

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