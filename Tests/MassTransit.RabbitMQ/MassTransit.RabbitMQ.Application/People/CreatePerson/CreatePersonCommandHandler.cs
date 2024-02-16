using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Domain.Entities;
using MassTransit.RabbitMQ.Domain.Repositories;
using MassTransit.RabbitMQ.Services;
using MassTransit.RabbitMQ.Services.People;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransit.RabbitMQ.Application.People.CreatePerson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreatePersonCommandHandler(IPersonRepository personRepository, IEventBus eventBus)
        {
            _personRepository = personRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var entity = new Person
            {
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            _personRepository.Add(entity);
            _eventBus.Send(new CreatePersonIdentity
            {
                FirstName = request.FirstName,
                LastName = request.LastName
            });
        }
    }
}