using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.CreateAggregateRootLong
{
    public class CreateAggregateRootLongCommand : IRequest<long>, ICommand
    {
        public CreateAggregateRootLongCommand(string attribute,
            CreateAggregateRootLongCompositeOfAggrLongDto? compositeOfAggrLong)
        {
            Attribute = attribute;
            CompositeOfAggrLong = compositeOfAggrLong;
        }

        public string Attribute { get; set; }
        public CreateAggregateRootLongCompositeOfAggrLongDto? CompositeOfAggrLong { get; set; }
    }
}