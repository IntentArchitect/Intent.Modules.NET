using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMappingTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ObjectMappingTest.Application.Products.GetDigitalProductById
{
    public class GetDigitalProductById : IRequest<DigitalProductDto>, IQuery
    {
        public GetDigitalProductById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}