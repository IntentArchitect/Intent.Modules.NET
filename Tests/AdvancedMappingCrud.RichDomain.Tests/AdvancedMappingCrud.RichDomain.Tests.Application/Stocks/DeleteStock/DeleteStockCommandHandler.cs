using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Stocks.DeleteStock
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteStockCommandHandler : IRequestHandler<DeleteStockCommand>
    {
        private readonly IStockRepository _stockRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteStockCommandHandler(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteStockCommand request, CancellationToken cancellationToken)
        {
            var stock = await _stockRepository.FindByIdAsync(request.Data.Id, cancellationToken);
            if (stock is null)
            {
                throw new NotFoundException($"Could not find Stock '{request.Data.Id}'");
            }

            _stockRepository.Remove(stock);
        }
    }
}