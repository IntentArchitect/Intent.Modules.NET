using System;
using AspNetCore.Controllers.Secured.Application.Common.Interfaces;
using AspNetCore.Controllers.Secured.Application.Common.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers.DeleteBuyer
{
    [Authorize(Roles = "Role1", Policy = "Policy2")]
    [Authorize(Roles = "Role2", Policy = "Policy2")]
    public class DeleteBuyerCommand : IRequest, ICommand
    {
        public DeleteBuyerCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}