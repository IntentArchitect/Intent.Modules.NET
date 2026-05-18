using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.CreateCustomerSegments
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCustomerSegmentsCommandHandler : IRequestHandler<CreateCustomerSegmentsCommand, Guid>
    {
        private readonly ICustomerSegmentsRepository _customerSegmentsRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCustomerSegmentsCommandHandler(ICustomerSegmentsRepository customerSegmentsRepository)
        {
            _customerSegmentsRepository = customerSegmentsRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCustomerSegmentsCommand request, CancellationToken cancellationToken)
        {
            var customerSegments = new Domain.Entities.CustomerSegments
            {
                SegmentId = request.SegmentId,
                CustomerId = request.CustomerId,
                Confidence = request.Confidence
            };

            _customerSegmentsRepository.Add(customerSegments);
            await _customerSegmentsRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return customerSegments.Id;
        }
    }
}