using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.QueryDtoParameter;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallHasDtoParameter
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallHasDtoParameterQueryHandler : IRequestHandler<CallHasDtoParameterQuery, int>
    {
        private readonly IQueryDtoParameterService _queryDtoParameterService;

        [IntentManaged(Mode.Merge)]
        public CallHasDtoParameterQueryHandler(IQueryDtoParameterService queryDtoParameterService)
        {
            _queryDtoParameterService = queryDtoParameterService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<int> Handle(CallHasDtoParameterQuery request, CancellationToken cancellationToken)
        {
            var result = await _queryDtoParameterService.HasDtoParameterAsync(new QueryDtoParameterCriteria
            {
                Field1 = request.Field1,
                Field2 = request.Field2,
                Nested = request.Nested
            }, cancellationToken);

            // TODO: Implement return type mapping...
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}