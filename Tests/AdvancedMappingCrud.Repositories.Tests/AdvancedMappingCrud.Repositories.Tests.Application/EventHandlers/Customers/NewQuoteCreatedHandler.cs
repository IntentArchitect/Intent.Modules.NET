using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Eventing;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Models;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.Customers;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Events;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AdvancedMappingCrud.Repositories.Tests.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EventHandlers.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NewQuoteCreatedHandler : INotificationHandler<DomainEventNotification<NewQuoteCreated>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonService _personService;
        private readonly IEventBus _eventBus;
        [IntentManaged(Mode.Merge)]
        public NewQuoteCreatedHandler(IUserRepository userRepository, IPersonService personService, IEventBus eventBus)
        {
            _userRepository = userRepository;
            _personService = personService;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(
            DomainEventNotification<NewQuoteCreated> notification,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(notification.DomainEvent.Quote.PersonId, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{notification.DomainEvent.Quote.PersonId}'");
            }
            var result = await _personService.GetPersonById(notification.DomainEvent.Quote.PersonId, cancellationToken);

            user.Email = result.Email;
            user.Name = result.Name;
            user.Surname = result.Surname;
            user.QuoteId = notification.DomainEvent.Quote.Id;
            var domainEvent = notification.DomainEvent;
            _eventBus.Publish(new QuoteCreatedIntegrationEvent
            {
                Id = domainEvent.Quote.Id,
                RefNo = domainEvent.Quote.RefNo,
                PersonId = domainEvent.Quote.PersonId,
                PersonEmail = domainEvent.Quote.PersonEmail,
                QuoteLines = domainEvent.Quote.QuoteLines
                    .Select(ql => new QuoteCreatedIntegrationEventQuoteLinesDto
                    {
                        Id = ql.Id,
                        ProductId = ql.ProductId
                    })
                    .ToList()
            });
        }
    }
}