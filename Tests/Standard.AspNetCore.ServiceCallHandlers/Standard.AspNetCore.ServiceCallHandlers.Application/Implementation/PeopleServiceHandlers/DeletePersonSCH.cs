using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Common.Eventing;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Common.Exceptions;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Repositories;
using Standard.AspNetCore.ServiceCallHandlers.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.PeopleServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class DeletePersonSCH
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public DeletePersonSCH(IPersonRepository personRepository, IMapper mapper, IEventBus eventBus)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(Guid id, CancellationToken cancellationToken = default)
        {
            var existingPerson = await _personRepository.FindByIdAsync(id, cancellationToken);

            if (existingPerson is null)
            {
                throw new NotFoundException($"Could not find Person {id}");
            }
            _personRepository.Remove(existingPerson);
            _eventBus.Publish(existingPerson.MapToPersonDeletedEvent());
        }
    }
}