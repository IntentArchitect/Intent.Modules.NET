using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Domain.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.TestNullablities.UpdateTestNullablity
{
    public class UpdateTestNullablityCommand : IRequest, ICommand
    {
        public UpdateTestNullablityCommand(Guid id,
            NoDefaultLiteralEnum sampleEnum,
            string str,
            DateTime date,
            DateTime dateTime,
            Guid? nullableGuid,
            NoDefaultLiteralEnum? nullableEnum,
            Guid nullabilityPeerId,
            DefaultLiteralEnum defaultLiteralEnum)
        {
            Id = id;
            SampleEnum = sampleEnum;
            Str = str;
            Date = date;
            DateTime = dateTime;
            NullableGuid = nullableGuid;
            NullableEnum = nullableEnum;
            NullabilityPeerId = nullabilityPeerId;
            DefaultLiteralEnum = defaultLiteralEnum;
        }

        public Guid Id { get; set; }
        public NoDefaultLiteralEnum SampleEnum { get; set; }
        public string Str { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime { get; set; }
        public Guid? NullableGuid { get; set; }
        public NoDefaultLiteralEnum? NullableEnum { get; set; }
        public Guid NullabilityPeerId { get; set; }
        public DefaultLiteralEnum DefaultLiteralEnum { get; set; }
    }
}