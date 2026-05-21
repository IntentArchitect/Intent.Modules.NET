using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.UpdateCustomerSegments
{
    public class UpdateCustomerSegmentsCommand : IRequest, ICommand
    {
        public UpdateCustomerSegmentsCommand(Guid id, Guid segmentId, Guid customerId, decimal confidence)
        {
            Id = id;
            SegmentId = segmentId;
            CustomerId = customerId;
            Confidence = confidence;
        }

        public Guid Id { get; set; }
        public Guid SegmentId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Confidence { get; set; }
    }
}