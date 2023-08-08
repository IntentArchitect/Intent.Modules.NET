using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common.Exceptions;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics.UpdateCustomerAnemic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCustomerAnemicCommandHandler : IRequestHandler<UpdateCustomerAnemicCommand>
    {
        private readonly ICustomerAnemicRepository _customerAnemicRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCustomerAnemicCommandHandler(ICustomerAnemicRepository customerAnemicRepository)
        {
            _customerAnemicRepository = customerAnemicRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCustomerAnemicCommand request, CancellationToken cancellationToken)
        {
            var existingCustomerAnemic = await _customerAnemicRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingCustomerAnemic is null)
            {
                throw new NotFoundException($"Could not find CustomerAnemic '{request.Id}'");
            }

            existingCustomerAnemic.Name = request.Name;
            existingCustomerAnemic.Address = CreateAddress(request.Address);

        }

        [IntentManaged(Mode.Fully)]
        public static Address CreateAddress(UpdateCustomerAnemicAddressDto dto)
        {
            return new Address(line1: dto.Line1, line2: dto.Line2, city: dto.City);
        }
    }
}