using EntityFramework.Application.LinqExtensions.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFramework.Application.LinqExtensions.Tests.Application.Customers.GetCustomersWithStatus
{
    public class GetCustomersWithStatusQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public GetCustomersWithStatusQuery(bool isActive)
        {
            IsActive = isActive;
        }

        public bool IsActive { get; set; }
    }
}