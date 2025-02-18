using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.AdvChanceAccountHolderName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AdvChanceAccountHolderNameHandler : IRequestHandler<AdvChanceAccountHolderName>
    {
        private readonly IAccountHolderRepository _accountHolderRepository;

        [IntentManaged(Mode.Merge)]
        public AdvChanceAccountHolderNameHandler(IAccountHolderRepository accountHolderRepository)
        {
            _accountHolderRepository = accountHolderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(AdvChanceAccountHolderName request, CancellationToken cancellationToken)
        {
            var entity = await _accountHolderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find AccountHolder '{request.Id}'");
            }
            entity.ChangeName(request.Name);
        }
    }
}