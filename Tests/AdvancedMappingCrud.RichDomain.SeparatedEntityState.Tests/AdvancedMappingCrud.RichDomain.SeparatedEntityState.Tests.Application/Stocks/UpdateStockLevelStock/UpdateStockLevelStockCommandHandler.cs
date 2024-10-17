using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Stocks.UpdateStockLevelStock
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateStockLevelStockCommandHandler : IRequestHandler<UpdateStockLevelStockCommand>
    {
        private readonly IStockRepository _stockRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateStockLevelStockCommandHandler(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateStockLevelStockCommand request, CancellationToken cancellationToken)
        {
            var stock = await _stockRepository.FindAsync(x => x.Id == request.Data.Id && x.Name == request.Data.Name && x.Total == request.Data.Total && x.AddedUser == request.Data.User, cancellationToken);
            if (stock is null)
            {
                throw new NotFoundException($"Could not find Stock '({request.Data.Id}, {request.Data.Name}, {request.Data.Total}, {request.Data.User})'");
            }

            stock.UpdateStockLevel(request.Data.Id, request.Data.Total, request.Data.CurrentDateTime);
        }
    }
}