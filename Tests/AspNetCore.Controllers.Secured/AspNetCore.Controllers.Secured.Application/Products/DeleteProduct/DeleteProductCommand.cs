using System;
using AspNetCore.Controllers.Secured.Application.Common.Interfaces;
using AspNetCore.Controllers.Secured.Application.Common.Security;
using AspNetCore.Controllers.Secured.Application.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Products.DeleteProduct
{
    [Authorize(Roles = Permissions.RoleAdmin)]
    public class DeleteProductCommand : IRequest, ICommand
    {
        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}