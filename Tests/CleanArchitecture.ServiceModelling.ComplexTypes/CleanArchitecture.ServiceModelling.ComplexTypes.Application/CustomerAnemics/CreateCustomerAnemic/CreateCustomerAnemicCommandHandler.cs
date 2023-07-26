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

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics.CreateCustomerAnemic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCustomerAnemicCommandHandler : IRequestHandler<CreateCustomerAnemicCommand, Guid>
    {
        private readonly ICustomerAnemicRepository _customerAnemicRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCustomerAnemicCommandHandler(ICustomerAnemicRepository customerAnemicRepository)
        {
            _customerAnemicRepository = customerAnemicRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCustomerAnemicCommand request, CancellationToken cancellationToken)
        {
            var newCustomerAnemic = new CustomerAnemic
            {
                Name = request.Name,
                Address = CreateAddress(request.Address),
            };

            _customerAnemicRepository.Add(newCustomerAnemic);
            await _customerAnemicRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newCustomerAnemic.Id;
        }

        [IntentManaged(Mode.Fully)]
        public static Address CreateAddress(CreateCustomerAnemicAddressDto dto)
        {
            return new Address(line1: dto.Line1, line2: dto.Line2, city: dto.City);
        }
    }
}