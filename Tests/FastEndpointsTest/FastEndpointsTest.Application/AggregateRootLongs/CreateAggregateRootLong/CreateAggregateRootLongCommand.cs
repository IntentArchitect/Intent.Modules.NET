using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs.CreateAggregateRootLong
{
    public class CreateAggregateRootLongCommand : IRequest<long>, ICommand
    {
        public CreateAggregateRootLongCommand(string attribute,
            CreateAggregateRootLongCommandCompositeOfAggrLongDto? compositeOfAggrLong)
        {
            Attribute = attribute;
            CompositeOfAggrLong = compositeOfAggrLong;
        }

        public string Attribute { get; set; }
        public CreateAggregateRootLongCommandCompositeOfAggrLongDto? CompositeOfAggrLong { get; set; }
    }
}