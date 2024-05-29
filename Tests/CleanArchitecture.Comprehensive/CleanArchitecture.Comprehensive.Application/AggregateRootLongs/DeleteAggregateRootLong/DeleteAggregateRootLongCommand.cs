using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.DeleteAggregateRootLong
{
    public class DeleteAggregateRootLongCommand : IRequest, ICommand
    {
        public DeleteAggregateRootLongCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}