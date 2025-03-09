using System;
using GrpcServer.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GrpcServer.Application.Products.DeleteProduct
{
    public class DeleteProductCommand : IRequest, ICommand
    {
        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}