using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Common.Eventing;
using Standard.AspNetCore.ServiceCallHandlers.Application.People;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Common.Exceptions;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Repositories;
using Standard.AspNetCore.ServiceCallHandlers.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.PeopleServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class UpdatePersonSCH
    {
        private readonly IPersonRepository _personRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public UpdatePersonSCH(IPersonRepository personRepository, IEventBus eventBus)
        {
            _personRepository = personRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(Guid id, PersonUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var existingPerson = await _personRepository.FindByIdAsync(id, cancellationToken);

            if (existingPerson is null)
            {
                throw new NotFoundException($"Could not find Person {id}");
            }
            existingPerson.Name = dto.Name;
            _eventBus.Publish(existingPerson.MapToPersonUpdatedEvent());
        }
    }
}