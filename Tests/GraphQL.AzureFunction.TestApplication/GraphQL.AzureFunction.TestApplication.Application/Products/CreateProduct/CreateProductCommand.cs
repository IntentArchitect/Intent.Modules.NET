using System;
using System.Collections.Generic;
using GraphQL.AzureFunction.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Products.CreateProduct
{
    public class CreateProductCommand : IRequest<ProductDto>, ICommand
    {
        public CreateProductCommand(string name)
        {
            Name = name;
        }
        public string Name { get; set; }

    }
}