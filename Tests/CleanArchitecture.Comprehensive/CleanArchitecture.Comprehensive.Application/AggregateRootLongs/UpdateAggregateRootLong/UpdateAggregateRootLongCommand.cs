using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.UpdateAggregateRootLong
{
    public class UpdateAggregateRootLongCommand : IRequest, ICommand
    {
        public UpdateAggregateRootLongCommand(long id,
            string attribute,
            UpdateAggregateRootLongCompositeOfAggrLongDto? compositeOfAggrLong)
        {
            Id = id;
            Attribute = attribute;
            CompositeOfAggrLong = compositeOfAggrLong;
        }

        public long Id { get; set; }
        public string Attribute { get; set; }
        public UpdateAggregateRootLongCompositeOfAggrLongDto? CompositeOfAggrLong { get; set; }
    }
}