using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.ParamConversion.CheckTypeConversionsOnProxy;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.ParamConversion
{
    public class CheckTypeConversionsOnProxyEndpoint : Endpoint<CheckTypeConversionsOnProxy, bool>
    {
        private readonly ISender _mediator;

        public CheckTypeConversionsOnProxyEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/param-conversion/check-type-conversions-on-proxy");
            Description(b =>
            {
                b.WithTags("ParamConversion");
                b.Accepts<CheckTypeConversionsOnProxy>();
                b.Produces<bool>(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CheckTypeConversionsOnProxy req, CancellationToken ct)
        {
            var result = default(bool);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}