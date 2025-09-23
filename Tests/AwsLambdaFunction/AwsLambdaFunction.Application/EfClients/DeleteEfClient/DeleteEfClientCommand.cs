using System;
using AwsLambdaFunction.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfClients.DeleteEfClient
{
    public class DeleteEfClientCommand : IRequest, ICommand
    {
        public DeleteEfClientCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}