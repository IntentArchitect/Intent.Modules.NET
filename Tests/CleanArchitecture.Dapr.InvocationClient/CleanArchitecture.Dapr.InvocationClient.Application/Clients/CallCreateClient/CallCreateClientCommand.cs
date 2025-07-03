using CleanArchitecture.Dapr.InvocationClient.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallCreateClient
{
    public class CallCreateClientCommand : IRequest, ICommand
    {
        public CallCreateClientCommand(string name, List<string> tagsIds)
        {
            Name = name;
            TagsIds = tagsIds;
        }

        public string Name { get; set; }
        public List<string> TagsIds { get; set; }
    }
}