using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Domain.Enums;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ClassWithEnums.UpdateClassWithEnums
{
    public class UpdateClassWithEnumsCommand : IRequest, ICommand
    {
        public UpdateClassWithEnumsCommand(Guid id,
            EnumWithDefaultLiteral enumWithDefaultLiteral,
            EnumWithoutDefaultLiteral enumWithoutDefaultLiteral,
            EnumWithoutValues enumWithoutValues,
            EnumWithDefaultLiteral? nullibleEnumWithDefaultLiteral,
            EnumWithoutDefaultLiteral? nullibleEnumWithoutDefaultLiteral,
            EnumWithoutValues? nullibleEnumWithoutValues)
        {
            Id = id;
            EnumWithDefaultLiteral = enumWithDefaultLiteral;
            EnumWithoutDefaultLiteral = enumWithoutDefaultLiteral;
            EnumWithoutValues = enumWithoutValues;
            NullibleEnumWithDefaultLiteral = nullibleEnumWithDefaultLiteral;
            NullibleEnumWithoutDefaultLiteral = nullibleEnumWithoutDefaultLiteral;
            NullibleEnumWithoutValues = nullibleEnumWithoutValues;
        }

        public Guid Id { get; set; }
        public EnumWithDefaultLiteral EnumWithDefaultLiteral { get; set; }
        public EnumWithoutDefaultLiteral EnumWithoutDefaultLiteral { get; set; }
        public EnumWithoutValues EnumWithoutValues { get; set; }
        public EnumWithDefaultLiteral? NullibleEnumWithDefaultLiteral { get; set; }
        public EnumWithoutDefaultLiteral? NullibleEnumWithoutDefaultLiteral { get; set; }
        public EnumWithoutValues? NullibleEnumWithoutValues { get; set; }
    }
}