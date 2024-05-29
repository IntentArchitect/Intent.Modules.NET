using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using CleanArchitecture.Comprehensive.Domain.Services.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.AccountTransfer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AccountTransferHandler : IRequestHandler<AccountTransfer>
    {
        private readonly IAccountHolderRepository _accountHolderRepository;
        private readonly IAccountingDomainService _domainService;

        [IntentManaged(Mode.Merge)]
        public AccountTransferHandler(IAccountHolderRepository accountHolderRepository,
            IAccountingDomainService domainService)
        {
            _accountHolderRepository = accountHolderRepository;
            _domainService = domainService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(AccountTransfer request, CancellationToken cancellationToken)
        {
            var existingAccountHolder = await _accountHolderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingAccountHolder is null)
            {
                throw new NotFoundException($"Could not find AccountHolder '{request.Id}'");
            }

            existingAccountHolder.Transfer(request.Description, _domainService, request.Amount, request.Currency);

        }
    }
}