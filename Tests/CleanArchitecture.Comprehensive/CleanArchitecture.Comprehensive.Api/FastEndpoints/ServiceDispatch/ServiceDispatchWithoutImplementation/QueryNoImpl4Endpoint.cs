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
    public class QueryNoImpl4Endpoint : Endpoint<QueryNoImpl4RequestModel, string>
    {
        private readonly IServiceDispatchWithoutImplementationService _appService;

        public QueryNoImpl4Endpoint(IServiceDispatchWithoutImplementationService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        public override void Configure()
        {
            Get("api/service-dispatch-without-implementation/query-param");
            Description(b =>
            {
                b.WithTags("ServiceDispatchWithoutImplementation");
                b.Accepts<QueryNoImpl4RequestModel>();
                b.Produces<string>(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(QueryNoImpl4RequestModel req, CancellationToken ct)
        {
            var result = default(string);
            result = _appService.QueryNoImpl4(req.Param);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }

    public class QueryNoImpl4RequestModel
    {
        [FromQueryParams]
        public string Param { get; set; }
    }
}