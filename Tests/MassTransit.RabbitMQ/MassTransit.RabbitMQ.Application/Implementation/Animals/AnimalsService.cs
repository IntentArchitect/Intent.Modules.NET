using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Animals;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Application.Interfaces.Animals;
using MassTransit.RabbitMQ.Domain.Entities;
using MassTransit.RabbitMQ.Domain.Repositories;
using MassTransit.RabbitMQ.Services.Animals;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.Implementation.Animals
{
    [IntentManaged(Mode.Fully)]
    public class AnimalsService : IAnimalsService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Fully)]
        public AnimalsService(IAnimalRepository animalRepository, IEventBus eventBus)
        {
            _animalRepository = animalRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateAnimal(CreateAnimalDto dto, CancellationToken cancellationToken = default)
        {
            var entity = new Animal
            {
                Name = dto.Name,
                Type = dto.Type
            };

            _animalRepository.Add(entity);
            _eventBus.Send(new OrderAnimal
            {
                Name = dto.Name,
                Type = dto.Type
            });
        }
    }
}