using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.CreateCustomerSegments
{
    public class CreateCustomerSegmentsCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerSegmentsCommand(Guid segmentId,
            Guid customerId,
            ClassificationSource classificationSource,
            decimal confidence)
        {
            SegmentId = segmentId;
            CustomerId = customerId;
            ClassificationSource = classificationSource;
            Confidence = confidence;
        }

        public Guid SegmentId { get; set; }
        public Guid CustomerId { get; set; }
        public ClassificationSource ClassificationSource { get; set; }
        public decimal Confidence { get; set; }
    }
}