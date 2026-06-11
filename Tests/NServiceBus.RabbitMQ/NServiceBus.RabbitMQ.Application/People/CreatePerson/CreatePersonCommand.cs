using Intent.RoslynWeaver.Attributes;
using MediatR;
using NServiceBus.RabbitMQ.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace NServiceBus.RabbitMQ.Application.People.CreatePerson
{
    public class CreatePersonCommand : IRequest, ICommand
    {
        public CreatePersonCommand(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}