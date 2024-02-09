using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.DeleteBadSignatures
{
    public class DeleteBadSignaturesCommand : IRequest, ICommand
    {
        public DeleteBadSignaturesCommand(Guid id, string more)
        {
            Id = id;
            More = more;
        }

        public Guid Id { get; set; }
        public string More { get; set; }
    }
}