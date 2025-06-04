using AspNetCoreCleanArchitecture.Sample.Domain;
using AspNetCoreCleanArchitecture.Sample.Domain.Common.Exceptions;
using AspNetCoreCleanArchitecture.Sample.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.UpdateBuyer
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
            var buyer = await _buyerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (buyer is null)
            {
                throw new NotFoundException($"Could not find Buyer '{request.Id}'");
            }

            buyer.Name = request.Name;
            buyer.Surname = request.Surname;
            buyer.Email = request.Email;
            buyer.IsActive = request.IsActive;
            buyer.Address = new Address(
                line1: request.Address.Line1,
                line2: request.Address.Line2,
                city: request.Address.City,
                postalCode: request.Address.PostalCode);
        }
    }
}