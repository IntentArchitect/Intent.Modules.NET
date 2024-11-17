using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Controllers.Secured.Domain.Entities;
using AspNetCore.Controllers.Secured.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers.CreateBuyer
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
            var newBuyer = new Buyer
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
            };

            _buyerRepository.Add(newBuyer);
            await _buyerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newBuyer.Id;
        }
    }
}