using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateUniquePersonEntity
{
    public class CreateUniquePersonEntityCommand : IRequest, ICommand
    {
        public CreateUniquePersonEntityCommand(string firstName, string lastName, string? contactNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            ContactNumber = contactNumber;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ContactNumber { get; set; }
    }
}