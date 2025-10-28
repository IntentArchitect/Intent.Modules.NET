using System;
using AzureFunctions.NET8.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.NET8.Application.ResponseCodes.DeleteResponseCode
{
    public class DeleteResponseCodeCommand : IRequest, ICommand
    {
        public DeleteResponseCodeCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}