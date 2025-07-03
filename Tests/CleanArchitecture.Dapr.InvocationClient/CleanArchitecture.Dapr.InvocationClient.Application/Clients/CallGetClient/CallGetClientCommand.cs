using CleanArchitecture.Dapr.InvocationClient.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallGetClient
{
    public class CallGetClientCommand : IRequest, ICommand
    {
        public CallGetClientCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}