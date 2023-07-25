using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Contracts;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.ChangeAddress
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ChangeAddressCommandHandler : IRequestHandler<ChangeAddressCommand>
    {
        private readonly ICustomerRichRepository _customerRichRepository;

        [IntentManaged(Mode.Merge)]
        public ChangeAddressCommandHandler(ICustomerRichRepository customerRichRepository)
        {
            _customerRichRepository = customerRichRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ChangeAddressCommand request, CancellationToken cancellationToken)
        {
            var entity = await _customerRichRepository.FindByIdAsync(request.Id, cancellationToken);
            entity.UpdateAddress(CreateAddressDC(request.Address));

        }

        [IntentManaged(Mode.Fully)]
        public static AddressDC CreateAddressDC(ChangeAddressDCDto dto)
        {
            return new AddressDC(line1: dto.Line1, line2: dto.Line2, city: dto.City);
        }
    }
}