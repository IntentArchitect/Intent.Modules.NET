using AwsLambdaFunction.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynClients.DeleteDynClient
{
    public class DeleteDynClientCommand : IRequest, ICommand
    {
        public DeleteDynClientCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}