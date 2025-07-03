using CleanArchitecture.Dapr.InvocationClient.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallUpdateClient
{
    public class CallUpdateClientCommand : IRequest, ICommand
    {
        public CallUpdateClientCommand(string id, string name, List<string> tagsIds)
        {
            Id = id;
            Name = name;
            TagsIds = tagsIds;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> TagsIds { get; set; }
    }
}