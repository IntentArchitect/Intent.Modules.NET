using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.UniqueConVals.CreateUniqueConVal
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateUniqueConValCommandHandler : IRequestHandler<CreateUniqueConValCommand, Guid>
    {
        private readonly IUniqueConValRepository _uniqueConValRepository;

        [IntentManaged(Mode.Merge)]
        public CreateUniqueConValCommandHandler(IUniqueConValRepository uniqueConValRepository)
        {
            _uniqueConValRepository = uniqueConValRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateUniqueConValCommand request, CancellationToken cancellationToken)
        {
            var uniqueConVal = new UniqueConVal
            {
                Att1 = request.Att1,
                Att2 = request.Att2,
                AttInclude = request.AttInclude
            };

            _uniqueConValRepository.Add(uniqueConVal);
            await _uniqueConValRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return uniqueConVal.Id;
        }
    }
}