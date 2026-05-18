using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Segments.CreateSegment
{
    public class CreateSegmentCommand : IRequest<Guid>, ICommand
    {
        public CreateSegmentCommand(SegmentType segmentType, int priority)
        {
            SegmentType = segmentType;
            Priority = priority;
        }

        public SegmentType SegmentType { get; set; }
        public int Priority { get; set; }
    }
}