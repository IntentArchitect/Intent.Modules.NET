using System;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.Interfaces.ServiceDispatch;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.ServiceDispatch.ServiceDispatchWithoutImplementation
{
    public class QueryNoReturnTypeEndpoint : EndpointWithoutRequest
    {
        private readonly IServiceDispatchWithoutImplementationService _appService;

        public QueryNoReturnTypeEndpoint(IServiceDispatchWithoutImplementationService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        public override void Configure()
        {
            Get("api/service-dispatch-without-implementation/no-return-type");
            Description(b =>
            {
                b.WithTags("ServiceDispatchWithoutImplementation");
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await _appService.QueryNoReturnType(ct);
            await Send.ResultAsync(TypedResults.NoContent());
        }
    }
}