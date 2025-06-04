using AspNetCoreCleanArchitecture.Sample.Domain;
using AspNetCoreCleanArchitecture.Sample.Domain.Entities;
using AspNetCoreCleanArchitecture.Sample.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.CreateBuyer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateBuyerCommandHandler : IRequestHandler<CreateBuyerCommand, Guid>
    {
        private readonly IBuyerRepository _buyerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateBuyerCommandHandler(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateBuyerCommand request, CancellationToken cancellationToken)
        {
            var buyer = new Buyer
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                IsActive = request.IsActive,
                Address = new Address(
                    line1: request.Address.Line1,
                    line2: request.Address.Line2,
                    city: request.Address.City,
                    postalCode: request.Address.PostalCode)
            };

            _buyerRepository.Add(buyer);
            await _buyerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return buyer.Id;
        }
    }
}