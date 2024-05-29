using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Domain.Enums;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ClassWithEnums.CreateClassWithEnums
{
    public class CreateClassWithEnumsCommand : IRequest<Guid>, ICommand
    {
        public CreateClassWithEnumsCommand(EnumWithDefaultLiteral enumWithDefaultLiteral,
            EnumWithoutDefaultLiteral enumWithoutDefaultLiteral,
            EnumWithoutValues enumWithoutValues,
            EnumWithDefaultLiteral? nullibleEnumWithDefaultLiteral,
            EnumWithoutDefaultLiteral? nullibleEnumWithoutDefaultLiteral,
            EnumWithoutValues? nullibleEnumWithoutValues,
            IEnumerable<EnumWithDefaultLiteral> collectionEnum,
            List<string> collectionStrings)
        {
            EnumWithDefaultLiteral = enumWithDefaultLiteral;
            EnumWithoutDefaultLiteral = enumWithoutDefaultLiteral;
            EnumWithoutValues = enumWithoutValues;
            NullibleEnumWithDefaultLiteral = nullibleEnumWithDefaultLiteral;
            NullibleEnumWithoutDefaultLiteral = nullibleEnumWithoutDefaultLiteral;
            NullibleEnumWithoutValues = nullibleEnumWithoutValues;
            CollectionEnum = collectionEnum;
            CollectionStrings = collectionStrings;
        }

        public EnumWithDefaultLiteral EnumWithDefaultLiteral { get; set; }
        public EnumWithoutDefaultLiteral EnumWithoutDefaultLiteral { get; set; }
        public EnumWithoutValues EnumWithoutValues { get; set; }
        public EnumWithDefaultLiteral? NullibleEnumWithDefaultLiteral { get; set; }
        public EnumWithoutDefaultLiteral? NullibleEnumWithoutDefaultLiteral { get; set; }
        public EnumWithoutValues? NullibleEnumWithoutValues { get; set; }
        public IEnumerable<EnumWithDefaultLiteral> CollectionEnum { get; set; }
        public List<string> CollectionStrings { get; set; }
    }
}