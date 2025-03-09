using System;
using GrpcServer.Application.Common.Interfaces;
using GrpcServer.Application.Common.Security;
using GrpcServer.Application.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GrpcServer.Application.Products.UpdateProduct
{
    [Authorize(Roles = Permissions.SomeRole)]
    public class UpdateProductCommand : IRequest, ICommand
    {
        public UpdateProductCommand(string name, Guid id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}