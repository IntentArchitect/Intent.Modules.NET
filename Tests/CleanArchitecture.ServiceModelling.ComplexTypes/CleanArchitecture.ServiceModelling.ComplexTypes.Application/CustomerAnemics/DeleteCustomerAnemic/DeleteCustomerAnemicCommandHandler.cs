using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common.Exceptions;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics.DeleteCustomerAnemic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteCustomerAnemicCommandHandler : IRequestHandler<DeleteCustomerAnemicCommand>
    {
        private readonly ICustomerAnemicRepository _customerAnemicRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteCustomerAnemicCommandHandler(ICustomerAnemicRepository customerAnemicRepository)
        {
            _customerAnemicRepository = customerAnemicRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteCustomerAnemicCommand request, CancellationToken cancellationToken)
        {
            var existingCustomerAnemic = await _customerAnemicRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingCustomerAnemic is null)
            {
                throw new NotFoundException($"Could not find CustomerAnemic '{request.Id}'");
            }

            _customerAnemicRepository.Remove(existingCustomerAnemic);

        }
    }
}