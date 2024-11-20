using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Controllers.Secured.Domain.Common.Exceptions;
using AspNetCore.Controllers.Secured.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers.UpdateBuyer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateBuyerCommandHandler : IRequestHandler<UpdateBuyerCommand>
    {
        private readonly IBuyerRepository _buyerRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateBuyerCommandHandler(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateBuyerCommand request, CancellationToken cancellationToken)
        {
            var existingBuyer = await _buyerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingBuyer is null)
            {
                throw new NotFoundException($"Could not find Buyer '{request.Id}'");
            }

            existingBuyer.Name = request.Name;
            existingBuyer.Surname = request.Surname;
            existingBuyer.Email = request.Email;
        }
    }
}