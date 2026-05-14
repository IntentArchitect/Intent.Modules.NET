using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Common.Exceptions;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.UpdateCustomerSegments
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCustomerSegmentsCommandHandler : IRequestHandler<UpdateCustomerSegmentsCommand>
    {
        private readonly ICustomerSegmentsRepository _customerSegmentsRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCustomerSegmentsCommandHandler(ICustomerSegmentsRepository customerSegmentsRepository)
        {
            _customerSegmentsRepository = customerSegmentsRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCustomerSegmentsCommand request, CancellationToken cancellationToken)
        {
            var customerSegments = await _customerSegmentsRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customerSegments is null)
            {
                throw new NotFoundException($"Could not find CustomerSegments '{request.Id}'");
            }

            customerSegments.SegmentId = request.SegmentId;
            customerSegments.CustomerId = request.CustomerId;
            customerSegments.ClassificationSource = request.ClassificationSource;
            customerSegments.Confidence = request.Confidence;
        }
    }
}