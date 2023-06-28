using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories.ComplexTypes;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.CustomerCTS.UpdateCustomerCT
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCustomerCTCommandHandler : IRequestHandler<UpdateCustomerCTCommand>
    {
        private readonly ICustomerCTRepository _customerCTRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCustomerCTCommandHandler(ICustomerCTRepository customerCTRepository)
        {
            _customerCTRepository = customerCTRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateCustomerCTCommand request, CancellationToken cancellationToken)
        {
            var existingCustomerCT = await _customerCTRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingCustomerCT is null)
            {
                throw new NotFoundException($"Could not find CustomerCT {request.Id}");
            }
            existingCustomerCT.Name = request.Name;
            return Unit.Value;
        }
    }
}