using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Segments.CreateSegment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateSegmentCommandHandler : IRequestHandler<CreateSegmentCommand, Guid>
    {
        private readonly ISegmentRepository _segmentRepository;

        [IntentManaged(Mode.Merge)]
        public CreateSegmentCommandHandler(ISegmentRepository segmentRepository)
        {
            _segmentRepository = segmentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateSegmentCommand request, CancellationToken cancellationToken)
        {
            var segment = new Segment
            {
                SegmentType = request.SegmentType,
                Priority = request.Priority
            };

            _segmentRepository.Add(segment);
            await _segmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return segment.Id;
        }
    }
}