using System;
using System.Collections.Generic;
using GrpcServer.Application.Common.Interfaces;
using GrpcServer.Application.Common.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GrpcServer.Application.Products.CreateProduct
{
    [Authorize]
    public class CreateProductCommand : IRequest<Guid>, ICommand
    {
        public CreateProductCommand(string name, List<string>? strings)
        {
            Name = name;
            Strings = strings;
        }

        public string Name { get; set; }
        public List<string>? Strings { get; set; }
    }
}