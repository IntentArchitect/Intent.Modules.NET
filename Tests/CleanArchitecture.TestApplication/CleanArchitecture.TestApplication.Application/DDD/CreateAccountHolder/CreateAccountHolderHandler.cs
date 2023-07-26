using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.DDD;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.DDD.CreateAccountHolder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAccountHolderHandler : IRequestHandler<CreateAccountHolder, Guid>
    {
        private readonly IAccountHolderRepository _accountHolderRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateAccountHolderHandler(IAccountHolderRepository accountHolderRepository)
        {
            _accountHolderRepository = accountHolderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAccountHolder request, CancellationToken cancellationToken)
        {
            var entity = new AccountHolder(request.Name);

            _accountHolderRepository.Add(entity);
            await _accountHolderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}