using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.DeleteTag
{
    public class DeleteTagCommand : IRequest, ICommand
    {
        public DeleteTagCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}