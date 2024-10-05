using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.UpdateHasDateOnlyField
{
    public class UpdateHasDateOnlyFieldCommand : IRequest, ICommand
    {
        public UpdateHasDateOnlyFieldCommand(DateOnly myDate, Guid id)
        {
            MyDate = myDate;
            Id = id;
        }

        public DateOnly MyDate { get; set; }
        public Guid Id { get; set; }
    }
}