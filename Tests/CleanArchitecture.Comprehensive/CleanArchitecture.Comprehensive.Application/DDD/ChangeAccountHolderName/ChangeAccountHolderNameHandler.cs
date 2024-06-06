using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.ChangeAccountHolderName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ChangeAccountHolderNameHandler : IRequestHandler<ChangeAccountHolderName>
    {
        private readonly IAccountHolderRepository _accountHolderRepository;
        [IntentManaged(Mode.Merge)]
        public ChangeAccountHolderNameHandler(IAccountHolderRepository accountHolderRepository)
        {
            _accountHolderRepository = accountHolderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ChangeAccountHolderName request, CancellationToken cancellationToken)
        {
            var existingAccountHolder = await _accountHolderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingAccountHolder is null)
            {
                throw new NotFoundException($"Could not find AccountHolder '{request.Id}'");
            }

            existingAccountHolder.ChangeName(request.Name);
        }
    }
}