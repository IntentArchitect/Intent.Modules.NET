using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.DDD;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.CreateTransaction
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand>
    {
        private readonly ITransactionRepository _transactionRepository;

        [IntentManaged(Mode.Merge)]
        public CreateTransactionCommandHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var newTransaction = new Domain.Entities.DDD.Transaction
            {
                Current = CreateMoney(request.Current),
                Description = request.Description,
                AccountId = request.AccountId,
#warning Field not a composite association: Account
            };

            _transactionRepository.Add(newTransaction);
        }

        [IntentManaged(Mode.Fully)]
        public static Money CreateMoney(CreateMoneyDto dto)
        {
            return new Money(currency: dto.Currency, amount: dto.Amount);
        }
    }
}