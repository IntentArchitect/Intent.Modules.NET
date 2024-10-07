using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Interfaces.ServiceDispatch;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.ServiceDispatch.ServiceDispatchWithoutImplementation
{
    public class QueryNoImpl6AsyncEndpoint : EndpointWithoutRequest<string>
    {
        private readonly IServiceDispatchWithoutImplementationService _appService;

        public QueryNoImpl6AsyncEndpoint(IServiceDispatchWithoutImplementationService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        public override void Configure()
        {
            Get("api/service-dispatch-without-implementation/query-async");
            Description(b =>
            {
                b.WithTags("ServiceDispatchWithoutImplementation");
                b.Produces<string>(StatusCodes.Status200OK);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(string);
            result = await _appService.QueryNoImpl6Async(ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}