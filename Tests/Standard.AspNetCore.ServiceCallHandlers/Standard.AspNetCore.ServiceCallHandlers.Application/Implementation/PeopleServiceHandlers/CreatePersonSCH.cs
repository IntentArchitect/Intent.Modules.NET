using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Common.Eventing;
using Standard.AspNetCore.ServiceCallHandlers.Application.People;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Entities;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Repositories;
using Standard.AspNetCore.ServiceCallHandlers.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.PeopleServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class CreatePersonSCH
    {
        private readonly IPersonRepository _personRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreatePersonSCH(IPersonRepository personRepository, IEventBus eventBus)
        {
            _personRepository = personRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(PersonCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newPerson = new Person
            {
                Name = dto.Name,
            };
            _personRepository.Add(newPerson);
            await _personRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(newPerson.MapToPersonCreatedEvent());
            return newPerson.Id;
        }
    }
}