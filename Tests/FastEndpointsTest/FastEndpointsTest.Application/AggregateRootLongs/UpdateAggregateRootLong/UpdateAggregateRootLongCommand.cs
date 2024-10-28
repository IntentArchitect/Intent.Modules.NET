using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs.UpdateAggregateRootLong
{
    public class UpdateAggregateRootLongCommand : IRequest, ICommand
    {
        public UpdateAggregateRootLongCommand(string attribute,
            long id,
            UpdateAggregateRootLongCommandCompositeOfAggrLongDto? compositeOfAggrLong)
        {
            Attribute = attribute;
            Id = id;
            CompositeOfAggrLong = compositeOfAggrLong;
        }

        public string Attribute { get; set; }
        public long Id { get; set; }
        public UpdateAggregateRootLongCommandCompositeOfAggrLongDto? CompositeOfAggrLong { get; set; }
    }
}