using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.PartialCruds.CreatePartialCrud
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreatePartialCrudCommandHandler : IRequestHandler<CreatePartialCrudCommand, Guid>
    {
        private readonly IPartialCrudRepository _partialCrudRepository;

        [IntentManaged(Mode.Merge)]
        public CreatePartialCrudCommandHandler(IPartialCrudRepository partialCrudRepository)
        {
            _partialCrudRepository = partialCrudRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreatePartialCrudCommand request, CancellationToken cancellationToken)
        {
            var partialCrud = new PartialCrud
            {
                Name = request.Name
            };

            _partialCrudRepository.Add(partialCrud);
            await _partialCrudRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return partialCrud.Id;
        }
    }
}