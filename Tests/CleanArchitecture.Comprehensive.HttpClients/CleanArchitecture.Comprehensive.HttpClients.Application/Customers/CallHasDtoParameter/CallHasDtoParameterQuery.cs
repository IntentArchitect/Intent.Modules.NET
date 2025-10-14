using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.QueryDtoParameter;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallHasDtoParameter
{
    public class CallHasDtoParameterQuery : IRequest<int>, IQuery
    {
        public CallHasDtoParameterQuery(string field1, string field2, NestedQueryDto nested)
        {
            Field1 = field1;
            Field2 = field2;
            Nested = nested;
        }

        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public NestedQueryDto Nested { get; set; }
    }
}