using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Clients.DeleteClient
{
    public class DeleteClientCommand : IRequest, ICommand
    {
        public DeleteClientCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}