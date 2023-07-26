using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.CreateCustomerRich
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCustomerRichCommandHandler : IRequestHandler<CreateCustomerRichCommand, Guid>
    {
        private readonly ICustomerRichRepository _customerRichRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCustomerRichCommandHandler(ICustomerRichRepository customerRichRepository)
        {
            _customerRichRepository = customerRichRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCustomerRichCommand request, CancellationToken cancellationToken)
        {
            var entity = new CustomerRich(CreateAddress(request.Address));

            _customerRichRepository.Add(entity);
            await _customerRichRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        [IntentManaged(Mode.Fully)]
        public static Address CreateAddress(CreateCustomerRichAddressDto dto)
        {
            return new Address(line1: dto.Line1, line2: dto.Line2, city: dto.City);
        }
    }
}