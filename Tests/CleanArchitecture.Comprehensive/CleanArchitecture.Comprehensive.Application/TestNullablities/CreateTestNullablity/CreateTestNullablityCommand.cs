using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Domain.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.TestNullablities.CreateTestNullablity
{
    public class CreateTestNullablityCommand : IRequest<Guid>, ICommand
    {
        public CreateTestNullablityCommand(Guid id,
            NoDefaultLiteralEnum myEnum,
            string str,
            DateTime date,
            DateTime dateTime,
            Guid? nullableGuid,
            NoDefaultLiteralEnum? nullableEnum,
            DefaultLiteralEnum defaultLiteralEnum)
        {
            Id = id;
            MyEnum = myEnum;
            Str = str;
            Date = date;
            DateTime = dateTime;
            NullableGuid = nullableGuid;
            NullableEnum = nullableEnum;
            DefaultLiteralEnum = defaultLiteralEnum;
        }

        public Guid Id { get; set; }
        public NoDefaultLiteralEnum MyEnum { get; set; }
        public string Str { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime { get; set; }
        public Guid? NullableGuid { get; set; }
        public NoDefaultLiteralEnum? NullableEnum { get; set; }
        public DefaultLiteralEnum DefaultLiteralEnum { get; set; }
    }
}