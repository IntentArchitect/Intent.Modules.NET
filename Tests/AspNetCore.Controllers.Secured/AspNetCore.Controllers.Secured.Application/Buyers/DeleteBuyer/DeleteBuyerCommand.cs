using System;
using AspNetCore.Controllers.Secured.Application.Common.Interfaces;
using AspNetCore.Controllers.Secured.Application.Common.Security;
using AspNetCore.Controllers.Secured.Application.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers.DeleteBuyer
{
    [Authorize(Roles = Permissions.Role1, Policy = Permissions.Policy2)]
    [Authorize(Roles = Permissions.Role2, Policy = Permissions.Policy2)]
    public class DeleteBuyerCommand : IRequest, ICommand
    {
        public DeleteBuyerCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}