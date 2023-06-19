using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.TestNullablities.UpdateTestNullablity
{
    public class UpdateTestNullablityCommand : IRequest, ICommand
    {
        public UpdateTestNullablityCommand(Guid id,
            MyEnum myEnum,
            string str,
            DateTime date,
            DateTime dateTime,
            Guid? nullableGuid,
            MyEnum? nullableEnum,
            Guid nullabilityPeerId)
        {
            Id = id;
            MyEnum = myEnum;
            Str = str;
            Date = date;
            DateTime = dateTime;
            NullableGuid = nullableGuid;
            NullableEnum = nullableEnum;
            NullabilityPeerId = nullabilityPeerId;
        }

        public Guid Id { get; set; }
        public MyEnum MyEnum { get; set; }
        public string Str { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime { get; set; }
        public Guid? NullableGuid { get; set; }
        public MyEnum? NullableEnum { get; set; }
        public Guid NullabilityPeerId { get; set; }
    }
}