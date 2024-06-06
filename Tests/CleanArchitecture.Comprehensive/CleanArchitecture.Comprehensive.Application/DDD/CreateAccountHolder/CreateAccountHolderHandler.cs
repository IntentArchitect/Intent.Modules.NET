using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.CreateAccountHolder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAccountHolderHandler : IRequestHandler<CreateAccountHolder, Guid>
    {
        private readonly IAccountHolderRepository _accountHolderRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAccountHolderHandler(IAccountHolderRepository accountHolderRepository)
        {
            _accountHolderRepository = accountHolderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAccountHolder request, CancellationToken cancellationToken)
        {
            var newAccountHolder = new AccountHolder(request.Name);

            _accountHolderRepository.Add(newAccountHolder);
            await _accountHolderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newAccountHolder.Id;
        }
    }
}