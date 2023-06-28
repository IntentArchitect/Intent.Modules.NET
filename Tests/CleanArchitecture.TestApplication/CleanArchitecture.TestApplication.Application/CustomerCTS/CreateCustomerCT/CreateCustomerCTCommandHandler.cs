using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.ComplexTypes;
using CleanArchitecture.TestApplication.Domain.Entities.ComplexTypes;
using CleanArchitecture.TestApplication.Domain.Repositories.ComplexTypes;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.CustomerCTS.CreateCustomerCT
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCustomerCTCommandHandler : IRequestHandler<CreateCustomerCTCommand, Guid>
    {
        private readonly ICustomerCTRepository _customerCTRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCustomerCTCommandHandler(ICustomerCTRepository customerCTRepository)
        {
            _customerCTRepository = customerCTRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCustomerCTCommand request, CancellationToken cancellationToken)
        {
            var entity = new CustomerCT(request.Name, CreateAddressCT(request.AddressCT));

            _customerCTRepository.Add(entity);
            await _customerCTRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        [IntentManaged(Mode.Fully)]
        public static AddressCT CreateAddressCT(CreateCustomerCTAddressCTDto dto)
        {
            return new AddressCT(line1: dto.Line1, line2: dto.Line2, city: dto.City);
        }
    }
}