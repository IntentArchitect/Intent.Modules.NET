using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.CreateCheckNewCompChildCrud
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCheckNewCompChildCrudCommandHandler : IRequestHandler<CreateCheckNewCompChildCrudCommand, Guid>
    {
        private readonly ICheckNewCompChildCrudRepository _checkNewCompChildCrudRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCheckNewCompChildCrudCommandHandler(ICheckNewCompChildCrudRepository checkNewCompChildCrudRepository)
        {
            _checkNewCompChildCrudRepository = checkNewCompChildCrudRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCheckNewCompChildCrudCommand request, CancellationToken cancellationToken)
        {
            var checkNewCompChildCrud = new CheckNewCompChildCrud
            {
                Name = request.Name
            };

            _checkNewCompChildCrudRepository.Add(checkNewCompChildCrud);
            await _checkNewCompChildCrudRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return checkNewCompChildCrud.Id;
        }
    }
}