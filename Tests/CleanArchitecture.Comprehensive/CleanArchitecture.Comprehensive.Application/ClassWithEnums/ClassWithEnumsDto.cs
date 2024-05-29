using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.Enums;
using CleanArchitecture.Comprehensive.Domain.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ClassWithEnums
{
    public class ClassWithEnumsDto : IMapFrom<Domain.Entities.Enums.ClassWithEnums>
    {
        public ClassWithEnumsDto()
        {
        }

        public Guid Id { get; set; }
        public EnumWithDefaultLiteral EnumWithDefaultLiteral { get; set; }
        public EnumWithoutDefaultLiteral EnumWithoutDefaultLiteral { get; set; }
        public EnumWithoutValues EnumWithoutValues { get; set; }
        public EnumWithDefaultLiteral? NullibleEnumWithDefaultLiteral { get; set; }
        public EnumWithoutDefaultLiteral? NullibleEnumWithoutDefaultLiteral { get; set; }
        public EnumWithoutValues? NullibleEnumWithoutValues { get; set; }

        public static ClassWithEnumsDto Create(
            Guid id,
            EnumWithDefaultLiteral enumWithDefaultLiteral,
            EnumWithoutDefaultLiteral enumWithoutDefaultLiteral,
            EnumWithoutValues enumWithoutValues,
            EnumWithDefaultLiteral? nullibleEnumWithDefaultLiteral,
            EnumWithoutDefaultLiteral? nullibleEnumWithoutDefaultLiteral,
            EnumWithoutValues? nullibleEnumWithoutValues)
        {
            return new ClassWithEnumsDto
            {
                Id = id,
                EnumWithDefaultLiteral = enumWithDefaultLiteral,
                EnumWithoutDefaultLiteral = enumWithoutDefaultLiteral,
                EnumWithoutValues = enumWithoutValues,
                NullibleEnumWithDefaultLiteral = nullibleEnumWithDefaultLiteral,
                NullibleEnumWithoutDefaultLiteral = nullibleEnumWithoutDefaultLiteral,
                NullibleEnumWithoutValues = nullibleEnumWithoutValues
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.Enums.ClassWithEnums, ClassWithEnumsDto>();
        }
    }
}