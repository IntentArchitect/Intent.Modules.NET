using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.CreateRichProduct
{
    public class CreateRichProductCommand : IRequest<Guid>, ICommand
    {
        public CreateRichProductCommand(Guid brandId, string name)
        {
            BrandId = brandId;
            Name = name;
        }

        public Guid BrandId { get; set; }
        public string Name { get; set; }
    }
}