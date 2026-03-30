using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateUniquePersonEntity
{
    public class UpdateUniquePersonEntityCommand : IRequest, ICommand
    {
        public UpdateUniquePersonEntityCommand(Guid id, string firstName, string lastName, string? contactNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            ContactNumber = contactNumber;
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ContactNumber { get; set; }
    }
}