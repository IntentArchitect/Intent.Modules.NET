using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Stocks.CreateStock
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateStockCommandHandler : IRequestHandler<CreateStockCommand, Guid>
    {
        private readonly IStockRepository _stockRepository;

        [IntentManaged(Mode.Merge)]
        public CreateStockCommandHandler(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateStockCommand request, CancellationToken cancellationToken)
        {
            var stock = new Stock(
                name: request.Data.Name,
                total: request.Data.Total,
                addedUser: request.Data.User);

            _stockRepository.Add(stock);
            await _stockRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return stock.Id;
        }
    }
}