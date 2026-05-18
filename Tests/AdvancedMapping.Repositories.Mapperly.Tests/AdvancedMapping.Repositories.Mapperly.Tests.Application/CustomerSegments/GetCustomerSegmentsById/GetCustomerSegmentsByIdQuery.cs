using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.GetCustomerSegmentsById
{
    public class GetCustomerSegmentsByIdQuery : IRequest<CustomerSegmentsDto>, IQuery
    {
        public GetCustomerSegmentsByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}