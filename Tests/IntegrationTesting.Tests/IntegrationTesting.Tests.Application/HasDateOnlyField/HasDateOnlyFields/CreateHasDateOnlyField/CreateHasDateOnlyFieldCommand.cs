using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields.CreateHasDateOnlyField
{
    public class CreateHasDateOnlyFieldCommand : IRequest<Guid>, ICommand
    {
        public CreateHasDateOnlyFieldCommand(DateOnly myDate)
        {
            MyDate = myDate;
        }

        public DateOnly MyDate { get; set; }
    }
}